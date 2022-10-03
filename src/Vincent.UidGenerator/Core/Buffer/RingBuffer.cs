using System;
using Microsoft.Extensions.Logging;
using Vincent.UidGenerator.Exception;
using Vincent.UidGenerator.Utils;

namespace Vincent.UidGenerator.Core.Buffer;

public class RingBuffer
{
    private ILogger<RingBuffer> _logger;

    private static readonly int StartPoint = -1;

    private readonly long CanPutFlag = 0L;
    private readonly long CanTakeFlag = 1L;

    /** The size of RingBuffer's slots, each slot hold a UID */
    private readonly int _bufferSize;

    private readonly long _indexMask;
    private readonly long[] _slots;
    private readonly AtomicLong[] _flags;

    /** Tail: last position sequence to produce */
    private readonly AtomicLong _tail = new PaddedAtomicLong(StartPoint);

    /** Cursor: current position sequence to consume */
    private readonly AtomicLong _cursor = new PaddedAtomicLong(StartPoint);

    /** Threshold for trigger padding buffer*/
    private readonly int _paddingThreshold;

    /** Reject put/take buffer handle policy */
    private readonly Action<RingBuffer, long> _rejectedPutHandler;

    private readonly Action<RingBuffer> _rejectedTakeHandler;

    /** Executor of padding buffer */
    private BufferPaddingExecutor _bufferPaddingExecutor;

    private readonly object _syncLock = new object();
    
    /// <summary>
    /// Constructor with buffer size & padding factor
    /// </summary>
    /// <param name="bufferSize">bufferSize must be positive & a power of 2</param>
    /// <param name="paddingFactor">paddingFactor percent in (0 - 100). When the count of rest available UIDs reach the threshold, it will trigger padding buffer
    ///       Sample: paddingFactor=20, bufferSize=1000 -> threshold=1000 * 20 /100,  
    ///    padding buffer will be triggered when tail-cursor<threshold</param>
    /// <exception cref="ArgumentException"></exception>
    public RingBuffer(int bufferSize, int paddingFactor, Action<RingBuffer, long> rejectedPutBufferHandler=null,
        Action<RingBuffer> rejectedTakeBufferHandler=null)
    {
        // check buffer size is positive & a power of 2; padding factor in (0, 100)
        if (bufferSize <= 0L)
        {
            throw new ArgumentException("RingBuffer size must be positive");
        }

        if ((bufferSize & (bufferSize - 1)) != 0)
        {
            throw new ArgumentException("RingBuffer size must be a power of 2");
        }

        if (paddingFactor is <= 0 or >= 100)
        {
            throw new ArgumentException("RingBuffer size must be positive");
        }

        _bufferSize = bufferSize;
        _indexMask = bufferSize - 1;
        _slots = new long[bufferSize];
        _flags = InitFlags(bufferSize);

        _paddingThreshold = bufferSize * paddingFactor / 100;
        using var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        _logger = loggerFactory.CreateLogger<RingBuffer>();
        
        _rejectedPutHandler =
            rejectedPutBufferHandler ?? throw new ArgumentNullException(nameof(rejectedPutBufferHandler));
        _rejectedTakeHandler = rejectedTakeBufferHandler ??
                               throw new ArgumentNullException(nameof(rejectedTakeBufferHandler));
    }

    /// <summary>
    /// Put an UID in the ring & tail moved
    /// We use 'synchronized' to guarantee the UID fill in slot & publish new tail sequence as atomic operations
    /// </summary>
    /// <remarks>
    /// Note that: It is recommended to put UID in a serialize way, cause we once batch generate a series UIDs and put
    /// the one by one into the buffer, so it is unnecessary put in multi-threads
    /// </remarks>
    /// <param name="uid"></param>
    /// <returns>false means that the buffer is full, apply <see cref="CachedUidGeneratorOptions.RejectedPutBufferHandler"/></returns>
    public bool Put(long uid)
    {
        lock (_syncLock)
        {
            long currentTail = _tail.Get();
            long currentCursor = _cursor.Get();

            // tail catches the cursor, means that you can't put any cause of RingBuffer is full
            long distance = currentTail - (currentCursor == StartPoint ? 0 : currentCursor);
            if (distance == _bufferSize - 1)
            {
                _rejectedPutHandler(this, uid);
                return false;
            }

            // 1. pre-check whether the flag is CAN_PUT_FLAG
            int nextTailIndex = CalSlotIndex(currentTail + 1);
            if (_flags[nextTailIndex] != CanPutFlag)
            {
                _rejectedPutHandler(this, uid);
                return false;
            }

            // 2. put UID in the next slot
            // 3. update next slot' flag to CAN_TAKE_FLAG
            // 4. publish tail with sequence increase by one
            _slots[nextTailIndex] = uid;
            _flags[nextTailIndex].Set(CanTakeFlag);
            _tail.Increment();
            //Interlocked.Increment(ref tail);
            //tail.IncrementAndGet();

            // The atomicity of operations above, guarantees by 'synchronized'. In another word,
            // the take operation can't consume the UID we just put, until the tail is published(tail.incrementAndGet())
            return true;
        }
    }


    /// <summary>
    /// Take an UID of the ring at the next cursor, this is a lock free operation by using atomic cursor
    /// Before getting the UID, we also check whether reach the padding threshold,
    /// the padding buffer operation will be triggered in another thread
    /// If there is no more available UID to be taken, the specified <see cref="_rejectedTakeHandler"/> will be applied
    /// </summary>
    /// <returns>UID</returns>
    /// <exception cref="UidGenerateException">IllegalStateException if the cursor moved back</exception>
    public long Take()
    {
        // spin get next available cursor
        long currentCursor = _cursor;

        //long nextCursor = cursor.updateAndGet(old -> old == tail.get() ? old : old + 1);
        long nextCursor = _cursor.UpdateAndGet(old => old == _tail.Get() ? old : old + 1);

        // check for safety consideration, it never occurs
        if (nextCursor < currentCursor)
        {
            throw new UidGenerateException("Cursor can't move back");
        }
        //Assert.isTrue(nextCursor >= currentCursor, "Curosr can't move back");

        // trigger padding in an async-mode if reach the threshold
        long currentTail = _tail.Get();
        if (currentTail - nextCursor < _paddingThreshold)
        {
#if DEBUG
            _logger.LogInformation(
                $"Reach the padding threshold:{_paddingThreshold}. tail:{currentTail}, cursor:{nextCursor}, rest:{currentTail - nextCursor}");
#endif

            _bufferPaddingExecutor.PaddingBufferAsync();
        }

        // cursor catch the tail, means that there is no more available UID to take
        if (nextCursor == currentCursor)
        {
            _rejectedTakeHandler(this);
        }

        // 1. check next slot flag is CAN_TAKE_FLAG
        int nextCursorIndex = CalSlotIndex(nextCursor);
        if (_flags[nextCursorIndex].Get() != CanTakeFlag)
        {
            throw new UidGenerateException("Cursor not in can take status");
        }

        //Assert.isTrue(flags[nextCursorIndex].get() == CAN_TAKE_FLAG, "Curosr not in can take status");

        // 2. get UID from next slot
        // 3. set next slot flag as CAN_PUT_FLAG.
        long uid = _slots[nextCursorIndex];
        _flags[nextCursorIndex].Set(CanPutFlag);

        // Note that: Step 2,3 can not swap. If we set flag before get value of slot, the producer may overwrite the
        // slot with a new UID, and this may cause the consumer take the UID twice after walk a round the ring
        return uid;
    }

    /// <summary>
    /// Calculate slot index with the slot sequence (sequence % bufferSize) 
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    protected int CalSlotIndex(long sequence)
    {
        return (int) (sequence & _indexMask);
    }


    /// <summary>
    /// Initialize flags as CAN_PUT_FLAG
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    private PaddedAtomicLong[] InitFlags(int bufferSize)
    {
        PaddedAtomicLong[] flags = new PaddedAtomicLong[bufferSize];
        for (int i = 0; i < bufferSize; i++)
        {
            flags[i] = new PaddedAtomicLong(CanPutFlag);
        }

        return flags;
    }

    /// <summary>
    /// 因为 <see cref="RingBuffer"/> 和 <see cref="BufferPaddingExecutor"/> 互相依赖，使用此方法代替构造函数
    /// </summary>
    /// <param name="bufferPaddingExecutor"></param>
    public void SetBufferPaddingExecutor(BufferPaddingExecutor bufferPaddingExecutor)
    {
        _bufferPaddingExecutor = bufferPaddingExecutor;
    }
}
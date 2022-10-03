using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vincent.UidGenerator.Utils;

namespace Vincent.UidGenerator.Core.Buffer;

/// <summary>
/// Buffer Padding Executor
/// </summary>
public class BufferPaddingExecutor
{
    /// <summary>
    /// Whether buffer padding is running
    /// </summary>
    private readonly AtomicBool Running = new AtomicBool(false);

    /// <summary>
    /// We can borrow UIDs from the future, here store the last second we have consumed
    /// </summary>
    private readonly PaddedAtomicLong _lastSecond;

    /// <summary>
    /// RingBuffer & BufferUidProvider
    /// </summary>
    private readonly RingBuffer _ringBuffer;

    private readonly Func<long, List<long>> _uidProvider;


    private readonly System.Timers.Timer? _bufferPadSchedule;

    private readonly ILogger<BufferPaddingExecutor>? _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ringBuffer"></param>
    /// <param name="uidProvider"></param>
    /// <param name="usingSchedule"></param>
    /// <param name="scheduleInterval"></param>
    /// <exception cref="ArgumentException"></exception>
    public BufferPaddingExecutor(RingBuffer ringBuffer, Func<long, List<long>> uidProvider, bool usingSchedule = false,
        int scheduleInterval = -1)
    {
        using var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        _logger = loggerFactory.CreateLogger<BufferPaddingExecutor>();
        
        _lastSecond = new PaddedAtomicLong(DateTimeOffset.Now.ToUnixTimeSeconds());
        _ringBuffer = ringBuffer;
        _uidProvider = uidProvider;

        // initialize schedule thread
        if (usingSchedule)
        {
            if (scheduleInterval <= 0)
            {
                throw new ArgumentException("Schedule interval must positive!");
            }

            _bufferPadSchedule = new System.Timers.Timer();
            _bufferPadSchedule.Elapsed += (sender, args) => PaddingBuffer();
            _bufferPadSchedule.Interval = scheduleInterval;
        }
        else
        {
            _bufferPadSchedule = null;
        }
    }

    /// <summary>
    /// Start executors such as schedule
    /// </summary>
    public void Start()
    {
        _bufferPadSchedule?.Start();
    }

    /// <summary>
    /// Shutdown executors.
    /// </summary>
    public void Shutdown()
    {
        _bufferPadSchedule?.Close();
    }


    /// <summary>
    /// Padding buffer async
    /// </summary>
    public void PaddingBufferAsync()
    {
        Task.Factory.StartNew(PaddingBuffer, TaskCreationOptions.LongRunning);
    }

   /// <summary>
   /// Padding buffer fill the slots until to catch the cursor
   /// </summary>
    public void PaddingBuffer()
    {
#if DEBUG
        _logger.LogInformation($"Ready to padding buffer lastSecond:{_lastSecond.Get()}. {_ringBuffer}");
#endif
        
        // is still running
        if (!Running.CompareAndSet(false, true))
        {
#if DEBUG
            _logger.LogInformation($"Padding buffer is still running. {_ringBuffer}");
#endif
            return;
        }

        // fill the rest slots until to catch the cursor
        bool isFullRingBuffer = false;
        while (!isFullRingBuffer)
        {
            List<long> uidList = _uidProvider(_lastSecond.PreIncrement());
            foreach (long uid in uidList)
            {
                isFullRingBuffer = !_ringBuffer.Put(uid);
                if (isFullRingBuffer)
                {
                    break;
                }
            }
        }

        // not running now
        Running.Set(false);
#if DEBUG
        _logger.LogInformation($"End to padding buffer lastSecond:{_lastSecond.Get()}. {_ringBuffer}");
#endif
    }
   
}
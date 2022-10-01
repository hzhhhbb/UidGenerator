using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Vincent.UidGenerator.Core.Buffer;
using Vincent.UidGenerator.Exception;

namespace Vincent.UidGenerator.Core;

/// <summary>
/// Represents a cached implementation of <see cref="IUidGenerator"/> extends from <see cref="DefaultUidGenerator"/>, based on a lock free <see cref="RingBuffer"/>
/// </summary>
public class CachedUidGenerator : DefaultUidGenerator, IDisposable
{
    private readonly int _paddingFactor = RingBuffer.DefaultPaddingPercent;

    private readonly RingBuffer _ringBuffer;

    private readonly BufferPaddingExecutor _bufferPaddingExecutor;

    public override long GetUid()
    {
        try
        {
            return _ringBuffer.Take();
        }
        catch (System.Exception e)
        {
            throw new UidGenerateException("Generate unique id exception. ", e);
        }
    }

    public CachedUidGenerator(CachedUidGeneratorOptions options) : base(options)
    {
        if (options.RejectedPutBufferHandler == null)
        {
            throw new ArgumentNullException("RejectedPutBufferHandler can not be null. you can use default value");
        }

        if (options.RejectedTakeBufferHandler == null)
        {
            throw new ArgumentNullException("RejectedTakeBufferHandler can not be null. you can use default value");
        }

        if (options.BoostPower <= 0)
        {
            throw new ArgumentException("Boost power must be positive!");
        }

        // initialize RingBuffer
        int bufferSize = ((int) BitsAllocator.MaxSequence + 1) << options.BoostPower;

        _ringBuffer = new RingBuffer(bufferSize, _paddingFactor, options.RejectedPutBufferHandler,
            options.RejectedTakeBufferHandler);
        _bufferPaddingExecutor = new BufferPaddingExecutor(_ringBuffer, nextIdsForOneSecond, options.UseScheduler,
            options.ScheduleInterval);
        _ringBuffer.SetBufferPaddingExecutor(_bufferPaddingExecutor);

        // fill in all slots of the RingBuffer
        _bufferPaddingExecutor.PaddingBuffer();

        // start buffer padding threads
        _bufferPaddingExecutor.Start();
    }

    public void Dispose()
    {
        _bufferPaddingExecutor.Shutdown();
    }

    /// <summary>
    /// Get the UIDs in the same specified second under the max sequence
    /// </summary>
    /// <param name="currentSecond"></param>
    /// <returns>UID list, size of <see cref="BitsAllocator.MaxSequence"/></returns>
    public List<long> nextIdsForOneSecond(long currentSecond)
    {
        // Initialize result list size of (max sequence + 1)
        int listSize = (int) BitsAllocator.MaxSequence + 1;
        List<long> uIds = new List<long>(listSize);

        // Allocate the first sequence of the second, the others can be calculated with the offset
        long firstSeqUid = BitsAllocator.Allocate(currentSecond - StartSeconds, WorkerId, 0L);
        for (int offset = 0; offset < listSize; offset++)
        {
            uIds.Add(firstSeqUid + offset);
        }

        return uIds;
    }

    protected virtual void InitLogger()
    {
        // todo init logger from outside
        using var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        Logger = loggerFactory.CreateLogger<DefaultUidGenerator>();
    }
}
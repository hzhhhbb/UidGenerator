using System;
using Microsoft.Extensions.Logging;
using Vincent.UidGenerator.Exception;

namespace Vincent.UidGenerator.Core;

public class DefaultUidGenerator : IUidGenerator
{
    private static readonly object _lock = new object();
    private readonly DefaultUidGeneratorOptions _options;
    protected readonly BitsAllocator BitsAllocator;
    protected readonly long WorkerId;
    private long _lastSecond = -1L;
    private long _sequence;
    protected readonly long StartSeconds;
    protected ILogger<DefaultUidGenerator> Logger;

    public DefaultUidGenerator(DefaultUidGeneratorOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        using var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        Logger = loggerFactory.CreateLogger<DefaultUidGenerator>();
        WorkerId = options.WorkerId;
        BitsAllocator = new BitsAllocator(_options.TimeBits, _options.WorkerBits, _options.SequenceBits);
        if ( WorkerId  > BitsAllocator.MaxWorkerId)
        {
            throw new UidGenerateException($"Worker id {WorkerId} exceeds the max {BitsAllocator.MaxWorkerId}");
        }
        _options = options;
        StartSeconds = new DateTimeOffset(_options.StartTime.ToUniversalTime()).ToUnixTimeSeconds();
    }

    public virtual long GetUid()
    {
        try
        {
            return NextId();
        }
        catch (System.Exception e)
        {
            throw new UidGenerateException("Generate unique id exception.", e);
        }
    }

    public string ParseUid(long uid)
    {
        long totalBits = BitsAllocator.TotalBits;
        long signBits = BitsAllocator.SignBits;
        long timestampBits = BitsAllocator.TimestampBits;
        long workerIdBits = BitsAllocator.WorkerIdBits;
        long sequenceBits = BitsAllocator.SequenceBits;

        // parse UID
        long sequence = (long) (ulong) (uid << (int) (totalBits - sequenceBits)) >> (int) (totalBits - sequenceBits);
        long workerId = (long) (ulong) (uid << (int) (timestampBits + signBits)) >> (int) (totalBits - workerIdBits);
        long deltaSeconds = uid >> (int) (workerIdBits + sequenceBits);

        DateTime thatTime = DateTimeOffset.FromUnixTimeSeconds(StartSeconds + deltaSeconds).DateTime;
        string thatTimeStr = thatTime.ToString("yyyy-MM-dd HH:mm:ss");

        string leftParenthesis = "{";
        string rightParenthesis = "}";

        // format as string
        return
            $"{leftParenthesis}\"UID\":\"{uid}\",\"timestamp\":\"{thatTimeStr}\",\"workerId\":\"{workerId}\",\"sequence\":\"{sequence}\"{rightParenthesis}";
    }

    protected long NextId()
    {
        lock (_lock)
        {
            long currentSecond = GetCurrentSecond();

            // Clock moved backwards, refuse to generate uid
            if (currentSecond < _lastSecond)
            {
                long refusedSeconds = _lastSecond - currentSecond;
                throw new UidGenerateException($"Clock moved backwards. Refusing for {refusedSeconds} seconds");
            }

            // At the same second, increase sequence
            if (currentSecond == _lastSecond)
            {
                _sequence = (_sequence + 1) & BitsAllocator.MaxSequence;
                // Exceed the max sequence, we wait the next second to generate uid
                if (_sequence == 0)
                {
                    currentSecond = GetNextSecond(_lastSecond);
                }

                // At the different second, sequence restart from zero
            }
            else
            {
                _sequence = 0L;
            }

            _lastSecond = currentSecond;

            // Allocate bits for UID
            return BitsAllocator.Allocate(currentSecond - StartSeconds, WorkerId, _sequence);
        }
    }

    /// <summary>
    /// Get next second
    /// </summary>
    /// <param name="lastTimestamp"></param>
    /// <returns></returns>
    private long GetNextSecond(long lastTimestamp)
    {
        long timestamp = GetCurrentSecond();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentSecond();
        }

        return timestamp;
    }

    /// <summary>
    /// Get current second
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UidGenerateException">When timestamp bits is exhausted</exception>
    private long GetCurrentSecond()
    {
        long currentSecond = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (currentSecond - StartSeconds > BitsAllocator.MaxDeltaSeconds)
        {
            throw new UidGenerateException("Timestamp bits is exhausted. Refusing UID generate. Now: " + currentSecond);
        }

        return currentSecond;
    }
}
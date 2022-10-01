using System;

namespace Vincent.UidGenerator.Core;

/// <summary>
///  * Allocate 64 bits for the UID(long)
/// </summary>
///<remarks>sign (fixed 1bit) -> deltaSecond -> workerId -> sequence(within the same second)</remarks>  
public class BitsAllocator
{
    /// <summary>
    /// Total 64 bits
    /// </summary>
    public readonly int TotalBits = 1 << 6;

    /**
     * Bits for [sign-> second-> workId-> sequence]
     */
    internal readonly int SignBits = 1;
    internal readonly int TimestampBits;
    internal readonly int WorkerIdBits;
    internal readonly int SequenceBits;

    /// <summary>
    /// 最大增量秒
    /// </summary>
    public readonly long MaxDeltaSeconds;

    /// <summary>
    /// Max workId
    /// </summary>
    public readonly long MaxWorkerId;

    /// <summary>
    /// Max sequence
    /// </summary>
    public readonly long MaxSequence;

    /// <summary>
    /// Shift for timestamp
    /// </summary>
    private readonly int _timestampShift;

    /// <summary>
    /// Shift for workId
    /// </summary>
    private readonly int _workerIdShift;
    
    /// <summary>
    /// Constructor with timestampBits, workerIdBits, sequenceBits
    /// The highest bit used for sign, so <code>63</code> bits for timestampBits, workerIdBits, sequenceBits
    /// </summary>
    /// <param name="timestampBits"></param>
    /// <param name="workerIdBits"></param>
    /// <param name="sequenceBits"></param>
    /// <exception cref="ArgumentException">当配置的位长不满64位时</exception>
    public BitsAllocator(int timestampBits, int workerIdBits, int sequenceBits)
    {
        // make sure allocated 64 bits
        int allocateTotalBits = SignBits + timestampBits + workerIdBits + sequenceBits;
        if (allocateTotalBits != TotalBits)
        {
            throw new ArgumentException("allocate not enough 64 bits");
        }

        // initialize bits
        TimestampBits = timestampBits;
        WorkerIdBits = workerIdBits;
        SequenceBits = sequenceBits;

        // initialize max value
        MaxDeltaSeconds = ~(-1L << timestampBits);
        MaxWorkerId = ~(-1L << workerIdBits);
        MaxSequence = ~(-1L << sequenceBits);

        // initialize shift
        _timestampShift = workerIdBits + sequenceBits;
        _workerIdShift = sequenceBits;
    }

    /// <summary>
    /// Allocate bits for UID according to delta seconds & workerId & sequence
    /// </summary>
    /// <remarks>The highest bit will always be 0 for sign</remarks>
    /// <param name="deltaSeconds"></param>
    /// <param name="workerId"></param>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public long Allocate(long deltaSeconds, long workerId, long sequence)
    {
        return 0 | (deltaSeconds << _timestampShift) | (workerId << _workerIdShift) | sequence;
    }
}
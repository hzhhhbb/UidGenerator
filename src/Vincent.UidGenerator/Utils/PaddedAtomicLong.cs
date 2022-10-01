namespace Vincent.UidGenerator.Utils;

public class PaddedAtomicLong : AtomicLong
{
    /// <summary>
    /// Padded 6 long (48 bytes)
    /// </summary>
    public long P1, P2, P3, P4, P5, P6 = 7L;

    /// <summary>
    /// Constructors from <see cref="AtomicLong"/>
    /// </summary>
    public PaddedAtomicLong() : base()
    {
    }

    /// <summary>
    /// Constructors from <see cref="AtomicLong"/>
    /// </summary>
    /// <param name="initialValue">initial value</param>
    public PaddedAtomicLong(long initialValue) : base(initialValue)
    {
    }

    /// <summary>
    /// To prevent GC optimizations for cleaning unused padded references
    /// </summary>
    /// <returns></returns>
    public long SumPaddingToPreventOptimization()
    {
        return P1 + P2 + P3 + P4 + P5 + P6;
    }
}
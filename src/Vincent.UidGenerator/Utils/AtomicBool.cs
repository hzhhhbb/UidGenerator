using System.Threading;

namespace Vincent.UidGenerator.Utils;

public class AtomicBool
{
    private volatile int _value;

    public AtomicBool()
    {
    }

    public AtomicBool(bool initialValue)
    {
        _value = initialValue ? 1 : 0;
    }

    public bool Get()
    {
        return _value == 1;
    }

    /// <summary>
    /// This method sets the current value atomically.
    /// </summary>
    /// <param name="value">
    /// The new value to set.
    /// </param>
    public void Set(bool value)
    {
        int v = value ? 1 : 0;
        Interlocked.Exchange(ref _value, v);
    }


    /// <summary>
    /// Atomically sets the value to the given updated value
    /// if the current value equals to the expected value.
    /// </summary>
    /// <param name="expected">expect the expected value</param>
    /// <param name="result">update the new value</param>
    /// <returns>if successful. False return indicates that
    /// the actual value was not equal to the expected value.
    /// </returns>
    public bool CompareAndSet(bool expected, bool result)
    {
        int e = expected ? 1 : 0;
        int r = result ? 1 : 0;
        return Interlocked.CompareExchange(ref _value, r, e) == e;
    }

    /// <summary>
    /// This operator allows an implicit cast from <c>AtomicLong</c> to <c>long</c>.
    /// </summary>
    public static implicit operator bool(AtomicBool value)
    {
        return value.Get();
    }
}
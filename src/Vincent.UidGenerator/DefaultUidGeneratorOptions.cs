using System;

namespace Vincent.UidGenerator;

/// <summary>
/// default options
/// </summary>
public class DefaultUidGeneratorOptions
{
    /// <summary>
    /// 时间位长
    /// </summary>
    public short TimeBits { get; set; } = 29;

    /// <summary>
    /// WorkId 位长
    /// </summary>
    public short WorkerBits { get; set; } = 21;
    
    /// <summary>
    /// 序列号位长
    /// </summary>
    public short SequenceBits { get; set; } = 13;

    /// <summary>
    /// start worker utc dateTime. default 2022-09-06 01:01:01
    /// </summary>
    public DateTime StartTime { get; set; } = new DateTime(2022, 09, 06, 00, 00, 00);
}
namespace Vincent.UidGenerator.Worker;

/// <summary>
/// 分配 workId 的方案
/// </summary>
public enum AssignWorkIdScheme
{
    /// <summary>
    /// use SqlServer to assign workerId
    /// </summary>
    SqlServer = 1,

    /// <summary>
    /// use MySQL to assign workerId
    /// </summary>
    MySQL = 2,

    /// <summary>
    /// Single machine
    /// </summary>
    SingleMachine = 3
}
namespace Vincent.UidGenerator.Worker;

/// <summary>
/// 分配 workId 的方案
/// </summary>
public enum AssignWorkIdScheme
{
    /// <summary>
    /// SqlServer 数据库
    /// </summary>
    SqlServer = 1,

    /// <summary>
    /// MySql 数据库
    /// </summary>
    MySql = 2
}
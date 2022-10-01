namespace Vincent.UidGenerator;

/// <summary>
/// Uid 生成器
/// </summary>
public interface IUidGenerator
{
    /// <summary>
    /// 获取 Uid
    /// </summary>
    /// <returns></returns>
    long GetUid();
    
    /// <summary>
    /// Parse the UID into elements which are used to generate the UID.
    /// Such as timestamp & workerId & sequence...
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>Parsed info</returns>
    string ParseUid(long uid);
}
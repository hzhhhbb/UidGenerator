using System;
using Vincent.UidGenerator.Core.Buffer;
using Vincent.UidGenerator.Exception;

namespace Vincent.UidGenerator;

/// <summary>
/// 缓存 Uid 生成器配置
/// <remarks>如未指定将采用默认值</remarks>
/// </summary>
public class CachedUidGeneratorOptions:DefaultUidGeneratorOptions
{
	/// <summary>
	/// RingBuffer size扩容参数, 可提高UID生成的吞吐量.
	/// <remarks> 默认:3，原bufferSize=8192, 扩容后bufferSize=8192<<3=65536</remarks>
	/// </summary>
	public int BoostPower { get; set; } = 3;

	/// <summary>
	/// 指定何时向RingBuffer中填充UID, 取值为百分比(0, 100), 默认为50
	/// <remarks>举例: bufferSize=1024, paddingFactor=50 -> threshold=1024 * 50 / 100 = 512.
	/// 当环上可用UID数量小于512时, 将自动对RingBuffer进行填充补全
	/// </remarks>
	/// </summary>
	public int PaddingFactor { get; set; } = 50;

	/// <summary>
	/// 另外一种RingBuffer填充时机, 在Schedule线程中, 周期性检查填充
	/// <remarks>默认:不配置此项, 即不使用Schedule线程</remarks>
	/// </summary>
	public bool UseScheduler { get; set; } = false;
	
	/// <summary>
	/// Schedule线程时间间隔, 单位:秒
	/// </summary>
	public int ScheduleInterval { get; set; } = -1;

	/// <summary>
	/// 拒绝策略: 当环已满, 无法继续填充时
	/// <remarks>默认无需指定, 将丢弃Put操作, 仅日志记录. 如有特殊需求, 请传入 Action </remarks>
	/// </summary>
	public Action<RingBuffer,long> RejectedPutBufferHandler { get; set; } = (uid,ringBuffer) => { Console.WriteLine($"Rejected putting buffer for uid:{uid}. {ringBuffer}");};
	
	/// <summary>
	/// 拒绝策略: 当环已空, 无法继续获取时
	/// <remarks>默认无需指定, 将记录日志, 并抛出UidGenerateException异常. 如有特殊需求, 请传入 Action </remarks>
	/// </summary>
	public Action<RingBuffer> RejectedTakeBufferHandler { get; set; }= (x) => throw new UidGenerateException("Rejected take buffer.");

	
}
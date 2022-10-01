using System;

namespace Vincent.UidGenerator.Worker.Entity;

public class WorkerNodeEntity
{
    /// <summary>
    /// Entity unique id (table unique)
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Host Name
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// Local machine ip
    /// </summary>
    public string Ip { get; set; } = string.Empty;
    
    /// <summary>
    /// type of <see cref="WorkerNodeType"/>
    /// </summary>
    public WorkerNodeType Type { get; set; }

    /**
     * Worker launch date, default now
     */
    public DateTime LaunchDate = DateTime.UtcNow;
}
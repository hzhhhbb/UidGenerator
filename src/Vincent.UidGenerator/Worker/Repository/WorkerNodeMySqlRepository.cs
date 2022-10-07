using System;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Vincent.UidGenerator.Worker.Entity;

namespace Vincent.UidGenerator.Worker.Repository;

internal class WorkerNodeMySqlRepository : IWorkerNodeRepository
{
    private readonly WorkerOptions _options;

    public WorkerNodeMySqlRepository(IOptions<WorkerOptions> options):this(options.Value)
    {
    }
    
    public WorkerNodeMySqlRepository(WorkerOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new ArgumentNullException(nameof(options.ConnectionString));
        }
        
        _options = options;
    }

    public long GetWorkNodeId( WorkerNodeEntity workerNodeEntity)
    {
        using MySqlConnection connection = new MySqlConnection(_options.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = EntityToSql(workerNodeEntity);

        var workId = command.ExecuteScalar();
        return (long) (ulong) workId;
    }

    private string EntityToSql(WorkerNodeEntity workerNodeEntity)
    {
        return
            $"insert into UidWorkerNode (HostName, Ip, Type) VALUE ('{workerNodeEntity.HostName}','{workerNodeEntity.Ip}',{(int) workerNodeEntity.Type});select last_insert_id();";
    }
}
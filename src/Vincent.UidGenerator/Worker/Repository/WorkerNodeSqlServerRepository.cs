using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Vincent.UidGenerator.Worker.Entity;

namespace Vincent.UidGenerator.Worker.Repository;

internal class WorkerNodeSqlServerRepository : IWorkerNodeRepository
{
    private readonly WorkerOptions _options;

    public WorkerNodeSqlServerRepository(IOptions<WorkerOptions> options):this(options.Value)
    {
    }
    
    public WorkerNodeSqlServerRepository(WorkerOptions options)
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
        using SqlConnection connection = new SqlConnection(_options.ConnectionString);
         connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = EntityToSql(workerNodeEntity);
        
        var workId =  command.ExecuteScalar();
        return Convert.ToInt64(workId);
    }

    private string EntityToSql(WorkerNodeEntity workerNodeEntity)
    {
        return
            $"insert into UidWorkerNode (HostName, Ip, Type) VALUES ('{workerNodeEntity.HostName}','{workerNodeEntity.Ip}',{(int) workerNodeEntity.Type})Select @@IDENTITY;";
    }
}
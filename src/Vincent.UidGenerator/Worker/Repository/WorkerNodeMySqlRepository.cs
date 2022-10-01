using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Vincent.UidGenerator.Worker.Entity;

namespace Vincent.UidGenerator.Worker.Repository;

internal class WorkerNodeMySqlRepository:IWorkerNodeRepository
{
    public long GetWorkNodeId(string connectionString,WorkerNodeEntity workerNodeEntity)
    {
        using  MySqlConnection connection = new MySqlConnection(connectionString);
         connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = EntityToSql(workerNodeEntity);
        
        var workId =  command.ExecuteScalar();
        return (long)(ulong)workId;
    }

    public string EntityToSql(WorkerNodeEntity workerNodeEntity)
    {
        return $"insert into UidWorkerNode (HostName, Ip, Type) VALUE ('{workerNodeEntity.HostName}','{workerNodeEntity.Ip}',{(int)workerNodeEntity.Type});select last_insert_id();";
    }
}
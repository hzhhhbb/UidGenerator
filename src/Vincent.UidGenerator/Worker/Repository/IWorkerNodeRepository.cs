using System.Threading.Tasks;
using Vincent.UidGenerator.Worker.Entity;

namespace Vincent.UidGenerator.Worker.Repository;

internal interface IWorkerNodeRepository
{
    long GetWorkNodeId(string connectionString,WorkerNodeEntity workerNodeEntity);

    string EntityToSql(WorkerNodeEntity workerNodeEntity);
}
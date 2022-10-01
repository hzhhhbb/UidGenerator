
using System.Threading.Tasks;
using Vincent.UidGenerator.Utils;
using Vincent.UidGenerator.Worker.Entity;
using Vincent.UidGenerator.Worker.Repository;

namespace Vincent.UidGenerator.Worker;

public static class WorkerIdAssigner 
{
    public static long AssignWorkerId(string connectionString,AssignWorkIdScheme assignWorkIdScheme)
    {
        var workerNodeRepository = WorkerNodeRepositoryFactory.Build(assignWorkIdScheme);
        var workerNodeEntity = BuildWorkerNode();
        return  workerNodeRepository.GetWorkNodeId(connectionString,workerNodeEntity);
    }

    /// <summary>
    /// Build worker node entity by IP and PORT
    /// </summary>
    /// <returns></returns>
    private static WorkerNodeEntity BuildWorkerNode()
    {
        WorkerNodeEntity workerNodeEntity = new WorkerNodeEntity();
        workerNodeEntity.Ip = HostInfoHelper.Ip;
        workerNodeEntity.HostName = HostInfoHelper.HostName;
        if (HostInfoHelper.IsDocker)
        {
            workerNodeEntity.Type = HostInfoHelper.IsDocker ? WorkerNodeType.Container : WorkerNodeType.Actual;
        }

        return workerNodeEntity;
    }
}
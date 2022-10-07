using Vincent.UidGenerator.Utils;
using Vincent.UidGenerator.Worker.Entity;
using Vincent.UidGenerator.Worker.Repository;

namespace Vincent.UidGenerator.Worker;

public class WorkerIdAssigner : IWorkerIdAssigner
{
    private readonly IWorkerNodeRepository _workerNodeRepository;

    public WorkerIdAssigner(IWorkerNodeRepository workerNodeRepository)
    {
        _workerNodeRepository = workerNodeRepository;
    }

    public long AssignWorkerId()
    {
        var workerNodeEntity = BuildWorkerNode();
        return _workerNodeRepository.GetWorkNodeId(workerNodeEntity);
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
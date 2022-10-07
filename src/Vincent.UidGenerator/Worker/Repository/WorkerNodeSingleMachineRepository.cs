using Vincent.UidGenerator.Worker.Entity;

namespace Vincent.UidGenerator.Worker.Repository;

public class WorkerNodeSingleMachineRepository:IWorkerNodeRepository
{
    public long GetWorkNodeId(WorkerNodeEntity workerNodeEntity)
    {
        return 1024L + 17L;
    }
}
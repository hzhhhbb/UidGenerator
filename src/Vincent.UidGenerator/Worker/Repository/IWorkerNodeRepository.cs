using Vincent.UidGenerator.Worker.Entity;

namespace Vincent.UidGenerator.Worker.Repository;

public interface IWorkerNodeRepository
{
    long GetWorkNodeId(WorkerNodeEntity workerNodeEntity);
}
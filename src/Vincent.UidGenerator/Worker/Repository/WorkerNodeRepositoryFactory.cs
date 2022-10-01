using System;

namespace Vincent.UidGenerator.Worker.Repository;

internal class WorkerNodeRepositoryFactory
{
    public static IWorkerNodeRepository Build(AssignWorkIdScheme assignWorkIdScheme)
    {
        switch (assignWorkIdScheme)
        {
            case AssignWorkIdScheme.SqlServer:
                return new WorkerNodeSqlServerRepository();
            case AssignWorkIdScheme.MySql:
                return new WorkerNodeMySqlRepository();
            default:
                throw new NotSupportedException(
                    $"This workerId type is not supported. WorkerId type: {assignWorkIdScheme.ToString()}");
        }
    }
}
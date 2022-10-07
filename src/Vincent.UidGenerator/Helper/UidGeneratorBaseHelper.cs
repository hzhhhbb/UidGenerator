using Vincent.UidGenerator.Worker;
using Vincent.UidGenerator.Worker.Repository;

namespace Vincent.UidGenerator.Helper;

public static  class UidGeneratorBaseHelper
{
    public static IWorkerIdAssigner BuildWorkerIdAssignerWithSQLServer(string connectionString)
    {
        var workerOptions = new WorkerOptions()
        {
            ConnectionString = connectionString
        };
        IWorkerNodeRepository workerNodeRepository = new WorkerNodeSqlServerRepository(workerOptions);
        IWorkerIdAssigner workerIdAssigner = new WorkerIdAssigner(workerNodeRepository);
        return workerIdAssigner;
    }
    
    public static IWorkerIdAssigner BuildWorkerIdAssignerWithMySQL(string connectionString)
    {
        var workerOptions = new WorkerOptions()
        {
            ConnectionString = connectionString
        };
        IWorkerNodeRepository workerNodeRepository = new WorkerNodeMySqlRepository(workerOptions);
        IWorkerIdAssigner workerIdAssigner = new WorkerIdAssigner(workerNodeRepository);
        return workerIdAssigner;
    }
    
    public static IWorkerIdAssigner BuildWorkerIdAssignerWithSingleMachine()
    {
        IWorkerNodeRepository workerNodeRepository = new WorkerNodeSingleMachineRepository();
        IWorkerIdAssigner workerIdAssigner = new WorkerIdAssigner(workerNodeRepository);
        return workerIdAssigner;
    }
}
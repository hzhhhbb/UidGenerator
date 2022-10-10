using System;
using Vincent.UidGenerator.Core;
using Vincent.UidGenerator.Worker;

namespace Vincent.UidGenerator.Helper;

public static class CachedUidGeneratorHelper 
{
    private static IUidGenerator _uidGenerator;

    private  static object _lock = new object();
    
    public static void InitWithSQLServerWorker(string connectionString,Action<CachedUidGeneratorOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        
        if (_uidGenerator != null)
        {
            return;
        }

        lock (_lock)
        {
            if (_uidGenerator == null)
            {
                var cachedOptions = new CachedUidGeneratorOptions();
                options(cachedOptions);
                _uidGenerator = new CachedUidGenerator(cachedOptions,UidGeneratorBaseHelper.BuildWorkerIdAssignerWithSQLServer(connectionString));
            }
        }
    }
 
    public static void InitWithMySQLWorker(string connectionString,Action<CachedUidGeneratorOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        
        if (_uidGenerator != null)
        {
            return;
        }

        lock (_lock)
        {
            if (_uidGenerator == null)
            {
                var cachedOptions = new CachedUidGeneratorOptions();
                options(cachedOptions);
                _uidGenerator = new CachedUidGenerator(cachedOptions,UidGeneratorBaseHelper.BuildWorkerIdAssignerWithMySQL(connectionString));
            }
        }
    }

    public static void InitWithSingleMachineWorker(Action<CachedUidGeneratorOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        
        if (_uidGenerator != null)
        {
            return;
        }

        lock (_lock)
        {
            if (_uidGenerator == null)
            {
                var cachedOptions = new CachedUidGeneratorOptions();
                options(cachedOptions);
                _uidGenerator = new CachedUidGenerator(cachedOptions,UidGeneratorBaseHelper.BuildWorkerIdAssignerWithSingleMachine());
            }
        }
    }

    /// <summary>
    /// get uid
    /// </summary>
    /// <remarks>Call the method <see cref="CachedUidGeneratorBaseHelper.Init"/> before using</remarks>
    /// <returns></returns>
    public static long GetUid()
    {
        return _uidGenerator.GetUid();
    }
}
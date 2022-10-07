using System;
using Vincent.UidGenerator.Core;
using Vincent.UidGenerator.Worker;

namespace Vincent.UidGenerator.Helper;

public static class DefaultUidGeneratorHelper
{
    private static IUidGenerator _uidGenerator;

    private  static object _lock = new object();
    
    public static void InitWithSQLServerWorker(string connectionString,Action<DefaultUidGeneratorOptions> options)
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
                var defaultOptions = new DefaultUidGeneratorOptions();
                options(defaultOptions);
                _uidGenerator = new DefaultUidGenerator(defaultOptions,UidGeneratorBaseHelper.BuildWorkerIdAssignerWithSQLServer(connectionString));
            }
        }
    }
 
    public static void InitWithMySQLWorker(string connectionString,Action<DefaultUidGeneratorOptions> options)
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
                var defaultOptions = new DefaultUidGeneratorOptions();
                options(defaultOptions);
                _uidGenerator = new DefaultUidGenerator(defaultOptions,UidGeneratorBaseHelper.BuildWorkerIdAssignerWithMySQL(connectionString));
            }
        }
    }

    public static void InitWithSingleMachineWorker(Action<DefaultUidGeneratorOptions> options)
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
                var defaultOptions = new DefaultUidGeneratorOptions();
                options(defaultOptions);
                _uidGenerator = new DefaultUidGenerator(defaultOptions,UidGeneratorBaseHelper.BuildWorkerIdAssignerWithSingleMachine());
            }
        }
    }


    /// <summary>
    /// get uid
    /// </summary>
    /// <remarks>Call the method <see cref="DefaultUidGeneratorHelper.Init"/> before using</remarks>
    /// <returns></returns>
    public static long GetUid()
    {
        return _uidGenerator.GetUid();
    }
}
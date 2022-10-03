using System;
using Vincent.UidGenerator.Core;
using Vincent.UidGenerator.Worker;

namespace Vincent.UidGenerator.Helper;

public static class DefaultUidGeneratorHelper
{
    private static DefaultUidGenerator _uidGenerator;

    private  static object _lock = new object();
    
    public static void Init(DefaultUidGeneratorOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        if (_uidGenerator != null) return;
        
        lock (_lock)
        {
            _uidGenerator ??= new DefaultUidGenerator(options);
        }
    }
    
    public static void Init(AssignWorkIdScheme assignWorkIdScheme,string connectionString,DefaultUidGeneratorOptions options)
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
                var workerId= WorkerIdAssigner.AssignWorkerId(connectionString,assignWorkIdScheme);
                options.WorkerId = workerId;
                _uidGenerator = new DefaultUidGenerator(options);
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
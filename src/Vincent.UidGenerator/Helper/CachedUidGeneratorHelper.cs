using System;
using Vincent.UidGenerator.Core;
using Vincent.UidGenerator.Worker;

namespace Vincent.UidGenerator.Helper;

public class CachedUidGeneratorHelper
{
    private static CachedUidGenerator _uidGenerator;

    private  static object _lock = new object();
    
    public static void Init(AssignWorkIdScheme assignWorkIdScheme,string connectionString,CachedUidGeneratorOptions options)
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
                _uidGenerator = new CachedUidGenerator(options);
            }
        }
    }

    /// <summary>
    /// get uid
    /// </summary>
    /// <remarks>Call the method <see cref="CachedUidGeneratorHelper.Init"/> before using</remarks>
    /// <returns></returns>
    public static long GetUid()
    {
        return _uidGenerator.GetUid();
    }
}
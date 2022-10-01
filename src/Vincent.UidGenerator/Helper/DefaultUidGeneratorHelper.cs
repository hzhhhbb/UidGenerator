using System;
using Vincent.UidGenerator.Core;

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
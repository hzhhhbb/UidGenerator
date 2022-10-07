using System;
using Vincent.UidGenerator;
using Vincent.UidGenerator.Core;
using Vincent.UidGenerator.Worker;
using Vincent.UidGenerator.Worker.Repository;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class UidGeneratorServiceCollectionExtensions
{
    /// <summary>
    /// Registers services required by DefaultUidGenerator services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options"> Configure generator options </param>
    /// <returns>A <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddDefaultUidGenerator(this IServiceCollection services,
        Action<DefaultUidGeneratorOptions> options)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        services.Configure(options);

        services.AddSingleton<IUidGenerator, DefaultUidGenerator>();

        return services;
    }

   
    public static IServiceCollection AddSQLServerWorker(this IServiceCollection services ,string connectionString)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        services.Configure<WorkerOptions>(options => options.ConnectionString = connectionString);
        services.AddTransient<IWorkerNodeRepository, WorkerNodeSqlServerRepository>();
        services.AddWorkerIdAssignerServices();

        return services;
    }
    
    public static IServiceCollection AddMySQLWorker(this IServiceCollection services ,string connectionString)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        services.Configure<WorkerOptions>(options => options.ConnectionString = connectionString);
        services.AddTransient<IWorkerNodeRepository, WorkerNodeMySqlRepository>();
        services.AddWorkerIdAssignerServices();

        return services;
    }
    
    public static IServiceCollection AddSingleMachineWorker(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        
        services.AddTransient<IWorkerNodeRepository, WorkerNodeSingleMachineRepository>();
        services.AddWorkerIdAssignerServices();

        return services;
    }

    
    /// <summary>
    /// Registers services required by DefaultUidGenerator services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options"> Configure generator options </param>
    /// <returns>A <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCachedUidGenerator(this IServiceCollection services,
        Action<CachedUidGeneratorOptions> options)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        services.Configure(options);
        
        services.AddSingleton<IUidGenerator,CachedUidGenerator>();

        return services;
    }

    private static IServiceCollection AddWorkerIdAssignerServices(this IServiceCollection services)
    {
        services.AddTransient<IWorkerIdAssigner, WorkerIdAssigner>();
        
        return services;
    }
    
    
}
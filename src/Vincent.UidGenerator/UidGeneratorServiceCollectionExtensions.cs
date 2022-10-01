using System;
using Microsoft.Extensions.DependencyInjection;
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
    /// <param name="options"> option action </param>
    /// <returns>A <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddDefaultUidGeneratorService(this IServiceCollection services,
        Action<DefaultUidGeneratorOptions> options = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var defaultOption = new DefaultUidGeneratorOptions();
        if (options != null)
        {
            options(defaultOption);
        }

        services.AddSingleton<IUidGenerator>(new DefaultUidGenerator(defaultOption));

        return services;
    }

    /// <summary>
    /// Registers services required by CachedUidGenerator services with MySQL
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="connectionString">DB connection string, depends on <see cref="AssignWorkIdScheme"/></param>
    /// <param name="options"></param>
    /// <param name="assignWorkIdScheme">SQLService or MySQL for now</param>
    /// <returns>A <see cref="IServiceCollection"/>return <see cref="services"/></returns>
    public static IServiceCollection AddCachedUidGeneratorService(this IServiceCollection services,
        AssignWorkIdScheme assignWorkIdScheme,
        string connectionString,
        Action<CachedUidGeneratorOptions> options = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        var defaultOptions = new CachedUidGeneratorOptions();
        if (options != null)
        {
            options(defaultOptions);
        }

        var workerId = WorkerIdAssigner.AssignWorkerId(connectionString, assignWorkIdScheme);
        defaultOptions.WorkerId = workerId;

        services.AddSingleton<IUidGenerator>(new CachedUidGenerator(defaultOptions));

        return services;
    }
}
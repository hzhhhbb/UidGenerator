using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vincent.UidGenerator.Worker;

namespace Vincent.UidGenerator.Tests;

public class UidGeneratorServiceCollectionExtensionsTests
{
    [Test]
    public void ShouldBeSingletonServiceOfDefaultUidGenerator()
    {
        var services = new ServiceCollection();
        services.AddDefaultUidGeneratorService();
        var defaultUidGeneratorService =
            services.FirstOrDefault(service => service.ServiceType == typeof(IUidGenerator));

        defaultUidGeneratorService.ShouldNotBeNull();
        defaultUidGeneratorService.Lifetime.ShouldBeEquivalentTo(ServiceLifetime.Singleton);
    }

    [Test]
    public void ShouldBeGetUidNormallyWithDefaultUidGenerator()
    {
        var services = new ServiceCollection();
        services.AddDefaultUidGeneratorService();

        var provider = services.BuildServiceProvider();
        var uidGenerator = provider.GetRequiredService<IUidGenerator>();
        uidGenerator.GetUid().ShouldBePositive();
    }

    [Test]
    public void ShouldBeThrowExceptionWhenDefaultServicesIsNull()
    {
        var services = (ServiceCollection) null;

        Should.Throw<ArgumentNullException>(() => services.AddDefaultUidGeneratorService());
    }

    [Test]
    public void ShouldBeThrowExceptionWhenCachedServicesIsNull1()
    {
        var services = (ServiceCollection) null;

        Should.Throw<ArgumentNullException>(() => services.AddCachedUidGeneratorService(AssignWorkIdScheme.MySql,"11"));
    }
    
    [Test]
    public void ShouldBeThrowExceptionWhenConnectionStringIsEmpty()
    {
        var services = new ServiceCollection();

        Should.Throw<ArgumentNullException>(() => services.AddCachedUidGeneratorService(AssignWorkIdScheme.MySql,""));
    }
    
}
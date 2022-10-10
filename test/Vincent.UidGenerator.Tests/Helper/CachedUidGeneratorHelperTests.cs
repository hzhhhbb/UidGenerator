using Shouldly;
using Vincent.UidGenerator.Helper;

namespace Vincent.UidGenerator.Tests.Helper;

public class CachedUidGeneratorHelperTests
{
    [Test]
    public void ShouldBeGetUidNormally()
    {
        CachedUidGeneratorHelper.InitWithSingleMachineWorker(options=>{});
        CachedUidGeneratorHelper.GetUid().ShouldBePositive();
    }
    
    [Test]
    public void ShouldBeThrowExceptionWhenOptionsIsNull()
    {
        Should.Throw<ArgumentException>(() => CachedUidGeneratorHelper.InitWithSingleMachineWorker(null));
    }
}
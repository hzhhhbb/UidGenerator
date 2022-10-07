using Shouldly;
using Vincent.UidGenerator.Helper;

namespace Vincent.UidGenerator.Tests.Helper;

public class DefaultUidGeneratorHelperTests
{
    [Test]
    public void ShouldBeGetUidNormally()
    {
        DefaultUidGeneratorHelper.InitWithSingleMachineWorker(options=>{});
        DefaultUidGeneratorHelper.GetUid().ShouldBePositive();
    }
    
    [Test]
    public void ShouldBeThrowExceptionWhenOptionsIsNull()
    {
        Should.Throw<ArgumentException>(() => DefaultUidGeneratorHelper.InitWithSingleMachineWorker(null));
    }
}
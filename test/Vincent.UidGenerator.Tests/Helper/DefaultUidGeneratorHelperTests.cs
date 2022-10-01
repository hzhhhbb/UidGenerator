using Shouldly;
using Vincent.UidGenerator.Helper;

namespace Vincent.UidGenerator.Tests.Helper;

public class DefaultUidGeneratorHelperTests
{
    [Test]
    public void ShouldBeGetUidNormally()
    {
        DefaultUidGeneratorHelper.Init(new DefaultUidGeneratorOptions());
        DefaultUidGeneratorHelper.GetUid().ShouldBePositive();
    }
    
    [Test]
    public void ShouldBeThrowExceptionWhenOptionsIsNull()
    {
        Should.Throw<ArgumentException>(() => DefaultUidGeneratorHelper.Init(null));
    }
}
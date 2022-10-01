using Shouldly;
using Vincent.UidGenerator.Core;

namespace Vincent.UidGenerator.Tests.Core;

public class BitsAllocatorTests
{
    [Test]
    public void ShouldBeThrowExceptionWhenBitLengthIsNotEqualsTo64()
    {
        Should.Throw<ArgumentException>(() => new BitsAllocator(1, 1, 1));
    }
    
    [Test]
    public void ShouldBeTAllocateBitCorrect()
    {
        var bitsAllocator = new BitsAllocator(29, 21, 13);
        bitsAllocator.Allocate(1993472,1041,0).ShouldBeEquivalentTo(34247588190494720L);
    }
}
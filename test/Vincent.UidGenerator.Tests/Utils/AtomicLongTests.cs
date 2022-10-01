using Shouldly;
using Vincent.UidGenerator.Utils;

namespace Vincent.UidGenerator.Tests.Utils;

public class AtomicLongTests
{
    [Test]
    public void ShouldBeExecNormallyWhenSerialized()
    {
        AtomicLong atomicLong = new AtomicLong(1);
        atomicLong.UpdateAndGet(old => old == 1L ? old : 2L);

        var expected = new AtomicLong(1L);
        atomicLong.Get().ShouldBeEquivalentTo(expected.Get());
    }
    
    [Test]
    public void ShouldBeExecNormallyWhenParallelized()
    {
        AtomicLong atomicLong = new AtomicLong(0);

        List<Task> tasks = new List<Task>();
        int taskSize = 1 * 1000;
        for (int i = 0; i < taskSize; i++)
        {
            tasks.Add(Task.Factory.StartNew(()=>atomicLong.UpdateAndGet(old => old+1)));
        }

        Task.WaitAll(tasks.ToArray());
        
        var expected = new AtomicLong(1000L);
        atomicLong.Get().ShouldBeEquivalentTo(expected.Get());
    }
}
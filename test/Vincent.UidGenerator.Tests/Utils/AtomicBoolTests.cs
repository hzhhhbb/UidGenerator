using Shouldly;
using Vincent.UidGenerator.Utils;

namespace Vincent.UidGenerator.Tests.Utils;

public class AtomicBoolTests
{
    [Test]
    public void ShouldBeSetValueCorrectWhenSerialized()
    {
        AtomicBool atomicLong = new AtomicBool(false);
        atomicLong.CompareAndSet(false,true);

        
        atomicLong.Get().ShouldBeTrue();
    }
    
    [Test]
    public void ShouldBeSetValueCorrectWhenParallelized()
    {
        AtomicBool atomicLong = new AtomicBool(false);

        List<Task> tasks = new List<Task>();
        int taskSize = 1 * 1000;
        for (int i = 0; i < taskSize; i++)
        {
            tasks.Add(Task.Factory.StartNew(()=>atomicLong.CompareAndSet(false,true)));
        }


        Task.WaitAll(tasks.ToArray());
        
        atomicLong.Get().ShouldBeTrue();
    }
}
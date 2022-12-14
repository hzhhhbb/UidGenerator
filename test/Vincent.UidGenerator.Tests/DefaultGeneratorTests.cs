using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vincent.UidGenerator.Core;
using Vincent.UidGenerator.Utils;

namespace Vincent.UidGenerator.Tests;

public class DefaultGeneratorTests
{
    private long SIZE;
    private IUidGenerator uidGenerator;
    private bool VERBOSE = true;
    private int THREADS = Environment.ProcessorCount << 1;

    [SetUp]
    public void Setup()
    {
        SIZE = 1 * 1000;
        var services = new ServiceCollection();
        services.AddDefaultUidGenerator(options => { }).AddSingleMachineWorker().AddLogging();
        using var servicesProvider = services.BuildServiceProvider();
        uidGenerator = servicesProvider.GetRequiredService<IUidGenerator>();
    }

    [Test]
    public void ShouldBeGetUidNormallyWhenSerialized()
    {
        // Generate UID serially
        ConcurrentDictionary<long, byte> uidSet = new ConcurrentDictionary<long, byte>();
        for (int i = 0; i < SIZE; i++)
        {
            DoGenerate(uidSet, i);
        }

        // Check UIDs are all unique
        CheckUniqueId(uidSet);
    }

    /// <summary>
    /// Test for parallel generate
    /// </summary>
    [Test]
    public void ShouldBeGetUidNormallyWhenParallelized()
    {
        AtomicLong control = new AtomicLong(-1L);
        var uidSet = new ConcurrentDictionary<long, byte>();

        // Initialize threads
        List<Task> tasks = new List<Task>(THREADS);

        for (int i = 0; i < THREADS; i++)
        {
            tasks.Add(Task.Factory.StartNew(() => workerRun(uidSet, control)));
        }

        // Wait for worker done
        Task.WaitAll(tasks.ToArray());

        // Check generate 700w times
        control.Get().ShouldBeEquivalentTo(SIZE);

        // Check UIDs are all unique
        CheckUniqueId(uidSet);
    }

    private void workerRun(ConcurrentDictionary<long, byte> uIds, AtomicLong control)
    {
        while (true)
        {
            long myPosition = control.UpdateAndGet(old => old == SIZE ? SIZE : old + 1);
            if (myPosition == SIZE)
            {
                return;
            }

            DoGenerate(uIds, myPosition);
        }
    }

    /// <summary>
    /// generate uid
    /// </summary>
    /// <param name="uidSet"></param>
    /// <param name="index"></param>
    private void DoGenerate(ConcurrentDictionary<long, byte> uidSet, long index)
    {
        long uid = uidGenerator.GetUid();
        bool notExisted = uidSet.TryAdd(uid, 1);
        string parsedInfo = uidGenerator.ParseUid(uid);
        if (!notExisted)
        {
            Console.WriteLine("Found duplicate UID " + uid);
        }

        // Check UID is positive, and can be parsed
        uid.ShouldBeGreaterThan(0L);
        parsedInfo.ShouldNotBeNullOrWhiteSpace(parsedInfo);

        if (VERBOSE)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " No." + index + " >>> " + parsedInfo);
        }
    }

    /// <summary>
    /// Check UIDs are all unique
    /// </summary>
    /// <param name="uidSet">UIDs</param>
    private void CheckUniqueId(ConcurrentDictionary<long, byte> uidSet)
    {
        Console.WriteLine(uidSet.Count);
        uidSet.Count.ShouldBeEquivalentTo((int) SIZE);
    }
}
// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NanoByte.Common.Native;

/// <summary>
/// Contains test methods for <see cref="AppMutex"/>.
/// </summary>
public class AppMutexTest
{
    public AppMutexTest()
    {
#if !NET
        Assert.SkipUnless(WindowsUtils.IsWindowsNT, reason: "AppMutexes are only available on the Windows NT platform when using the .NET Framework or Mono.");
#endif
    }

    private static string RandomName() => $"unit-tests-{Path.GetRandomFileName()}";

    /// <summary>
    /// Ensures the methods <see cref="AppMutex.Probe"/>, <see cref="AppMutex.Create"/> and <see cref="AppMutex.Dispose"/> work correctly together.
    /// </summary>
    [Fact]
    public void TestProbeCreateClose()
    {
        string mutexName = RandomName();
        AppMutex.Probe(mutexName).Should().BeFalse();
        var mutex = AppMutex.Create(mutexName);
        AppMutex.Probe(mutexName).Should().BeTrue();
        mutex.Dispose();
        AppMutex.Probe(mutexName).Should().BeFalse();
    }

    /// <summary>
    /// Ensures a mutex remains detectable until the last instance holding it is closed.
    /// </summary>
    [Fact]
    public void TestMultipleHolders()
    {
        string mutexName = RandomName();
        var mutex1 = AppMutex.Create(mutexName);
        var mutex2 = AppMutex.Create(mutexName);

        mutex1.Dispose();
        AppMutex.Probe(mutexName).Should().BeTrue();
        mutex2.Dispose();
        AppMutex.Probe(mutexName).Should().BeFalse();
    }

    /// <summary>
    /// Ensures a mutex is not released by the garbage collector if the caller does not keep a reference to it.
    /// </summary>
    [Fact]
    public void TestSurvivesGarbageCollection()
    {
        string mutexName = RandomName();
        CreateWithoutReference(mutexName);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        AppMutex.Probe(mutexName).Should().BeTrue();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateWithoutReference(string mutexName)
        => AppMutex.Create(mutexName);

    /// <summary>
    /// Ensures names do not need to be valid file names.
    /// </summary>
    [Fact]
    public void TestNameWithPathSeparators()
    {
        string mutexName = $"unit-tests/../{Path.GetRandomFileName()}/{new string('x', 500)}";
        using (AppMutex.Create(mutexName))
            AppMutex.Probe(mutexName).Should().BeTrue();
        AppMutex.Probe(mutexName).Should().BeFalse();
    }

    /// <summary>
    /// Ensures concurrent probes are not mistaken for each other.
    /// </summary>
    [Fact]
    public void TestConcurrentProbesOfUnheldMutex()
    {
        string mutexName = RandomName();
        using (AppMutex.Create(mutexName)) {} // Ensure lock file exists on Unixoid systems

        Parallel.For(0, 200, new ParallelOptions {MaxDegreeOfParallelism = 8}, _ =>
            AppMutex.Probe(mutexName).Should().BeFalse());
    }

    /// <summary>
    /// Ensures creating a mutex does not fail due to concurrent probes.
    /// </summary>
    [Fact]
    public void TestCreateDuringConcurrentProbes()
    {
        string mutexName = RandomName();
        using (AppMutex.Create(mutexName)) {} // Ensure lock file exists on Unixoid systems

        using var stop = new ManualResetEventSlim();
        var prober = new Thread(() =>
        {
            while (!stop.IsSet) AppMutex.Probe(mutexName);
        });
        prober.Start();

        try
        {
            for (int i = 0; i < 50; i++)
            {
                using (AppMutex.Create(mutexName))
                    AppMutex.Probe(mutexName).Should().BeTrue();
            }
        }
        finally
        {
            stop.Set();
            prober.Join();
        }
    }
}

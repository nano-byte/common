// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

public class DeferredProgressTest
{
    [Fact]
    public void ForwardsToTarget()
    {
        var deferred = new DeferredProgress<int>();

        var target = new Mock<IProgress<int>>(MockBehavior.Strict);
        deferred.SetTarget(target.Object);

        target.Setup(x => x.Report(1));
        target.Setup(x => x.Report(2));
        deferred.Report(1);
        deferred.Report(2);
        target.VerifyAll();
    }

    [Fact]
    public void ForwardsLastRecordedToTarget()
    {
        var deferred = new DeferredProgress<int>();
        deferred.Report(1);
        deferred.Report(2);

        var target = new Mock<IProgress<int>>(MockBehavior.Strict);
        target.Setup(x => x.Report(2));
        deferred.SetTarget(target.Object);
        target.VerifyAll();
    }
}

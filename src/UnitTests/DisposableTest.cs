// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Contains test methods for <see cref="Disposable"/>.
/// </summary>
public class DisposableTest
{
    [Fact]
    public void CallsCallbackOnDispose()
    {
        bool called = false;
        var disposable = new Disposable(() => called = true);
        
        called.Should().BeFalse();
        disposable.Dispose();
        called.Should().BeTrue();
    }

    [Fact]
    public void CallsCallbackOnlyOnce()
    {
        int callCount = 0;
        var disposable = new Disposable(() => callCount++);
        
        disposable.Dispose();
        disposable.Dispose();
        callCount.Should().Be(2);
    }

    [Fact]
    public void CanUseWithUsingStatement()
    {
        bool called = false;
        using (new Disposable(() => called = true))
        {
            called.Should().BeFalse();
        }
        called.Should().BeTrue();
    }
}

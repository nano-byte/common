// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Contains test methods for <see cref="Disposable"/>.
/// </summary>
public class DisposableTest
{
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

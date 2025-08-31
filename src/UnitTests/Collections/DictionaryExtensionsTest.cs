// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="DictionaryExtensions"/>.
/// </summary>
public class DictionaryExtensionsTest
{
    [Fact]
    public void TestGetValueOrDefaultNullable()
    {
        var dict = new Dictionary<string, string> {{"x", "a"}};
        dict.GetValueOrDefault("x").Should().Be("a");
        dict.GetValueOrDefault("y").Should().BeNull();
        dict.GetValueOrDefault("y", "default").Should().Be("default");
    }

    [Fact]
    public void TestGetValueOrDefaultNonNullable()
    {
        var dict = new Dictionary<string, int> {{"x", 1}};
        dict.GetValueOrDefault("x").Should().Be(1);
        dict.GetValueOrDefault("y").Should().Be(0);
        dict.GetValueOrDefault("y", 2).Should().Be(2);
    }

    [Fact]
    public void TestGetOrAdd()
    {
        var dict = new Dictionary<string, string>();
        dict.GetOrAdd("x", () => "a");
        dict.GetOrAdd("x", () => throw new Exception("Should not be reached."));

        dict.Should().Equal(new Dictionary<string, string> {["x"] = "a"});
    }

    [Fact]
    public async Task TestGetOrAddAsync()
    {
        var dict = new Dictionary<string, string>();
        await dict.GetOrAddAsync("x", () => Task.FromResult("a"));
        await dict.GetOrAddAsync("x", () => Task.FromResult("b"));

        dict.Should().Equal(new Dictionary<string, string> {["x"] = "a"});
    }

    private class Mock : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
    }

    [Fact]
    public async Task TestGetOrAddAsyncRace()
    {
        var mock1 = new Mock();
        var dict = new Dictionary<string, Mock> {["x"] = mock1};

        var mock2 = new Mock();
        var delayedSource = new TaskCompletionSource<Mock>();
        var task = dict.GetOrAddAsync("x", () => delayedSource.Task);
        delayedSource.SetResult(mock2);
        await task;

        dict.Should().Equal(new Dictionary<string, Mock> {["x"] = mock1});

        mock1.IsDisposed.Should().BeFalse();
        mock2.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void TestUnsequencedEquals()
    {
        new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.UnsequencedEquals(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}).Should().BeTrue();
        new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.UnsequencedEquals(new Dictionary<int, string> {[1] = "C", [2] = "B", [3] = "A"}).Should().BeFalse();
        new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.UnsequencedEquals(new Dictionary<int, string> {[1] = "X", [2] = "Y", [3] = "Z"}).Should().BeFalse();
        new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.UnsequencedEquals(new Dictionary<int, string> {[1] = "A", [2] = "B"}).Should().BeFalse();
        new Dictionary<int, object> {[1] = new()}.UnsequencedEquals(new Dictionary<int, object> {[1] = new()}).Should().BeFalse();
    }

    [Fact]
    public void TestGetUnsequencedHashCode()
    {
        new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode().Should().Be(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode());
        new Dictionary<int, string>().GetUnsequencedHashCode().Should().Be(new Dictionary<int, string>().GetUnsequencedHashCode());
        new Dictionary<int, string> {[1] = "X", [2] = "Y", [3] = "Z"}.GetUnsequencedHashCode().Should().NotBe(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode());
        new Dictionary<int, string> {[1] = "A", [2] = "B"}.GetUnsequencedHashCode().Should().NotBe(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode());
    }
}
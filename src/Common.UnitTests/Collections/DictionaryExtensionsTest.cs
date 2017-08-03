/*
 * Copyright 2006-2016 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="DictionaryExtensions"/>.
    /// </summary>
    public class DictionaryExtensionsTest
    {
        [Fact]
        public void TestGetOrDefault()
        {
            var dict = new Dictionary<string, string> {{"x", "a"}};
            dict.GetOrDefault("x").Should().Be("a");
            dict.GetOrDefault("y").Should().BeNull();
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
            new string[0].UnsequencedEquals(new string[0]).Should().BeTrue();
            new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.UnsequencedEquals(new Dictionary<int, string> {[1] = "C", [2] = "B", [3] = "A"}).Should().BeFalse();
            new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.UnsequencedEquals(new Dictionary<int, string> {[1] = "X", [2] = "Y", [3] = "Z"}).Should().BeFalse();
            new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.UnsequencedEquals(new Dictionary<int, string> {[1] = "A", [2] = "B"}).Should().BeFalse();
            new Dictionary<int, object> {[1] = new object()}.UnsequencedEquals(new Dictionary<int, object> {[1] = new object()}).Should().BeFalse();
        }

        [Fact]
        public void TestGetUnsequencedHashCode()
        {
            new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode().Should().Be(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode());
            new Dictionary<int, string>().GetUnsequencedHashCode().Should().Be(new Dictionary<int, string>().GetUnsequencedHashCode());
            new Dictionary<int, string> {[1] = "C", [2] = "B", [3] = "A"}.GetUnsequencedHashCode().Should().NotBe(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode());
            new Dictionary<int, string> {[1] = "X", [2] = "Y", [3] = "Z"}.GetUnsequencedHashCode().Should().NotBe(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode());
            new Dictionary<int, string> {[1] = "A", [2] = "B"}.GetUnsequencedHashCode().Should().NotBe(new Dictionary<int, string> {[1] = "A", [2] = "B", [3] = "C"}.GetUnsequencedHashCode());
        }
    }
}

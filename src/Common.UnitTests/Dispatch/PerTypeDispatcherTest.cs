/*
 * Copyright 2006-2015 Bastian Eicher
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

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Contains test methods for <see cref="PerTypeDispatcher{TBase,TResult}"/>.
    /// </summary>
    public class PerTypeDispatcherTest
    {
        private abstract class Base
        {}

        private class Sub1 : Base
        {}

        private class Sub2 : Base
        {}

        [Fact]
        public void TestDispatchAction()
        {
            var sub1Orig = new Sub1();
            Sub1 sub1Dispatched = null;
            var sub2Orig = new Sub2();
            Sub2 sub2Dispatched = null;

            var dispatcher = new PerTypeDispatcher<Base>(false)
            {
                (Sub1 sub1) => sub1Dispatched = sub1,
                (Sub2 sub2) => sub2Dispatched = sub2
            };

            dispatcher.Dispatch(sub1Orig);
            sub1Dispatched.Should().BeSameAs(sub1Orig);

            dispatcher.Dispatch(sub2Orig);
            sub2Dispatched.Should().BeSameAs(sub2Orig);
        }

        [Fact]
        public void TestDispatchActionExceptions()
        {
            new PerTypeDispatcher<Base>(false) {(Sub1 sub1) => {}}
                .Invoking(x => x.Dispatch(new Sub2()))
                .Should().Throw<KeyNotFoundException>();
            new PerTypeDispatcher<Base>(true) {(Sub1 sub1) => {}}
                .Invoking(x => x.Dispatch(new Sub2()))
                .Should().NotThrow<KeyNotFoundException>();
        }

        [Fact]
        public void TestDispatchFunc()
        {
            var sub1Orig = new Sub1();
            var sub2Orig = new Sub2();

            var dispatcher = new PerTypeDispatcher<Base, Base>(false)
            {
                (Sub1 sub1) => sub1,
                (Sub2 sub2) => sub2
            };

            dispatcher.Dispatch(sub1Orig).Should().BeSameAs(sub1Orig);
            dispatcher.Dispatch(sub2Orig).Should().BeSameAs(sub2Orig);
        }

        [Fact]
        public void TestDispatchFuncExceptions()
        {
            new PerTypeDispatcher<Base, Base>(false) {(Sub1 sub1) => sub1}
                .Invoking(x => x.Dispatch(new Sub2()))
                .Should().Throw<KeyNotFoundException>();
            new PerTypeDispatcher<Base, Base>(true) {(Sub1 sub1) => sub1}
                .Invoking(x => x.Dispatch(new Sub2()))
                .Should().NotThrow<KeyNotFoundException>();
        }
    }
}

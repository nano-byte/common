// Copyright Bastian Eicher
// Licensed under the MIT License

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
            Sub1? sub1Dispatched = null;
            var sub2Orig = new Sub2();
            Sub2? sub2Dispatched = null;

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
            new PerTypeDispatcher<Base>(false) {(Sub1 _) => {}}
               .Invoking(x => x.Dispatch(new Sub2()))
               .Should().Throw<KeyNotFoundException>();
            new PerTypeDispatcher<Base>(true) {(Sub1 _) => {}}
               .Invoking(x => x.Dispatch(new Sub2()))
               .Should().NotThrow<KeyNotFoundException>();
        }

        [Fact]
        public void TestDispatchFunc()
        {
            var sub1Orig = new Sub1();
            var sub2Orig = new Sub2();

            var dispatcher = new PerTypeDispatcher<Base, Base>
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
            new PerTypeDispatcher<Base, Base> {(Sub1 sub1) => sub1}
               .Invoking(x => x.Dispatch(new Sub2()))
               .Should().Throw<KeyNotFoundException>();
        }
    }
}

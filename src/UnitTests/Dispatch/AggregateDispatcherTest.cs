// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Dispatch;

/// <summary>
/// Contains test methods for <see cref="AggregateDispatcher{TBase,TResultElement}"/>.
/// </summary>
public class AggregateDispatcherTest
{
    private abstract class Base;

    private class Sub1 : Base;

    private class Sub2 : Sub1;

    [Fact]
    public void Aggregate()
    {
        var dispatcher = new AggregateDispatcher<Base, string>
        {
            (Sub1 _) => new[] {"sub1"},
            (Sub2 _) => new[] {"sub2"}
        };

        dispatcher.Dispatch(new Sub1()).Should().Equal("sub1");
        dispatcher.Dispatch(new Sub2()).Should().Equal("sub1", "sub2");
    }

    [Fact]
    public void UnknownType()
    {
        var dispatcher = new AggregateDispatcher<Base, string>();
        dispatcher.Dispatch(new Sub1()).Should().BeEmpty();
    }
}

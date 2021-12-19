// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Contains test methods for <see cref="ForEachTask{T}"/>.
    /// </summary>
    public class ForEachTaskTest
    {
        [Fact]
        public void TestCallback()
        {
            var target = new[] {"element1", "element2", "element2"};
            var calledFor = new List<string>();

            var task = new ForEachTask<string>("Test task", target, calledFor.Add);
            task.Run();

            calledFor.Should().Equal(target);
        }

        [Fact]
        public void TestExceptionPassing()
        {
            new ForEachTask<string>("Test task", new[] {""}, delegate { throw new IOException("Test exception"); })
               .Invoking(x => x.Run())
               .Should().Throw<IOException>();
            new ForEachTask<string>("Test task", new[] {""}, delegate { throw new WebException("Test exception"); })
               .Invoking(x => x.Run())
               .Should().Throw<WebException>();
        }
    }
}

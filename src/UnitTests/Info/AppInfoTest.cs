// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Info
{
    public class AppInfoTest
    {
        [Fact]
        public void GetsForCurrentLibrary()
        {
            AppInfo.CurrentLibrary.Should().BeEquivalentTo(new AppInfo
            {
                Name = "NanoByte.Common.UnitTests",
                ProductName = "NanoByte.Common.UnitTests",
                Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion?.GetLeftPartAtFirstOccurrence('+'),
                Description = "Unit test for NanoByte.Common.",
                Copyright = "Copyright Bastian Eicher et al."
            });
        }
    }
}

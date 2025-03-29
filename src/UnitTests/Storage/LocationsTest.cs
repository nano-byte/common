// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

public class LocationsTest
{
    [Fact]
    public void InstallBaseDoesNotEndWithSlash()
    {
        Locations.InstallBase.Should().NotEndWith(Path.DirectorySeparatorChar.ToString());
    }
}

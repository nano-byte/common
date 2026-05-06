// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

public class TemporaryFlagFileTest
{
    [Fact]
    public void TestDefault()
    {
        using var flag = new TemporaryFlagFile("unit-tests");

        flag.Set.Should().BeFalse();
        File.Exists(flag.Path).Should().BeFalse();
    }

    [Fact]
    public void TestSet()
    {
        using var flag = new TemporaryFlagFile("unit-tests");

        flag.Set = true;
        flag.Set.Should().BeTrue();
        File.Exists(flag.Path).Should().BeTrue();
    }

    [Fact]
    public void TestUnset()
    {
        using var flag = new TemporaryFlagFile("unit-tests");
        flag.Set = true;

        flag.Set = false;
        flag.Set.Should().BeFalse();
        File.Exists(flag.Path).Should().BeFalse();
    }
}

// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Values;

/// <summary>
/// Contains test methods for <see cref="EnumExtensions"/>.
/// </summary>
public class EnumExtensionsTest
{
    public enum TestEnum
    {
        None = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4,
        Combined = Flag1 | Flag2
    }

    [Flags]
    public enum TestFlagsEnum
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        ReadWrite = Read | Write
    }

    [Theory]
    [InlineData(TestFlagsEnum.ReadWrite, TestFlagsEnum.Read, true)]
    [InlineData(TestFlagsEnum.ReadWrite, TestFlagsEnum.Write, true)]
    [InlineData(TestFlagsEnum.ReadWrite, TestFlagsEnum.Execute, false)]
    [InlineData(TestFlagsEnum.Read, TestFlagsEnum.Write, false)]
    public void HasFlagEnum(TestFlagsEnum enumRef, TestFlagsEnum flag, bool expected)
        => enumRef.HasFlag(flag).Should().Be(expected);

    [Theory]
    [InlineData((ushort)3, (ushort)1, true)]
    [InlineData((ushort)3, (ushort)2, true)]
    [InlineData((ushort)3, (ushort)4, false)]
    [InlineData((ushort)1, (ushort)2, false)]
    public void HasFlagUshort(ushort enumRef, ushort flag, bool expected)
        => enumRef.HasFlag(flag).Should().Be(expected);

    [Theory]
    [InlineData(3, 1, true)]
    [InlineData(3, 2, true)]
    [InlineData(3, 4, false)]
    [InlineData(1, 2, false)]
    public void HasFlagInt(int enumRef, int flag, bool expected)
        => enumRef.HasFlag(flag).Should().Be(expected);

    private enum AttributeTestEnum
    {
        [Description("Value Description")]
        ValueWithAttribute,
        ValueWithoutAttribute
    }

    [Fact]
    public void GetEnumAttributePresent()
        => AttributeTestEnum.ValueWithAttribute.GetEnumAttribute<DescriptionAttribute>()
            ?.Description.Should().Be("Value Description");

    [Fact]
    public void GetEnumAttributeMissing()
        => AttributeTestEnum.ValueWithoutAttribute.GetEnumAttribute<DescriptionAttribute>()
            .Should().BeNull();
}

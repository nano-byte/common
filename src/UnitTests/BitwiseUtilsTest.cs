// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

public class BitwiseUtilsTest
{
    [Fact]
    public void TestXOr()
    {
        byte[] array1 = [0x01, 0x10];
        byte[] array2 = [0x10, 0x11];
        BitwiseUtils.XOr(array1, array2)
                 .Should().BeEquivalentTo([0x11, 0x01]);
    }
}

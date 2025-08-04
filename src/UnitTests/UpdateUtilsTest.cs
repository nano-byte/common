// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

public class UpdateUtilsTest
{
    [Fact]
    public void To_Function()
    {
        Assert.Equal("TEST", "test".To(x => x.ToUpper()));
    }

    [Fact]
    public void To_ValueChanged_UpdatesOriginalAndSetsFlag()
    {
        const int newValue = 42;
        int value = 0;
        bool updated = false;

        newValue.To(ref value, ref updated);

        Assert.True(updated);
        Assert.Equal(newValue, value);
    }

    [Fact]
    public void To_ValueChanged_UpdatesOriginalAndSetsFlag_Null()
    {
        string? newValue = null;
        string? value = "value";
        bool updated = false;

        newValue.To(ref value, ref updated);

        Assert.True(updated);
        Assert.Equal(newValue, value);
    }

    [Fact]
    public void To_ValueDidNotChange_DoesNotUpdateOriginalOrFlag()
    {
        const int newValue = 42;
        int value = 42;
        bool updated = false;

        newValue.To(ref value, ref updated);

        Assert.False(updated);
        Assert.Equal(42, value);
    }

    [Fact]
    public void To_TwoFlags_BothFlagsSetIfValueChanged()
    {
        const int newValue = 42;
        int value = 0;
        bool updated1 = false, updated2 = false;

        newValue.To(ref value, ref updated1, ref updated2);

        Assert.True(updated1);
        Assert.True(updated2);
        Assert.Equal(newValue, value);
    }

    [Fact]
    public void To_TwoFlags_NoFlagsSetIfValueDidNotChange()
    {
        const int newValue = 42;
        int value = 42;
        bool updated1 = false, updated2 = false;

        newValue.To(ref value, ref updated1, ref updated2);

        Assert.False(updated1);
        Assert.False(updated2);
        Assert.Equal(42, value);
    }

    [Fact]
    public void To_ValueChanged_ExecutesCallback()
    {
        const int newValue = 42;
        int value = 0;
        bool callbackExecuted = false;

        newValue.To(ref value, () => callbackExecuted = true);

        Assert.True(callbackExecuted);
        Assert.Equal(newValue, value);
    }

    [Fact]
    public void To_ValueChanged_ExecutesCallback_Null()
    {
        string? newValue = null;
        string? value = "value";
        bool callbackExecuted = false;

        newValue.To(ref value, () => callbackExecuted = true);

        Assert.True(callbackExecuted);
        Assert.Equal(newValue, value);
    }

    [Fact]
    public void Swap()
    {
        int value1 = 10, value2 = 20;

        UpdateUtils.Swap(ref value1, ref value2);

        Assert.Equal(20, value1);
        Assert.Equal(10, value2);
    }
}

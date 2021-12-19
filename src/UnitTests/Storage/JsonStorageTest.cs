// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Contains test methods for <see cref="JsonStorage"/>.
/// </summary>
public class JsonStorageTest
{
    /// <summary>
    /// A data-structure used to test serialization.
    /// </summary>
    public class TestData
    {
        public string? Data { get; set; }
    }

    /// <summary>
    /// Ensures <see cref="JsonStorage.SaveJson{T}(T,string)"/> and <see cref="JsonStorage.LoadJson{T}(string)"/> work correctly.
    /// </summary>
    [Fact]
    public void TestFile()
    {
        TestData testData1 = new() {Data = "Hello"}, testData2;
        using (var tempFile = new TemporaryFile("unit-tests"))
        {
            // Write and read file
            testData1.SaveJson(tempFile);
            testData2 = JsonStorage.LoadJson<TestData>(tempFile);
        }

        // Ensure data stayed the same
        testData2.Data.Should().Be(testData1.Data);
    }

    [Fact]
    public void TestToJsonString()
        => new TestData {Data = "Hello"}.ToJsonString().Should().Be("{\"Data\":\"Hello\"}");

    [Fact]
    public void TestFromJsonString()
        => JsonStorage.FromJsonString<TestData>("{\"Data\":\"Hello\"}").Data.Should().Be("Hello");
}
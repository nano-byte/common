// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Text;
using NanoByte.Common.Values.Design;

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

    public class DerivedData : TestData
    {
        public string? Extra { get; set; }
    }

    public class FieldData
    {
        public int Field;
        public string? Data { get; set; }
    }

    [TypeConverter(typeof(StringConstructorConverter<Name>))]
    public sealed record Name(string Value)
    {
        public override string ToString() => Value;
    }

    public class ConverterData
    {
        public Name? Single { get; set; }
        public Dictionary<Name, Name> Map { get; } = [];
        public List<string> Items { get; } = [];
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
        => new TestData {Data = "Hello"}.ToJsonString().Should().Be("""{"Data":"Hello"}""");

    [Fact]
    public void TestFromJsonString()
        => JsonStorage.FromJsonString<TestData>("""{"Data":"Hello"}""").Data.Should().Be("Hello");

    // The following tests ensure System.Text.Json (.NET) behaves like Newtonsoft.Json (.NET Framework)

    [Fact]
    public void TestFromJsonStringInvalid()
        => FluentActions.Invoking(() => JsonStorage.FromJsonString<TestData>("""{"Data":"""))
                        .Should().Throw<InvalidDataException>();

    [Fact]
    public void TestFromJsonStringNull()
        => FluentActions.Invoking(() => JsonStorage.FromJsonString<TestData>("null"))
                        .Should().Throw<InvalidDataException>();

    [Fact]
    public void TestFromJsonStringLenient()
    {
        var data = JsonStorage.FromJsonString<FieldData>("""
            {
              // Comment
              "data": "Hello",
              "Field": "42",
            }
            """);
        data.Data.Should().Be("Hello", because: "property names should be case-insensitive");
        data.Field.Should().Be(42, because: "numbers should be readable from strings");
    }

    [Fact]
    public void TestToJsonStringField()
        => new FieldData {Data = "Hello", Field = 42}.ToJsonString().Should().Contain("\"Field\":42").And.Contain("\"Data\":\"Hello\"");

    [Fact]
    public void TestToJsonStringUnescaped()
        => new TestData {Data = "Gr\u00F6\u00DFe <x>"}.ToJsonString().Should().Be("{\"Data\":\"Gr\u00F6\u00DFe <x>\"}");

    [Fact]
    public void TestToJsonStringRuntimeType()
    {
        TestData data = new DerivedData {Data = "Hello", Extra = "World"};
        data.ToJsonString().Should().Contain("\"Extra\":\"World\"").And.Contain("\"Data\":\"Hello\"");
    }

    [Fact]
    public void TestLoadJsonUtf16()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Encoding.Unicode, bufferSize: 1024, leaveOpen: true))
            writer.Write("""{"Data":"Hello"}""");
        JsonStorage.LoadJson<TestData>(stream).Data.Should().Be("Hello");
    }

    [Fact]
    public void TestFromJsonStringAnonymous()
        => JsonStorage.FromJsonString("""{"Data":"Hello","Count":3}""", new {Data = "", Count = 0})
                      .Should().Be(new {Data = "Hello", Count = 3});

    [Fact]
    public void TestToJsonStringTypeConverter()
    {
        var data = new ConverterData {Single = new("a"), Map = {[new("k")] = new("v")}, Items = {"x"}};
        data.ToJsonString().Should().Be("""{"Single":"a","Map":{"k":"v"},"Items":["x"]}""");
    }

    [Fact]
    public void TestFromJsonStringTypeConverter()
    {
        var data = JsonStorage.FromJsonString<ConverterData>("""{"Single":"a","Map":{"k":"v"},"Items":["x"]}""");
        data.Single.Should().Be(new Name("a"));
        data.Map.Should().Equal(new Dictionary<Name, Name> {[new("k")] = new("v")}, because: "get-only collections should be populated");
        data.Items.Should().Equal("x");
    }

    [Fact]
    public void TestReparseAsJson()
        => new DerivedData {Data = "Hello", Extra = "World"}.ReparseAsJson<TestData>().Data.Should().Be("Hello");

#if NET
    [Fact]
    public void TestFileTypeInfo()
    {
        TestData testData1 = new() {Data = "Hello"}, testData2;
        using (var tempFile = new TemporaryFile("unit-tests"))
        {
            // Write and read file
            testData1.SaveJson(tempFile, TestJsonContext.Default.TestData);
            testData2 = JsonStorage.LoadJson(tempFile, TestJsonContext.Default.TestData);
        }

        // Ensure data stayed the same
        testData2.Data.Should().Be(testData1.Data);
    }

    [Fact]
    public void TestToJsonStringTypeInfo()
        => new TestData {Data = "Hello"}.ToJsonString(TestJsonContext.Default.TestData).Should().Be("""{"Data":"Hello"}""");

    [Fact]
    public void TestFromJsonStringTypeInfo()
        => JsonStorage.FromJsonString("""{"Data":"Hello"}""", TestJsonContext.Default.TestData).Data.Should().Be("Hello");

    [Fact]
    public void TestFromJsonStringTypeInfoInvalid()
        => FluentActions.Invoking(() => JsonStorage.FromJsonString("""{"Data":""", TestJsonContext.Default.TestData))
                        .Should().Throw<InvalidDataException>();
#endif
}

#if NET
[System.Text.Json.Serialization.JsonSerializable(typeof(JsonStorageTest.TestData))]
internal partial class TestJsonContext : System.Text.Json.Serialization.JsonSerializerContext;
#endif

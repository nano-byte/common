// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Contains test methods for <see cref="XmlStorage"/>.
    /// </summary>
    [Collection("WorkingDir")]
    public class XmlStorageTest
    {
        /// <summary>
        /// A data-structure used to test serialization.
        /// </summary>
        [XmlNamespace("", "")]
        public class TestData
        {
            public string? Data { get; set; }
        }

        /// <summary>
        /// Ensures <see cref="XmlStorage.SaveXml{T}(T,string,string)"/> and <see cref="XmlStorage.LoadXml{T}(string)"/> work correctly.
        /// </summary>
        [Fact]
        public void TestFile()
        {
            TestData testData1 = new() {Data = "Hello"}, testData2;
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                // Write and read file
                testData1.SaveXml(tempFile);
                testData2 = XmlStorage.LoadXml<TestData>(tempFile);
            }

            // Ensure data stayed the same
            testData2.Data.Should().Be(testData1.Data);
        }

        /// <summary>
        /// Ensures <see cref="XmlStorage.SaveXml{T}(T,string,string)"/> and <see cref="XmlStorage.LoadXml{T}(string)"/> work correctly with relative paths.
        /// </summary>
        [Fact]
        public void TestFileRelative()
        {
            TestData testData1 = new() {Data = "Hello"}, testData2;
            using (new TemporaryWorkingDirectory("unit-tests"))
            {
                // Write and read file
                testData1.SaveXml("file.xml");
                testData2 = XmlStorage.LoadXml<TestData>("file.xml");
            }

            // Ensure data stayed the same
            testData2.Data.Should().Be(testData1.Data);
        }

        [Fact]
        public void TestToXmlString()
            => new TestData {Data = "Hello"}.ToXmlString().Should().Be("<?xml version=\"1.0\"?>\n<TestData>\n  <Data>Hello</Data>\n</TestData>\n");

        [Fact]
        public void TestFromXmlString()
            => XmlStorage.FromXmlString<TestData>("<?xml version=\"1.0\"?><TestData><Data>Hello</Data></TestData>").Data.Should().Be("Hello");
    }
}

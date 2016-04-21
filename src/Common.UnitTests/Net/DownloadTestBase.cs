using System.IO;
using NanoByte.Common.Streams;
using NUnit.Framework;

namespace NanoByte.Common.Net
{
    public abstract class DownloadTestBase
    {
        protected MicroServer Server;

        [SetUp]
        public virtual void SetUp()
        {
            Server = new MicroServer("file", GetTestFileStream());
        }

        [TearDown]
        public virtual void TearDown()
        {
            Server.Dispose();
        }

        protected static MemoryStream GetTestFileStream()
        {
            return "abc".ToStream();
        }
    }
}
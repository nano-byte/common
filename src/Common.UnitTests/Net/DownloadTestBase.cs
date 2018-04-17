// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Net
{
    public abstract class DownloadTestBase : IDisposable
    {
        protected readonly MicroServer Server = new MicroServer("file", GetTestFileStream());

        protected static MemoryStream GetTestFileStream() => "abc".ToStream();

        public virtual void Dispose() => Server.Dispose();
    }
}

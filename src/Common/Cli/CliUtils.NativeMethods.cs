// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace NanoByte.Common.Cli
{
    partial class CliUtils
    {
        [SuppressUnmanagedCodeSecurity]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static class SafeNativeMethods
        {
            public enum StdHandle
            {
                Stdin = -10,
                Stdout = -11,
                Stderr = -12
            }

            [DllImport("kernel32")]
            public static extern IntPtr GetStdHandle(StdHandle nStdHandle);

            public enum FileType
            {
                Unknown = 0,
                Disk = 1,
                Char = 2,
                Pipe = 3
            }

            [DllImport("kernel32")]
            public static extern FileType GetFileType(IntPtr hFile);
        }
    }
}

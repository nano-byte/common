/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
            };

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

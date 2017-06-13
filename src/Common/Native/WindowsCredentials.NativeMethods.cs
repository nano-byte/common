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
using System.Runtime.InteropServices;
using System.Text;

namespace NanoByte.Common.Native
{
    static partial class WindowsCredentials
    {
        private const int ErrorNoSuchLogonSession = 1312;

        private const int MaxUsernameLength = 512;
        private const int MaxPasswordLength = 256;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CredUIInfo
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            public IntPtr hbmBanner;
        }

        private static CredUIInfo CreateCredUIInfo(IntPtr owner, string title, string message)
        {
            var credUI = new CredUIInfo();
            credUI.cbSize = Marshal.SizeOf(credUI);
            credUI.hwndParent = owner;
            credUI.pszCaptionText = title;
            credUI.pszMessageText = message;
            return credUI;
        }

        private static class NativeMethods
        {
            [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CredEnumerate(string filter, int flag, out int count, out IntPtr credentials);

            [DllImport("credui.dll", CharSet = CharSet.Unicode)]
            public static extern int CredUIPromptForCredentials(ref CredUIInfo uiInfo, string targetName, IntPtr reserved1, int iError, StringBuilder userName, int maxUserName, StringBuilder password, int maxPassword, [MarshalAs(UnmanagedType.Bool)] ref bool pfSave, WindowsCredentialsFlags flags);

            [DllImport("credui.dll", CharSet = CharSet.Unicode)]
            public static extern int CredUICmdLinePromptForCredentials(string targetName, IntPtr reserved1, int iError, StringBuilder userName, int maxUserName, StringBuilder password, int maxPassword, [MarshalAs(UnmanagedType.Bool)] ref bool pfSave, WindowsCredentialsFlags flags);
        }
    }
}

// Copyright Bastian Eicher
// Licensed under the MIT License

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

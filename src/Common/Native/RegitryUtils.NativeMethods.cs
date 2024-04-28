// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace NanoByte.Common.Native;

partial class RegistryUtils
{
    private static class NativeMethods
    {
#if !NET20
        [DllImport("advapi32.dll", EntryPoint = "RegQueryInfoKey", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int RegQueryInfoKey(SafeRegistryHandle handle, IntPtr lpClass, IntPtr lpcbClass, IntPtr lpReserved, IntPtr lpcSubKeys, IntPtr lpcbMaxSubKeyLen, IntPtr lpcbMaxClassLen, IntPtr lpcValues, IntPtr lpcbMaxValueNameLen, IntPtr lpcbMaxValueLen, IntPtr lpcbSecurityDescriptor, out long lpftLastWriteTime);
#endif
    }
}

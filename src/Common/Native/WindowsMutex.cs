// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides Windows-specific API calls for cross-process Mutexes.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static partial class WindowsMutex
    {
        private const uint Synchronize = 0x00100000;

        /// <summary>
        /// Creates a new (or opens an existing) mutex.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <param name="alreadyExists"><c>true</c> if an existing mutex was opened; <c>false</c> if a new one was created.</param>
        /// <returns>The handle for the mutex. Can be used in <see cref="Close"/>. Will automatically be released once the process terminates.</returns>
        /// <exception cref="Win32Exception">The native subsystem reported a problem.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        public static IntPtr Create([Localizable(false)] string name, out bool alreadyExists)
        {
            if (!WindowsUtils.IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            // Create new or open existing mutex
            var handle = NativeMethods.CreateMutex(IntPtr.Zero, false, name ?? throw new ArgumentNullException(nameof(name)));

            int error = Marshal.GetLastWin32Error();
            switch (error)
            {
                case 0:
                    alreadyExists = false;
                    return handle;

                case WindowsUtils.Win32ErrorAlreadyExists:
                    alreadyExists = true;
                    return handle;

                default:
                    throw new Win32Exception(error);
            }
        }

        /// <summary>
        /// Checks whether a specific mutex exists without opening a lasting handle.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns><c>true</c> if an existing mutex was found; <c>false</c> if none existed.</returns>
        /// <exception cref="Win32Exception">The native subsystem reported a problem.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        public static bool Probe([Localizable(false)] string name)
        {
            if (!WindowsUtils.IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            // Try to open existing mutex
            var handle = NativeMethods.OpenMutex(Synchronize, false, name ?? throw new ArgumentNullException(nameof(name)));

            if (handle == IntPtr.Zero)
            {
                return Marshal.GetLastWin32Error() switch
                {
                    WindowsUtils.Win32ErrorFileNotFound => false, // No existing mutex found
                    { } code => throw new Win32Exception(code)
                };
            }

            // Existing mutex opened, close handle again
            NativeMethods.CloseHandle(handle);
            return true;
        }

        /// <summary>
        /// Closes an existing mutex handle. The mutex is destroyed if this is the last handle.
        /// </summary>
        /// <param name="handle">The mutex handle to be closed.</param>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        public static void Close(IntPtr handle)
        {
            if (!WindowsUtils.IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            NativeMethods.CloseHandle(handle);
        }
    }
}

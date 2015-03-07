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
using System.IO;
using System.Security;
using JetBrains.Annotations;
using Microsoft.Win32;
using NanoByte.Common.Native;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides utility and extension methods for <see cref="Registry"/> access.
    /// </summary>
    public static class RegistryUtils
    {
        #region DWORD
        /// <summary>
        /// Reads a DWORD value from the registry.
        /// </summary>
        /// <param name="keyName">The full path of the key to read from.</param>
        /// <param name="valueName">The name of the value to read.</param>
        /// <param name="defaultValue">The default value to return if the key or value does not exist.</param>
        public static int GetDword([NotNull] string keyName, [CanBeNull] string valueName, int defaultValue = 0)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            try
            {
                return Registry.GetValue(keyName, valueName, defaultValue) as int? ?? defaultValue;
            }
                #region Error handling
            catch (SecurityException ex)
            {
                Log.Warn(ex);
                return defaultValue;
            }
            #endregion
        }

        /// <summary>
        /// Sets a DWORD value in the registry.
        /// </summary>
        /// <param name="keyName">The full path of the key to write to.</param>
        /// <param name="valueName">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        public static void SetDword([NotNull] string keyName, [CanBeNull] string valueName, int value)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            try
            {
                Registry.SetValue(keyName, valueName, value, RegistryValueKind.DWord);
            }
                #region Error handling
            catch (SecurityException ex)
            {
                // Wrap exception since only certain exception types are allowed
                throw new UnauthorizedAccessException(ex.Message, ex);
            }
            #endregion
        }
        #endregion

        #region String
        /// <summary>
        /// Reads a string value from the registry.
        /// </summary>
        /// <param name="keyName">The full path of the key to read from.</param>
        /// <param name="valueName">The name of the value to read.</param>
        /// <param name="defaultValue">The default value to return if the key or value does not exist.</param>
        [ContractAnnotation("defaultValue:notnull => notnull")]
        public static string GetString([NotNull] string keyName, [CanBeNull] string valueName, [CanBeNull] string defaultValue = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            try
            {
                return Registry.GetValue(keyName, valueName, defaultValue) as string ?? defaultValue;
            }
                #region Error handling
            catch (SecurityException ex)
            {
                Log.Warn(ex);
                return defaultValue;
            }
            #endregion
        }

        /// <summary>
        /// Sets a string value in the registry.
        /// </summary>
        /// <param name="keyName">The full path of the key to write to.</param>
        /// <param name="valueName">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        public static void SetString([NotNull] string keyName, [CanBeNull] string valueName, [NotNull] string value)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            try
            {
                Registry.SetValue(keyName, valueName, value, RegistryValueKind.String);
            }
                #region Error handling
            catch (SecurityException ex)
            {
                // Wrap exception since only certain exception types are allowed
                throw new UnauthorizedAccessException(ex.Message, ex);
            }
            #endregion
        }
        #endregion

        #region Software
        private const string HklmSoftwareKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\";
        private const string HklmWowSoftwareKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\";
        private const string HkcuSoftwareKey = @"HKEY_CURRENT_USER\SOFTWARE\";

        /// <summary>
        /// Reads a string value from one of the SOFTWARE keys in the registry. Checks HKLM\SOFTWARE, HKLM\SOFTWARE\Wow6432Node and HKCU\SOFTWARE.
        /// </summary>
        /// <param name="subkeyName">The path of the key relative to the SOFTWARE key.</param>
        /// <param name="valueName">The name of the value to read.</param>
        /// <param name="defaultValue">The default value to return if the key or value does not exist.</param>
        [ContractAnnotation("defaultValue:notnull => notnull")]
        public static string GetSoftwareString([NotNull] string subkeyName, [CanBeNull] string valueName, [CanBeNull] string defaultValue = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            return
                GetString(HklmSoftwareKey + subkeyName, valueName,
                    GetString(HklmWowSoftwareKey + subkeyName, valueName,
                        GetString(HkcuSoftwareKey + subkeyName, valueName, defaultValue)));
        }

        /// <summary>
        /// Sets a string value in one or more of the SOFTWARE keys in the registry. Checks HKLM\SOFTWARE, HKLM\SOFTWARE\Wow6432Node and HKCU\SOFTWARE.
        /// </summary>
        /// <remarks>Will only write to HKLM if <see cref="WindowsUtils.IsAdministrator"/>. Will only write to HKCU if there is no corresponding HKLM entry yet.</remarks>
        /// <param name="subkeyName">The path of the key relative to the SOFTWARE key.</param>
        /// <param name="valueName">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        public static void SetSoftwareString([NotNull] string subkeyName, [CanBeNull] string valueName, [NotNull] string value)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            if (WindowsUtils.IsAdministrator)
            {
                SetString(HklmSoftwareKey + subkeyName, valueName, value);
                if (WindowsUtils.Is64BitProcess) SetString(HklmWowSoftwareKey + subkeyName, valueName, value);
            }

            if (GetString(HklmSoftwareKey + subkeyName, valueName) != value)
                SetString(HkcuSoftwareKey + subkeyName, valueName, value);
        }
        #endregion

        #region Enumeration
        /// <summary>
        /// Retrieves the names of all values within a specific subkey of a registry root.
        /// </summary>
        /// <param name="key">The root key to look within.</param>
        /// <param name="subkeyName">The path of the subkey below <paramref name="key"/>.</param>
        /// <returns>A list of value names; an empty array if the key does not exist.</returns>
        [NotNull, ItemNotNull]
        public static string[] GetValueNames([NotNull] this RegistryKey key, [NotNull] string subkeyName)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            try
            {
                using (var subkey = key.OpenSubKey(subkeyName))
                    return (subkey == null) ? new string[0] : subkey.GetValueNames();
            }
                #region Error handling
            catch (SecurityException ex)
            {
                Log.Warn(ex);
                return new string[0];
            }
            #endregion
        }

        /// <summary>
        /// Retrieves the names of all subkeys within a specific subkey of a registry root.
        /// </summary>
        /// <param name="key">The root key to look within.</param>
        /// <param name="subkeyName">The path of the subkey below <paramref name="key"/>.</param>
        /// <returns>A list of key names; an empty array if the key does not exist.</returns>
        [NotNull, ItemNotNull]
        public static string[] GetSubKeyNames([NotNull] RegistryKey key, [NotNull] string subkeyName)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            try
            {
                using (var subkey = key.OpenSubKey(subkeyName))
                    return (subkey == null) ? new string[0] : subkey.GetSubKeyNames();
            }
                #region Error handling
            catch (SecurityException ex)
            {
                Log.Warn(ex);
                return new string[0];
            }
            #endregion
        }
        #endregion

        #region Keys
        /// <summary>
        /// Like <see cref="RegistryKey.OpenSubKey(string,bool)"/> but with no <see langword="null"/> return values.
        /// </summary>
        /// <param name="key">The key to open a subkey in.</param>
        /// <param name="subkeyName">The name of the subkey to open.</param>
        /// <param name="writable"><see langword="true"/> for write-access to the key.</param>
        /// <returns>The newly created subkey.</returns>
        /// <exception cref="IOException">Failed to open the key.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the key is not permitted.</exception>
        [NotNull]
        public static RegistryKey OpenSubKeyChecked([NotNull] this RegistryKey key, [NotNull] string subkeyName, bool writable = false)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            try
            {
                var result = key.OpenSubKey(subkeyName, writable);
                if (result == null) throw new IOException(string.Format("Failed to open subkey '{1}' in '{0}'.", key, subkeyName));
                return result;
            }
                #region Error handling
            catch (SecurityException ex)
            {
                // Wrap exception since only certain exception types are allowed
                throw new UnauthorizedAccessException(ex.Message, ex);
            }
            #endregion
        }

        /// <summary>
        /// Like <see cref="RegistryKey.CreateSubKey(string)"/> but with no <see langword="null"/> return values.
        /// </summary>
        /// <param name="key">The key to create a subkey in.</param>
        /// <param name="subkeyName">The name of the subkey to create.</param>
        /// <returns>The newly created subkey.</returns>
        /// <exception cref="IOException">Failed to create the key.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        [NotNull]
        public static RegistryKey CreateSubKeyChecked([NotNull] this RegistryKey key, [NotNull] string subkeyName)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            try
            {
                var result = key.CreateSubKey(subkeyName);
                if (result == null) throw new IOException(string.Format("Failed to create subkey '{1}' in '{0}'.", key, subkeyName));
                return result;
            }
                #region Error handling
            catch (SecurityException ex)
            {
                // Wrap exception since only certain exception types are allowed
                throw new UnauthorizedAccessException(ex.Message, ex);
            }
            #endregion
        }

        /// <summary>
        /// Opens a HKEY_LOCAL_MACHINE key in the registry for reading, first trying to find the 64-bit version of it, then falling back to the 32-bit version.
        /// </summary>
        /// <param name="subkeyName">The path to the key below HKEY_LOCAL_MACHINE.</param>
        /// <param name="x64">Indicates whether a 64-bit key was opened.</param>
        /// <returns>The opened registry key or <see langword="null"/> if it could not found.</returns>
        /// <exception cref="IOException">Failed to open the key.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the key is not permitted.</exception>
        [NotNull]
        public static RegistryKey OpenHklmKey([NotNull] string subkeyName, out bool x64)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            if (WindowsUtils.Is64BitProcess)
            {
                var result = Registry.LocalMachine.OpenSubKey(subkeyName);
                if (result != null)
                {
                    x64 = true;
                    return result;
                }
                else
                {
                    x64 = false;
                    return OpenSubKeyChecked(Registry.LocalMachine, @"WOW6432Node\" + subkeyName);
                }
            }
            else
            {
                x64 = false;
                return OpenSubKeyChecked(Registry.LocalMachine, subkeyName);
            }
        }
        #endregion
    }
}

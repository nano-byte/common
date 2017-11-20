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
using System.ComponentModel;
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
        /// <exception cref="IOException">Registry access failed.</exception>
        [Pure]
        public static int GetDword([NotNull, Localizable(false)] string keyName, [CanBeNull, Localizable(false)] string valueName, int defaultValue = 0)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException(nameof(keyName));
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
        /// <exception cref="IOException">Registry access failed.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        public static void SetDword([NotNull, Localizable(false)] string keyName, [CanBeNull, Localizable(false)] string valueName, int value)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException(nameof(keyName));
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
        /// <exception cref="IOException">Registry access failed.</exception>
        [Pure, ContractAnnotation("defaultValue:notnull => notnull")]
        public static string GetString([NotNull, Localizable(false)] string keyName, [CanBeNull, Localizable(false)] string valueName, [CanBeNull, Localizable(false)] string defaultValue = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException(nameof(keyName));
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
        /// <exception cref="IOException">Registry access failed.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        public static void SetString([NotNull, Localizable(false)] string keyName, [CanBeNull, Localizable(false)] string valueName, [NotNull, Localizable(false)] string value)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException(nameof(keyName));
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
        /// Reads a string value from one of the SOFTWARE keys in the registry.
        /// </summary>
        /// <remarks>Checks HKLM/SOFTWARE, HKLM/SOFTWARE/Wow6432Node and HKCU/SOFTWARE in that order.</remarks>
        /// <param name="subkeyName">The path of the key relative to the SOFTWARE key.</param>
        /// <param name="valueName">The name of the value to read.</param>
        /// <param name="defaultValue">The default value to return if the key or value does not exist.</param>
        /// <exception cref="IOException">Registry access failed.</exception>
        [Pure, ContractAnnotation("defaultValue:notnull => notnull")]
        public static string GetSoftwareString([NotNull, Localizable(false)] string subkeyName, [CanBeNull, Localizable(false)] string valueName, [CanBeNull, Localizable(false)] string defaultValue = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
            #endregion

            return
                GetString(HklmSoftwareKey + subkeyName, valueName,
                    GetString(HklmWowSoftwareKey + subkeyName, valueName,
                        GetString(HkcuSoftwareKey + subkeyName, valueName, defaultValue)));
        }

        /// <summary>
        /// Reads a string value from one of the SOFTWARE keys in the registry.
        /// </summary>
        /// <param name="subkeyName">The path of the key relative to the SOFTWARE key.</param>
        /// <param name="valueName">The name of the value to read.</param>
        /// <param name="machineWide"><c>true</c> to read from HKLM/SOFTWARE (and HKLM/SOFTWARE/Wow6432Node if <see cref="OSUtils.Is64BitProcess"/>); <c>false</c> to read from HCKU/SOFTWARE.</param>
        /// <exception cref="IOException">Registry access failed.</exception>
        [Pure, CanBeNull]
        public static string GetSoftwareString([NotNull, Localizable(false)] string subkeyName, [CanBeNull, Localizable(false)] string valueName, bool machineWide)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
            #endregion

            return machineWide
                ? GetString(HklmSoftwareKey + subkeyName, valueName,
                    GetString(HklmWowSoftwareKey + subkeyName, valueName))
                : GetString(HkcuSoftwareKey + subkeyName, valueName);
        }

        /// <summary>
        /// Sets a string value in one or more of the SOFTWARE keys in the registry.
        /// </summary>
        /// <param name="subkeyName">The path of the key relative to the SOFTWARE key.</param>
        /// <param name="valueName">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="machineWide"><c>true</c> to write to HKLM/SOFTWARE (and HKLM/SOFTWARE/Wow6432Node if <see cref="OSUtils.Is64BitProcess"/>); <c>false</c> to write to HCKU/SOFTWARE.</param>
        /// <exception cref="IOException">Registry access failed.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        public static void SetSoftwareString([NotNull, Localizable(false)] string subkeyName, [CanBeNull, Localizable(false)] string valueName, [NotNull, Localizable(false)] string value, bool machineWide = false)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
            #endregion

            if (machineWide)
            {
                SetString(HklmSoftwareKey + subkeyName, valueName, value);
                if (OSUtils.Is64BitProcess) SetString(HklmWowSoftwareKey + subkeyName, valueName, value);
            }
            else SetString(HkcuSoftwareKey + subkeyName, valueName, value);
        }

        /// <summary>
        /// Deletes a value from one of the SOFTWARE keys in the registry.
        /// </summary>
        /// <remarks>Does not throw an exception for missing keys or values.</remarks>
        /// <param name="subkeyName">The path of the key relative to the SOFTWARE key.</param>
        /// <param name="valueName">The name of the value to delete.</param>
        /// <param name="machineWide"><c>true</c> to delete from HKLM/SOFTWARE (and HKLM/SOFTWARE/Wow6432Node if <see cref="OSUtils.Is64BitProcess"/>); <c>false</c> to delete from HCKU/SOFTWARE.</param>
        /// <exception cref="IOException">Registry access failed.</exception>
        public static void DeleteSoftwareValue([NotNull, Localizable(false)] string subkeyName, [NotNull, Localizable(false)] string valueName, bool machineWide)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
            if (string.IsNullOrEmpty(valueName)) throw new ArgumentNullException(nameof(valueName));
            #endregion

            if (machineWide)
            {
                DeleteValue(Registry.LocalMachine, @"SOFTWARE\" + subkeyName, valueName);
                if (OSUtils.Is64BitProcess) DeleteValue(Registry.LocalMachine, @"SOFTWARE\Wow6432Node\" + subkeyName, valueName);
            }
            else
                DeleteValue(Registry.CurrentUser, @"SOFTWARE\" + subkeyName, valueName);
        }

        private static void DeleteValue(RegistryKey root, string subkeyName, string valueName)
        {
            var key = root.OpenSubKey(subkeyName, writable: true);
            key?.DeleteValue(valueName, throwOnMissingValue: false);
        }
        #endregion

        #region Enumeration
        /// <summary>
        /// Retrieves the names of all values within a specific subkey of a registry root.
        /// </summary>
        /// <param name="key">The root key to look within.</param>
        /// <param name="subkeyName">The path of the subkey below <paramref name="key"/>.</param>
        /// <returns>A list of value names; an empty array if the key does not exist.</returns>
        /// <exception cref="IOException">Registry access failed.</exception>
        [Pure, NotNull, ItemNotNull]
        public static string[] GetValueNames([NotNull, Localizable(false)] this RegistryKey key, [NotNull, Localizable(false)] string subkeyName)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
            #endregion

            try
            {
                using (var subkey = key.OpenSubKey(subkeyName))
                    return subkey?.GetValueNames() ?? new string[0];
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
        /// <exception cref="IOException">Registry access failed.</exception>
        [Pure, NotNull, ItemNotNull]
        public static string[] GetSubKeyNames([NotNull, Localizable(false)] RegistryKey key, [NotNull, Localizable(false)] string subkeyName)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
            #endregion

            try
            {
                using (var subkey = key.OpenSubKey(subkeyName))
                    return subkey?.GetSubKeyNames() ?? new string[0];
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
        /// Like <see cref="RegistryKey.OpenSubKey(string,bool)"/> but with no <c>null</c> return values.
        /// </summary>
        /// <param name="key">The key to open a subkey in.</param>
        /// <param name="subkeyName">The name of the subkey to open.</param>
        /// <param name="writable"><c>true</c> for write-access to the key.</param>
        /// <returns>The newly created subkey.</returns>
        /// <exception cref="IOException">Failed to open the key.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the key is not permitted.</exception>
        [NotNull]
        public static RegistryKey OpenSubKeyChecked([NotNull, Localizable(false)] this RegistryKey key, [NotNull, Localizable(false)] string subkeyName, bool writable = false)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
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
        /// Like <see cref="RegistryKey.CreateSubKey(string)"/> but with no <c>null</c> return values.
        /// </summary>
        /// <param name="key">The key to create a subkey in.</param>
        /// <param name="subkeyName">The name of the subkey to create.</param>
        /// <returns>The newly created subkey.</returns>
        /// <exception cref="IOException">Failed to create the key.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the key is not permitted.</exception>
        [NotNull]
        public static RegistryKey CreateSubKeyChecked([NotNull, Localizable(false)] this RegistryKey key, [NotNull, Localizable(false)] string subkeyName)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
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
        /// <returns>The opened registry key or <c>null</c> if it could not found.</returns>
        /// <exception cref="IOException">Failed to open the key.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the key is not permitted.</exception>
        [NotNull]
        public static RegistryKey OpenHklmKey([NotNull, Localizable(false)] string subkeyName, out bool x64)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException(nameof(subkeyName));
            #endregion

            if (OSUtils.Is64BitProcess)
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

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        [ContractAnnotation("defaultValue:notnull => notnull")]
        public static string GetString([NotNull] string keyName, [CanBeNull] string valueName, [CanBeNull] string defaultValue = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            return Registry.GetValue(keyName, valueName, defaultValue) as string ?? defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        public static void SetString([NotNull] string keyName, [CanBeNull] string valueName, [NotNull] string value)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            Registry.SetValue(keyName, valueName, value, RegistryValueKind.String);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetDword([NotNull] string keyName, [CanBeNull] string valueName, int defaultValue = 0)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            return Registry.GetValue(keyName, valueName, defaultValue) as int? ?? defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        public static void SetDword([NotNull] string keyName, [CanBeNull] string valueName, int value)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            #endregion

            Registry.SetValue(keyName, valueName, value, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Opens a HKEY_LOCAL_MACHINE key in the registry for reading, first trying to find the 64-bit version of it, then falling back to the 32-bit version.
        /// </summary>
        /// <param name="subkeyName">The path to the key below HKEY_LOCAL_MACHINE.</param>
        /// <param name="x64">Indicates whether a 64-bit key was opened.</param>
        /// <returns>The opened registry key or <see langword="null"/> if it could not found.</returns>
        /// <exception cref="IOException">There was an error accessing the registry or the <paramref name="subkeyName"/> was not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the registry was not permitted.</exception>
        /// <exception cref="SecurityException">Access to the registry was not permitted.</exception>
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
                    return Registry.LocalMachine.OpenSubKeyChecked(@"WOW6432Node\" + subkeyName);
                }
            }
            else
            {
                x64 = false;
                return Registry.LocalMachine.OpenSubKeyChecked(subkeyName);
            }
        }

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

            using (var subkey = key.OpenSubKey(subkeyName))
                return subkey == null ? new string[0] : subkey.GetValueNames();
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

            using (var subkey = key.OpenSubKey(subkeyName))
                return subkey == null ? new string[0] : subkey.GetSubKeyNames();
        }

        /// <summary>
        /// Like <see cref="RegistryKey.OpenSubKey(string,bool)"/> but with no <see langword="null"/> return values.
        /// </summary>
        /// <param name="key">The key to open a subkey in.</param>
        /// <param name="subkeyName">The name of the subkey to open.</param>
        /// <param name="writable"><see langword="true"/> for write-access to the key.</param>
        /// <returns>The newly created subkey.</returns>
        /// <exception cref="IOException">Failed to open the key.</exception>
        [NotNull]
        public static RegistryKey OpenSubKeyChecked([NotNull] this RegistryKey key, [NotNull] string subkeyName, bool writable = false)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            var result = key.OpenSubKey(subkeyName, writable);
            if (result == null) throw new IOException(string.Format("Failed to open subkey '{1}' in '{0}'.", key, subkeyName));
            return result;
        }

        /// <summary>
        /// Like <see cref="RegistryKey.CreateSubKey(string)"/> but with no <see langword="null"/> return values.
        /// </summary>
        /// <param name="key">The key to create a subkey in.</param>
        /// <param name="subkeyName">The name of the subkey to create.</param>
        /// <returns>The newly created subkey.</returns>
        /// <exception cref="IOException">Failed to create the key.</exception>
        [NotNull]
        public static RegistryKey CreateSubKeyChecked([NotNull] this RegistryKey key, [NotNull] string subkeyName)
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(subkeyName)) throw new ArgumentNullException("subkeyName");
            #endregion

            var result = key.CreateSubKey(subkeyName);
            if (result == null) throw new IOException(string.Format("Failed to create subkey '{1}' in '{0}'.", key, subkeyName));
            return result;
        }
    }
}

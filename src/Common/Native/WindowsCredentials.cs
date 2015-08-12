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
using System.IO;
using System.Net;
using System.Text;
using JetBrains.Annotations;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides an interface to the Windows Credentials API. Supported on Windows XP or newer.
    /// </summary>
    public static partial class WindowsCredentials
    {
        /// <summary>
        /// Determines whether there are any credentials stored for a specific target.
        /// </summary>
        /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
        /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
        public static bool IsCredentialStored([NotNull] string target)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
            #endregion

            if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

            int count;
            IntPtr credentials;
            NativeMethods.CredEnumerate(target, 0, out count, out credentials);

            return (count > 0);
        }

        /// <summary>
        /// Prompts the user for credentials using a GUI dialog.
        /// </summary>
        /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
        /// <param name="flags">Flags for configuring the prompt.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        /// <param name="owner">The parent window for the dialog; can be <c>null</c>.</param>
        /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
        public static NetworkCredential PromptDialog([NotNull] string target, WindowsCredentialsFlags flags, [CanBeNull] string title = null, [CanBeNull] string message = null, IntPtr owner = default(IntPtr))
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
            #endregion

            if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

            var usernameBuffer = new StringBuilder(1000);
            var passwordBuffer = new StringBuilder(1000);

            var credUI = CreateCredUIInfo(owner, title, message);
            bool persist = true;
            int result = NativeMethods.CredUIPromptForCredentials(ref credUI, target, IntPtr.Zero, 0, usernameBuffer, MaxUsernameLength, passwordBuffer, MaxPasswordLength, ref persist, flags);
            HandleResult(result);

            return new NetworkCredential(usernameBuffer.ToString(), passwordBuffer.ToString());
        }

        /// <summary>
        /// Prompts the user for credentials using a command-line interface.
        /// </summary>
        /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
        /// <param name="flags">Flags for configuring the prompt.</param>
        /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
        public static NetworkCredential PromptCli([NotNull] string target, WindowsCredentialsFlags flags)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
            #endregion

            if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

            var usernameBuffer = new StringBuilder(1000);
            var passwordBuffer = new StringBuilder(1000);

            bool persist = true;
            int result = NativeMethods.CredUICmdLinePromptForCredentials(target, IntPtr.Zero, 0, usernameBuffer, MaxUsernameLength, passwordBuffer, MaxPasswordLength, ref persist, flags);
            HandleResult(result);

            return new NetworkCredential(usernameBuffer.ToString(), passwordBuffer.ToString());
        }

        private static void HandleResult(int result)
        {
            switch (result)
            {
                case 0:
                    break;
                case WindowsUtils.Win32ErrorCancelled:
                    throw new OperationCanceledException();
                case ErrorNoSuchLogonSession:
                    throw new NotSupportedException();
                default:
                    throw new IOException();
            }
        }
    }
}

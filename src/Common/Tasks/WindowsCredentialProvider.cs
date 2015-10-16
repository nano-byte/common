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
using System.Net;
using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Asks for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using <see cref="WindowsCredentials"/>.
    /// </summary>
    /// <remarks>Use one instance per remote resource.</remarks>
    public abstract class WindowsCredentialProvider : ICredentialProvider
    {
        private readonly bool _silent;

        /// <summary>
        /// Creates a new GUI credential provider.
        /// </summary>
        /// <param name="silent">Set to <see langword="true"/> to serve persisted credentials but not show any UI asking for new credentials.</param>
        protected WindowsCredentialProvider(bool silent)
        {
            _silent = silent;
        }

        /// <summary>Used to signal that the next <see cref="GetCredential"/> call will be a retry due to incorrect credentials.</summary>
        private bool _retry;

        /// <inheritdoc/>
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            string target = uri.ToStringRfc();

            if (_silent && !WindowsCredentials.IsCredentialStored(target))
                return null;

            var flags = WindowsCredentialsFlags.GenericCredentials | WindowsCredentialsFlags.ExcludeCertificates | WindowsCredentialsFlags.Persist;
            if (_retry)
            {
                _retry = false;
                flags |= WindowsCredentialsFlags.IncorrectPassword | WindowsCredentialsFlags.AlwaysShowUI;
            }

            return Prompt(target, flags);
        }

        /// <summary>
        /// Performs the actual <see cref="WindowsCredentials"/> API call to prompt the user or the credential store for credentials.
        /// </summary>
        /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
        /// <param name="flags">Flags for configuring the prompt.</param>
        protected abstract NetworkCredential Prompt(string target, WindowsCredentialsFlags flags);

        /// <inheritdoc/>
        public bool RequestRetry()
        {
            _retry = true;
            return !_silent;
        }
    }
}
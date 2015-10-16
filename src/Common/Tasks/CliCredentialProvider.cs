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
using NanoByte.Common.Cli;
using NanoByte.Common.Net;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Asks the user for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using a command-line prompt.
    /// </summary>
    /// <remarks>Use one instance per remote resource.</remarks>
    public class CliCredentialProvider : ICredentialProvider
    {
        /// <summary>Used to signal that the next <see cref="GetCredential"/> call will be a retry due to incorrect credentials.</summary>
        private bool _retry;

        /// <inheritdoc/>
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            Log.Debug("Prompt for credentials on command-line: " + uri.ToStringRfc());
            if (_retry)
            {
                Log.Error(string.Format(Resources.InvalidCredentials, uri.ToStringRfc()));
                _retry = false;
            }
            Console.Error.WriteLine(Resources.PleasEnterCredentials, uri.ToStringRfc());
            return new NetworkCredential(
                CliUtils.ReadString(Resources.UserName),
                CliUtils.ReadPassword(Resources.Password));
        }

        /// <inheritdoc/>
        public bool RequestRetry()
        {
            _retry = true;
            return true;
        }
    }
}
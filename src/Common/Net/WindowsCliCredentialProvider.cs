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
using NanoByte.Common.Properties;
using NanoByte.Common.Values;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using <see cref="WindowsCredentials.PromptCli"/>.
    /// </summary>
    public class WindowsCliCredentialProvider : WindowsCredentialProvider
    {
        /// <summary>
        /// Creates a new Windows command-line credential provider.
        /// </summary>
        /// <param name="interactive">Indicates whether the credential provider is interactive, i.e., can ask the user for input.</param>
        public WindowsCliCredentialProvider(bool interactive) : base(interactive)
        {}

        /// <inheritdoc/>
        protected override NetworkCredential Prompt(string target, WindowsCredentialsFlags flags)
        {
            if (flags.HasFlag(WindowsCredentialsFlags.IncorrectPassword))
                Log.Error(string.Format(Resources.InvalidCredentials, target));
            return WindowsCredentials.PromptCli(target, flags);
        }
    }
}
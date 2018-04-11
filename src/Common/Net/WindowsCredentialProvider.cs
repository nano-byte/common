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
using System.Net;
using JetBrains.Annotations;
using NanoByte.Common.Native;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Asks for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s using <see cref="WindowsCredentials"/>.
    /// </summary>
    public abstract class WindowsCredentialProvider : CredentialProviderBase
    {
        /// <inheritdoc/>
        protected WindowsCredentialProvider([NotNull] ITaskHandler handler)
            : base(handler)
        {}

        /// <inheritdoc/>
        public override NetworkCredential GetCredential(Uri uri, string authType)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            string target = uri.ToStringRfc();

            if (!Interactive && !WindowsCredentials.IsCredentialStored(target))
                return null;

            var flags = WindowsCredentialsFlags.GenericCredentials | WindowsCredentialsFlags.ExcludeCertificates | WindowsCredentialsFlags.ShowSaveCheckBox;
            if (WasReportedInvalid(uri))
                flags |= WindowsCredentialsFlags.IncorrectPassword | WindowsCredentialsFlags.AlwaysShowUI;

            return Prompt(target, flags);
        }

        /// <summary>
        /// Performs the actual <see cref="WindowsCredentials"/> API call to prompt the user or the credential store for credentials.
        /// </summary>
        /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
        /// <param name="flags">Flags for configuring the prompt.</param>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
        protected abstract NetworkCredential Prompt(string target, WindowsCredentialsFlags flags);
    }
}

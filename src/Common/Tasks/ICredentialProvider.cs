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

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Asks the user or a keyring for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s.
    /// </summary>
    public interface ICredentialProvider : ICredentials
    {
        /// <summary>
        /// Indicate that the last <see cref="NetworkCredential"/>s that were retrieved were incorrect and requests a retry on the next <see cref="ICredentials.GetCredential"/> call to get correct credentials.
        /// </summary>
        /// <returns><see langword="true"/> if the provider is now ready for a retry; <see langword="false"/> if the provider does not support retrys, e.g., because it is non-interactive.</returns>
        bool RequestRetry();
    }
}
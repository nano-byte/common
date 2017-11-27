#if NETSTANDARD2_0
/*
 * Copyright 2006-2017 Bastian Eicher
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
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Gets credentials from <see cref="IConfiguration"/>.
    /// </summary>
    /// <seealso cref="ConfigurationCredentialProviderRegisration.ConfigureCredentials"/>
    [CLSCompliant(false)]
    public class ConfigurationCredentialProvider : ICredentialProvider
    {
        [NotNull]
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates a new configuration credential provider.
        /// </summary>
        /// <param name="configuration">The configuration containing credentials. Each URI must be a subsection with <c>User</c> and <c>Password</c> values.</param>
        public ConfigurationCredentialProvider([NotNull] IConfiguration configuration)
            => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        /// <inheritdoc/>
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            #region Sanity checks
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            #endregion

            var cred = _configuration.GetSection(uri.ToStringRfc());
            return new NetworkCredential(cred["User"], cred["Password"]);
        }

        /// <inheritdoc/>
        public bool Interactive => false;

        /// <inheritdoc/>
        public void ReportInvalid(Uri uri)
        {}
    }

    /// <summary>
    /// Provides extension methods for registering <seealso cref="ConfigurationCredentialProvider"/> instances.
    /// </summary>
    [CLSCompliant(false)]
    [PublicAPI]
    public static class ConfigurationCredentialProviderRegisration
    {
        /// <summary>
        /// Registers an <see cref="ICredentialProvider"/> with credentials from <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="services">The service collection to perform the registration on.</param>
        /// <param name="configuration">The configuration containing credentials. Each URI must be a subsection with <c>User</c> and <c>Password</c> values.</param>
        public static IServiceCollection ConfigureCredentials(this IServiceCollection services, IConfiguration configuration)
            => services.AddSingleton<ICredentialProvider>(new ConfigurationCredentialProvider(configuration));
    }
}
#endif

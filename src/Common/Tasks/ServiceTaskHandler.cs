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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Executes tasks silently and suppresses any questions.
    /// Automatically uses <see cref="ILogger{TCategoryName}"/> and <see cref="ICredentialProvider"/> if available via <seealso cref="IServiceProvider"/>.
    /// </summary>
    /// <seealso cref="ConfigurationCredentialProviderRegisration.ConfigureCredentials"/>
    [CLSCompliant(false)]
    public sealed class ServiceTaskHandler : SilentTaskHandler
    {
        private readonly ILogger<ServiceTaskHandler> _logger;

        public ServiceTaskHandler([NotNull] IServiceProvider provider)
        {
            #region Sanity checks
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            #endregion

            _logger = provider.GetService<ILogger<ServiceTaskHandler>>();
            if (_logger != null)
                Log.Handler += LogHandler;

            CredentialProvider = provider.GetService<ICredentialProvider>();
        }

        private void LogHandler(LogSeverity severity, string message)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    _logger.LogDebug(message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(message);
                    break;
                case LogSeverity.Warn:
                    _logger.LogWarning(message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(message);
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            if (_logger != null)
                Log.Handler -= LogHandler;
        }

        /// <inheritdoc/>
        public override ICredentialProvider CredentialProvider { get; }
    }
}
#endif

// Copyright Bastian Eicher
// Licensed under the MIT License

#if NETSTANDARD
using System;
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
        private CancellationTokenSource _cancellationTokenSource;

        public ServiceTaskHandler(IServiceProvider provider)
        {
            #region Sanity checks
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            #endregion

            _logger = provider.GetService<ILogger<ServiceTaskHandler>>();
            if (_logger != null)
                Log.Handler += LogHandler;

            CredentialProvider = provider.GetService<ICredentialProvider>();
            _cancellationTokenSource = provider.GetService<CancellationTokenSource>();
        }

        /// <inheritdoc/>
        public override void Dispose() => Log.Handler -= LogHandler;

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
        public override CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? base.CancellationToken;

        /// <inheritdoc/>
        public override ICredentialProvider CredentialProvider { get; }
    }
}
#endif

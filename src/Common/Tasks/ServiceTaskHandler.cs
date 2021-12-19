// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NETFRAMEWORK
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Uses <see cref="ILogger{TCategoryName}"/>, <see cref="ICredentialProvider"/> and <see cref="CancellationTokenSource"/> from <see cref="IServiceProvider"/> if available.
/// Executes tasks silently and suppresses any questions.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
/// <seealso cref="ConfigurationCredentialProviderRegistration.ConfigureCredentials"/>
[CLSCompliant(false)]
public class ServiceTaskHandler : SilentTaskHandler
{
    private readonly ILogger<ServiceTaskHandler>? _logger;

    /// <inheritdoc />
    public override ICredentialProvider? CredentialProvider { get; }

    /// <summary>
    /// Creates a new service task handler.
    /// Registers a <see cref="Log.Handler"/> if <paramref name="provider"/> provides <see cref="ILogger{TCategoryName}"/>.
    /// </summary>
    /// <param name="provider">The DI container to check for  <see cref="ILogger{TCategoryName}"/>, <see cref="ICredentialProvider"/> and <see cref="CancellationTokenSource"/>.</param>
    public ServiceTaskHandler(IServiceProvider provider)
    {
        #region Sanity checks
        if (provider == null) throw new ArgumentNullException(nameof(provider));
        #endregion

        _logger = provider.GetService<ILogger<ServiceTaskHandler>>();
        if (_logger != null)
            Log.Handler += LogHandler;

        CancellationTokenSource = provider.GetService<CancellationTokenSource>() ?? new();
        CredentialProvider = provider.GetService<ICredentialProvider>();
    }

    /// <summary>
    /// Unregisters the <see cref="Log.Handler"/>.
    /// </summary>
    public override void Dispose()
    {
        try
        {
            Log.Handler -= LogHandler;
        }
        finally
        {
            base.Dispose();
        }
    }

    [SuppressMessage("ReSharper", "TemplateIsNotCompileTimeConstantProblem")]
    private void LogHandler(LogSeverity severity, string message)
    {
        switch (severity)
        {
            case LogSeverity.Debug:
                _logger?.LogDebug(message);
                break;
            case LogSeverity.Info:
                _logger?.LogInformation(message);
                break;
            case LogSeverity.Warn:
                _logger?.LogWarning(message);
                break;
            case LogSeverity.Error:
                _logger?.LogError(message);
                break;
        }
    }
}
#endif

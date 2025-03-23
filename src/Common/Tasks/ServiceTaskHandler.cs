// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET
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
[MustDisposeResource]
public class ServiceTaskHandler : ITaskHandler
{
    private readonly ILogger<ServiceTaskHandler>? _logger;
    private readonly ICredentialProvider? _credentialProvider;

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

        CancellationToken = provider.GetService<CancellationTokenSource>()?.Token ?? CancellationToken.None;
        _credentialProvider = provider.GetService<ICredentialProvider>();
    }

    /// <summary>
    /// Unregisters the <see cref="Log.Handler"/>.
    /// </summary>
    public virtual void Dispose()
    {
        Log.Handler -= LogHandler;
    }

    private void LogHandler(LogSeverity severity, string? message, Exception? exception)
    {
        _logger?.Log(severity switch
        {
            LogSeverity.Debug => LogLevel.Debug,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Warn => LogLevel.Warning,
            LogSeverity.Error => LogLevel.Error,
            _ => LogLevel.Critical
        }, exception, "{Message}", message ?? exception?.Message);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc/>
    public void RunTask(ITask task)
        => task.Run(CancellationToken, _credentialProvider);

    /// <summary>
    /// Always returns <see cref="NanoByte.Common.Tasks.Verbosity.Batch"/>.
    /// </summary>
    public Verbosity Verbosity
    {
        get => Verbosity.Batch;
        set {}
    }

    /// <summary>
    /// Returns <paramref name="defaultAnswer"/> if specified or <c>false</c> otherwise.
    /// </summary>
    public bool Ask(string question, bool? defaultAnswer = null, string? alternateMessage = null)
    {
        _logger?.LogInformation("{Question}\nReturning: {Answer}", alternateMessage ?? question, defaultAnswer ?? false);
        return defaultAnswer ?? false;
    }

    /// <inheritdoc/>
    public void Output(string title, string message)
        => _logger?.LogInformation("{Title}\n{Message}", title, message);

    /// <inheritdoc/>
    public void Output<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string title, IEnumerable<T> data)
        => Output(title, StringUtils.Join(Environment.NewLine, data.Select(x => x?.ToString() ?? "")));

    /// <inheritdoc/>
    public void Output<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string title, NamedCollection<T> data) where T : INamed
        => Output(title, data.AsEnumerable());

    /// <inheritdoc/>
    public void Error(Exception exception)
        => _logger?.LogError(exception, "{Message}", exception.Message);
}
#endif

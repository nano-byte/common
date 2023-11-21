// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets credentials from <see cref="IConfiguration"/>.
/// </summary>
/// <param name="configuration">The configuration containing credentials. Each URI must be a subsection with <c>User</c> and <c>Password</c> values.</param>
/// <seealso cref="ConfigurationCredentialProviderRegistration.ConfigureCredentials"/>
[CLSCompliant(false)]
public class ConfigurationCredentialProvider(IConfiguration configuration) : ICredentialProvider
{
    /// <inheritdoc/>
    public NetworkCredential GetCredential(Uri uri, bool previousIncorrect = false)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        var cred = configuration.GetSection(uri.ToStringRfc());
        return new NetworkCredential(cred["User"], cred["Password"]);
    }
}

/// <summary>
/// Provides extension methods for registering <see cref="ConfigurationCredentialProvider"/> instances.
/// </summary>
[CLSCompliant(false)]
public static class ConfigurationCredentialProviderRegistration
{
    /// <summary>
    /// Registers an <see cref="ICredentialProvider"/> with credentials from <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services">The service collection to perform the registration on.</param>
    /// <param name="configuration">The configuration containing credentials. Each URI must be a subsection with <c>User</c> and <c>Password</c> values.</param>
    public static IServiceCollection ConfigureCredentials(this IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton<ICredentialProvider>(new ConfigurationCredentialProvider(configuration));
}
#endif

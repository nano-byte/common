// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// <see cref="NetworkCredential"/>s entered by the user in a prompt, along with the user's choice whether to remember them.
/// </summary>
/// <param name="userName">The user name associated with the credentials.</param>
/// <param name="password">The password for the user name associated with the credentials.</param>
/// <param name="remember">Indicates whether the user asked to remember the credentials, e.g., in a <see cref="KeyringCredentialProvider"/>.</param>
/// <seealso cref="ICredentialFeedback"/>
public class PromptedCredential(string userName, string password, bool remember) : NetworkCredential(userName, password)
{
    /// <summary>
    /// Indicates whether the user asked to remember the credentials, e.g., in a <see cref="KeyringCredentialProvider"/>.
    /// </summary>
    /// <remarks>The credentials should only be saved once they have been accepted. See <see cref="ICredentialFeedback"/>.</remarks>
    public bool Remember { get; } = remember;
}

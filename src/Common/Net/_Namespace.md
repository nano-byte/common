---
uid: NanoByte.Common.Net
summary: Network communication and credential handling.
---
## Downloads

<xref:NanoByte.Common.Net.DownloadFile> downloads a file via HTTP(S) as an <xref:NanoByte.Common.Tasks.ITask>, providing progress reporting and cancellation via the standard task system.

<xref:NanoByte.Common.Net.HttpClientExtensions> adds helper methods on top of <xref:System.Net.Http.HttpClient>.

## Credentials

<xref:NanoByte.Common.Net.ICredentialProvider> abstracts the source of HTTP credentials, decoupling network code from any specific UI or storage backend. Implementations include:

- <xref:NanoByte.Common.Net.NetrcCredentialProvider> reads credentials from a `.netrc` file (parsed via <xref:NanoByte.Common.Net.Netrc>).
- <xref:NanoByte.Common.Net.WindowsCliCredentialProvider> prompts the user on the console using the Windows Credential Manager UI.
- <xref:NanoByte.Common.Net.WindowsGuiCredentialProvider> prompts the user with a graphical credential dialog.
- <xref:NanoByte.Common.Net.WindowsNonInteractiveCredentialProvider> reads stored credentials from the Windows Credential Manager without prompting.
- <xref:NanoByte.Common.Net.LinuxCredentialProvider> reads stored credentials from the Linux keyring (e.g., GNOME Keyring or KWallet) via the Secret Service API.
- <xref:NanoByte.Common.Net.MacOSCredentialProvider> reads stored internet passwords from the MacOS X Keychain.

<xref:NanoByte.Common.Net.KeyringCredentialProvider.ForCurrentOS*> picks the right keyring provider for the current Unix-like operating system.

Providers can implement <xref:NanoByte.Common.Net.ICredentialFeedback> to learn when the credentials they returned were accepted by the server. <xref:NanoByte.Common.Net.DownloadFile> reports this. Keyring providers use it to save <xref:NanoByte.Common.Net.PromptedCredential>s that the user asked to remember in a prompt.

## API

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

## API

// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Native;

[Flags]
[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Native API")]
public enum WindowsCredentialsFlags
{
    None,
    IncorrectPassword = 0x00001,
    DoNotPersist = 0x00002,
    RequestAdministrator = 0x00004,
    ExcludeCertificates = 0x00008,
    RequireCertificate = 0x00010,
    ShowSaveCheckBox = 0x00040,
    AlwaysShowUI = 0x00080,
    RequireSmartcard = 0x00100,
    PasswordOnlyOK = 0x00200,
    ValidateUsername = 0x00400,
    CompleteUsername = 0x00800,
    Persist = 0x01000,
    ServerCredential = 0x04000,
    ExpectConfirmation = 0x20000,
    GenericCredentials = 0x40000,
    UsernameTargetCredentials = 0x80000,
    KeepUsername = 0x100000
}
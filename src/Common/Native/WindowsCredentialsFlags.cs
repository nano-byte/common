/*
 * Copyright 2006-2015 Bastian Eicher
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
using System.Diagnostics.CodeAnalysis;

namespace NanoByte.Common.Native
{
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
}
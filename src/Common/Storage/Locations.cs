// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Security.AccessControl;
using System.Security.Principal;
using NanoByte.Common.Native;

#if !NET
using System.Reflection;
#endif

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Provides easy access to platform-specific common directories for storing settings and application data.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Environment.SpecialFolder"/> on Windows and the freedesktop.org basedir spec (XDG) on Linux.
    /// See http://freedesktop.org/wiki/Standards/basedir-spec
    /// </remarks>
    public static partial class Locations
    {
        /// <summary>
        /// The directory the application binaries are located in.
        /// </summary>
        /// <remarks>
        /// Uses the location of the NanoByte.Common DLL, not the calling EXE. Walks up one directory level if placed within a dir called "lib".
        /// Works with ngened and shadow copied assemblies. Does not work with GACed assemblies.
        /// </remarks>
        public static string InstallBase { get; private set; } = GetInstallBase();

        /// <summary>
        /// Override the automatically determined <see cref="InstallBase"/> with a custom <paramref name="path"/>.
        /// </summary>
        /// <remarks>Use with caution. Be aware of possible race conditions. Intended for unit testing, runtime relocation, etc..</remarks>
        public static void OverrideInstallBase(string path) => InstallBase = path;

        private static string GetInstallBase()
        {
            string? codeBase = null;
#if !NET
            try
            {
                codeBase = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            }
            catch (PlatformNotSupportedException)
            {}
#endif

            return codeBase ?? AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
        }

        /// <summary>Thread-specific override for <see cref="PortableBase"/>.</summary>
        [ThreadStatic]
        private static string? _redirectBase;

        /// <summary>
        /// Temporarily overrides <see cref="IsPortable"/> and <see cref="PortableBase"/> for the current thread. Useful for unit tests.
        /// </summary>
        /// <param name="path">The <see cref="PortableBase"/> value to set.</param>
        /// <returns>Call <see cref="IDisposable.Dispose"/> to restore the original values of <see cref="IsPortable"/> and <see cref="PortableBase"/>.</returns>
        /// <exception cref="InvalidOperationException">This method has already been called on this thread.</exception>
        public static IDisposable Redirect(string path)
        {
            _redirectBase = path;
            return new Disposable(() => _redirectBase = null);
        }

        /// <summary>
        /// The name of the flag file whose existence determines whether <see cref="IsPortable"/> is set to <c>true</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
        public const string PortableFlagName = "_portable";

        private static readonly bool _isPortable = File.Exists(Path.Combine(InstallBase, PortableFlagName));

        /// <summary>
        /// Indicates whether the application is currently operating in portable mode.
        /// </summary>
        /// <remarks>
        ///   <para>Portable mode is activated by placing a file named <see cref="PortableFlagName"/> in <see cref="InstallBase"/>.</para>
        ///   <para>When portable mode is active files are stored and loaded from <see cref="PortableBase"/> instead of the user profile and system directories.</para>
        /// </remarks>
        public static bool IsPortable
            => _isPortable || _redirectBase != null;

        /// <summary>
        /// The directory used for storing files if <see cref="IsPortable"/> is <c>true</c>.
        /// </summary>
        public static string PortableBase
            => _redirectBase ?? InstallBase;

        /// <summary>
        /// Returns the value of an environment variable or a default value if it isn't set.
        /// </summary>
        /// <param name="variable">The name of the environment variable to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the environment variable was not set.</param>
        /// <returns>The value of the environment variable or <paramref name="defaultValue"/>.</returns>
        private static string GetEnvironmentVariable(string variable, string defaultValue)
        {
            string? value = Environment.GetEnvironmentVariable(variable);
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        #region ACL Security
        /// <summary>
        /// ACL that gives normal users read and execute access and admins and the the system full access.
        /// </summary>
        private static readonly DirectorySecurity? _secureSharedAcl;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Locations()
        {
            if (WindowsUtils.IsWindowsNT)
            {
                _secureSharedAcl = new();
                _secureSharedAcl.SetOwner(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null));
                _secureSharedAcl.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
                _secureSharedAcl.AddAccessRule(new(new SecurityIdentifier("S-1-1-0" /*Everyone*/), FileSystemRights.ReadAndExecute, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                _secureSharedAcl.AddAccessRule(new(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), FileSystemRights.ReadAndExecute, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                _secureSharedAcl.AddAccessRule(new(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                _secureSharedAcl.AddAccessRule(new(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            }
        }

        /// <summary>
        /// Creates a directory with ACLs that block write-access for regular users.
        /// </summary>
        /// <exception cref="NotAdminException">A directory does not exist yet and the user is not an administrator.</exception>
        private static void CreateSecureMachineWideDir([Localizable(false)] string path)
        {
            var directory = new DirectoryInfo(path);
            if (directory.Exists) return;

            if (WindowsUtils.IsWindowsNT && _secureSharedAcl != null)
            {
                if (!WindowsUtils.IsAdministrator) throw new NotAdminException();
                directory.Create(_secureSharedAcl);
            }
            else directory.Create();
        }
        #endregion
    }
}

// Copyright Bastian Eicher
// Licensed under the MIT License

#if NETFRAMEWORK
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Microsoft.CSharp;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods for compiling .NET code at runtime.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class CompilerUtils
    {
        /// <summary>
        /// Compiles a string of C# code using the newest C# compiler available on the system.
        /// </summary>
        /// <param name="compilerParameters">The compiler configuration (e.g. output file path).</param>
        /// <param name="code">The C# code to compile.</param>
        /// <param name="manifest">The contents of the Win32 manifest to apply to the output file. Will only be applied if a C# 3.0 or newer compiler is available.</param>
        public static void CompileCSharp(this CompilerParameters compilerParameters, [Localizable(false)] string code, [Localizable(false)] string manifest)
        {
            #region Sanity checks
            if (compilerParameters == null) throw new ArgumentNullException(nameof(compilerParameters));
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrEmpty(manifest)) throw new ArgumentNullException(nameof(manifest));
            #endregion

            // Make sure the containing directory exists
            string directory = Path.GetDirectoryName(Path.GetFullPath(compilerParameters.OutputAssembly));
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (new WorkingDirectory(Environment.SystemDirectory)) // Prevent DLLs in current working directory from influencing build
            {
                using var manifestFile = new TemporaryFile("0install");
                File.WriteAllText(manifestFile, manifest);

                var compiler = GetCSharpCompiler(compilerParameters, manifestFile);
                var compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, code);

                if (compilerResults.Errors.HasErrors)
                {
                    var error = compilerResults.Errors[0];
                    if (error.ErrorNumber == "CS0016") throw new IOException(error.ErrorText);
                    else throw new InvalidOperationException($"Compilation error {error.ErrorNumber} in line {error.Line}{Environment.NewLine}{error.ErrorText}");
                }
            }
        }

        /// <summary>
        /// Detects the best possible C# compiler version and instantiates it.
        /// </summary>
        /// <param name="compilerParameters">The compiler parameters to be used. Version-specific options may be set.</param>
        /// <param name="manifestFilePath">The path of an assembly file to be added to compiled binaries if possible.</param>
        private static CodeDomProvider GetCSharpCompiler(this CompilerParameters compilerParameters, string manifestFilePath)
        {
            if (Environment.Version.Major == 4)
            { // C# 4.0/5.0 (.NET 4.0/4.5)
                compilerParameters.CompilerOptions += $" /win32manifest:{manifestFilePath.EscapeArgument()}";
                return new CSharpCodeProvider();
            }
            else if (File.Exists(Path.Combine(WindowsUtils.GetNetFxDirectory(WindowsUtils.NetFx35), "csc.exe")))
            { // C# 3.0 (.NET 3.5)
                compilerParameters.CompilerOptions += $" /win32manifest:{manifestFilePath.EscapeArgument()}";
                return NewCSharpCodeProvider(WindowsUtils.NetFx35);
            }
            else
            { // C# 2.0 (.NET 2.0/3.0)
                return new CSharpCodeProvider();
            }
        }

        // Instantiates a post-v2.0 C# compiler in a 2.0 .NET runtime environment.
        // Extracted to a separate method in case this is older than .NET 2.0 SP2 and the required constructor is therefore missing.
        [SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "Microsoft.CSharp.CSharpCodeProvider.#.ctor(System.Collections.Generic.IDictionary`2<System.String,System.String>)", Justification = "Will only be called on post-2.0 .NET versions")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static CodeDomProvider NewCSharpCodeProvider(string version)
            => new CSharpCodeProvider(new Dictionary<string, string> {{"CompilerVersion", version}});
    }
}
#endif

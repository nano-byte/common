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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;
using Microsoft.CSharp;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods for compiling .NET code at runtime.
    /// </summary>
    public static class CompilerUtils
    {
        /// <summary>
        /// Compiles a string of C# code using the newest C# compiler available on the system.
        /// </summary>
        /// <param name="compilerParameters">The compiler configuration (e.g. output file path).</param>
        /// <param name="code">The C# code to compile.</param>
        /// <param name="manifest">The contents of the Win32 manifest to apply to the output file. Will only be applied if a C# 3.0 or newer compiler is available.</param>
        public static void CompileCSharp([NotNull] this CompilerParameters compilerParameters, [NotNull, Localizable(false)] string code, [NotNull, Localizable(false)] string manifest)
        {
            #region Sanity checks
            if (compilerParameters == null) throw new ArgumentNullException("compilerParameters");
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            if (string.IsNullOrEmpty(manifest)) throw new ArgumentNullException("manifest");
            #endregion

            // Make sure the containing directory exists
            string directory = Path.GetDirectoryName(Path.GetFullPath(compilerParameters.OutputAssembly));
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var manifestFile = new TemporaryFile("0install"))
            {
                File.WriteAllText(manifestFile, manifest);

                var compiler = GetCSharpCompiler(compilerParameters, manifestFile);
                var compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, code);

                if (compilerResults.Errors.HasErrors)
                {
                    var error = compilerResults.Errors[0];
                    if (error.ErrorNumber == "CS0016") throw new IOException(error.ErrorText);
                    else throw new InvalidOperationException("Compilation error " + error.ErrorNumber + " in line " + error.Line + "\n" + error.ErrorText);
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
                compilerParameters.CompilerOptions += " /win32manifest:" + manifestFilePath.EscapeArgument();
                return new CSharpCodeProvider();
            }
            else if (WindowsUtils.HasNetFxVersion(WindowsUtils.NetFx35))
            { // C# 3.0 (.NET 3.5)
                compilerParameters.CompilerOptions += " /win32manifest:" + manifestFilePath.EscapeArgument();
                return NewCSharpCodeProviderEx(WindowsUtils.NetFx35);
            }
            else
            { // C# 2.0 (.NET 2.0/3.0)
                return new CSharpCodeProvider();
            }
        }

        /// <summary>
        /// Instantiates a post-v2.0 C# compiler in a 2.0 .NET runtime environment.
        /// </summary>
        /// <param name="version">The full .NET version number including the leading "v". Use predefined constants when possible.</param>
        /// <remarks>Extracted to a separate method in case this is older than .NET 2.0 SP2 and the required constructor is therefore missing.</remarks>
        [SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "Microsoft.CSharp.CSharpCodeProvider.#.ctor(System.Collections.Generic.IDictionary`2<System.String,System.String>)", Justification = "Will only be called on post-2.0 .NET versions")]
        private static CodeDomProvider NewCSharpCodeProviderEx(string version)
        {
            return new CSharpCodeProvider(new Dictionary<string, string> {{"CompilerVersion", version}});
        }
    }
}

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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides helper methods for launching <see cref="Thread"/>s.
    /// </summary>
    public static class ThreadUtils
    {
        /// <summary>
        /// Starts executing a delegate in a new thread suitable for WinForms.
        /// </summary>
        /// <param name="execute">The delegate to execute.</param>
        /// <param name="name">A short name for the new thread; can be <c>null</c>.</param>
        /// <returns>The newly launched thread.</returns>
        [PublicAPI, NotNull]
        public static Thread StartAsync([NotNull] ThreadStart execute, [CanBeNull, Localizable(false)] string name = null)
        {
            #region Sanity checks
            if (execute == null) throw new ArgumentNullException("execute");
            #endregion

            Log.Debug("Starting async thread: " + name);

            var thread = new Thread(execute) {Name = name};
            thread.SetApartmentState(ApartmentState.STA); // Make COM work
            thread.Start();
            return thread;
        }

        /// <summary>
        /// Starts executing a delegate in a new background thread (automatically terminated when application exits).
        /// </summary>
        /// <param name="execute">The delegate to execute.</param>
        /// <param name="name">A short name for the new thread; can be <c>null</c>.</param>
        /// <returns>The newly launched thread.</returns>
        [PublicAPI, NotNull]
        public static Thread StartBackground([NotNull] ThreadStart execute, [CanBeNull, Localizable(false)] string name = null)
        {
            #region Sanity checks
            if (execute == null) throw new ArgumentNullException("execute");
            #endregion

            Log.Debug("Starting background thread: " + name);

            var thread = new Thread(execute) {Name = name, IsBackground = true};
            thread.Start();
            return thread;
        }

        /// <summary>
        /// Executes a delegate in a new <see cref="ApartmentState.STA"/> thread. Blocks the caller until the execution completes.
        /// </summary>
        /// <param name="execute">The delegate to execute.</param>
        /// <remarks>This is useful for code that needs to be executed in a Single-Threaded Apartment (e.g. WinForms code) when the calling thread is not set up to handle COM.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are rethrown on calling thread.")]
        [PublicAPI]
        public static void RunSta([NotNull, InstantHandle] Action execute)
        {
            #region Sanity checks
            if (execute == null) throw new ArgumentNullException("execute");
            #endregion

            Log.Debug("Running STA thread");

            Exception error = null;
            var thread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    execute();
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (error != null) throw error.PreserveStack();
        }

        /// <summary>
        /// Executes a delegate in a new <see cref="ApartmentState.STA"/> thread. Blocks the caller until the execution completes.
        /// </summary>
        /// <typeparam name="T">The type of the return value of <paramref name="execute"/>.</typeparam>
        /// <param name="execute">The delegate to execute.</param>
        /// <returns>The return value of <paramref name="execute"/></returns>
        /// <remarks>This is useful for code that needs to be executed in a Single-Threaded Apartment (e.g. WinForms code) when the calling thread is not set up to handle COM.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are rethrown on calling thread.")]
        [PublicAPI]
        public static T RunSta<T>([NotNull, InstantHandle] Func<T> execute)
        {
            #region Sanity checks
            if (execute == null) throw new ArgumentNullException("execute");
            #endregion

            Log.Debug("Running STA thread");

            T result = default(T);
            Exception error = null;
            var thread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    result = execute();
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (error != null) throw error.PreserveStack();
            return result;
        }
    }
}

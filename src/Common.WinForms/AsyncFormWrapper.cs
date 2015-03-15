/*
 * Copyright 2006-2014 Bastian Eicher
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
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace NanoByte.Common
{
    /// <summary>
    /// Lazily starts a separate thread with a message loop for a <see cref="Form"/>.
    /// </summary>
    /// <typeparam name="T">The type of the form to wrap.</typeparam>
    public sealed class AsyncFormWrapper<T> : IDisposable
        where T : Form
    {
        private readonly Func<T> _init;

        /// <summary>
        /// Creates a new asynchronous form wrapper.
        /// </summary>
        /// <param name="init">Callback that creates an instance of the form for the message loop.</param>
        public AsyncFormWrapper([NotNull] Func<T> init)
        {
            #region Sanity checks
            if (init == null) throw new ArgumentNullException("init");
            #endregion

            _init = init;
        }

        private T _form;

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "handle", Justification = "Need to retrieve value from Form.Handle to force window handle creation")]
        [SuppressMessage("ReSharper", "UnusedVariable", MessageId = "handle", Justification = "Need to retrieve value from Form.Handle to force window handle creation")]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        private void InitializeForm()
        {
            if (_form != null) return;

            using (var handleCreatedEvent = new ManualResetEvent(false))
            {
                ProcessUtils.RunAsync(() =>
                {
                    _form = _init();

                    // Force creation of handle without showing the form
                    var handle = _form.Handle;

                    // Signal the calling thread the form is ready
                    handleCreatedEvent.Set();

                    // Run message loop (will take ownership of the form)
                    Application.Run();
                }, "AsyncFormWrapper");

                handleCreatedEvent.WaitOne();
            }
        }

        private readonly object _lock = new object();

        /// <summary>
        /// Starts the message loop if it is not running yet and executes an action on its thread waiting for it to complete.
        /// </summary>
        /// <param name="action">A delegate that is passed the <see cref="Form"/> instance.</param>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public void Post([NotNull, InstantHandle] Action<T> action)
        {
            #region Sanity checks
            if (action == null) throw new ArgumentNullException("action");
            #endregion

            lock (_lock)
            {
                InitializeForm();

                try
                {
                    _form.Invoke(action, _form);
                }
                    #region Sanity checks
                catch (InvalidOperationException)
                {
                    throw new OperationCanceledException();
                }
                catch (InvalidAsynchronousStateException)
                {
                    throw new OperationCanceledException();
                }
                #endregion
            }
        }

        /// <summary>
        /// Starts the message loop if it is not running yet and executes an action on its thread waiting for it to complete.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by <paramref name="action"/>.</typeparam>
        /// <param name="action">A delegate that is passed the <see cref="Form"/> instance and returns a result.</param>
        /// <returns>The result returned by <paramref name="action"/>.</returns>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public TResult Post<TResult>([NotNull, InstantHandle] Func<T, TResult> action)
        {
            #region Sanity checks
            if (action == null) throw new ArgumentNullException("action");
            #endregion

            lock (_lock)
            {
                InitializeForm();

                try
                {
                    return (TResult)_form.Invoke(action, _form);
                }
                    #region Sanity checks
                catch (InvalidOperationException)
                {
                    throw new OperationCanceledException();
                }
                catch (InvalidAsynchronousStateException)
                {
                    throw new OperationCanceledException();
                }
                #endregion
            }
        }

        /// <summary>
        /// Starts the message loop if it is not running yet and executes an action on its thread without waiting for it to complete.
        /// </summary>
        /// <param name="action">A delegate that is passed the <see cref="Form"/> instance.</param>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public void Send([NotNull] Action<T> action)
        {
            #region Sanity checks
            if (action == null) throw new ArgumentNullException("action");
            #endregion

            lock (_lock)
            {
                InitializeForm();

                try
                {
                    _form.BeginInvoke(action, _form);
                }
                    #region Sanity checks
                catch (InvalidOperationException)
                {
                    throw new OperationCanceledException();
                }
                catch (InvalidAsynchronousStateException)
                {
                    throw new OperationCanceledException();
                }
                #endregion
            }
        }

        /// <summary>
        /// Sets <see cref="Control.Enabled"/> for the <see cref="Form"/> to <see langword="false"/> if it was already created.
        /// </summary>
        /// <remarks>Does nothing if the <see cref="Form"/> was not yet created.</remarks>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public void Disable()
        {
            lock (_lock)
            {
                if (_form == null) return;

                try
                {
                    _form.Invoke(new Action(() => { _form.Enabled = false; }));
                }
                    #region Sanity checks
                catch (InvalidOperationException)
                {
                    throw new OperationCanceledException();
                }
                catch (InvalidAsynchronousStateException)
                {
                    throw new OperationCanceledException();
                }
                #endregion
            }
        }

        /// <summary>
        /// Closes the <see cref="Form"/> and stops the message loop.
        /// </summary>
        /// <remarks>Does nothing if the <see cref="Form"/> was not yet created.</remarks>
        public void Close()
        {
            lock (_lock)
            {
                if (_form == null) return;

                try
                {
                    _form.Invoke(new Action(() =>
                    {
                        Application.ExitThread();
                        _form.Dispose();
                    }));
                }
                    #region Sanity checks
                catch (InvalidOperationException)
                {
                    // Don't worry if the form was already closing
                }
                catch (InvalidAsynchronousStateException)
                {
                    // Don't worry if the form was already closing
                }
                catch (RemotingException)
                {
                    // Remoting exceptions on clean-up are not critical
                }
                catch (NullReferenceException)
                {
                    // Rare .NET bug
                }
                #endregion
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Close();
        }
    }
}

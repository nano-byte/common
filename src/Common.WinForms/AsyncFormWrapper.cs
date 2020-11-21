// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;

#if NETFRAMEWORK
using System.Runtime.Remoting;
#endif

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
        public AsyncFormWrapper(Func<T> init)
        {
            _init = init ?? throw new ArgumentNullException(nameof(init));
        }

        private readonly object _lock = new();

        private T? _form;

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "handle", Justification = "Need to retrieve value from Form.Handle to force window handle creation")]
        [SuppressMessage("ReSharper", "UnusedVariable", MessageId = "handle", Justification = "Need to retrieve value from Form.Handle to force window handle creation")]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        private T InitializeForm()
        {
            lock (_lock)
            {
                if (_form != null) return _form;

                T form = null!;
                using (var handleCreatedEvent = new ManualResetEvent(false))
                {
                    ThreadUtils.StartAsync(() =>
                    {
                        form = _init();

                        // Force creation of handle without showing the form
                        var handle = form.Handle;

                        // Signal the calling thread the form is ready
                        handleCreatedEvent.Set();

                        // Run message loop (will take ownership of the form)
                        Application.Run();
                    }, "AsyncFormWrapper");

                    handleCreatedEvent.WaitOne();
                }
                return _form = form;
            }
        }

        /// <summary>
        /// Starts the message loop if it is not running yet and executes an action on its thread waiting for it to complete.
        /// </summary>
        /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public void Post(Action<T> action)
        {
            var form = InitializeForm();
            try
            {
                form.Invoke(action ?? throw new ArgumentNullException(nameof(action)), form);
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            catch (InvalidAsynchronousStateException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            #endregion
        }

        /// <summary>
        /// Starts the message loop if it is not running yet and executes an action on its thread waiting for it to complete.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by <paramref name="action"/>.</typeparam>
        /// <param name="action">A delegate that is passed the <see cref="Form"/> instance and returns a result.</param>
        /// <returns>The result returned by <paramref name="action"/>.</returns>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public TResult Post<TResult>(Func<T, TResult> action)
        {
            var form = InitializeForm();
            try
            {
                return (TResult)form.Invoke(action ?? throw new ArgumentNullException(nameof(action)), form);
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            catch (InvalidAsynchronousStateException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            #endregion
        }

        /// <summary>
        /// Starts the message loop if it is not running yet and executes an action on its thread without waiting for it to complete.
        /// </summary>
        /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public void Send(Action<T> action)
        {
            var form = InitializeForm();
            try
            {
                form.BeginInvoke(action ?? throw new ArgumentNullException(nameof(action)), form);
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            catch (InvalidAsynchronousStateException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            #endregion
        }

        /// <summary>
        /// Executes an action on the message loop thread without waiting for it to complete.
        /// </summary>
        /// <remarks>Does nothing if the <see cref="Form"/> was not yet created.</remarks>
        /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
        /// <exception cref="OperationCanceledException">The form was closed.</exception>
        public void SendLow(Action<T> action)
        {
            var form = _form;
            if (form == null) return;

            try
            {
                form.BeginInvoke(action ?? throw new ArgumentNullException(nameof(action)), form);
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            catch (InvalidAsynchronousStateException ex)
            {
                Log.Debug(ex);
                throw new OperationCanceledException();
            }
            #endregion
        }

        /// <summary>
        /// Closes the <see cref="Form"/> and stops the message loop.
        /// </summary>
        /// <remarks>Does nothing if the <see cref="Form"/> was not yet created.</remarks>
        public void Close()
        {
            T form;
            lock (_lock)
            {
                if (_form == null) return;
                form = _form;
                _form = null;
            }

            try
            {
                form.Invoke(new Action(() =>
                {
                    Application.ExitThread();
                    form.Dispose();
                }));
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                // Don't worry if the form was already closing
                Log.Debug(ex);
            }
            catch (InvalidAsynchronousStateException ex)
            {
                // Don't worry if the form was already closing
                Log.Debug(ex);
            }
#if NETFRAMEWORK
            catch (RemotingException ex)
            {
                // Remoting exceptions on clean-up are not critical
                Log.Debug(ex);
            }
#endif
            catch (NullReferenceException ex)
            {
                // Rare .NET bug
                Log.Debug(ex);
            }
            #endregion
        }

        /// <inheritdoc/>
        public void Dispose() => Close();
    }
}

// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20

#if NETFRAMEWORK
using System.Runtime.Remoting;
#endif

using System.Threading.Tasks;

namespace NanoByte.Common.Threading;

/// <summary>
/// Lazily starts a separate thread with a message loop for a <see cref="Form"/>.
/// </summary>
/// <typeparam name="T">The type of the form to wrap.</typeparam>
public sealed class AsyncFormWrapper<T> : IDisposable
    where T : Form
{
    private readonly Lazy<T> _form;

    /// <summary>
    /// Creates a new asynchronous form wrapper.
    /// </summary>
    /// <param name="init">Callback that creates an instance of the form for the message loop.</param>
    public AsyncFormWrapper(Func<T> init)
    {
        #region Sanity checks
        if (init == null) throw new ArgumentNullException(nameof(init));
        #endregion

        _form = new(() =>
        {
            var formSource = new TaskCompletionSource<T>();

            ThreadUtils.StartAsync(() =>
            {
                try
                {
                    var form = init();
                    var _ = form.Handle; // Force creation of handle without showing the form
                    formSource.SetResult(form);
                }
                catch (Exception e)
                {
                    formSource.SetException(e);
                    return;
                }

                // Run message loop (will take ownership of the form)
                Application.Run();
            }, "AsyncFormWrapper: " + typeof(T).Name);

            try
            {
                return formSource.Task.Result;
            }
            catch (AggregateException ex)
            {
                throw ex.RethrowFirstInner();
            }
        });
    }

    /// <summary>
    /// Starts the message loop if it is not running yet and executes an action on its thread waiting for it to complete.
    /// </summary>
    /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
    /// <exception cref="OperationCanceledException">The form was closed.</exception>
    public void Post(Action<T> action)
    {
        try
        {
            _form.Value.Invoke(action ?? throw new ArgumentNullException(nameof(action)), _form.Value);
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or InvalidAsynchronousStateException)
        {
            Log.Debug($"Message post for {typeof(T).Name} ignored because it was already closed");
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
        try
        {
            return (TResult)_form.Value.Invoke(action ?? throw new ArgumentNullException(nameof(action)), _form.Value);
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or InvalidAsynchronousStateException)
        {
            Log.Debug($"Message post for {typeof(T).Name} ignored because it was already closed");
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
        if (_form.IsValueCreated && _form.Value.IsDisposed) throw new OperationCanceledException();

        try
        {
            _form.Value.BeginInvoke(action ?? throw new ArgumentNullException(nameof(action)), _form.Value);
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or InvalidAsynchronousStateException)
        {
            Log.Debug($"Message send for {typeof(T).Name} ignored because it was already closed");
            throw new OperationCanceledException();
        }
        #endregion
    }

    /// <summary>
    /// Executes an action on the message loop thread without waiting for it to complete.
    /// </summary>
    /// <remarks>Does nothing if the <see cref="Form"/> was not yet created.</remarks>
    /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
    public void SendLow(Action<T> action)
    {
        if (!_form.IsValueCreated || _form.Value.IsDisposed) return;

        try
        {
            _form.Value.BeginInvoke(action ?? throw new ArgumentNullException(nameof(action)), _form.Value);
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or InvalidAsynchronousStateException)
        {
            Log.Debug($"Low-priority message send for {typeof(T).Name} ignored because it was already closed");
        }
        #endregion
    }

    /// <summary>
    /// Closes the <see cref="Form"/> and stops the message loop.
    /// </summary>
    /// <remarks>Does nothing if the <see cref="Form"/> was not yet created.</remarks>
    public void Close()
    {
        if (!_form.IsValueCreated || _form.Value.IsDisposed) return;

        try
        {
            _form.Value.Invoke(() =>
            {
                Application.ExitThread();
                _form.Value.Dispose();
            });
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or InvalidAsynchronousStateException)
        {
            Log.Debug($"Close request for {typeof(T).Name} ignored because it was already closed");
        }
#if NETFRAMEWORK
        catch (RemotingException ex)
        {
            Log.Debug($"Remoting error during close of {typeof(T).Name} ignored", ex);
        }
#endif
        catch (NullReferenceException)
        {
            Log.Debug("Workaround for .NET bug");
        }
        #endregion
    }

    /// <inheritdoc/>
    public void Dispose() => Close();
}
#endif

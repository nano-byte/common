// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

/// <summary>
/// Lazily creates a <see cref="Form"/> on the Eto.Forms UI thread and marshals interactions onto it.
/// </summary>
/// <typeparam name="T">The type of the form to wrap.</typeparam>
/// <remarks>
/// This does not start a separate message loop. Eto.Forms runs a single <see cref="Application"/> on the main thread, so all interactions are marshaled onto that thread via <see cref="Application.Invoke(System.Action)"/> and <see cref="Application.AsyncInvoke"/>.
/// A running <see cref="Application"/> is therefore a precondition for using this class.
/// </remarks>
public sealed class EtoFormDispatcher<T> : IDisposable
    where T : Form
{
    private readonly Lazy<T> _form;
    private volatile bool _closed;

    /// <summary>
    /// Creates a new asynchronous form wrapper.
    /// </summary>
    /// <param name="init">Callback that creates an instance of the form. Invoked on the UI thread on first access.</param>
    public EtoFormDispatcher(Func<T> init)
    {
        #region Sanity checks
        if (init == null) throw new ArgumentNullException(nameof(init));
        #endregion

        _form = new(() => Application.Instance.Invoke(() =>
        {
            var form = init();
            form.Closed += delegate { _closed = true; };
            return form;
        }));
    }

    /// <summary>
    /// Executes an action on the UI thread, waiting for it to complete.
    /// </summary>
    /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
    /// <exception cref="OperationCanceledException">The form was closed.</exception>
    public void Post(Action<T> action)
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        if (_closed) throw new OperationCanceledException();

        try
        {
            Application.Instance.Invoke(() => action(_form.Value));
        }
        #region Error handling
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException)
        {
            Log.Debug($"Message post for {typeof(T).Name} ignored because it was already closed");
            throw new OperationCanceledException();
        }
        #endregion
    }

    /// <summary>
    /// Executes a function on the UI thread, waiting for it to complete.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by <paramref name="action"/>.</typeparam>
    /// <param name="action">A delegate that is passed the <typeparamref name="T"/> instance and returns a result.</param>
    /// <returns>The result returned by <paramref name="action"/>.</returns>
    /// <exception cref="OperationCanceledException">The form was closed.</exception>
    public TResult Post<TResult>(Func<T, TResult> action)
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        if (_closed) throw new OperationCanceledException();

        try
        {
            return Application.Instance.Invoke(() => action(_form.Value));
        }
        #region Error handling
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException)
        {
            Log.Debug($"Message post for {typeof(T).Name} ignored because it was already closed");
            throw new OperationCanceledException();
        }
        #endregion
    }

    /// <summary>
    /// Executes an action on the UI thread without waiting for it to complete.
    /// </summary>
    /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
    /// <exception cref="OperationCanceledException">The form was closed.</exception>
    public void Send(Action<T> action)
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        if (_closed) throw new OperationCanceledException();

        try
        {
            Application.Instance.AsyncInvoke(() => action(_form.Value));
        }
        #region Error handling
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException)
        {
            Log.Debug($"Message send for {typeof(T).Name} ignored because it was already closed");
            throw new OperationCanceledException();
        }
        #endregion
    }

    /// <summary>
    /// Executes an action on the UI thread without waiting for it to complete.
    /// </summary>
    /// <remarks>Does nothing if the <see cref="Form"/> was not yet created or is already closed.</remarks>
    /// <param name="action">The action to execute; gets passed the <typeparamref name="T"/> instance.</param>
    public void SendLow(Action<T> action)
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        if (!_form.IsValueCreated || _closed) return;

        try
        {
            Application.Instance.AsyncInvoke(() => action(_form.Value));
        }
        #region Error handling
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException)
        {
            Log.Debug($"Low-priority message send for {typeof(T).Name} ignored because it was already closed");
        }
        #endregion
    }

    /// <summary>
    /// Closes the <see cref="Form"/>.
    /// </summary>
    /// <remarks>Does nothing if the <see cref="Form"/> was not yet created or is already closed.</remarks>
    public void Close()
    {
        if (!_form.IsValueCreated || _closed) return;

        try
        {
            Application.Instance.Invoke(() => _form.Value.Close());
        }
        #region Error handling
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException)
        {
            Log.Debug($"Close request for {typeof(T).Name} ignored because it was already closed");
        }
        #endregion
    }

    /// <inheritdoc/>
    public void Dispose() => Close();
}

// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;
using Windows.Win32.UI.Shell.PropertiesSystem;
using Windows.Win32.UI.WindowsAndMessaging;
using CShellLink = Windows.Win32.UI.Shell.ShellLink;

namespace NanoByte.Common.Native;

/// <summary>
/// Provides helper methods and API calls specific to the Windows 7 or newer taskbar.
/// </summary>
[SupportedOSPlatform("windows6.1")]
public static class WindowsTaskbar
{
    /// <summary>
    /// Represents the thumbnail progress bar state.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "These enum values are mutually exclusive and not meant to be ORed like flags")]
    public enum ProgressBarState
    {
        /// <summary>
        /// No progress is displayed.
        /// </summary>
        NoProgress = 0,

        /// <summary>
        /// The progress is indeterminate (marquee).
        /// </summary>
        Indeterminate = 0x1,

        /// <summary>
        /// Normal progress is displayed.
        /// </summary>
        Normal = 0x2,

        /// <summary>
        /// An error occurred (red).
        /// </summary>
        Error = 0x4,

        /// <summary>
        /// The operation is paused (yellow).
        /// </summary>
        Paused = 0x8
    }

    /// <summary>
    /// Represents a shell link targeting a file.
    /// </summary>
    public struct ShellLink
    {
        /// <summary>The title/name of the task link.</summary>
        public readonly string Title;

        /// <summary>The target path the link shall point to.</summary>
        public readonly string Path;

        /// <summary>Additional arguments for <see cref="Title"/>; can be <c>null</c>.</summary>
        public readonly string? Arguments;

        /// <summary>The path of the icon for the link.</summary>
        public readonly string IconPath;

        /// <summary>The resource index within the file specified by <see cref="IconPath"/>.</summary>
        public readonly int IconIndex;

        /// <summary>
        /// Creates a new shell link structure.
        /// </summary>
        /// <param name="title">The title/name of the task link.</param>
        /// <param name="path">The target path the link shall point to and to get the icon from.</param>
        /// <param name="arguments">Additional arguments for <paramref name="title"/>; can be <c>null</c>.</param>
        public ShellLink([Localizable(true)] string title, [Localizable(false)] string path, [Localizable(false)] string? arguments = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            Title = title;
            IconPath = Path = path;
            Arguments = arguments;
            IconIndex = 0;
        }

        /// <summary>
        /// Creates a new shell link structure
        /// </summary>
        /// <param name="title">The title/name of the task link.</param>
        /// <param name="path">The target path the link shall point to.</param>
        /// <param name="arguments">Additional arguments for <paramref name="title"/>; can be <c>null</c>.</param>
        /// <param name="iconPath">The path of the icon for the link.</param>
        /// <param name="iconIndex">The resource index within the file specified by <paramref name="iconPath"/>.</param>
        public ShellLink([Localizable(true)] string title, [Localizable(false)] string path, [Localizable(false)] string arguments, [Localizable(false)] string iconPath, int iconIndex)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrEmpty(iconPath)) throw new ArgumentNullException(nameof(iconPath));
            #endregion

            Title = title;
            Path = path;
            Arguments = arguments;
            IconPath = iconPath;
            IconIndex = iconIndex;
        }
    }

    /// <summary>
    /// The taskbar list for the current thread.
    /// Interface pointers are not marshaled between COM apartments, so each thread needs its own instance.
    /// </summary>
    [ThreadStatic]
    private static unsafe ITaskbarList4* _taskbarList;

    [ThreadStatic]
    private static bool _taskbarListFailed;

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
    private static unsafe ITaskbarList4* GetTaskbarList()
    {
        if (_taskbarList == null && !_taskbarListFailed && WindowsUtils.IsWindows7)
        {
            ITaskbarList4* taskbarList = null;
            try
            {
                TaskbarList.CreateInstance(out taskbarList).ThrowOnFailure();
                taskbarList->HrInit();
                _taskbarList = taskbarList;
            }
            #region Error handling
            catch (Exception ex)
            {
                if (taskbarList != null) taskbarList->Release();
                _taskbarListFailed = true;
                Log.Warn("Failed to initialize Windows Taskbar access", ex);
            }
            #endregion
        }

        return _taskbarList;
    }

    /// <summary>
    /// Sets the state of the taskbar progress indicator.
    /// </summary>
    /// <param name="handle">The handle of the window whose taskbar button contains the progress indicator.</param>
    /// <param name="state">The state of the progress indicator.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
    public static unsafe void SetProgressState(IntPtr handle, ProgressBarState state)
    {
        var taskbarList = GetTaskbarList();
        if (taskbarList == null) return;

        try
        {
            taskbarList->SetProgressState((HWND)handle, (TBPFLAG)state);
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Warn("Failed to set taskbar progress state", ex);
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the taskbar progress indicator.
    /// </summary>
    /// <param name="handle">The handle of the window whose taskbar button contains the progress indicator.</param>
    /// <param name="currentValue">The current value of the progress indicator.</param>
    /// <param name="maximumValue">The value <paramref name="currentValue"/> will have when the operation is complete.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
    public static unsafe void SetProgressValue(IntPtr handle, int currentValue, int maximumValue)
    {
        var taskbarList = GetTaskbarList();
        if (taskbarList == null) return;

        try
        {
            taskbarList->SetProgressValue((HWND)handle, Convert.ToUInt32(currentValue), Convert.ToUInt32(maximumValue));
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Warn("Failed to set taskbar progress value", ex);
        }
        #endregion
    }

    /// <summary>
    /// Sets a specific window's explicit application user model ID.
    /// </summary>
    /// <param name="hwnd">A handle to the window to set the ID for.</param>
    /// <param name="appID">The application ID to set.</param>
    /// <param name="relaunchCommand">The command to use for relaunching this specific window if it was pinned to the taskbar; can be <c>null</c>.</param>
    /// <param name="relaunchIcon">The icon to use for pinning this specific window to the taskbar (written as Path,ResourceIndex); can be <c>null</c>.</param>
    /// <param name="relaunchName">The user-friendly name to associate with <paramref name="relaunchCommand"/>; can be <c>null</c>.</param>
    /// <remarks>The application ID is used to group related windows in the taskbar.</remarks>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
    public static unsafe void SetWindowAppID(IntPtr hwnd, [Localizable(false)] string appID, [Localizable(false)] string? relaunchCommand = null, [Localizable(false)] string? relaunchIcon = null, [Localizable(true)] string? relaunchName = null)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appID)) throw new ArgumentNullException(nameof(appID));
        #endregion

        if (!WindowsUtils.IsWindows7) return;

        IPropertyStore* propertyStore = null;
        try
        {
            PInvoke.SHGetPropertyStoreForWindow((HWND)hwnd, out propertyStore).ThrowOnFailure();

            SetPropertyValue(propertyStore, PInvoke.PKEY_AppUserModel_ID, appID);
            if (!string.IsNullOrEmpty(relaunchCommand)) SetPropertyValue(propertyStore, PInvoke.PKEY_AppUserModel_RelaunchCommand, relaunchCommand!);
            if (!string.IsNullOrEmpty(relaunchIcon)) SetPropertyValue(propertyStore, PInvoke.PKEY_AppUserModel_RelaunchIconResource, relaunchIcon!);
            if (!string.IsNullOrEmpty(relaunchName)) SetPropertyValue(propertyStore, PInvoke.PKEY_AppUserModel_RelaunchDisplayNameResource, relaunchName!);
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Warn("Failed to window application user model ID", ex);
        }
        #endregion
        finally
        {
            if (propertyStore != null) propertyStore->Release();
        }
    }

    /// <summary>
    /// Adds user-task links to the taskbar jumplist. Any existing task links are removed.
    /// </summary>
    /// <param name="appID">The application ID of the jumplist to add the task to.</param>
    /// <param name="links">The links to add to the jumplist.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
    public static unsafe void AddTaskLinks([Localizable(false)] string appID, IEnumerable<ShellLink> links)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appID)) throw new ArgumentNullException(nameof(appID));
        if (links == null) throw new ArgumentNullException(nameof(links));
        #endregion

        if (!WindowsUtils.IsWindows7) return;

        ICustomDestinationList* destinationList = null;
        IObjectArray* removedDestinations = null;
        IObjectCollection* taskContent = null;
        try
        {
            DestinationList.CreateInstance(out destinationList).ThrowOnFailure();
            destinationList->SetAppID(appID);
            destinationList->BeginList(out _, out removedDestinations);

            EnumerableObjectCollection.CreateInstance(out taskContent).ThrowOnFailure();
            foreach (var shellLink in links)
            {
                var nativeShellLink = ConvertShellLink(shellLink);
                try
                {
                    taskContent->AddObject((IUnknown*)nativeShellLink);
                }
                finally
                {
                    nativeShellLink->Release();
                }
            }

            // IObjectCollection inherits from IObjectArray
            destinationList->AddUserTasks((IObjectArray*)taskContent);
            destinationList->CommitList();
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Warn("Failed to add links to taskbar jumplist", ex);
        }
        #endregion
        finally
        {
            if (taskContent != null) taskContent->Release();
            if (removedDestinations != null) removedDestinations->Release();
            if (destinationList != null) destinationList->Release();
        }
    }

    /// <summary>
    /// Prevents a specific window from being pinned to the taskbar.
    /// </summary>
    /// <param name="hwnd">A handle to the window to prevent from being pinned.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
    public static unsafe void PreventPinning(IntPtr hwnd)
    {
        if (!WindowsUtils.IsWindows7) return;

        IPropertyStore* propertyStore = null;
        try
        {
            PInvoke.SHGetPropertyStoreForWindow((HWND)hwnd, out propertyStore).ThrowOnFailure();
            SetPropertyValue(propertyStore, PInvoke.PKEY_AppUserModel_PreventPinning, "1");
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Warn("Failed to prevent pinning a window", ex);
        }
        #endregion
        finally
        {
            if (propertyStore != null) propertyStore->Release();
        }
    }

    /// <summary>
    /// Converts a managed shell link structure to a COM object.
    /// </summary>
    /// <returns>A COM object the caller must release.</returns>
    private static unsafe IShellLinkW* ConvertShellLink(ShellLink shellLink)
    {
        CShellLink.CreateInstance(out IShellLinkW* nativeShellLink).ThrowOnFailure();
        IPropertyStore* propertyStore = null;
        try
        {
            nativeShellLink->SetPath(shellLink.Path);
            if (!string.IsNullOrEmpty(shellLink.Arguments)) nativeShellLink->SetArguments(shellLink.Arguments!);
            if (!string.IsNullOrEmpty(shellLink.IconPath)) nativeShellLink->SetIconLocation(shellLink.IconPath, shellLink.IconIndex);
            nativeShellLink->SetShowCmd(SHOW_WINDOW_CMD.SW_SHOWNORMAL);

            nativeShellLink->QueryInterface(out propertyStore).ThrowOnFailure();
            SetPropertyValue(propertyStore, PInvoke.PKEY_Title, shellLink.Title);
            propertyStore->Commit();

            return nativeShellLink;
        }
        catch
        {
            nativeShellLink->Release();
            throw;
        }
        finally
        {
            if (propertyStore != null) propertyStore->Release();
        }
    }

    /// <summary>
    /// Sets a string property value.
    /// </summary>
    /// <param name="propertyStore">The property store to set the property in.</param>
    /// <param name="key">The property to set.</param>
    /// <param name="value">The value to set the property to.</param>
    private static unsafe void SetPropertyValue(IPropertyStore* propertyStore, in PROPERTYKEY key, string value)
    {
        var variant = new PROPVARIANT();
        variant.vt = VARENUM.VT_LPWSTR;
        variant.pwszVal = (char*)Marshal.StringToCoTaskMemUni(value);
        try
        {
            propertyStore->SetValue(key, variant);
        }
        finally
        {
            PInvoke.PropVariantClear(ref variant);
        }
    }
}

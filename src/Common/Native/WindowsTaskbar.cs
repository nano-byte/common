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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods and API calls specific to the Windows 7 or newer taskbar.
    /// </summary>
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global"), SuppressMessage("ReSharper", "UnusedMember.Local"), SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
    public static partial class WindowsTaskbar
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
            [NotNull]
            public readonly string Title;

            /// <summary>The target path the link shall point to.</summary>
            [NotNull]
            public readonly string Path;

            /// <summary>Additional arguments for <see cref="Title"/>; can be <see langword="null"/>.</summary>
            [CanBeNull]
            public readonly string Arguments;

            /// <summary>The path of the icon for the link.</summary>
            [NotNull]
            public readonly string IconPath;

            /// <summary>The resouce index within the file specified by <see cref="IconPath"/>.</summary>
            public readonly int IconIndex;

            /// <summary>
            /// Creates a new shell link structure.
            /// </summary>
            /// <param name="title">The title/name of the task link.</param>
            /// <param name="path">The target path the link shall point to and to get the icon from.</param>
            /// <param name="arguments">Additional arguments for <paramref name="title"/>; can be <see langword="null"/>.</param>
            [PublicAPI]
            public ShellLink([NotNull, Localizable(true)] string title, [NotNull, Localizable(false)] string path, [CanBeNull, Localizable(false)] string arguments = null)
            {
                #region Sanity checks
                if (string.IsNullOrEmpty(title)) throw new ArgumentNullException("title");
                if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
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
            /// <param name="arguments">Additional arguments for <paramref name="title"/>; can be <see langword="null"/>.</param>
            /// <param name="iconPath">The path of the icon for the link.</param>
            /// <param name="iconIndex">The resouce index within the file specified by <paramref name="iconPath"/>.</param>
            [PublicAPI]
            public ShellLink([NotNull, Localizable(true)] string title, [NotNull, Localizable(false)] string path, [NotNull, Localizable(false)] string arguments, [NotNull, Localizable(false)] string iconPath, int iconIndex)
            {
                #region Sanity checks
                if (string.IsNullOrEmpty(title)) throw new ArgumentNullException("title");
                if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
                if (string.IsNullOrEmpty(iconPath)) throw new ArgumentNullException("iconPath");
                #endregion

                Title = title;
                Path = path;
                Arguments = arguments;
                IconPath = iconPath;
                IconIndex = iconIndex;
            }
        }

        /// <summary>
        /// Sets the state of the taskbar progress indicator.
        /// </summary>
        /// <param name="handle">The handle of the window whose taskbar button contains the progress indicator.</param>
        /// <param name="state">The state of the progress indicator.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
        public static void SetProgressState(IntPtr handle, ProgressBarState state)
        {
            if (_taskbarList == null) return;

            try
            {
                lock (_taskbarList)
                    _taskbarList.SetProgressState(handle, state);
            }
                #region Error handling
            catch (Exception ex)
            {
                Log.Warn(ex);
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
        public static void SetProgressValue(IntPtr handle, int currentValue, int maximumValue)
        {
            if (_taskbarList == null) return;

            try
            {
                lock (_taskbarList)
                    _taskbarList.SetProgressValue(handle, Convert.ToUInt32(currentValue), Convert.ToUInt32(maximumValue));
            }
                #region Error handling
            catch (Exception ex)
            {
                Log.Warn(ex);
            }
            #endregion
        }

        /// <summary>
        /// Sets a specific window's explicit application user model ID.
        /// </summary>
        /// <param name="hwnd">A handle to the window to set the ID for.</param>
        /// <param name="appID">The application ID to set.</param>
        /// <param name="relaunchCommand">The command to use for relaunching this specific window if it was pinned to the taskbar; can be <see langword="null"/>.</param>
        /// <param name="relaunchIcon">The icon to use for pinning this specific window to the taskbar (written as Path,ResourceIndex); can be <see langword="null"/>.</param>
        /// <param name="relaunchName">The user-friendly name to associate with <paramref name="relaunchCommand"/>; can be <see langword="null"/>.</param>
        /// <remarks>The application ID is used to group related windows in the taskbar.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
        public static void SetWindowAppID(IntPtr hwnd, [NotNull, Localizable(false)] string appID, [CanBeNull, Localizable(false)] string relaunchCommand = null, [CanBeNull, Localizable(false)] string relaunchIcon = null, [CanBeNull, Localizable(true)] string relaunchName = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(appID)) throw new ArgumentNullException("appID");
            #endregion

            if (!WindowsUtils.IsWindows7) return;

            try
            {
                IPropertyStore propertyStore = GetWindowPropertyStore(hwnd);

                var stringFormat = new Guid("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}");
                SetPropertyValue(propertyStore, new PropertyKey(stringFormat, 5), appID);
                if (!string.IsNullOrEmpty(relaunchCommand)) SetPropertyValue(propertyStore, new PropertyKey(stringFormat, 2), relaunchCommand);
                if (!string.IsNullOrEmpty(relaunchIcon)) SetPropertyValue(propertyStore, new PropertyKey(stringFormat, 3), relaunchIcon);
                if (!string.IsNullOrEmpty(relaunchName)) SetPropertyValue(propertyStore, new PropertyKey(stringFormat, 4), relaunchName);

                Marshal.ReleaseComObject(propertyStore);
            }
                #region Error handling
            catch (Exception ex)
            {
                Log.Warn(ex);
            }
            #endregion
        }

        /// <summary>
        /// Adds user-task links to the taskbar jumplist. Any existing task links are removed.
        /// </summary>
        /// <param name="appID">The application ID of the jumplist to add the task to.</param>
        /// <param name="links">The links to add to the jumplist.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
        public static void AddTaskLinks([NotNull, Localizable(false)] string appID, [NotNull, ItemNotNull] IEnumerable<ShellLink> links)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(appID)) throw new ArgumentNullException("appID");
            if (links == null) throw new ArgumentNullException("links");
            #endregion

            if (!WindowsUtils.IsWindows7) return;

            try
            {
                var customDestinationList = (ICustomDestinationList)new CDestinationList();
                customDestinationList.SetAppID(appID);

                var objectArray = new Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9");
                object removedItems;
                uint maxSlots;
                customDestinationList.BeginList(out maxSlots, ref objectArray, out removedItems);

                var taskContent = (IObjectCollection)new CEnumerableObjectCollection();
                foreach (var shellLink in links)
                    taskContent.AddObject(ConvertShellLink(shellLink));

                customDestinationList.AddUserTasks((IObjectArray)taskContent);
                customDestinationList.CommitList();
            }
                #region Error handling
            catch (Exception ex)
            {
                Log.Warn(ex);
            }
            #endregion
        }

        /// <summary>
        /// Prevents a specific window from being pinned to the taskbar.
        /// </summary>
        /// <param name="hwnd">A handle to the window to prevent from being pinned.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
        public static void PreventPinning(IntPtr hwnd)
        {
            if (!WindowsUtils.IsWindows7) return;

            try
            {
                IPropertyStore propertyStore = GetWindowPropertyStore(hwnd);

                var preventPinningProperty = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 9);
                SetPropertyValue(propertyStore, preventPinningProperty, "1");

                Marshal.ReleaseComObject(propertyStore);
            }
                #region Error handling
            catch (Exception ex)
            {
                Log.Warn(ex);
            }
            #endregion
        }

        /// <summary>
        /// Converts a managed shell link structure to a COM object.
        /// </summary>
        private static IShellLinkW ConvertShellLink(ShellLink shellLink)
        {
            var nativeShellLink = (IShellLinkW)new CShellLink();
            var nativePropertyStore = (IPropertyStore)nativeShellLink;

            nativeShellLink.SetPath(shellLink.Path);
            if (!string.IsNullOrEmpty(shellLink.Arguments)) nativeShellLink.SetArguments(shellLink.Arguments);
            if (!string.IsNullOrEmpty(shellLink.IconPath)) nativeShellLink.SetIconLocation(shellLink.IconPath, shellLink.IconIndex);

            nativeShellLink.SetShowCmd(1); // Normal window state

            SetPropertyValue(nativePropertyStore, new PropertyKey(new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), 2), shellLink.Title);
            nativePropertyStore.Commit();

            return nativeShellLink;
        }

        /// <summary>
        /// Retrieves the property store for a window.
        /// </summary>
        /// <param name="hwnd">A handle to the window to retrieve the property store for.</param>
        private static IPropertyStore GetWindowPropertyStore(IntPtr hwnd)
        {
            IPropertyStore propStore;
            var guid = new Guid(PropertyStoreGuid);
            int rc = NativeMethods.SHGetPropertyStoreForWindow(hwnd, ref guid, out propStore);
            if (rc != 0) throw Marshal.GetExceptionForHR(rc);
            return propStore;
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <param name="propertyStore">The property store to set the property in.</param>
        /// <param name="property">The property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        private static void SetPropertyValue(IPropertyStore propertyStore, PropertyKey property, string value)
        {
            var variant = new PropertyVariant(value);
            propertyStore.SetValue(ref property, ref variant);
            variant.Dispose();
        }
    }
}

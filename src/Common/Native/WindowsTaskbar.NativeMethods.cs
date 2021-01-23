// Copyright Bastian Eicher
// Licensed under the MIT License

#pragma warning disable 0618

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NanoByte.Common.Native
{
    static partial class WindowsTaskbar
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local"), SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
        private struct PropertyKey
        {
            private Guid formatID;
            private int propertyID;

            public PropertyKey(Guid formatID, int propertyID)
            {
                this.formatID = formatID;
                this.propertyID = propertyID;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyVariant : IDisposable
        {
            private ushort valueType;
            private ushort wReserved1, wReserved2, wReserved3;
            private IntPtr valueData;
            private int valueDataExt;

            public PropertyVariant(string value)
            {
                valueType = (ushort)VarEnum.VT_LPWSTR;
                wReserved1 = 0;
                wReserved2 = 0;
                wReserved3 = 0;
                valueData = Marshal.StringToCoTaskMemUni(value);
                valueDataExt = 0;
            }

            public void Dispose()
            {
                PropertyVariant var = this;
                NativeMethods.PropVariantClear(ref var);

                valueType = (ushort)VarEnum.VT_EMPTY;
                wReserved1 = wReserved2 = wReserved3 = 0;
                valueData = IntPtr.Zero;
                valueDataExt = 0;
            }
        }

        private const string PropertyStoreGuid = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";

        [ComImport, Guid(PropertyStoreGuid), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyStore
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetCount([Out] out uint cProps);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetAt([In] uint iProp, out PropertyKey pkey);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetValue([In] ref PropertyKey key, out PropertyVariant pv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            [return: MarshalAs(UnmanagedType.I4)]
            int SetValue([In] ref PropertyKey key, [In] ref PropertyVariant pv);

            [PreserveSig]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Commit();
        }

        [ComImport, Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList4
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();

            [PreserveSig]
            void AddTab(IntPtr hwnd);

            [PreserveSig]
            void DeleteTab(IntPtr hwnd);

            [PreserveSig]
            void ActivateTab(IntPtr hwnd);

            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);

            [PreserveSig]
            void SetProgressState(IntPtr hwnd, ProgressBarState tbpFlags);

            [PreserveSig]
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);

            [PreserveSig]
            void UnregisterTab(IntPtr hwndTab);

            [PreserveSig]
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);

            [PreserveSig]
            void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);

            [PreserveSig]
            uint ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] ThumbnailButton[] pButtons);

            [PreserveSig]
            uint ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] ThumbnailButton[] pButtons);

            [PreserveSig]
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);

            [PreserveSig]
            void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

            [PreserveSig]
            void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

            [PreserveSig]
            void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);

            // ITaskbarList4
            void SetTabProperties(IntPtr hwndTab, StpFlag stpFlags);
        }

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        private struct ThumbnailButton
        {
            [MarshalAs(UnmanagedType.U4)]
            private ThumbnailMask dwMask;

            private uint iId;
            private uint iBitmap;
            private IntPtr hIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            private string szTip;

            [MarshalAs(UnmanagedType.U4)]
            private ThumbnailFlags dwFlags;
        }

        [ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090"), ClassInterface(ClassInterfaceType.None)]
        private class CTaskbarList
        {}

        [ComImport, Guid("6332DEBF-87B5-4670-90C0-5E57B408A49E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ICustomDestinationList
        {
            void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

            [PreserveSig]
            uint BeginList(out uint cMaxSlots, ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppvObject);

            [PreserveSig]
            uint AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string pszCategory, [MarshalAs(UnmanagedType.Interface)] IObjectArray poa);

            void AppendKnownCategory([MarshalAs(UnmanagedType.I4)] KnownDestinationCategory category);

            [PreserveSig]
            uint AddUserTasks([MarshalAs(UnmanagedType.Interface)] IObjectArray poa);

            void CommitList();
            void GetRemovedDestinations(ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppvObject);
            void DeleteList([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
            void AbortList();
        }

        private enum ThumbnailMask
        {
            Bitmap = 0x1,
            Icon = 0x2,
            Tooltip = 0x4,
            Flags = 0x8
        }

        [Flags]
        private enum ThumbnailFlags
        {
            Enabled = 0x0,
            ThbfDisabled = 0x1,
            ThbfDismissonclick = 0x2,
            ThbfNobackground = 0x4,
            ThbfHidden = 0x8,
            ThbfNoninteractive = 0x10
        }

        [Flags]
        private enum StpFlag
        {
            None = 0x0,
            UseThumbnailalways = 0x1,
            UseThumbnailWhenActive = 0x2,
            UsePeekAlways = 0x4,
            UsePeekWhenActive = 0x8
        }

        private enum KnownDestinationCategory
        {
            Frequent = 1,
            Recent
        }

        [ComImport, Guid("77F10CF0-3DB5-4966-B520-B7C54FD35ED6"), ClassInterface(ClassInterfaceType.None)]
        private class CDestinationList
        {}

        [ComImport, Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IObjectArray
        {
            void GetCount(out uint cObjects);
            void GetAt(uint iIndex, ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        }

        [ComImport, Guid("5632B1A4-E38A-400A-928A-D4CD63230295"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IObjectCollection
        {
            // IObjectArray
            [PreserveSig]
            void GetCount(out uint cObjects);

            [PreserveSig]
            void GetAt(uint iIndex, ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppvObject);

            // IObjectCollection
            void AddObject([MarshalAs(UnmanagedType.Interface)] object pvObject);
            void AddFromArray([MarshalAs(UnmanagedType.Interface)] IObjectArray poaSource);
            void RemoveObject(uint uiIndex);
            void Clear();
        }

        [ComImport, Guid("2D3468C1-36A7-43B6-AC24-D3F02FD9607A"), ClassInterface(ClassInterfaceType.None)]
        private class CEnumerableObjectCollection
        {}

        [ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotKey(out short wHotKey);
            void SetHotKey(short wHotKey);
            void GetShowCmd(out uint iShowCmd);
            void SetShowCmd(uint iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath, int cchIconPath, out int iIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
            void Resolve(IntPtr hwnd, uint fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport, Guid("00021401-0000-0000-C000-000000000046"), ClassInterface(ClassInterfaceType.None)]
        private class CShellLink
        {}

        private static readonly ITaskbarList4? _taskbarList;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Must perform COM call during init")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "COM calls throw unpredictable exceptions and this methods successful execution is not critical.")]
        static WindowsTaskbar()
        {
            if (WindowsUtils.IsWindows7)
            {
                try
                {
                    _taskbarList = (ITaskbarList4)new CTaskbarList();
                    _taskbarList.HrInit();
                }
                #region Error handling
                catch (Exception ex)
                {
                    Log.Warn(ex);
                }
                #endregion
            }
        }

        private static class NativeMethods
        {
            [DllImport("shell32", SetLastError = true)]
            public static extern int SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid iid, [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyStore propertyStore);

            [DllImport("ole32", PreserveSig = false)]
            public static extern void PropVariantClear([In, Out] ref PropertyVariant pvar);
        }
    }
}

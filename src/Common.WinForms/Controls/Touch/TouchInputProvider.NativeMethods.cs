// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;

namespace NanoByte.Common.Controls.Touch;

partial class TouchInputProvider
{
    private static class NativeMethods
    {
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPointerFrameInfoHistory(uint pointerId, ref uint entriesCount, ref uint pointerCount, [In, Out] PointerInfo[]? pointerInfo);

        public enum PointerInputType : uint
        {
            Pointer = 0x00000001,
            Touch = 0x00000002,
            Pen = 0x00000003,
            Mouse = 0x00000004,
            TouchPad = 0x00000005
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointerInfo
        {
            public PointerInputType pointerType;
            public uint pointerId;
            public uint frameId;
            public uint pointerFlags;
            public IntPtr sourceDevice;
            public IntPtr hwndTarget;
            public Point ptPixelLocation;
            public Point ptHimetricLocation;
            public Point ptPixelLocationRaw;
            public Point ptHimetricLocationRaw;
            public uint dwTime;
            public uint historyCount;
            public int inputData;
            public uint dwKeyStates;
            public ulong performanceCount;
            public int buttonChangeType;
        }

        [DllImport("ninput")]
        public static extern int CreateInteractionContext(out IntPtr interactionContext);

        [DllImport("ninput")]
        public static extern int DestroyInteractionContext(IntPtr interactionContext);

        [DllImport("ninput")]
        public static extern int SetPropertyInteractionContext(IntPtr interactionContext, InteractionContextProperty contextProperty, uint value);

        [DllImport("ninput")]
        public static extern int RegisterOutputCallbackInteractionContext(IntPtr interactionContext, InteractionOutputCallback outputCallback, IntPtr clientData);

        [DllImport("ninput")]
        public static extern int SetInteractionConfigurationInteractionContext(IntPtr interactionContext, uint configurationCount, [In] InteractionContextConfiguration[] configuration);

        [DllImport("ninput")]
        public static extern int AddPointerInteractionContext(IntPtr interactionContext, uint pointerId);

        [DllImport("ninput")]
        public static extern int RemovePointerInteractionContext(IntPtr interactionContext, uint pointerId);

        [DllImport("ninput")]
        public static extern int ProcessPointerFramesInteractionContext(IntPtr interactionContext, uint entriesCount, uint pointerCount, [In] PointerInfo[] pointerInfo);

        [DllImport("ninput")]
        public static extern int ProcessInertiaInteractionContext(IntPtr interactionContext);

        [DllImport("ninput")]
        public static extern int StopInteractionContext(IntPtr interactionContext);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void InteractionOutputCallback(IntPtr clientData, IntPtr output);

        public enum InteractionContextProperty : uint
        {
            MeasurementUnits = 1,
            InteractionUIFeedback = 2,
            FilterPointers = 3
        }

        /// <summary>Values for <see cref="InteractionContextProperty.MeasurementUnits"/>.</summary>
        public const uint MeasurementUnitsScreen = 1;

        public enum InteractionId : uint
        {
            None = 0,
            Manipulation = 1,
            Tap = 2,
            SecondaryTap = 3,
            Hold = 4,
            Drag = 5,
            CrossSlide = 6
        }

        [Flags]
        public enum InteractionFlags : uint
        {
            None = 0x0000,
            Begin = 0x0001,
            End = 0x0002,
            Cancel = 0x0004,
            Inertia = 0x0008
        }

        // Configuration flags are interpreted relative to the InteractionId, so values overlap between groups.
        public const uint ConfigManipulation = 0x00000001;
        public const uint ConfigManipulationTranslationX = 0x00000002;
        public const uint ConfigManipulationTranslationY = 0x00000004;
        public const uint ConfigManipulationRotation = 0x00000008;
        public const uint ConfigManipulationScaling = 0x00000010;
        public const uint ConfigManipulationTranslationInertia = 0x00000020;
        public const uint ConfigManipulationRotationInertia = 0x00000040;
        public const uint ConfigManipulationScalingInertia = 0x00000080;
        public const uint ConfigTap = 0x00000001;
        public const uint ConfigTapDouble = 0x00000002;

        [StructLayout(LayoutKind.Sequential)]
        public struct InteractionContextConfiguration
        {
            public InteractionId interactionId;
            public uint enable;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ManipulationTransform
        {
            public float translationX, translationY, scale, expansion, rotation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ManipulationVelocity
        {
            public float velocityX, velocityY, velocityExpansion, velocityAngular;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ManipulationArguments
        {
            public ManipulationTransform delta;
            public ManipulationTransform cumulative;
            public ManipulationVelocity velocity;
            public uint railsState;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InteractionContextOutput
        {
            [FieldOffset(0)] public InteractionId interactionId;
            [FieldOffset(4)] public InteractionFlags interactionFlags;
            [FieldOffset(8)] public PointerInputType inputType;
            [FieldOffset(12)] public float x;
            [FieldOffset(16)] public float y;
            [FieldOffset(20)] public ManipulationArguments manipulation;
            [FieldOffset(20)] public uint tapCount;
        }
    }
}

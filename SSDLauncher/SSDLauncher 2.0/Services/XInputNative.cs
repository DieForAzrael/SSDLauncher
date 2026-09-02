using System.Runtime.InteropServices;

namespace SSDLauncher_2._0.Services
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputGamepad
    {
        public ushort wButtons;
        public byte bLeftTrigger;
        public byte bRightTrigger;
        public short sThumbLX;
        public short sThumbLY;
        public short sThumbRX;
        public short sThumbRY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInputState
    {
        public uint dwPacketNumber;
        public XInputGamepad Gamepad;
    }

    internal static class XInputNative
    {
        public const ushort DPadUp = 0x0001;
        public const ushort DPadDown = 0x0002;
        public const ushort DPadLeft = 0x0004;
        public const ushort DPadRight = 0x0008;
        public const ushort ButtonA = 0x1000;
        public const ushort ButtonB = 0x2000;
        public const ushort ButtonY = 0x8000;

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
        public static extern int XInputGetState(int dwUserIndex, ref XInputState pState);
    }
}
using System;
using System.Runtime.InteropServices;

namespace Csharp_GTA_KeyAutomation.Input;

sealed class MacKeyboard : IKeyboard
{
    public void KeyDown(Key key) => Send(key, true);
    public void KeyUp(Key key) => Send(key, false);

    private static void Send(Key key, bool down)
    {
        ushort keyCode = Map(key);

        IntPtr evt = CGEventCreateKeyboardEvent(IntPtr.Zero, keyCode, down);
        CGEventPost(CGEventTapLocation.kCGHIDEventTap, evt);
        CFRelease(evt);
    }
private static ushort Map(Key key) => key switch
{
    // Control / navigation
    Key.Enter        => 0x24,
    Key.Escape      => 0x35,
    Key.Tab         => 0x30,
    Key.Space       => 0x31,
    Key.Backspace   => 0x33,
    Key.Delete      => 0x75,
    Key.Home        => 0x73,
    Key.End         => 0x77,
    Key.PageUp      => 0x74,
    Key.PageDown    => 0x79,

    // Arrows
    Key.Up           => 0x7E,
    Key.Down         => 0x7D,
    Key.Left         => 0x7B,
    Key.Right        => 0x7C,

    // Modifiers
    Key.LeftShift    => 0x38,
    Key.RightShift   => 0x3C,
    Key.LeftControl  => 0x3B,
    Key.RightControl => 0x3E,
    Key.LeftAlt      => 0x3A,
    Key.RightAlt     => 0x3D,
    Key.LeftMeta     => 0x37, // Command
    Key.RightMeta    => 0x36,
    Key.CapsLock     => 0x39,
    Key.Function     => 0x3F,

    // Letters
    Key.A => 0x00,
    Key.B => 0x0B,
    Key.C => 0x08,
    Key.D => 0x02,
    Key.E => 0x0E,
    Key.F => 0x03,
    Key.G => 0x05,
    Key.H => 0x04,
    Key.I => 0x22,
    Key.J => 0x26,
    Key.K => 0x28,
    Key.L => 0x25,
    Key.M => 0x2E,
    Key.N => 0x2D,
    Key.O => 0x1F,
    Key.P => 0x23,
    Key.Q => 0x0C,
    Key.R => 0x0F,
    Key.S => 0x01,
    Key.T => 0x11,
    Key.U => 0x20,
    Key.V => 0x09,
    Key.W => 0x0D,
    Key.X => 0x07,
    Key.Y => 0x10,
    Key.Z => 0x06,

    // Number row
    Key.D0 => 0x1D,
    Key.D1 => 0x12,
    Key.D2 => 0x13,
    Key.D3 => 0x14,
    Key.D4 => 0x15,
    Key.D5 => 0x17,
    Key.D6 => 0x16,
    Key.D7 => 0x1A,
    Key.D8 => 0x1C,
    Key.D9 => 0x19,

    // Symbols
    Key.Minus        => 0x1B,
    Key.Equals       => 0x18,
    Key.LeftBracket  => 0x21,
    Key.RightBracket => 0x1E,
    Key.Backslash    => 0x2A,
    Key.Semicolon    => 0x29,
    Key.Quote        => 0x27,
    Key.Comma        => 0x2B,
    Key.Period       => 0x2F,
    Key.Slash        => 0x2C,
    Key.Grave        => 0x32,

    // Function keys
    Key.F1  => 0x7A,
    Key.F2  => 0x78,
    Key.F3  => 0x63,
    Key.F4  => 0x76,
    Key.F5  => 0x60,
    Key.F6  => 0x61,
    Key.F7  => 0x62,
    Key.F8  => 0x64,
    Key.F9  => 0x65,
    Key.F10 => 0x6D,
    Key.F11 => 0x67,
    Key.F12 => 0x6F,
    Key.F13 => 0x69,
    Key.F14 => 0x6B,
    Key.F15 => 0x71,
    Key.F16 => 0x6A,
    Key.F17 => 0x40,
    Key.F18 => 0x4F,
    Key.F19 => 0x50,
    Key.F20 => 0x5A,

    // Numeric keypad
    Key.NumPad0        => 0x52,
    Key.NumPad1        => 0x53,
    Key.NumPad2        => 0x54,
    Key.NumPad3        => 0x55,
    Key.NumPad4        => 0x56,
    Key.NumPad5        => 0x57,
    Key.NumPad6        => 0x58,
    Key.NumPad7        => 0x59,
    Key.NumPad8        => 0x5B,
    Key.NumPad9        => 0x5C,
    Key.NumPadDecimal  => 0x41,
    Key.NumPadEnter    => 0x4C,
    Key.NumPadAdd      => 0x45,
    Key.NumPadSubtract => 0x4E,
    Key.NumPadMultiply => 0x43,
    Key.NumPadDivide   => 0x4B,
    Key.NumPadEquals   => 0x51,
    Key.NumPadClear    => 0x47,

    // Media / system (require special event handling in CGEvent)
    Key.VolumeUp       => 0x48,
    Key.VolumeDown     => 0x49,
    Key.Mute           => 0x4A,
    Key.BrightnessUp   => 0x72,
    Key.BrightnessDown => 0x73,
    Key.MediaPlayPause => 0x10, // NX_KEYTYPE_PLAY
    Key.MediaNext      => 0x11, // NX_KEYTYPE_NEXT
    Key.MediaPrevious  => 0x12, // NX_KEYTYPE_PREVIOUS

    _ => throw new ArgumentOutOfRangeException(nameof(key))
};


    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    static extern IntPtr CGEventCreateKeyboardEvent(
        IntPtr source,
        ushort virtualKey,
        bool keyDown
    );

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    static extern void CGEventPost(
        CGEventTapLocation tap,
        IntPtr @event
    );

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void CFRelease(IntPtr cf);

    enum CGEventTapLocation : uint
    {
        kCGHIDEventTap = 0
    }
}

using System;
using System.Runtime.InteropServices;

namespace Csharp_GTA_KeyAutomation.Input;

sealed class WindowsKeyboard : IKeyboard
{
    public void KeyDown(Key key)
    {
        byte vk = Map(key);
        keybd_event(vk, 0, 0, UIntPtr.Zero);
    }

    public void KeyUp(Key key)
    {
        byte vk = Map(key);
        keybd_event(vk, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    private static byte Map(Key key) => key switch
    {
        // Matches KeyMap.Resolve exactly
        Key.Enter     => VK_RETURN,      // CROSS
        Key.Backspace => VK_BACK,        // CIRCLE
        Key.C         => (byte)'C',      // TRIANGLE
        Key.Backslash => VK_OEM_5,       // SQUARE

        Key.Up    => VK_UP,
        Key.Down  => VK_DOWN,
        Key.Left  => VK_LEFT,
        Key.Right => VK_RIGHT,

        Key.O       => (byte)'O',        // OPTIONS
        Key.Escape => VK_ESCAPE,         // PSHOME
        Key.F       => (byte)'F',        // SHARE
        Key.T       => (byte)'T',        // TOUCHPAD
        Key.D       => (byte)'D',

        Key.D1 => (byte)'1',
        Key.D2 => (byte)'2',
        Key.D3 => (byte)'3',
        Key.D4 => (byte)'4',

        _ => throw new InvalidOperationException(
            $"Key '{key}' not supported on Windows."
        )
    };

    private const int KEYEVENTF_KEYUP = 0x0002;

    private const byte VK_BACK    = 0x08;
    private const byte VK_RETURN  = 0x0D;
    private const byte VK_ESCAPE  = 0x1B;

    private const byte VK_LEFT    = 0x25;
    private const byte VK_UP      = 0x26;
    private const byte VK_RIGHT   = 0x27;
    private const byte VK_DOWN    = 0x28;

    private const byte VK_OEM_5   = 0xDC; // \ |

    [DllImport("user32.dll")]
    private static extern void keybd_event(
        byte bVk,
        byte bScan,
        int dwFlags,
        UIntPtr dwExtraInfo
    );
}

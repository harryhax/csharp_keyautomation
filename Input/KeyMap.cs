using System;
using System.Collections.Generic;

namespace Csharp_GTA_KeyAutomation.Input;

static class KeyMap
{
    static readonly Dictionary<string, Key> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CROSS"] = Key.Enter,
        ["CIRCLE"] = Key.Backspace,
        ["TRIANGLE"] = Key.C,
        ["SQUARE"] = Key.Backslash,

        ["DPAD_UP"] = Key.Up,
        ["DPAD_DOWN"] = Key.Down,
        ["DPAD_LEFT"] = Key.Left,
        ["DPAD_RIGHT"] = Key.Right,


        ["OPTIONS"] = Key.O,
        ["PSHOME"] = Key.Escape,

        ["SHARE"] = Key.F,
        ["TOUCHPAD"] = Key.T,

        ["D"] = Key.D,

        ["L1"] = Key.D2,
        ["L2"] = Key.D1,
        ["R1"] = Key.D3,
        ["R2"] = Key.D4,


        ["LSTICK_LEFT"] = Key.LeftBracket,
        ["LSTICK_RIGHT"] = Key.RightBracket,

        ["RSTICK_LEFT"] = Key.Minus,
        ["RSTICK_RIGHT"] = Key.Equals,

    };

    public static Key Resolve(string name)
    {
        if (!Map.TryGetValue(name, out var key))
            throw new ArgumentException($"Unknown key name: {name}");

        return key;
    }
}

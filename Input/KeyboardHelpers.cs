using System.Threading;

namespace Csharp_GTA_KeyAutomation.Input;

static class KeyboardHelpers
{
    public static void KeyDownName(IKeyboard keyboard, string name)
    {
        keyboard.KeyDown(KeyMap.Resolve(name));
    }

    public static void KeyUpName(IKeyboard keyboard, string name)
    {
        keyboard.KeyUp(KeyMap.Resolve(name));
    }

    public static void TapName(IKeyboard keyboard, string name, int holdMs = 80)
    {
        Console.WriteLine("TAP: " + name + " Hold MS: " + holdMs);

        keyboard.KeyDown(KeyMap.Resolve(name));
        Thread.Sleep(holdMs);
        keyboard.KeyUp(KeyMap.Resolve(name));
    }
}

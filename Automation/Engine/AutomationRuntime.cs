using Csharp_GTA_KeyAutomation.Input;
using Csharp_GTA_KeyAutomation.Screen;
namespace Csharp_GTA_KeyAutomation.Automation.Engine;
public class AutomationRuntime
{
    public IScreenCapture Screen { get; }
    public IKeyboard Keyboard { get; }

    public AutomationRuntime(IScreenCapture screen, IKeyboard keyboard)
    {
        Screen = screen;
        Keyboard = keyboard;
    }
}


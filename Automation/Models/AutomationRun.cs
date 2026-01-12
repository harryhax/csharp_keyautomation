
namespace Csharp_GTA_KeyAutomation.Automation.Models;

public class AutomationRun
{
    public ScriptRepeat? Repeat { get; set; }
    public List<AutomationStep> Steps { get; set; } = new();
}
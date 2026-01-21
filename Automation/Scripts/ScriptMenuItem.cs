namespace Csharp_GTA_KeyAutomation.Automation.Models;

public enum ScriptMenuItemType
{
    Header,
    Script
}

public sealed class ScriptMenuItem
{
    public ScriptMenuItemType Type { get; init; }

    public string Title { get; init; } = "";

    public string? FilePath { get; init; }

    public bool IsCustom { get; init; }
}

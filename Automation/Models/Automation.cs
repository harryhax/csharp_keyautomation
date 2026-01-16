using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Csharp_GTA_KeyAutomation.Automation.Models;

public class AutomationRoot
{
    public Dictionary<string, AutomationScript> Scripts { get; set; } = new();
}

public class AutomationScriptFile
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    public AutomationScript? Script { get; set; }
}

public class AutomationScript
{
    public List<AutomationStep>? Setup { get; set; }
    public AutomationLoop? Loop { get; set; }
}

public class AutomationLoop
{
    public ScriptRepeat? Repeat { get; set; }
    public List<AutomationStep> Steps { get; set; } = new();
}

public class ScriptRepeat
{
    public string Mode { get; set; } = "fixed"; // fixed | prompt
    public int Default { get; set; } = 1;
}

public class AutomationStep
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";

    // keyboard
    public string? Key { get; set; }
    public int HoldMs { get; set; } = 80;
    public int SleepAfterMs { get; set; } = 0;

    // wait
    public int Ms { get; set; }

    // loop (step-level loop stays intact)
    public int Count { get; set; }
    public List<AutomationStep>? Steps { get; set; }

    // image
    public string? Template { get; set; }
    public double MinMatch { get; set; }
    public int MaxAttempts { get; set; }
    public int PollDelayMs { get; set; }
}

using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Csharp_GTA_KeyAutomation.Automation.Models;

namespace Csharp_GTA_KeyAutomation.Automation.Parsing;

public static class ScriptLoader
{
    public static List<AutomationScriptFile> LoadScripts(string folder)
    {
        var scripts = new List<AutomationScriptFile>();

        if (!Directory.Exists(folder))
            return scripts;

        foreach (var file in Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories))
        {
            var json = File.ReadAllText(file);

            var script = JsonSerializer.Deserialize(
                json,
                ScriptJsonContext.Default.AutomationScriptFile
            );

            if (script == null || string.IsNullOrWhiteSpace(script.Title))
                continue;

            scripts.Add(script);
        }

        return scripts;
    }
}

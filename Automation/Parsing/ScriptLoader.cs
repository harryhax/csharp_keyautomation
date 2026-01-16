using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Csharp_GTA_KeyAutomation.Automation.Models;

namespace Csharp_GTA_KeyAutomation.Automation.Parsing;

public static class ScriptLoader
{
    public static List<ScriptDescriptor> LoadScriptDescriptors(string folder)
    {
        var scripts = new List<ScriptDescriptor>();

        if (!Directory.Exists(folder))
            return scripts;

        foreach (var file in Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories))
        {
            using var stream = File.OpenRead(file);
            using var doc = JsonDocument.Parse(stream);

            if (!doc.RootElement.TryGetProperty("title", out var titleProp))
                continue;

            var title = titleProp.GetString();
            if (string.IsNullOrWhiteSpace(title))
                continue;

            var description =
                doc.RootElement.TryGetProperty("description", out var descProp)
                    ? descProp.GetString() ?? ""
                    : "";

            scripts.Add(new ScriptDescriptor
            {
                Title = title,
                Description = description,
                FilePath = file
            });
        }

        return scripts;
    }

    public static AutomationScriptFile LoadFullScript(string filePath)
    {
        var json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize(
            json,
            ScriptJsonContext.Default.AutomationScriptFile
        )!;
    }
}

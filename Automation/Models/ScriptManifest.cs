using System.Collections.Generic;

namespace Csharp_GTA_KeyAutomation.Automation.Models;

public sealed class ScriptManifest
{
    public List<ScriptManifestEntry> Scripts { get; set; } = new();
}

public sealed class ScriptManifestEntry
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Path { get; set; } = "";
    public string Version { get; set; } = "";
}

public sealed class ScriptMeta
{
    public string Id { get; set; } = "";
    public string Version { get; set; } = "";
}

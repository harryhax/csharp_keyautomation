using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Csharp_GTA_KeyAutomation.Automation.Models;

namespace Csharp_GTA_KeyAutomation.Automation.Scripts;

public static class ScriptRepositorySync
{
    private static readonly HttpClient Http = new();

    public static string BaseUrl = "";
    public static bool Debug = false;

    private const string ManifestFile = "manifest.json";

    public static async Task SyncAsync(string localRoot)
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
            throw new InvalidOperationException("ScriptRepositorySync.BaseUrl is not set.");

        Directory.CreateDirectory(localRoot);

        var manifestUrl = Combine(BaseUrl, ManifestFile);

        Log("---- Script repository sync ----");
        Log($"BaseUrl: {BaseUrl}");
        Log($"Manifest URL: {manifestUrl}");

        var manifestJson = await Http.GetStringAsync(manifestUrl);

        var manifest = JsonSerializer.Deserialize<ScriptManifest>(
            manifestJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
            ?? throw new Exception("Failed to parse manifest.json");

        Log($"Scripts in manifest: {manifest.Scripts.Count}");

        foreach (var script in manifest.Scripts)
            await SyncScript(script, localRoot);

        Log("---- Script sync complete ----");
    }
    private static async Task SyncScript(ScriptManifestEntry script, string localRoot)
    {
        Log("----------------------------------");
        Log($"Script: {script.Title}");
        Log($"Path: {script.Path}");

        // normalize manifest path
        var manifestPath = script.Path
            .Replace('\\', '/')
            .TrimStart('/');

        bool pathIsFile =
            manifestPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase);

        string remoteFilePath;
        string localDirectory;
        string localFileName;

        if (pathIsFile)
        {
            // manifest explicitly points to file
            remoteFilePath = manifestPath;

            localDirectory = Path.Combine(
                localRoot,
                Path.GetDirectoryName(manifestPath)!
                    .Replace('/', Path.DirectorySeparatorChar)
            );

            localFileName = Path.GetFileName(manifestPath);
        }
        else
        {
            // manifest points to folder
            if (!manifestPath.EndsWith("/"))
                manifestPath += "/";

            remoteFilePath = manifestPath + "index.json";

            localDirectory = Path.Combine(
                localRoot,
                manifestPath.Replace('/', Path.DirectorySeparatorChar)
            );

            localFileName = "index.json";
        }

        // ✅ THIS DIRECTORY WILL NEVER CONTAIN A FILE NAME
        Directory.CreateDirectory(localDirectory);

        var localFilePath = Path.Combine(localDirectory, localFileName);
        var remoteUrl = Combine(BaseUrl, remoteFilePath);

        Log($"Remote URL: {remoteUrl}");
        Log($"Local file: {localFilePath}");

        var json = await Http.GetStringAsync(remoteUrl);
        await File.WriteAllTextAsync(localFilePath, json);

        Console.WriteLine($"Updated script: {script.Title}");
    }


    private static string? ReadLocalVersion(string metaPath)
    {
        if (!File.Exists(metaPath))
            return null;

        try
        {
            var json = File.ReadAllText(metaPath);
            var meta = JsonSerializer.Deserialize<ScriptMeta>(json);
            return meta?.Version;
        }
        catch
        {
            return null;
        }
    }

    private static string Combine(string a, string b)
        => a.TrimEnd('/') + "/" + b.TrimStart('/');

    private static void Log(string msg)
    {
        if (Debug)
            Console.WriteLine("[ScriptSync] " + msg);
    }
}

using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Csharp_GTA_KeyAutomation.Infrastructure.Updates;

public sealed class UpdateCheckResult
{
    public bool UpdateAvailable { get; init; }
    public Version? LocalVersion { get; init; }
    public Version? RemoteVersion { get; init; }
    public string? ReleaseName { get; init; }
    public string? ReleaseNotes { get; init; }
}

public static class UpdateChecker
{
    private static readonly HttpClient Http = new();

    public static async Task<UpdateCheckResult> CheckAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var repo = config["Update:Repository"];
        var allowPrerelease = bool.Parse(
            config["Update:AllowPrerelease"] ?? "true"
        );

        if (string.IsNullOrWhiteSpace(repo))
            throw new Exception("Update:Repository not set in appsettings.json");

        var releasesUrl =
            $"https://api.github.com/repos/{repo}/releases";

        if (!Http.DefaultRequestHeaders.UserAgent.Any())
            Http.DefaultRequestHeaders.UserAgent.ParseAdd("harrykey-updater");

        var json = await Http.GetStringAsync(releasesUrl);

        using var doc = JsonDocument.Parse(json);

        Version? localVersion =
            Assembly.GetExecutingAssembly()
                .GetName()
                .Version;

        Version? remoteVersion = null;
        string? name = null;
        string? notes = null;

        foreach (var release in doc.RootElement.EnumerateArray())
        {
            if (release.GetProperty("draft").GetBoolean())
                continue;

            bool prerelease = release.GetProperty("prerelease").GetBoolean();

            if (!allowPrerelease && prerelease)
                continue;

            var tag = release.GetProperty("tag_name").GetString();

            if (string.IsNullOrWhiteSpace(tag))
                continue;

            tag = tag.TrimStart('v', 'V');

            if (!Version.TryParse(tag, out var parsed))
                continue;

            remoteVersion = parsed;
            name = release.GetProperty("name").GetString();
            notes = release.GetProperty("body").GetString();
            break;
        }

        if (remoteVersion == null || localVersion == null)
        {
            return new UpdateCheckResult
            {
                UpdateAvailable = false,
                LocalVersion = localVersion
            };
        }

        return new UpdateCheckResult
        {
            LocalVersion = localVersion,
            RemoteVersion = remoteVersion,
            ReleaseName = name,
            ReleaseNotes = notes,
            UpdateAvailable = remoteVersion > localVersion
        };
    }
}

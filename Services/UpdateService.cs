using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using ConverPro.Models;

namespace ConverPro.Services;

public sealed class UpdateService(HttpClient httpClient)
{
    public async Task<UpdateManifest?> CheckAsync(CancellationToken cancellationToken = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        var settingsPath = Path.Combine(AppContext.BaseDirectory, "update-settings.json");
        if (!File.Exists(settingsPath)) return null;
        var settings = JsonSerializer.Deserialize<UpdateSettings>(await File.ReadAllTextAsync(settingsPath, timeout.Token));
        if (string.IsNullOrWhiteSpace(settings?.ManifestUrl) || settings.ManifestUrl.Contains("SEU_USUARIO")) return null;
        var json = await httpClient.GetStringAsync(settings.ManifestUrl, timeout.Token);
        var manifest = JsonSerializer.Deserialize<UpdateManifest>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (!Version.TryParse(manifest?.Version, out var remote)) return null;
        var current = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0);
        return remote > current ? manifest : null;
    }

    public static void OpenDownload(UpdateManifest manifest)
    {
        if (!Uri.TryCreate(manifest.DownloadUrl, UriKind.Absolute, out _)) return;
        Process.Start(new ProcessStartInfo(manifest.DownloadUrl) { UseShellExecute = true });
    }
}

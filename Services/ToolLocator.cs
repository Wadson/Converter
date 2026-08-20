using System.Diagnostics;
using System.IO.Compression;

namespace ConverPro.Services;

public sealed class ToolLocator
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromMinutes(5) };
    private static readonly SemaphoreSlim DownloadGate = new(1, 1);
    private string UserTools => Path.Combine(FileSystem.AppDataDirectory, "Tools");
    public string? Find(string name)
    {
        var local = Path.Combine(AppContext.BaseDirectory, "Tools", name);
        if (File.Exists(local)) return local;

        var appData = Path.Combine(FileSystem.AppDataDirectory, "Tools", name);
        if (File.Exists(appData)) return appData;

        var path = Environment.GetEnvironmentVariable("PATH") ?? "";
        return path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(folder => Path.Combine(folder.Trim(), name))
            .FirstOrDefault(File.Exists);
    }

    public async Task<(string YtDlp, string? Deno)> EnsureYoutubeToolsAsync(
        IProgress<Models.OperationProgress>? progress, CancellationToken cancellationToken)
    {
        await DownloadGate.WaitAsync(cancellationToken);
        try
        {
            Directory.CreateDirectory(UserTools);
            var ytDlp = Path.Combine(UserTools, "yt-dlp.exe");
            var deno = Path.Combine(UserTools, "deno.exe");
            var stamp = Path.Combine(UserTools, "youtube-tools.updated");
            var update = !File.Exists(ytDlp) || !File.Exists(stamp) ||
                         DateTime.UtcNow - File.GetLastWriteTimeUtc(stamp) > TimeSpan.FromHours(24);

            if (update)
            {
                progress?.Report(new(0, "Atualizando o motor de download..."));
                await DownloadAsync("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe", ytDlp, cancellationToken);
                File.WriteAllText(stamp, DateTime.UtcNow.ToString("O"));
            }

            if (!File.Exists(deno))
            {
                progress?.Report(new(0, "Preparando o suporte JavaScript do YouTube..."));
                var zip = Path.Combine(UserTools, "deno.zip");
                await DownloadAsync("https://github.com/denoland/deno/releases/latest/download/deno-x86_64-pc-windows-msvc.zip", zip, cancellationToken);
                using var archive = ZipFile.OpenRead(zip);
                var entry = archive.GetEntry("deno.exe") ?? throw new InvalidDataException("O pacote do Deno é inválido.");
                entry.ExtractToFile(deno, true);
                File.Delete(zip);
            }
            return (ytDlp, File.Exists(deno) ? deno : null);
        }
        catch
        {
            var fallback = Find("yt-dlp.exe");
            if (fallback is null) throw;
            return (fallback, File.Exists(Path.Combine(UserTools, "deno.exe")) ? Path.Combine(UserTools, "deno.exe") : null);
        }
        finally { DownloadGate.Release(); }
    }

    private static async Task DownloadAsync(string url, string destination, CancellationToken cancellationToken)
    {
        var temporary = destination + ".download";
        using var response = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using (var input = await response.Content.ReadAsStreamAsync(cancellationToken))
        await using (var output = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await input.CopyToAsync(output, cancellationToken);
            await output.FlushAsync(cancellationToken);
        }
        File.Move(temporary, destination, true);
    }

    public async Task InstallWithWingetAsync(IProgress<Models.OperationProgress>? progress,
        CancellationToken cancellationToken)
    {
        var runner = new ProcessRunner();
        progress?.Report(new(0, "Instalando yt-dlp..."));
        await runner.RunAsync("winget", ["install", "--id", "yt-dlp.yt-dlp", "-e", "--accept-source-agreements", "--accept-package-agreements"], progress, cancellationToken);
        progress?.Report(new(50, "Instalando FFmpeg..."));
        await runner.RunAsync("winget", ["install", "--id", "Gyan.FFmpeg", "-e", "--accept-source-agreements", "--accept-package-agreements"], progress, cancellationToken);
        progress?.Report(new(100, "Ferramentas instaladas. Reinicie o ConverPro se elas ainda não forem detectadas."));
    }
}

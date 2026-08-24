using System.Diagnostics;
using System.IO.Compression;

namespace ConverPro.Services;

public enum MediaTool { YtDlp, Ffmpeg, Deno }
public sealed record ToolState(MediaTool Tool, string Name, string? Path, string? Version, bool IsValid, string Status);
public sealed record ToolInstallProgress(MediaTool Tool, string Stage, double Progress, long BytesReceived, long? TotalBytes, string Message);

public sealed class ToolLocator(HttpClient http)
{
    private static readonly SemaphoreSlim InstallGate = new(1, 1);
    private string UserTools => Path.Combine(FileSystem.AppDataDirectory, "Tools");

    public string? Find(string name)
    {
        var privatePath = Path.Combine(UserTools, name);
        if (File.Exists(privatePath)) return privatePath;
        var bundled = Path.Combine(AppContext.BaseDirectory, "Tools", name);
        if (File.Exists(bundled)) return bundled;
        var path = Environment.GetEnvironmentVariable("PATH") ?? "";
        return path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(folder => Path.Combine(folder.Trim(), name)).FirstOrDefault(File.Exists);
    }

    public async Task<ToolState> DetectAsync(MediaTool tool, CancellationToken token = default)
    {
        var name = ToolName(tool);
        var path = Find(FileName(tool));
        if (path is null) return new(tool, name, null, null, false, "Não encontrado");
        try
        {
            var version = await ReadVersionAsync(path, tool, token);
            return new(tool, name, path, version, true, "Detectado");
        }
        catch { return new(tool, name, path, null, false, "Inválido ou corrompido"); }
    }

    public async Task<ToolState> InstallAsync(MediaTool tool, IProgress<ToolInstallProgress> progress, CancellationToken token)
    {
        await InstallGate.WaitAsync(token);
        var staging = Path.Combine(UserTools, ".staging-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(staging);
            Directory.CreateDirectory(UserTools);
            progress.Report(new(tool, "Conectando", 0, 0, null, $"Conectando à fonte oficial do {ToolName(tool)}..."));
            var archive = Path.Combine(staging, tool == MediaTool.YtDlp ? "yt-dlp.download" : "package.zip.download");
            await DownloadAsync(DownloadUrl(tool), archive, tool, progress, token);
            progress.Report(new(tool, "Validando", .82, new FileInfo(archive).Length, new FileInfo(archive).Length, "Validando o pacote recebido..."));
            if (new FileInfo(archive).Length < 10_000) throw new InvalidDataException("O pacote recebido está vazio ou incompleto.");

            var candidates = new List<(string Source, string Destination)>();
            if (tool == MediaTool.YtDlp)
            {
                var executable = Path.Combine(staging, "yt-dlp.exe");
                File.Move(archive, executable);
                candidates.Add((executable, Path.Combine(UserTools, "yt-dlp.exe")));
            }
            else
            {
                progress.Report(new(tool, "Extraindo", .88, 0, null, "Extraindo somente os executáveis necessários..."));
                using var zip = ZipFile.OpenRead(archive);
                var required = tool == MediaTool.Deno ? new[] { "deno.exe" } : new[] { "ffmpeg.exe", "ffprobe.exe" };
                foreach (var file in required)
                {
                    var entry = zip.Entries.FirstOrDefault(item => item.FullName.Replace('\\', '/').EndsWith('/' + file, StringComparison.OrdinalIgnoreCase) || string.Equals(item.FullName, file, StringComparison.OrdinalIgnoreCase))
                        ?? throw new InvalidDataException($"O pacote não contém {file}.");
                    if (entry.Length < 10_000) throw new InvalidDataException($"O arquivo {file} é inválido.");
                    var extracted = Path.Combine(staging, file);
                    await using var input = entry.Open(); await using var output = new FileStream(extracted, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
                    await input.CopyToAsync(output, token);
                    candidates.Add((extracted, Path.Combine(UserTools, file)));
                }
            }

            progress.Report(new(tool, "Validando", .94, 0, null, "Executando a verificação de versão..."));
            await ReadVersionAsync(candidates[0].Source, tool, token);
            if (tool == MediaTool.Ffmpeg && candidates.Count > 1)
                await ReadVersionAsync(candidates[1].Source, MediaTool.Ffmpeg, token);
            progress.Report(new(tool, "Instalando", .97, 0, null, "Instalando de forma segura..."));
            foreach (var candidate in candidates) AtomicInstall(candidate.Source, candidate.Destination);
            var state = await DetectAsync(tool, token);
            if (!state.IsValid) throw new InvalidDataException("A ferramenta instalada não passou na validação final.");
            progress.Report(new(tool, "Concluído", 1, 0, null, $"{state.Name} {state.Version} instalado com sucesso."));
            return state;
        }
        finally
        {
            try { if (Directory.Exists(staging)) Directory.Delete(staging, true); } catch { }
            InstallGate.Release();
        }
    }

    public async Task<(string YtDlp, string? Deno)> EnsureYoutubeToolsAsync(IProgress<Models.OperationProgress>? progress, CancellationToken token)
    {
        var yt = await DetectAsync(MediaTool.YtDlp, token);
        if (!yt.IsValid)
        {
            var adapter = new Progress<ToolInstallProgress>(value => progress?.Report(new(value.Progress * 100, value.Message)));
            yt = await InstallAsync(MediaTool.YtDlp, adapter, token);
        }
        var deno = await DetectAsync(MediaTool.Deno, token);
        if (!deno.IsValid)
        {
            var adapter = new Progress<ToolInstallProgress>(value => progress?.Report(new(value.Progress * 100, value.Message)));
            deno = await InstallAsync(MediaTool.Deno, adapter, token);
        }
        return (yt.Path!, deno.IsValid ? deno.Path : null);
    }

    private async Task DownloadAsync(string url, string destination, MediaTool tool, IProgress<ToolInstallProgress> progress, CancellationToken token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        response.EnsureSuccessStatusCode();
        var total = response.Content.Headers.ContentLength;
        await using var input = await response.Content.ReadAsStreamAsync(token);
        await using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
        var buffer = new byte[81920]; long received = 0; int read;
        while ((read = await input.ReadAsync(buffer, token)) > 0)
        {
            await output.WriteAsync(buffer.AsMemory(0, read), token); received += read;
            var ratio = total is > 0 ? Math.Clamp(received / (double)total.Value, 0, 1) : 0;
            progress.Report(new(tool, "Baixando", .05 + ratio * .75, received, total, $"Baixando {ToolName(tool)}..."));
        }
        await output.FlushAsync(token);
    }

    private static async Task<string> ReadVersionAsync(string executable, MediaTool tool, CancellationToken token)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token); timeout.CancelAfter(TimeSpan.FromSeconds(12));
        using var process = new Process { StartInfo = new() { FileName = executable, UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true } };
        process.StartInfo.ArgumentList.Add(tool == MediaTool.Ffmpeg ? "-version" : "--version");
        if (!process.Start()) throw new InvalidOperationException("Não foi possível executar a ferramenta.");
        var stdout = process.StandardOutput.ReadLineAsync(timeout.Token); var stderr = process.StandardError.ReadLineAsync(timeout.Token);
        await process.WaitForExitAsync(timeout.Token);
        var line = (await stdout) ?? (await stderr);
        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(line)) throw new InvalidDataException("A ferramenta não respondeu corretamente.");
        return tool == MediaTool.Ffmpeg ? line.Replace("ffmpeg version ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries)[0] : line.Trim();
    }

    private static void AtomicInstall(string source, string destination)
    {
        var backup = destination + ".previous";
        if (File.Exists(backup)) File.Delete(backup);
        if (File.Exists(destination)) File.Move(destination, backup);
        try { File.Move(source, destination); if (File.Exists(backup)) File.Delete(backup); }
        catch { if (File.Exists(destination)) File.Delete(destination); if (File.Exists(backup)) File.Move(backup, destination); throw; }
    }

    private static string DownloadUrl(MediaTool tool) => tool switch
    {
        MediaTool.YtDlp => "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe",
        MediaTool.Deno => "https://github.com/denoland/deno/releases/latest/download/deno-x86_64-pc-windows-msvc.zip",
        _ => "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip"
    };
    private static string FileName(MediaTool tool) => tool switch { MediaTool.YtDlp => "yt-dlp.exe", MediaTool.Ffmpeg => "ffmpeg.exe", _ => "deno.exe" };
    private static string ToolName(MediaTool tool) => tool switch { MediaTool.YtDlp => "yt-dlp", MediaTool.Ffmpeg => "FFmpeg", _ => "Deno" };
}

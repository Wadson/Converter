using ConverPro.Models;

namespace ConverPro.Services;

public sealed class MediaService(ToolLocator tools, ProcessRunner runner)
{
    public async Task DownloadAsync(string url, MediaOptions options,
        IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
    {
        var youtubeTools = await tools.EnsureYoutubeToolsAsync(progress, cancellationToken);
        var ytdlp = youtubeTools.YtDlp;
        Directory.CreateDirectory(options.OutputDirectory);
        var args = new List<string> { "--newline", "--windows-filenames", "--remote-components", "ejs:github", "-P", options.OutputDirectory };
        if (youtubeTools.Deno is not null)
            args.AddRange(["--js-runtimes", $"deno:{youtubeTools.Deno}"]);
        var ffmpeg = tools.Find("ffmpeg.exe");
        if (ffmpeg is not null) args.AddRange(["--ffmpeg-location", ffmpeg]);

        if (options.Kind == DownloadKind.Audio)
        {
            args.AddRange(["-x", "--audio-format", options.AudioFormat.ToLowerInvariant(),
                "--audio-quality", options.AudioBitrate.Replace(" kbps", "K"), "-o", "%(title)s [%(id)s].%(ext)s"]);
        }
        else
        {
            var height = new string(options.VideoQuality.TakeWhile(char.IsDigit).ToArray());
            var format = string.IsNullOrEmpty(height) ? "bv*+ba/b" : $"bv*[height<={height}]+ba/b[height<={height}]";
            args.AddRange(["-f", format, "--merge-output-format", "mp4", "-o", "%(title)s [%(id)s].%(ext)s"]);
        }

        args.Add(url);
        await runner.RunAsync(ytdlp, args, progress, cancellationToken);
    }

    public Task ConvertToAudioAsync(string input, string outputDirectory, string format, string bitrate,
        IProgress<OperationProgress>? progress, CancellationToken cancellationToken, string? outputName = null)
    {
        var ffmpeg = Require("ffmpeg.exe", "FFmpeg");
        Directory.CreateDirectory(outputDirectory);
        var extension = format.ToLowerInvariant();
        var output = UniquePath(outputDirectory, outputName ?? Path.GetFileNameWithoutExtension(input), extension);
        var codecArgs = extension switch
        {
            "wav" => new[] { "-c:a", "pcm_s16le" },
            "flac" => new[] { "-c:a", "flac" },
            "ogg" => new[] { "-c:a", "libvorbis", "-b:a", bitrate.Replace(" kbps", "k") },
            "aac" or "m4a" => new[] { "-c:a", "aac", "-b:a", bitrate.Replace(" kbps", "k") },
            _ => new[] { "-c:a", "libmp3lame", "-b:a", bitrate.Replace(" kbps", "k") }
        };
        return runner.RunAsync(ffmpeg, ["-y", "-i", input, "-vn", .. codecArgs, "-progress", "pipe:2", output], progress, cancellationToken);
    }

    public Task CompressMp3Async(string input, string outputDirectory, string bitrate,
        IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
    {
        var ffmpeg = Require("ffmpeg.exe", "FFmpeg");
        Directory.CreateDirectory(outputDirectory);
        var output = UniquePath(outputDirectory, Path.GetFileNameWithoutExtension(input) + $"_{bitrate.Replace(" kbps", "k")}", "mp3");
        return runner.RunAsync(ffmpeg, ["-y", "-i", input, "-codec:a", "libmp3lame", "-b:a", bitrate.Replace(" kbps", "k"), "-progress", "pipe:2", output], progress, cancellationToken);
    }

    public Task EditAsync(string input, string output, string operation, string start, string end, double volume,
        IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
    {
        var ffmpeg = Require("ffmpeg.exe", "FFmpeg");
        var args = operation switch
        {
            "segment" => new[] { "-y", "-i", input, "-ss", start, "-to", end, "-af", $"volume={volume.ToString(System.Globalization.CultureInfo.InvariantCulture)}", output },
            "volume" => new[] { "-y", "-i", input, "-af", $"volume={volume.ToString(System.Globalization.CultureInfo.InvariantCulture)}", output },
            _ => new[] { "-y", "-i", input, "-filter_complex", $"[0:a]atrim=0:{start},asetpts=PTS-STARTPTS[a0];[0:a]atrim=start={end},asetpts=PTS-STARTPTS[a1];[a0][a1]concat=n=2:v=0:a=1[out]", "-map", "[out]", output }
        };
        return runner.RunAsync(ffmpeg, args, progress, cancellationToken);
    }

    private string Require(string fileName, string label) => tools.Find(fileName)
        ?? throw new FileNotFoundException($"{label} não foi encontrado. Clique em ‘Preparar ferramentas’ antes de continuar.");

    private static string UniquePath(string directory, string name, string extension)
    {
        var invalid = Path.GetInvalidFileNameChars();
        name = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray()).Trim().TrimEnd('.');
        if (string.IsNullOrWhiteSpace(name)) name = "midia";
        var candidate = Path.Combine(directory, $"{name}.{extension}");
        for (var index = 2; File.Exists(candidate); index++) candidate = Path.Combine(directory, $"{name} ({index}).{extension}");
        return candidate;
    }
}

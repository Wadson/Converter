using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using ConverPro.Models;
using System.Collections.Concurrent;

namespace ConverPro.Services;

public sealed partial class ProcessRunner
{
    public async Task RunAsync(string executable, IEnumerable<string> arguments,
        IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
    {
        var recentOutput = new ConcurrentQueue<string>();
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executable,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            },
            EnableRaisingEvents = true
        };

        foreach (var argument in arguments)
            process.StartInfo.ArgumentList.Add(argument);

        process.OutputDataReceived += (_, e) => Report(e.Data, progress, recentOutput);
        process.ErrorDataReceived += (_, e) => Report(e.Data, progress, recentOutput);

        if (!process.Start())
            throw new InvalidOperationException($"Não foi possível iniciar {executable}.");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await using var registration = cancellationToken.Register(() =>
        {
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); } catch { }
        });

        await process.WaitForExitAsync(cancellationToken);
        if (process.ExitCode != 0)
        {
            var detail = string.Join(Environment.NewLine, recentOutput
                .Where(line => line.Contains("ERROR", StringComparison.OrdinalIgnoreCase) || line.Contains("Error:", StringComparison.OrdinalIgnoreCase))
                .TakeLast(4));
            if (string.IsNullOrWhiteSpace(detail)) detail = string.Join(Environment.NewLine, recentOutput.TakeLast(4));
            throw new InvalidOperationException($"Não foi possível processar a mídia.{Environment.NewLine}{detail}".Trim());
        }
    }

    private static void Report(string? line, IProgress<OperationProgress>? progress, ConcurrentQueue<string> output)
    {
        if (string.IsNullOrWhiteSpace(line)) return;
        output.Enqueue(line.Trim());
        while (output.Count > 30) output.TryDequeue(out _);
        var match = PercentRegex().Match(line);
        var percent = match.Success && double.TryParse(match.Groups[1].Value,
            NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : -1;
        progress?.Report(new OperationProgress(percent, line.Trim()));
    }

    [GeneratedRegex(@"(?<!\d)(\d{1,3}(?:\.\d+)?)%")]
    private static partial Regex PercentRegex();
}

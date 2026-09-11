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
        long lastActivityTicks = DateTime.UtcNow.Ticks;
        var stdoutClosed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var stderrClosed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
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

        DataReceivedEventHandler outputHandler = (_, e) => Report(e.Data, progress, recentOutput, () => Interlocked.Exchange(ref lastActivityTicks, DateTime.UtcNow.Ticks), stdoutClosed);
        DataReceivedEventHandler errorHandler = (_, e) => Report(e.Data, progress, recentOutput, () => Interlocked.Exchange(ref lastActivityTicks, DateTime.UtcNow.Ticks), stderrClosed);
        process.OutputDataReceived += outputHandler;
        process.ErrorDataReceived += errorHandler;

        if (!process.Start())
            throw new InvalidOperationException($"Não foi possível iniciar {executable}.");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var exitTask = process.WaitForExitAsync(linked.Token);
            while (!exitTask.IsCompleted)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), linked.Token);
                if (DateTime.UtcNow - new DateTime(Interlocked.Read(ref lastActivityTicks), DateTimeKind.Utc) > TimeSpan.FromMinutes(5))
                {
                    KillTree(process);
                    throw new TimeoutException($"{Path.GetFileName(executable)} não apresentou atividade por 5 minutos.");
                }
            }
            await exitTask;
            await Task.WhenAll(stdoutClosed.Task.WaitAsync(TimeSpan.FromSeconds(10)), stderrClosed.Task.WaitAsync(TimeSpan.FromSeconds(10)));
            if (process.ExitCode != 0) throw new InvalidOperationException(BuildFailure(recentOutput));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            KillTree(process);
            try { await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(10)); }
            catch (TimeoutException) { throw new TimeoutException("O processo externo não encerrou após o cancelamento."); }
            throw;
        }
        finally
        {
            process.OutputDataReceived -= outputHandler;
            process.ErrorDataReceived -= errorHandler;
        }
    }

    private static void Report(string? line, IProgress<OperationProgress>? progress, ConcurrentQueue<string> output,
        Action markActivity, TaskCompletionSource closed)
    {
        if (line is null) { closed.TrySetResult(); return; }
        if (string.IsNullOrWhiteSpace(line)) return;
        markActivity();
        output.Enqueue(line.Trim());
        while (output.Count > 30) output.TryDequeue(out _);
        var match = DownloadPercentRegex().Match(line);
        var percent = match.Success && double.TryParse(match.Groups[1].Value,
            NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : -1;
        var message = percent >= 0 ? "Baixando mídia" : line.Contains("[Merger]", StringComparison.OrdinalIgnoreCase) ? "Mesclando áudio e vídeo"
            : line.Contains("Extracting audio", StringComparison.OrdinalIgnoreCase) ? "Convertendo áudio" : line.Trim();
        progress?.Report(new OperationProgress(percent, message));
    }

    private static void KillTree(Process process)
    {
        try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
        catch (InvalidOperationException ex) { Debug.WriteLine($"A árvore do processo já foi encerrada: {ex.Message}"); }
        catch (System.ComponentModel.Win32Exception ex) { Debug.WriteLine($"Não foi possível encerrar a árvore do processo: {ex.Message}"); }
    }

    private static string BuildFailure(ConcurrentQueue<string> output)
    {
        var details = output.Where(x => x.Contains("error", StringComparison.OrdinalIgnoreCase)).TakeLast(4).ToArray();
        if (details.Length == 0) details = output.TakeLast(4).ToArray();
        return $"Não foi possível processar a mídia.{(details.Length == 0 ? "" : Environment.NewLine + string.Join(Environment.NewLine, details))}";
    }

    [GeneratedRegex(@"^\[download\]\s+(\d{1,3}(?:\.\d+)?)%", RegexOptions.IgnoreCase)]
    private static partial Regex DownloadPercentRegex();
}

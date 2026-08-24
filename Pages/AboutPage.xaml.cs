using System.Runtime.InteropServices;
using ConverPro.Services;

namespace ConverPro.Pages;

public partial class AboutPage : ContentPage
{
    private readonly ToolLocator tools; private readonly UpdateService updates;
    private CancellationTokenSource? operation; private MediaTool? retryTool;
    public AboutPage(ToolLocator tools, UpdateService updates)
    {
        InitializeComponent(); this.tools = tools; this.updates = updates;
        VersionLabel.Text = $"Sistema: ConverPro · Versão {AppInfo.Current.VersionString}";
        BuildLabel.Text = $"Build: {AppInfo.Current.BuildString}";
        PlatformLabel.Text = $"Plataforma: {DeviceInfo.Current.Platform}";
        OsLabel.Text = $"Sistema operacional: {DeviceInfo.Current.VersionString}";
        ArchitectureLabel.Text = $"Arquitetura: processo {RuntimeInformation.ProcessArchitecture} · sistema {RuntimeInformation.OSArchitecture}";
        FrameworkLabel.Text = $"Tecnologia: .NET MAUI · {RuntimeInformation.FrameworkDescription}";
        Loaded += async (_, _) => await RefreshStatesAsync();
        Unloaded += (_, _) => operation?.Cancel();
    }

    private async Task RefreshStatesAsync()
    {
        SetChecking(YtButton, YtStatus); SetChecking(FfmpegButton, FfmpegStatus); SetChecking(DenoButton, DenoStatus);
        var states = await Task.WhenAll(tools.DetectAsync(MediaTool.YtDlp), tools.DetectAsync(MediaTool.Ffmpeg), tools.DetectAsync(MediaTool.Deno));
        foreach (var state in states) ApplyState(state);
    }

    private void ApplyState(ToolState state)
    {
        var (status, version, button) = Controls(state.Tool);
        status.Text = state.Status; status.TextColor = Color.FromArgb(state.IsValid ? "#15803D" : "#B45309");
        version.Text = state.IsValid ? $"Versão {state.Version}" : "Versão indisponível";
        button.Text = state.IsValid ? "Atualizar" : state.Path is null ? "Baixar" : "Reparar"; button.IsEnabled = true;
    }

    private async Task InstallAsync(MediaTool tool)
    {
        operation?.Cancel(); operation?.Dispose(); operation = new(); retryTool = null; RetryButton.IsVisible = false;
        var (_, _, button) = Controls(tool); button.IsEnabled = false; button.Text = "Aguarde...";
        try
        {
            var progress = new Progress<ToolInstallProgress>(ShowProgress);
            var state = await tools.InstallAsync(tool, progress, operation.Token); ApplyState(state);
        }
        catch (OperationCanceledException) { ProgressStage.Text = "Operação cancelada com segurança."; await RefreshStatesAsync(); }
        catch (Exception ex) { retryTool = tool; RetryButton.IsVisible = true; ProgressStage.Text = $"Falha: {Friendly(ex)}"; ProgressStage.TextColor = Color.FromArgb("#DC2626"); ApplyState(await tools.DetectAsync(tool)); }
    }

    private void ShowProgress(ToolInstallProgress value)
    {
        InstallProgress.Progress = value.Progress; ProgressPercent.Text = $"{value.Progress:P0}";
        ProgressStage.Text = $"{value.Stage}: {value.Message}"; ProgressStage.TextColor = Color.FromArgb("#64748B");
        ProgressBytes.Text = value.TotalBytes is > 0 ? $"{FormatBytes(value.BytesReceived)} de {FormatBytes(value.TotalBytes.Value)}" : value.BytesReceived > 0 ? $"{FormatBytes(value.BytesReceived)} baixados" : "";
    }

    private (Label Status, Label Version, Button Button) Controls(MediaTool tool) => tool switch
    {
        MediaTool.YtDlp => (YtStatus, YtVersion, YtButton), MediaTool.Ffmpeg => (FfmpegStatus, FfmpegVersion, FfmpegButton), _ => (DenoStatus, DenoVersion, DenoButton)
    };
    private static void SetChecking(Button button, Label status) { button.IsEnabled = false; button.Text = "Verificando..."; status.Text = "Verificando"; status.TextColor = Color.FromArgb("#64748B"); }
    private static string FormatBytes(long bytes) => bytes >= 1_048_576 ? $"{bytes / 1_048_576d:0.0} MB" : $"{bytes / 1024d:0.0} KB";
    private static string Friendly(Exception ex) => ex is HttpRequestException ? "não foi possível acessar a fonte oficial. Verifique a internet e tente novamente." : ex.Message;
    private async void InstallYt(object? s, EventArgs e) => await InstallAsync(MediaTool.YtDlp);
    private async void InstallFfmpeg(object? s, EventArgs e) => await InstallAsync(MediaTool.Ffmpeg);
    private async void InstallDeno(object? s, EventArgs e) => await InstallAsync(MediaTool.Deno);
    private async void Retry(object? s, EventArgs e) { if (retryTool is { } tool) await InstallAsync(tool); }
    private async void Refresh(object? s, EventArgs e) => await RefreshStatesAsync();
    private async void CheckUpdate(object? s, EventArgs e)
    {
        try { var update = await updates.CheckAsync(); if (update is null) { ProgressStage.Text = "O ConverPro está atualizado ou o canal ainda não foi configurado."; return; } if (await DisplayAlertAsync("Atualização disponível", $"A versão {update.Version} está disponível. Deseja abrir o download?", "Abrir", "Depois")) UpdateService.OpenDownload(update); }
        catch (Exception ex) { ProgressStage.Text = $"Não foi possível verificar o ConverPro: {Friendly(ex)}"; }
    }
}

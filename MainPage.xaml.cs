using System.Collections.ObjectModel;
using ConverPro.Models;
using ConverPro.Services;

namespace ConverPro;

public partial class MainPage : ContentPage
{
    private readonly MediaService _media; private readonly UpdateService _updates;
    private CancellationTokenSource? _operation;
    private readonly ObservableCollection<string> _urls = []; private readonly ObservableCollection<string> _convert = []; private readonly ObservableCollection<string> _compress = [];

    public MainPage(MediaService media, UpdateService updates)
    {
        InitializeComponent(); _media = media; _updates = updates;
        UrlList.ItemsSource = _urls; ConvertList.ItemsSource = _convert; CompressList.ItemsSource = _compress;
        SetPicker(DownloadKindPicker, 0, "Vídeo MP4", "Áudio MP3");
        SetPicker(DownloadQualityPicker, 3, "4320p (8K)", "2160p (4K)", "1440p", "1080p", "720p", "480p", "360p", "Melhor disponível");
        SetPicker(DownloadAudioPicker, 4, "320 kbps", "256 kbps", "192 kbps", "160 kbps", "128 kbps", "96 kbps");
        SetPicker(ConvertBitratePicker, 2, "320 kbps", "256 kbps", "192 kbps", "160 kbps", "128 kbps", "96 kbps");
        SetPicker(CompressBitratePicker, 3, "32 kbps", "64 kbps", "80 kbps", "128 kbps", "256 kbps");
        SetPicker(VolumePicker, 2, "50%", "75%", "100%", "125%", "150%");
        OutputEntry.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "ConvertPro");
        VersionLabel.Text = $"Versão {AppInfo.Current.VersionString}"; AboutVersionLabel.Text = $"Versão {AppInfo.Current.VersionString} • © 2026 WR Soft";
        SelectPanel(DownloadPanel, DownloadNav);
    }

    protected override async void OnAppearing() { base.OnAppearing(); if (!Preferences.Default.Get("checked_update", false)) { Preferences.Default.Set("checked_update", true); await CheckForUpdate(true); } }
    private void ShowDownload(object? s, EventArgs e) => SelectPanel(DownloadPanel, DownloadNav);
    private void ShowConvert(object? s, EventArgs e) => SelectPanel(ConvertPanel, ConvertNav);
    private void ShowCompress(object? s, EventArgs e) => SelectPanel(CompressPanel, CompressNav);
    private void ShowEditor(object? s, EventArgs e) => SelectPanel(EditorPanel, EditorNav);
    private void ShowAbout(object? s, EventArgs e) => SelectPanel(AboutPanel, AboutNav);
    private void SelectPanel(View panel, Border selected)
    {
        foreach (var view in new[] { DownloadPanel, ConvertPanel, CompressPanel, EditorPanel, AboutPanel }) view.IsVisible = view == panel;
        foreach (var item in new[] { DownloadNav, ConvertNav, CompressNav, EditorNav, AboutNav }) { item.BackgroundColor = Colors.Transparent; SetNavColor(item, Color.FromArgb("#B9C6DB")); }
        selected.BackgroundColor = Color.FromArgb("#5965E9"); SetNavColor(selected, Colors.White);
    }
    private void DownloadKindChanged(object? s, EventArgs e) { bool audio = DownloadKindPicker.SelectedIndex == 1; DownloadAudioPicker.IsVisible = audio; DownloadQualityPicker.IsVisible = !audio; }
    private async void AddUrl(object? s, EventArgs e) { var value = UrlEntry.Text?.Trim(); if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https")) { await DisplayAlertAsync("Link inválido", "Informe um link válido de vídeo ou playlist.", "OK"); return; } if (!_urls.Contains(value!)) _urls.Add(value!); UrlEntry.Text = ""; }
    private void RemoveUrl(object? s, EventArgs e) { if (s is Button { CommandParameter: string value }) _urls.Remove(value); }

    private async void PickConvertFiles(object? s, EventArgs e) { var files = await FilePicker.Default.PickMultipleAsync(new PickOptions { PickerTitle = "Selecione vídeos", FileTypes = Types(".mp4", ".mkv", ".avi", ".mov", ".webm") }); if (files is null) return; foreach (var file in files) { var path = file.FullPath; if (!string.IsNullOrWhiteSpace(path) && !_convert.Contains(path)) _convert.Add(path); } }
    private async void PickCompressFiles(object? s, EventArgs e) { var files = await FilePicker.Default.PickMultipleAsync(new PickOptions { PickerTitle = "Selecione arquivos MP3", FileTypes = Types(".mp3") }); if (files is null) return; foreach (var file in files) { var path = file.FullPath; if (!string.IsNullOrWhiteSpace(path) && !_compress.Contains(path)) _compress.Add(path); } }
    private async void PickEditorFile(object? s, EventArgs e) { var file = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Selecione um áudio", FileTypes = Types(".mp3", ".wav", ".flac", ".m4a", ".ogg") }); EditorFileEntry.Text = file?.FullPath ?? ""; }
    private static FilePickerFileType Types(params string[] extensions) => new(new Dictionary<DevicePlatform, IEnumerable<string>> { [DevicePlatform.WinUI] = extensions });

    private async void StartDownload(object? s, EventArgs e)
    {
        if (_urls.Count == 0) { await DisplayAlertAsync("Fila vazia", "Adicione pelo menos um link.", "OK"); return; }
        await RunQueue(_urls.ToList(), async (url, ct) => { var audio = DownloadKindPicker.SelectedIndex == 1; await _media.DownloadAsync(url, new(audio ? DownloadKind.Audio : DownloadKind.Video, Selected(DownloadQualityPicker), "MP3", Selected(DownloadAudioPicker), Output()), CreateProgress(), ct); }, "Downloads concluídos.");
    }
    private async void StartConvert(object? s, EventArgs e) { if (_convert.Count == 0) { await DisplayAlertAsync("Fila vazia", "Selecione um ou mais vídeos.", "OK"); return; } await RunQueue(_convert.ToList(), (f, ct) => _media.ConvertToAudioAsync(f, Output(), "MP3", Selected(ConvertBitratePicker), CreateProgress(), ct), "Conversões concluídas."); }
    private async void StartCompress(object? s, EventArgs e) { if (_compress.Count == 0) { await DisplayAlertAsync("Fila vazia", "Selecione um ou mais arquivos MP3.", "OK"); return; } await RunQueue(_compress.ToList(), (f, ct) => _media.CompressMp3Async(f, Output(), Selected(CompressBitratePicker), CreateProgress(), ct), "Compressões concluídas."); }
    private async Task RunQueue(IReadOnlyList<string> items, Func<string, CancellationToken, Task> action, string success)
    {
        await RunOperation(async ct => { for (int i = 0; i < items.Count; i++) { StatusLabel.Text = $"Processando {i + 1} de {items.Count}: {Path.GetFileName(items[i])}"; await action(items[i], ct); OperationProgressBar.Progress = (i + 1d) / items.Count; } }, success);
    }

    private async void ExportSegment(object? s, EventArgs e) => await RunEditor("segment", "_trecho");
    private async void RemoveSegment(object? s, EventArgs e) => await RunEditor("remove", "_editado");
    private async void ApplyVolume(object? s, EventArgs e) => await RunEditor("volume", "_volume");
    private async Task RunEditor(string operation, string suffix)
    {
        if (!File.Exists(EditorFileEntry.Text) || !TimeSpan.TryParse(StartTimeEntry.Text, out _) || !TimeSpan.TryParse(EndTimeEntry.Text, out _)) { await DisplayAlertAsync("Dados inválidos", "Escolha um áudio e informe os tempos no formato hh:mm:ss.", "OK"); return; }
        Directory.CreateDirectory(Output()); var output = Path.Combine(Output(), Path.GetFileNameWithoutExtension(EditorFileEntry.Text) + suffix + ".mp3");
        double volume = double.Parse(Selected(VolumePicker).TrimEnd('%')) / 100d;
        await RunOperation(ct => _media.EditAsync(EditorFileEntry.Text, output, operation, StartTimeEntry.Text, EndTimeEntry.Text, volume, CreateProgress(), ct), "Edição concluída.");
    }

    private async Task RunOperation(Func<CancellationToken, Task> action, string success)
    {
        if (_operation != null) return; _operation = new(); ProgressArea.IsVisible = true; OperationProgressBar.Progress = 0; StatusLabel.Text = "Iniciando...";
        try { Directory.CreateDirectory(Output()); await action(_operation.Token); OperationProgressBar.Progress = 1; StatusLabel.Text = success; await DisplayAlertAsync("Concluído", $"{success}\n\nPasta: {Output()}", "OK"); }
        catch (OperationCanceledException) { StatusLabel.Text = "Operação cancelada."; }
        catch (Exception ex) { StatusLabel.Text = ex.Message; await DisplayAlertAsync("Não foi possível concluir", ex.Message, "OK"); }
        finally { _operation.Dispose(); _operation = null; }
    }
    private IProgress<OperationProgress> CreateProgress() => new Progress<OperationProgress>(p => { if (p.Percent >= 0) OperationProgressBar.Progress = Math.Clamp(p.Percent / 100, 0, 1); StatusLabel.Text = p.Message; });
    private void CancelOperation(object? s, EventArgs e) => _operation?.Cancel();
    private async void PickOutputFolder(object? s, EventArgs e) { var picker = new Windows.Storage.Pickers.FolderPicker(); picker.FileTypeFilter.Add("*"); var window = Application.Current?.Windows[0].Handler?.PlatformView as Microsoft.UI.Xaml.Window; if (window == null) return; WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(window)); var folder = await picker.PickSingleFolderAsync(); if (folder != null) OutputEntry.Text = folder.Path; }

    private async void CheckUpdate(object? s, EventArgs e) => await CheckForUpdate(false);
    private async Task CheckForUpdate(bool silent) { try { var info = await _updates.CheckAsync(); if (info == null) { if (!silent) await DisplayAlertAsync("Atualizações", "Você já usa a versão mais recente.", "OK"); return; } if (await DisplayAlertAsync($"Nova versão {info.Version}", info.Notes, "Baixar", "Depois")) UpdateService.OpenDownload(info); } catch (Exception ex) { if (!silent) await DisplayAlertAsync("Atualizações", ex.Message, "OK"); } }
    private string Output() => string.IsNullOrWhiteSpace(OutputEntry.Text) ? FileSystem.AppDataDirectory : OutputEntry.Text.Trim();
    private static string Selected(Picker picker) => picker.SelectedItem?.ToString() ?? "";
    private static void SetPicker(Picker picker, int selected, params string[] values) { foreach (var value in values) picker.Items.Add(value); picker.SelectedIndex = selected; }
    private static void SetNavColor(Border item, Color color) { if (item.Content is Grid grid) foreach (var label in grid.Children.OfType<Label>()) label.TextColor = color; }
}

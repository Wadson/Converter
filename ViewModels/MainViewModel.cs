using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ConverPro.Models;
using ConverPro.Services;

namespace ConverPro.ViewModels;

public sealed class MainViewModel(YoutubeDownloadService youtube, MediaService media) : INotifyPropertyChanged, IDisposable
{
    private CancellationTokenSource? _operation;
    private bool _isBusy;
    private bool _isPaused;
    private double _progress;
    private string _status = "Pronto.";
    private string _counter = "0/0";
    private string _outputDirectory = DefaultOutput();

    public ObservableCollection<MediaQueueItem> Downloads { get; } = [];
    public ObservableCollection<MediaQueueItem> Conversions { get; } = [];
    public ObservableCollection<MediaQueueItem> Compressions { get; } = [];
    public ObservableCollection<ActivityLog> Logs { get; } = [];
    public IReadOnlyList<string> VideoQualities { get; } = ["4320p", "2160p", "1440p", "1080p", "720p", "480p", "360p", "Melhor disponível"];
    public IReadOnlyList<string> Bitrates { get; } = ["32 kbps", "64 kbps", "80 kbps", "96 kbps", "128 kbps", "160 kbps", "192 kbps", "256 kbps", "320 kbps"];
    public IReadOnlyList<string> AudioFormats { get; } = ["MP3", "AAC", "WAV", "FLAC", "OGG", "M4A"];
    public IReadOnlyList<string> DownloadKinds { get; } = ["Vídeo MP4", "Áudio MP3"];

    public int DownloadKindIndex { get; set; }
    public string SelectedVideoQuality { get; set; } = "1080p";
    public string SelectedDownloadBitrate { get; set; } = "192 kbps";
    public string SelectedConvertBitrate { get; set; } = "192 kbps";
    public string SelectedCompressBitrate { get; set; } = "128 kbps";
    public string SelectedAudioFormat { get; set; } = "MP3";
    public string OutputDirectory { get => _outputDirectory; set => Set(ref _outputDirectory, value); }
    public bool IsBusy { get => _isBusy; private set { if (Set(ref _isBusy, value)) { OnChanged(nameof(CanResume)); NotifyQueueState(); } } }
    public bool IsPaused { get => _isPaused; private set { if (Set(ref _isPaused, value)) OnChanged(nameof(CanResume)); } }
    public bool CanResume => IsPaused && !IsBusy;
    public bool CanStartDownloads => !IsBusy && Downloads.Any(x => x.IsSelected && x.Status is QueueItemStatus.Pending or QueueItemStatus.Failed);
    public bool CanStartConversions => !IsBusy && Conversions.Any(x => x.IsSelected && x.Status is QueueItemStatus.Pending or QueueItemStatus.Failed);
    public bool CanStartCompressions => !IsBusy && Compressions.Any(x => x.IsSelected && x.Status is QueueItemStatus.Pending or QueueItemStatus.Failed);
    public double Progress { get => _progress; private set => Set(ref _progress, value); }
    public string Status { get => _status; private set => Set(ref _status, value); }
    public string Counter { get => _counter; private set => Set(ref _counter, value); }

    public async Task AddYoutubeUrlAsync(string url, Func<Task<bool?>> choosePlaylist, CancellationToken token = default)
    {
        var parsed = youtube.Parse(url);
        if (parsed.IsPlaylist)
        {
            var choice = await choosePlaylist(); // true = playlist, false = current video, null = cancelled
            if (choice is null) return;
            if (choice.Value)
            {
                Status = "Carregando playlist...";
                var playlist = await youtube.GetPlaylistAsync(parsed.PlaylistId!, token);
                foreach (var item in playlist.Videos.Where(v => Downloads.All(x => x.Source != v.Source)))
                {
                    Downloads.Add(new MediaQueueItem(item.Source, item.Title) { PlaylistName = playlist.Title });
                }
                Status = $"Playlist carregada: {playlist.Videos.Count} vídeos.";
                NotifyQueueState();
                return;
            }
        }
        if (parsed.VideoId is null) throw new ArgumentException("O link não contém um vídeo válido.");
        var video = await youtube.GetVideoAsync(url, token);
        if (Downloads.All(x => x.Source != video.Source)) Downloads.Add(video);
        NotifyQueueState();
    }

    public void AddFiles(IEnumerable<string> paths, ObservableCollection<MediaQueueItem> target)
    {
        foreach (var path in paths.Where(File.Exists))
            if (target.All(x => !string.Equals(x.Source, path, StringComparison.OrdinalIgnoreCase)))
                target.Add(new(path, Path.GetFileName(path)));
        NotifyQueueState();
    }

    public void SelectAll(ObservableCollection<MediaQueueItem> source, bool selected) { foreach (var item in source) item.IsSelected = selected; NotifyQueueState(); }
    public void RemoveSelected(ObservableCollection<MediaQueueItem> source)
    {
        if (IsBusy) return;
        foreach (var item in source.Where(x => x.IsSelected).ToList()) source.Remove(item);
        NotifyQueueState();
    }
    public void RetryFailures() { foreach (var item in Downloads.Concat(Conversions).Concat(Compressions).Where(x => x.Status == QueueItemStatus.Failed)) { item.Status = QueueItemStatus.Pending; item.Error = null; item.Progress = 0; } NotifyQueueState(); }
    public void ClearLogs() => Logs.Clear();

    public Task<bool> StartDownloadsAsync() => RunQueueAsync(Downloads, async (item, progress, token) =>
    {
        var folder = item.PlaylistName is { Length: > 0 } playlist ? Path.Combine(OutputDirectory, YoutubeDownloadService.Sanitize(playlist)) : OutputDirectory;
        var kind = DownloadKindIndex == 1 ? DownloadKind.Audio : DownloadKind.Video;
        await youtube.DownloadAsync(item, new(kind, SelectedVideoQuality, "MP3", SelectedDownloadBitrate, folder), progress, token);
    });

    public Task<bool> StartConversionsAsync() => RunQueueAsync(Conversions, (item, progress, token) =>
        media.ConvertToAudioAsync(item.Source, OutputDirectory, SelectedAudioFormat, SelectedConvertBitrate, progress, token));

    public Task<bool> StartCompressionsAsync() => RunQueueAsync(Compressions, (item, progress, token) =>
        media.CompressMp3Async(item.Source, OutputDirectory, SelectedCompressBitrate, progress, token));

    public void Stop()
    {
        if (!IsBusy) return;
        IsPaused = true;
        Status = "Pausando com segurança...";
        _operation?.Cancel();
    }

    private async Task<bool> RunQueueAsync(ObservableCollection<MediaQueueItem> source,
        Func<MediaQueueItem, IProgress<OperationProgress>, CancellationToken, Task> action)
    {
        if (IsBusy) return false;
        var items = source.Where(x => x.IsSelected && x.Status is QueueItemStatus.Pending or QueueItemStatus.Running or QueueItemStatus.Failed).ToList();
        if (items.Count == 0) { Status = "Nenhum item pendente selecionado."; return false; }
        Directory.CreateDirectory(OutputDirectory);
        _operation = new(); IsBusy = true; IsPaused = false;
        var total = items.Count;
        var completed = false;
        try
        {
            for (var index = 0; index < total; index++)
            {
                var item = items[index];
                item.Status = QueueItemStatus.Running; item.Error = null; item.Progress = 0; NotifyQueueState();
                Counter = $"{index + 1}/{total}";
                var itemIndex = index;
                var progress = new Progress<OperationProgress>(p => MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (p.Percent >= 0) { item.Progress = Math.Clamp(p.Percent / 100d, 0, 1); Progress = Math.Clamp((itemIndex + item.Progress) / total, 0, 1); }
                    Status = $"Processando {Counter}: {p.Message}";
                }));
                try
                {
                    await action(item, progress, _operation.Token);
                    item.Status = QueueItemStatus.Completed; item.Progress = 1;
                    if (ReferenceEquals(source, Compressions))
                    {
                        var output = Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(item.Source) + $"_{SelectedCompressBitrate.Replace(" kbps", "k")}.mp3");
                        if (File.Exists(output)) item.OutputSize = new FileInfo(output).Length;
                    }
                    AddLog(OperationName(source), item.Title, "Concluído", "Processamento concluído com sucesso.");
                    Progress = (index + 1d) / total;
                }
                catch (OperationCanceledException) when (_operation.IsCancellationRequested)
                {
                    item.Status = QueueItemStatus.Pending;
                    throw;
                }
                catch (Exception ex)
                {
                    item.Status = QueueItemStatus.Failed; item.Error = ex.Message;
                    AddLog(OperationName(source), item.Title, "Falhou", "Não foi possível processar este item.", ex.ToString());
                }
            }
            Status = items.Any(x => x.Status == QueueItemStatus.Failed) ? "Fila concluída com algumas falhas." : "Fila concluída.";
            completed = true;
        }
        catch (OperationCanceledException) { Status = IsPaused ? "Operação pausada. Use Continuar." : "Operação cancelada."; }
        finally
        {
            _operation.Dispose(); _operation = null; IsBusy = false; NotifyQueueState();
        }
        return completed;
    }

    private void AddLog(string operation, string item, string status, string message, string? details = null) => MainThread.BeginInvokeOnMainThread(() =>
    {
        Logs.Insert(0, new(DateTime.Now, operation, item, status, message, details));
        while (Logs.Count > 100) Logs.RemoveAt(0);
    });

    private static string OperationName(ObservableCollection<MediaQueueItem> source) => source.FirstOrDefault()?.Source.StartsWith("http", StringComparison.OrdinalIgnoreCase) == true ? "Download" : source.FirstOrDefault()?.Source.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) == true ? "Compressão" : "Conversão";
    private void NotifyQueueState() { OnChanged(nameof(CanStartDownloads)); OnChanged(nameof(CanStartConversions)); OnChanged(nameof(CanStartCompressions)); }

    private static string DefaultOutput()
    {
        var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        return Directory.Exists(downloads) ? Path.Combine(downloads, "Converter") : Path.Combine(FileSystem.AppDataDirectory, "Converter");
    }

    public void Dispose() { _operation?.Cancel(); _operation?.Dispose(); }
    public event PropertyChangedEventHandler? PropertyChanged;
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    { if (EqualityComparer<T>.Default.Equals(field, value)) return false; field = value; OnChanged(name); return true; }
    private void OnChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
}

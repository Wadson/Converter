using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConverPro.Models;

public enum QueueItemStatus { Pending, Running, Completed, Failed }

public sealed class MediaQueueItem(string source, string title) : INotifyPropertyChanged
{
    private bool _isSelected = true;
    private QueueItemStatus _status;
    private string? _error;
    private double _progress;
    private long? _outputSize;

    public string Source { get; } = source;
    public string Title { get; } = title;
    public string? PlaylistName { get; init; }
    public long? OriginalSize { get; init; } = File.Exists(source) ? new FileInfo(source).Length : null;
    public long? OutputSize { get => _outputSize; set { if (Set(ref _outputSize, value)) OnChanged(nameof(SizeSummary)); } }
    public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }
    public QueueItemStatus Status { get => _status; set { if (Set(ref _status, value)) OnChanged(nameof(StatusText)); } }
    public string? Error { get => _error; set => Set(ref _error, value); }
    public double Progress { get => _progress; set => Set(ref _progress, Math.Clamp(value, 0, 1)); }
    public string SizeSummary => OriginalSize is null ? "" : OutputSize is null
        ? $"Original: {FormatSize(OriginalSize.Value)}"
        : $"{FormatSize(OriginalSize.Value)} → {FormatSize(OutputSize.Value)} ({100 - OutputSize.Value * 100d / OriginalSize.Value:0.#}% menor)";
    public string StatusText => Status switch
    {
        QueueItemStatus.Running => "Processando",
        QueueItemStatus.Completed => "Concluído",
        QueueItemStatus.Failed => "Falhou",
        _ => "Pendente"
    };

    private static string FormatSize(long bytes) => bytes >= 1_048_576 ? $"{bytes / 1_048_576d:0.##} MB" : $"{bytes / 1024d:0.##} KB";

    public event PropertyChangedEventHandler? PropertyChanged;
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value; OnChanged(name); return true;
    }
    private void OnChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
}

public sealed record ActivityLog(DateTime Time, string Operation, string Item, string Status, string Message, string? TechnicalDetails = null);

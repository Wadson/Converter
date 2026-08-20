namespace ConverPro.Models;

public enum DownloadKind { Video, Audio }

public sealed record MediaOptions(
    DownloadKind Kind,
    string VideoQuality,
    string AudioFormat,
    string AudioBitrate,
    string OutputDirectory);

public sealed record OperationProgress(double Percent, string Message);


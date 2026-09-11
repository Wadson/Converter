using ConverPro.Models;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace ConverPro.Services;

public sealed record YoutubeLinkInfo(bool IsPlaylist, string? PlaylistId, string? VideoId);
public sealed record YoutubePlaylistInfo(string Title, IReadOnlyList<MediaQueueItem> Videos);

public sealed class YoutubeDownloadService(YoutubeClient youtube, MediaService media)
{
    public YoutubeLinkInfo Parse(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            !(uri.Host.EndsWith("youtube.com", StringComparison.OrdinalIgnoreCase) ||
              uri.Host.EndsWith("youtu.be", StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Informe um link válido do YouTube.");

        PlaylistId? playlist = null;
        VideoId? video = null;
        playlist = PlaylistId.TryParse(url);
        video = VideoId.TryParse(url);
        return new(playlist is not null, playlist?.Value, video?.Value);
    }

    public async Task<YoutubePlaylistInfo> GetPlaylistAsync(string playlistId, CancellationToken token)
    {
        var playlist = await youtube.Playlists.GetAsync(playlistId, token);
        var items = new List<MediaQueueItem>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await foreach (var video in youtube.Playlists.GetVideosAsync(playlistId, token))
        {
            token.ThrowIfCancellationRequested();
            var id = VideoId.TryParse(video.Url)?.Value;
            if (id is not null && ids.Add(id)) items.Add(new MediaQueueItem(CanonicalVideoUrl(id), video.Title));
        }
        if (items.Count == 0) throw new InvalidOperationException("A playlist não contém vídeos disponíveis.");
        return new(Sanitize(playlist.Title), items);
    }

    public async Task<MediaQueueItem> GetVideoAsync(string url, CancellationToken token)
    {
        var video = await youtube.Videos.GetAsync(url, token);
        return new(CanonicalVideoUrl(video.Id.Value), video.Title);
    }

    public async Task DownloadAsync(MediaQueueItem item, MediaOptions options,
        IProgress<OperationProgress>? progress, CancellationToken token)
    {
        // YoutubeExplode is used above for typed parsing and metadata. yt-dlp is deliberately
        // used for transfer: its extractor and format fallbacks are updated more frequently.
        await media.DownloadAsync(item.Source, options, progress, token);
    }

    public static string Sanitize(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var result = new string(value.Select(c => invalid.Contains(c) ? '_' : c).ToArray()).Trim().TrimEnd('.');
        return string.IsNullOrWhiteSpace(result) ? "YouTube" : result;
    }

    public static string CanonicalVideoUrl(string videoId) => $"https://www.youtube.com/watch?v={Uri.EscapeDataString(videoId)}";

}

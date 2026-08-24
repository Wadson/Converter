using System.Text.Json.Serialization;

namespace ConverPro.Models;

public sealed class UpdateManifest
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("downloadUrl")]
    public string DownloadUrl { get; set; } = "";

    [JsonPropertyName("notes")]
    public string Notes { get; set; } = "";

    [JsonPropertyName("mandatory")]
    public bool Mandatory { get; set; }

    [JsonPropertyName("versao")]
    public string LegacyVersion { set => Version = value; }
    [JsonPropertyName("url")]
    public string LegacyUrl { set => DownloadUrl = value; }
    [JsonPropertyName("descricao")]
    public string LegacyNotes { set => Notes = value; }
}

public sealed class UpdateSettings
{
    [JsonPropertyName("manifestUrl")]
    public string ManifestUrl { get; set; } = "";
}

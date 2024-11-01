using System.Text.Json.Serialization;

namespace LodeKennes.Extensions.Scaleway.SecretManager.Models;

public sealed class ScalewayCliSecretListItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("project_id")]
    public Guid ProjectId { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("tags")] public string[] Tags { get; set; } = [];

    [JsonPropertyName("version_count")]
    public long VersionCount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }  = string.Empty;

    [JsonPropertyName("managed")]
    public bool Managed { get; set; }

    [JsonPropertyName("protected")]
    public bool Protected { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("region")] public string Region { get; set; } = string.Empty;
}
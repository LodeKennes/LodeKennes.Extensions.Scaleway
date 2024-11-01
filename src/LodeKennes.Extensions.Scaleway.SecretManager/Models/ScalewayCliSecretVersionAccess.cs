using System.Text.Json.Serialization;

namespace LodeKennes.Extensions.Scaleway.SecretManager.Models;

public sealed class ScalewayCliSecretVersionAccess
{
    [JsonPropertyName("secret_id")] public Guid SecretId { get; set; }

    [JsonPropertyName("revision")] public long Revision { get; set; }

    [JsonPropertyName("data")] public string Data { get; set; } = string.Empty;

    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
}
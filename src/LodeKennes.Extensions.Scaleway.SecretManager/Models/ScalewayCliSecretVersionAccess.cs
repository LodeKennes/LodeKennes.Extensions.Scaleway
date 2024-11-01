using System.Text.Json.Serialization;

namespace LodeKennes.Extensions.Scaleway.SecretManager.Models;

internal sealed class ScalewayCliSecretVersionAccess
{
    [JsonPropertyName("secret_id")] public Guid SecretId { get; set; }

    [JsonPropertyName("revision")] public long Revision { get; set; }

    [JsonPropertyName("data")] public string Data { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }
}
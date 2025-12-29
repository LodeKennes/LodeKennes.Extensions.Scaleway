using System.Text.Json;
using System.Text.Json.Serialization;
using LodeKennes.Extensions.Scaleway.SecretManager.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

public sealed class ScalewayHttpSecretManager(
    string secretKey,
    string organizationId,
    string projectId,
    string region,
    string[] tags)
    : IDisposable
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.scaleway.com"),
        DefaultRequestHeaders =
        {
            { "X-Auth-Token", secretKey }
        }
    };

    public ScalewaySecretItem[] RetrieveSecrets()
    {
        var list = new List<SecretsListResponse.Secret>();

        SecretsListResponse response;
        do
        {
            var url =
                $"/secret-manager/v1beta1/regions/{region}/secrets?organization_id={organizationId}&project_id={projectId}";

            if (tags.Length > 0)
            {
                url = tags.Aggregate(url, (current, tag) => current + $"&tags={string.Join(",", tag)}");
            }

            var responseTask = _httpClient.GetStringAsync(url);
            var responseString = responseTask.GetAwaiter().GetResult();
            response = JsonSerializer.Deserialize(responseString,
                SecretManagerJsonSerializerContext.Default.SecretsListResponse)!;
            list.AddRange(response.Secrets);
        } while (list.Count < response.TotalCount);

        var secrets = list.Select(secret => new ScalewaySecretItem
        {
            Id = Guid.Parse(secret.Id),
            ProjectId = Guid.Parse(projectId),
            Name = secret.Name,
            Region = region
        }).ToArray();
        return secrets!;
    }

    public ScalewayCliSecretVersionAccess RetrieveSecretValue(ScalewaySecretItem scalewaySecretItem)
    {
        var url = $"/secret-manager/v1beta1/regions/fr-par/secrets/{scalewaySecretItem.Id}/versions/latest/access";

        var responseTask = _httpClient.GetStringAsync(url);
        var responseString = responseTask.GetAwaiter().GetResult();
        var response = JsonSerializer.Deserialize(responseString,
            SecretManagerJsonSerializerContext.Default.SecretListVersionAccess)!;

        return new ScalewayCliSecretVersionAccess
        {
            Data = response.Data
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    internal sealed class SecretsListResponse
    {
        [JsonPropertyName("secrets")] public Secret[] Secrets { get; set; } = [];

        [JsonPropertyName("total_count")] public int TotalCount { get; set; } = 0;

        public class Secret
        {
            [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

            [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        }
    }

    internal sealed class SecretListVersionAccess
    {
        [JsonPropertyName("data")] public string Data { get; set; } = string.Empty;
    }
}
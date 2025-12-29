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
        var allSecrets = new Dictionary<string, SecretsListResponse.Secret>();

        if (tags.Length == 0)
        {
            // No tags specified, fetch all secrets without tag filtering
            FetchSecretsWithTag(null, allSecrets);
        }
        else
        {
            // Fetch secrets for each tag separately (OR logic)
            foreach (var tag in tags)
            {
                FetchSecretsWithTag(tag, allSecrets);
            }
        }

        var secrets = allSecrets.Values.Select(secret => new ScalewaySecretItem
        {
            Id = Guid.Parse(secret.Id),
            ProjectId = Guid.Parse(projectId),
            Name = secret.Name,
            Region = region
        }).ToArray();
        return secrets!;
    }

    private void FetchSecretsWithTag(string? tag, Dictionary<string, SecretsListResponse.Secret> allSecrets)
    {
        var list = new List<SecretsListResponse.Secret>();

        SecretsListResponse response;
        do
        {
            var url =
                $"/secret-manager/v1beta1/regions/{region}/secrets?organization_id={organizationId}&project_id={projectId}";

            if (tag is not null)
            {
                url += $"&tags={tag}";
            }

            var responseTask = _httpClient.GetStringAsync(url);
            var responseString = responseTask.GetAwaiter().GetResult();
            response = JsonSerializer.Deserialize(responseString,
                SecretManagerJsonSerializerContext.Default.SecretsListResponse)!;
            list.AddRange(response.Secrets);
        } while (list.Count < response.TotalCount);

        // Add to dictionary using Id as key to deduplicate secrets that have multiple matching tags
        foreach (var secret in list)
        {
            if (!allSecrets.ContainsKey(secret.Id))
            {
                allSecrets[secret.Id] = secret;
            }
        }
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
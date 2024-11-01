using System.Text.Json.Serialization;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;

namespace LodeKennes.Extensions.Scaleway.SecretManager.Json;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ScalewayCliSecretVersionAccess))]
[JsonSerializable(typeof(CachedScalewayDictionary))]
[JsonSerializable(typeof(ScalewayCliConfigInfo))]
[JsonSerializable(typeof(ScalewayHttpSecretManager.SecretListVersionAccess))]
[JsonSerializable(typeof(ScalewayCliConfigInfo.ScalewayCliConfigInfoProfile))]
[JsonSerializable(typeof(ScalewayHttpSecretManager.SecretsListResponse))]
[JsonSerializable(typeof(ScalewayHttpSecretManager.SecretsListResponse.Secret))]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class SecretManagerJsonSerializerContext : JsonSerializerContext;
using System.Text.Json.Serialization;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;

namespace LodeKennes.Extensions.Scaleway.SecretManager.Json;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ScalewayCliSecretVersionAccess))]
[JsonSerializable(typeof(CachedScalewayDictionary))]
[JsonSerializable(typeof(ScalewayCliSecretListItem[]))]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class SecretManagerJsonSerializerContext : JsonSerializerContext;
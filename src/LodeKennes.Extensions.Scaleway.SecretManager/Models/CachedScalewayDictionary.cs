namespace LodeKennes.Extensions.Scaleway.SecretManager.Models;

internal sealed class CachedScalewayDictionary
{
    public DateTimeOffset Expires { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, string> Items { get; set; } = [];
}
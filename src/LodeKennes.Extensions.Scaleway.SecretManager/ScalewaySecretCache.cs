using System.Text.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;
using Microsoft.AspNetCore.DataProtection;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

internal sealed class ScalewaySecretCache(string location, TimeSpan? ttl, IDataProtector dataProtector)
{
    private readonly string _cachePath = Path.Combine(location, "cache.json");
    
    public void Save(Dictionary<string, string> items)
    {
        var cached = new CachedScalewayDictionary
        {
            Expires = DateTimeOffset.UtcNow.Add(ttl ?? TimeSpan.FromMinutes(5)),
            Items = items
        };
        
        var serialized = JsonSerializer.Serialize(cached, SecretManagerJsonSerializerContext.Default.CachedScalewayDictionary);
        var protectedJson = dataProtector.Protect(serialized);
        File.WriteAllText(_cachePath, protectedJson);
    }
    
    public bool TryLoad(out Dictionary<string, string>? items)
    {
        if (!File.Exists(_cachePath))
        {
            items = null;
            return false;
        }
        
        var protectedJson = File.ReadAllText(_cachePath);
        var serialized = dataProtector.Unprotect(protectedJson);
        var cached = JsonSerializer.Deserialize(serialized, SecretManagerJsonSerializerContext.Default.CachedScalewayDictionary);
        
        if (cached == null || cached.Expires < DateTimeOffset.UtcNow)
        {
            items = null;
            return false;
        }
        
        items = cached.Items;
        return true;
    }
}
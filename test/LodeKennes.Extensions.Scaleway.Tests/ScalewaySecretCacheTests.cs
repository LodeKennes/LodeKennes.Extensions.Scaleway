using LodeKennes.Extensions.Scaleway.SecretManager;
using Microsoft.AspNetCore.DataProtection;

namespace LodeKennes.Extensions.Scaleway.Tests;

public sealed class ScalewaySecretCacheTests : IDisposable
{
    private readonly string _cacheLocation;
    private readonly ScalewaySecretCache _cache;
    
    [Fact]
    public void ReloadingCache()
    {
        var items = new Dictionary<string, string>
        {
            ["zone"] = "intellua.com"
        };
        
        _cache.Save(items);
        
        var loaded = _cache.TryLoad(out var loadedItems);
        
        Assert.True(loaded);
        Assert.NotNull(loadedItems);
        Assert.Equal(items, loadedItems);
    }

    public ScalewaySecretCacheTests()
    {
        _cacheLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_cacheLocation);
        _cache = new ScalewaySecretCache(_cacheLocation, Guid.NewGuid(), TimeSpan.FromMinutes(5), new EphemeralDataProtectionProvider().CreateProtector("test"));
    }
    
    public void Dispose()
    {
        Directory.Delete(_cacheLocation, true);
    }
}
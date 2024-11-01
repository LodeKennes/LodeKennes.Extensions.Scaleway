using System.Buffers.Text;
using System.Text;
using LodeKennes.Extensions.Scaleway.SecretManager;

namespace LodeKennes.Extensions.Scaleway.Tests;

public sealed class ScalewayHttpSecretManagerTests : IDisposable
{
    private readonly ScalewayHttpSecretManager _manager;

    public ScalewayHttpSecretManagerTests()
    {
        var client = new ScalewayCliManager();
        var config = client.RetrieveConfig();
        
        _manager = new ScalewayHttpSecretManager(config.Profile.SecretKey, config.Profile.DefaultOrganizationId.ToString(), "53c67a1c-be43-4329-80e4-30118ef65dd3", config.Profile.DefaultRegion);
    }
    
    
    [Fact]
    public void HasSecrets()
    {
        var secrets = _manager.RetrieveSecrets();
    
        Assert.NotEmpty(secrets);
    }
    
    [Fact]
    public void HasSecretValue()
    {
        var secrets = _manager.RetrieveSecrets();
        var secret = secrets.FirstOrDefault(x => x.Name.Equals("zone", StringComparison.InvariantCultureIgnoreCase));
    
        Assert.False(secret is null, "Secret not found");
    
        var secretValue = _manager.RetrieveSecretValue(secret!);
        
        Assert.NotNull(secretValue);
        Assert.True(Base64.IsValid(secretValue.Data));
        
        var base64Decoded = Encoding.UTF8.GetString(Convert.FromBase64String(secretValue.Data));
        Assert.NotEmpty(base64Decoded);
        Assert.Equal("intellua.com", base64Decoded);
    }
    
    public void Dispose()
    {
        _manager.Dispose();
    }
}
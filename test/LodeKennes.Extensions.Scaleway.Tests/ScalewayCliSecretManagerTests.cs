using System.Buffers.Text;
using System.Text;
using LodeKennes.Extensions.Scaleway.SecretManager;

namespace LodeKennes.Extensions.Scaleway.Tests;

public sealed class ScalewayCliSecretManagerTests
{
    private readonly ScalewayCliSecretManager _manager = new();

    [Fact]
    public void IsInstalled()
    {
        var isInstalled = _manager.IsInstalled();

        Assert.True(isInstalled);
    }

    [Theory]
    [InlineData("53c67a1c-be43-4329-80e4-30118ef65dd3", "fr-par")]
    public void HasSecrets(string projectId, string region)
    {
        var secrets = _manager.RetrieveSecrets(projectId, region);

        Assert.NotEmpty(secrets);
    }

    [Theory]
    [InlineData("53c67a1c-be43-4329-80e4-30118ef65dd3", "fr-par")]
    public void HasSecretValue(string projectId, string region)
    {
        var secrets = _manager.RetrieveSecrets(projectId, region);
        var secret = secrets.FirstOrDefault(x => x.Name.Equals("zone", StringComparison.InvariantCultureIgnoreCase));

        Assert.False(secret is null, "Secret not found");

        var secretValue = _manager.RetrieveSecretValue(secret!);
        
        Assert.NotNull(secretValue);
        Assert.True(Base64.IsValid(secretValue.Data));
        
        var base64Decoded = Encoding.UTF8.GetString(Convert.FromBase64String(secretValue.Data));
        Assert.NotEmpty(base64Decoded);
        Assert.True("intellua.com" == base64Decoded);
    }
}
using System.Buffers.Text;
using System.Text;
using LodeKennes.Extensions.Scaleway.SecretManager;

namespace LodeKennes.Extensions.Scaleway.Tests;

public sealed class ScalewayCliManagerTests
{
    private readonly ScalewayCliManager _manager = new();

    [Fact]
    public void IsInstalled()
    {
        var isInstalled = _manager.IsInstalled();

        Assert.True(isInstalled);
    }
    
    [Fact]
    public void CanRetrieveConfig()
    {
        var isInstalled = _manager.RetrieveConfig();

        Assert.NotNull(isInstalled.Profile);
        Assert.NotNull(isInstalled.Profile.SecretKey);
        Assert.NotNull(isInstalled.Profile.DefaultRegion);
        Assert.NotNull(isInstalled.Profile.AccessKey);
    }
}
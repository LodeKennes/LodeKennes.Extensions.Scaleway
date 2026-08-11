using System.Buffers.Text;
using System.Text;
using LodeKennes.Extensions.Scaleway.SecretManager;

namespace LodeKennes.Extensions.Scaleway.Tests;

public sealed class ScalewayCliManagerTests
{
    private readonly ScalewayCliManager _manager = new();

    [Fact]
    public void CanRetrieveConfig()
    {
        var isInstalled = _manager.RetrieveConfig();

        Assert.NotNull(isInstalled.Profile);
        Assert.NotNull(isInstalled.Profile.SecretKey);
        Assert.NotNull(isInstalled.Profile.DefaultRegion);
        Assert.NotNull(isInstalled.Profile.AccessKey);
    }

    [Theory]
    [InlineData(null, null, "scw")]
    [InlineData("", "", "scw")]
    [InlineData(null, "/custom/env/scw", "/custom/env/scw")]
    [InlineData("/custom/option/scw", "/custom/env/scw", "/custom/option/scw")]
    public void ResolvesCliPath(string? cliPath, string? environmentCliPath, string expected)
    {
        Assert.Equal(expected, ScalewayCliManager.ResolveCliPath(cliPath, environmentCliPath));
    }
}

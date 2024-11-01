using LodeKennes.Extensions.Scaleway.SecretManager;
using Microsoft.Extensions.Configuration;

namespace LodeKennes.Extensions.Scaleway.Tests;

public sealed class ConfigurationBuilderTests
{
    [Fact]
    public void ZoneVariableShouldLoad()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddScalewayCliSecrets(options =>
            {
                options.ProjectId = Guid.Parse("53c67a1c-be43-4329-80e4-30118ef65dd3");
            })
            .Build();
        
        Assert.NotNull(configurationBuilder);

        var zone = configurationBuilder["zone"];
        Assert.NotNull(zone);
        Assert.True("intellua.com" == zone);
    }
}
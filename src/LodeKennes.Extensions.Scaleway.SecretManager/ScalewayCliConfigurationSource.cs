using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

internal sealed class ScalewayCliConfigurationSource(ScalewayCliSecretManager scalewaySecretManager, ScalewaySecretCache scalewaySecretCache, bool cache, string projectId, string region) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var secrets = scalewaySecretManager.RetrieveSecrets(projectId, region);

        Dictionary<string, string> configurationSource;
        
        if (cache && scalewaySecretCache.TryLoad(out var cached))
        {
            configurationSource = cached!;
        }
        else
        {
            configurationSource = new Dictionary<string, string>();
            
            foreach (var scalewayCliSecretListItem in secrets)
            {
                var secretValue = scalewaySecretManager.RetrieveSecretValue(scalewayCliSecretListItem);
                configurationSource[scalewayCliSecretListItem.Name] = Encoding.UTF8.GetString(Convert.FromBase64String(secretValue.Data));
            }

            if (cache)
            {
                scalewaySecretCache.Save(configurationSource);
            }
        }
        
        return new MemoryConfigurationProvider(new MemoryConfigurationSource
        {
            InitialData = configurationSource!
        });
    }
}
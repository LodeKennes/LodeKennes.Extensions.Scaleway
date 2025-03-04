using System.Text;
using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

internal sealed class ScalewayCliConfigurationSource(ScalewayHttpSecretManager scalewayHttpSecretManager, ScalewaySecretCache scalewaySecretCache, bool cache) : IConfigurationSource
{
    private static readonly Mutex Mutex = new(true, nameof(ScalewayCliConfigurationSource));
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        Dictionary<string, string> configurationSource;

        // if (Mutex.WaitOne())
        // {
            var secrets = scalewayHttpSecretManager.RetrieveSecrets();
            
            if (cache && scalewaySecretCache.TryLoad(out var cached))
            {
                configurationSource = cached!;
            }
            else
            {
                configurationSource = new Dictionary<string, string>();
            
                foreach (var scalewayCliSecretListItem in secrets)
                {
                    var secretValue = scalewayHttpSecretManager.RetrieveSecretValue(scalewayCliSecretListItem);
                    configurationSource[scalewayCliSecretListItem.Name] = Encoding.UTF8.GetString(Convert.FromBase64String(secretValue.Data));
                }

                if (cache)
                {
                    scalewaySecretCache.Save(configurationSource);
                }
            }
        // }
        // else
        // {
        //     throw new ScalewayCliException("Mutex could not be acquired");
        // }
        
        return new MemoryConfigurationProvider(new MemoryConfigurationSource
        {
            InitialData = configurationSource!
        });
    }
}
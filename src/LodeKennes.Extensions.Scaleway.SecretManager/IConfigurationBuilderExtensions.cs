using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using Microsoft.Extensions.Configuration;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

public static class IConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddScalewayCliSecrets(this IConfigurationBuilder builder, Action<ScalewaySecretOptions> configure)
    {
        var options = new ScalewaySecretOptions();
        configure(options);
        
        var scalewaySecretManager = new ScalewayCliSecretManager();

        var isInstalled = scalewaySecretManager.IsInstalled();
        
        if (!isInstalled)
        {
            throw new ScalewayCliException("Scaleway CLI is not installed");
        }

        builder.Add(new ScalewayCliConfigurationSource(scalewaySecretManager, options.ProjectId.ToString(), options.Region));
        
        return builder;
    }
}
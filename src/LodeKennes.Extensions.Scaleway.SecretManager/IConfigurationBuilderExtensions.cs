using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

public static class IConfigurationBuilderExtensions
{
    private const string Purpose = "lk-scw-secrets";
    
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
        
        var dataProtectionKeysDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Purpose);
        var services = new ServiceCollection();
        
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysDirectory));

        var serviceProvider = services.BuildServiceProvider();
        var dp = serviceProvider.GetDataProtector(Purpose);
        var scalewaySecretCache = new ScalewaySecretCache(dataProtectionKeysDirectory, options.CacheTtl, dp);

        builder.Add(new ScalewayCliConfigurationSource(scalewaySecretManager, scalewaySecretCache, options.CacheEnabled, options.ProjectId.ToString(), options.Region));
        
        return builder;
    }
}
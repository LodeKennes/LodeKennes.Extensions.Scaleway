using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

public static class ConfigurationBuilderExtensions
{
    private const string Purpose = "lk-scw-secrets";
    
    public static IConfigurationBuilder AddScalewayCliSecrets(this IConfigurationBuilder builder, Action<ScalewaySecretOptions>? configure)
    {
        var options = new ScalewaySecretOptions();
        configure?.Invoke(options);

        var scalewaySecretManager = new ScalewayCliManager();

        var isInstalled = scalewaySecretManager.IsInstalled();
        
        if (!isInstalled)
        {
            throw new ScalewayCliException("Scaleway CLI is not installed");
        }
        
        var configInfo = scalewaySecretManager.RetrieveConfig();
        
        var dataProtectionKeysDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Purpose);
        var services = new ServiceCollection();
        
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysDirectory));

        var serviceProvider = services.BuildServiceProvider();
        var dp = serviceProvider.GetDataProtector(Purpose);
        var scalewaySecretCache = new ScalewaySecretCache(dataProtectionKeysDirectory, options.ProjectId, options.CacheTtl, dp);

        var scalewayHttpSecretManager = new ScalewayHttpSecretManager(configInfo.Profile.SecretKey, configInfo.Profile.DefaultOrganizationId.ToString(), options.ProjectId.ToString(), configInfo.Profile.DefaultRegion);
        
        builder.Add(new ScalewayCliConfigurationSource(scalewayHttpSecretManager, scalewaySecretCache, options.CacheEnabled));
        
        return builder;
    }
}
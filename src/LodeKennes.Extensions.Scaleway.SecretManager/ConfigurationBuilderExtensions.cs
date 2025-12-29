using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;
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
        
        if (options.SecretStrategy == null)
        {
            throw new ScalewayCliException("Secret strategy must be set");
        }
        
        ScalewayCliConfigInfo configInfo;
        
        if (options.SecretStrategy == ScalewaySecretOptions.ScalewaySecretStrategy.Cli)
        {
            var scalewaySecretManager = new ScalewayCliManager();
            
            configInfo = scalewaySecretManager.RetrieveConfig();
            
            if (configInfo.Profile == null)
            {
                throw new ScalewayCliException("No profile found in Scaleway CLI config");
            }
        }
        else
        {
            configInfo = new ScalewayCliConfigInfo
            {
                Profile = new ScalewayCliConfigInfo.ScalewayCliConfigInfoProfile
                {
                    SecretKey = options.SecretKey!,
                    DefaultRegion = options.Region!,
                    DefaultOrganizationId = Guid.Parse(options.OrganizationId!)
                }
            };
        }
        
        var dataProtectionKeysDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Purpose);
        var services = new ServiceCollection();
        
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysDirectory));

        var serviceProvider = services.BuildServiceProvider();
        var dp = serviceProvider.GetDataProtector(Purpose);
        var scalewaySecretCache = new ScalewaySecretCache(dataProtectionKeysDirectory, options.ProjectId, options.CacheTtl, dp);

        var scalewayHttpSecretManager = new ScalewayHttpSecretManager(configInfo.Profile.SecretKey, configInfo.Profile.DefaultOrganizationId.ToString(), options.ProjectId.ToString(), configInfo.Profile.DefaultRegion, options.Tags);
        
        builder.Add(new ScalewayCliConfigurationSource(scalewayHttpSecretManager, scalewaySecretCache, options.CacheEnabled));
        
        return builder;
    }
}
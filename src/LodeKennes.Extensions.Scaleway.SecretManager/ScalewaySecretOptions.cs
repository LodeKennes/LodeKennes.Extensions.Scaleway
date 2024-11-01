namespace LodeKennes.Extensions.Scaleway.SecretManager;

public sealed class ScalewaySecretOptions
{
    public Guid ProjectId { get; set; }
    
    internal bool CacheEnabled { get; private set; }
    internal TimeSpan? CacheTtl { get; private set; }
    
    internal ScalewaySecretStrategy? SecretStrategy { get; set; }
    internal string? SecretKey { get; set; }
    internal string? Region { get; set; }
    internal string? OrganizationId { get; set; }
    
    public void UseCli()
    {
        SecretStrategy = ScalewaySecretStrategy.Cli;
    }
    
    public void UseCredentials(string secretKey, string region, string organizationId)
    {
        SecretStrategy = ScalewaySecretStrategy.Credentials;
        SecretKey = secretKey;
        Region = region;
        OrganizationId = organizationId;
    }
    
    public void EnableCaching(TimeSpan? ttl = null)
    {
        CacheEnabled = true;
        CacheTtl = ttl ?? TimeSpan.FromMinutes(5);
    }
    
    public enum ScalewaySecretStrategy
    {
        Cli,
        Credentials
    }
}
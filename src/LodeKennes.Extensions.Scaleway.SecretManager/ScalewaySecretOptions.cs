namespace LodeKennes.Extensions.Scaleway.SecretManager;

public sealed class ScalewaySecretOptions
{
    public Guid ProjectId { get; set; }
    
    internal bool CacheEnabled { get; private set; }
    internal TimeSpan? CacheTtl { get; private set; }
    
    public void EnableCaching(TimeSpan? ttl = null)
    {
        CacheEnabled = true;
        CacheTtl = ttl ?? TimeSpan.FromMinutes(5);
    }
}
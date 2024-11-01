using System.Text.Json.Serialization;

namespace LodeKennes.Extensions.Scaleway.SecretManager.Models;

public sealed class ScalewayCliConfigInfo
{
    [JsonPropertyName("Profile")]
    public ScalewayCliConfigInfoProfile Profile { get; set; } = new();
    
    public sealed class ScalewayCliConfigInfoProfile
    {
        [JsonPropertyName("access-key")]
        public string AccessKey { get; set; } = string.Empty;
        
        [JsonPropertyName("secret-key")]
        public string SecretKey { get; set; } = string.Empty;
        
        [JsonPropertyName("default-region")]
        public string DefaultRegion { get; set; } = string.Empty;
        
        [JsonPropertyName("default-organization-id")]
        public Guid DefaultOrganizationId { get; set; }
    }
}
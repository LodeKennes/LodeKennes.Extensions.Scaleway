namespace LodeKennes.Extensions.Scaleway.SecretManager;

public sealed class ScalewaySecretOptions
{
    public Guid ProjectId { get; set; }
    public string Region { get; set; } = "fr-par";
}
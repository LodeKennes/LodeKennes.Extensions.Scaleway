namespace LodeKennes.Extensions.Scaleway.SecretManager.Models;

public sealed class ScalewaySecretItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
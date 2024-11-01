using System.Diagnostics;
using System.Text.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using LodeKennes.Extensions.Scaleway.SecretManager.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

internal sealed class ScalewayCliSecretManager
{
    public bool IsInstalled()
    {
        const string expectedOutput = "scw <command>";
        
        var result = Process.Start(new ProcessStartInfo
        {
            FileName = "scw",
            Arguments = "--help",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        })!;
        result.WaitForExit();
        
        var text = result.StandardOutput.ReadToEnd();
        var error = result.StandardError.ReadToEnd();
        
        if (!string.IsNullOrWhiteSpace(text))
        {
            throw new ScalewayCliException($"Error while executing command: {error}");
        }
        
        return error.Contains(expectedOutput, StringComparison.InvariantCultureIgnoreCase);
    }
    
    public ScalewayCliSecretListItem[] RetrieveSecrets(string projectId, string region)
    {
        var result = Process.Start(new ProcessStartInfo
        {
            FileName = "scw",
            Arguments = $"secret secret list --output json region={region} project-id={projectId}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        })!;
        result.WaitForExit();
        
        var text = result.StandardOutput.ReadToEnd();
        var error = result.StandardError.ReadToEnd();
        
        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new ScalewayCliException($"Error while executing command: {error}");
        }
        
        var secrets = JsonSerializer.Deserialize(text, SecretManagerJsonSerializerContext.Default.ScalewayCliSecretListItemArray);
        return secrets!;
    }
    
    public ScalewayCliSecretVersionAccess RetrieveSecretValue(ScalewayCliSecretListItem listItem)
    {
        var result = Process.Start(new ProcessStartInfo
        {
            FileName = "scw",
            Arguments = $"secret version access {listItem.Id} revision=latest --output json",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        })!;
        result.WaitForExitAsync();
        
        var text = result.StandardOutput.ReadToEnd();
        var error = result.StandardError.ReadToEnd();
        
        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new ScalewayCliException($"Error while executing command: {error}");
        }
        
        var secrets = JsonSerializer.Deserialize(text, SecretManagerJsonSerializerContext.Default.ScalewayCliSecretVersionAccess);
        return secrets!;
    }
}
using System.Diagnostics;
using System.Text.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using LodeKennes.Extensions.Scaleway.SecretManager.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

public sealed class ScalewayCliManager
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
        
        return error.IndexOf(expectedOutput, StringComparison.InvariantCultureIgnoreCase) > -1;
    }

    public ScalewayCliConfigInfo RetrieveConfig()
    {
        var result = Process.Start(new ProcessStartInfo
        {
            FileName = "scw",
            Arguments = "config info --output json",
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
            throw new ScalewayCliException($"Error retrieving config info: {error}");
        }

        throw new ScalewayCliException(text);

        // var configInfo = JsonSerializer.Deserialize(text, SecretManagerJsonSerializerContext.Default.ScalewayCliConfigInfo);
        // return configInfo!;
    }
}
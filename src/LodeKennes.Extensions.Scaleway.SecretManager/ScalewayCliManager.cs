using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;
using LodeKennes.Extensions.Scaleway.SecretManager.Json;
using LodeKennes.Extensions.Scaleway.SecretManager.Models;

namespace LodeKennes.Extensions.Scaleway.SecretManager;

public sealed class ScalewayCliManager(string? cliPath = null)
{
    private const string CliPathEnvironmentVariable = "SCW_CLI_PATH";
    private readonly string _cliPath = ResolveCliPath(cliPath, Environment.GetEnvironmentVariable(CliPathEnvironmentVariable));

    internal static string ResolveCliPath(string? cliPath, string? environmentCliPath)
    {
        if (!string.IsNullOrWhiteSpace(cliPath))
        {
            return cliPath!;
        }

        return string.IsNullOrWhiteSpace(environmentCliPath) ? "scw" : environmentCliPath!;
    }

    public ScalewayCliConfigInfo RetrieveConfig()
    {
        var result = Process.Start(new ProcessStartInfo
        {
            FileName = _cliPath,
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

        var objectRegex = new Regex("({.*})", RegexOptions.Compiled | RegexOptions.Multiline);
        
        var match = objectRegex.Match(text);
        if (!match.Success)
        {
            throw new ScalewayCliException("Could not find JSON object in output");
        }
        
        text = match.Value;
        var configInfo = JsonSerializer.Deserialize(text, SecretManagerJsonSerializerContext.Default.ScalewayCliConfigInfo);
        return configInfo!;
    }
}

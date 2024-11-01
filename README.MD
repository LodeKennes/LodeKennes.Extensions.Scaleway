# Scaleway Secret Manager Configuration Extension for .NET

This NuGet package provides an extension for integrating Scaleway Secret Manager secrets into your .NET application's configuration system. It allows seamless fetching and management of secrets stored in Scaleway using the Scaleway CLI or via self-defined credentials.

## Features

- Integrate Scaleway secrets into the .NET `IConfiguration` system.
- Ensure Scaleway CLI is installed and functional when using `UseCli`.
- Customizable options for specifying the Scaleway credentials.

## Installation

To install the package, use the following command in your terminal:

```bash
dotnet add package LodeKennes.Extensions.Scaleway.SecretManager
```

## Usage

To use the extension, add it to your IConfigurationBuilder like so:

```csharp
using LodeKennes.Extensions.Scaleway.SecretManager;
using Microsoft.Extensions.Configuration;

var configurationBuilder = new ConfigurationBuilder()
    .AddScalewayCliSecrets(options =>
    {
        options.ProjectId = Guid.Parse("your-project-id-here");
        options.Region = "your-region-here"; // Optional: specify the region if needed
        options.EnableCaching(TimeSpan.FromMinutes(1)); // Optional: enable caching to reduce the number of calls to the Scaleway API
        
        // Define the authentication strategy
        options.UseCli(); // use the Scaleway CLI to fetch credentials
        options.UseCredentials("<secretKey>", "<region>", "<organizationId>"); // use self-defined credentials
    })
    .Build();

var configuration = configurationBuilder.Build();
```

## Example

An example unit test illustrates how to use the AddScalewayCliSecrets extension method:

```csharp
[Fact]
public void ZoneVariableShouldLoad()
{
    var configurationBuilder = new ConfigurationBuilder()
        .AddScalewayCliSecrets(options =>
        {
            options.ProjectId = Guid.Parse("7C85EC8F-CAA2-4579-A6B9-FB6581A23E08");
            options.UseCli();
        })
        .Build();

    Assert.NotNull(configurationBuilder);

    var zone = configurationBuilder["zone"];
    Assert.NotNull(zone);
    Assert.True("intellua.com" == zone);
}
```

## Requirements

- Scaleway CLI: Ensure that the Scaleway CLI is installed and accessible from your environment.
- .NET SDK: This package requires .NET 6.0 or later versions.

### Exception Handling
ScalewayCliException: Thrown when Scaleway CLI is not installed.

## Contributing

Feel free to open an issue or submit a pull request if you have any improvements or additional features you'd like to contribute.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.
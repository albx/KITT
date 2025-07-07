using Azure.Extensions.AspNetCore.Configuration.Secrets;
using AzureKeyVaultEmulator.Aspire.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace KITT.Web.Shared.Azure;

public static class AzureKeyVaultExtensions
{
    public static IHostApplicationBuilder AddAzureKeyVaultClientWithEmulatorFallback(this IHostApplicationBuilder builder, string connectionStringName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Environment.IsDevelopment())
        {
            var vaultUri = builder.Configuration.GetConnectionString(connectionStringName);
            builder.Services.AddAzureKeyVaultEmulator(vaultUri);
        }
        else
        {
            builder.AddAzureKeyVaultClient(connectionStringName);
        }

        return builder;
    }

    public static IHostApplicationBuilder AddAzureKeyVaultSecretsWithEmulatorFallback(this IHostApplicationBuilder builder, string connectionStringName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Environment.IsDevelopment())
        {
            var vaultUri = builder.Configuration.GetConnectionString(connectionStringName);

            var secretClient = KeyVaultHelper.GetSecretClient(vaultUri);
            builder.Configuration.AddAzureKeyVault(
                secretClient,
                new AzureKeyVaultConfigurationOptions());
        }
        else
        {
            builder.Configuration.AddAzureKeyVaultSecrets(connectionStringName);
        }
        
        return builder;
    }
}

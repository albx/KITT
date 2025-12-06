using Azure.Extensions.AspNetCore.Configuration.Secrets;
using AzureKeyVaultEmulator.Aspire.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace KITT.Web.Shared.Azure;

public static class AzureKeyVaultExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddAzureKeyVaultClientWithEmulatorFallback(string connectionStringName)
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

        public IHostApplicationBuilder AddAzureKeyVaultSecretsWithEmulatorFallback(string connectionStringName)
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
}

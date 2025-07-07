using Azure.Core;
using Azure.Provisioning.KeyVault;
using AzureKeyVaultEmulator.Aspire.Hosting;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

#region EntraID configuration
var tenantId = builder.AddParameter("EntraIdTenantId", secret: true);
var domainName = builder.AddParameter("EntraIdDomainName", secret: true);

var cmsApiAppId = builder.AddParameter("CmsApiAppId", secret: true);

var proposalsApiAppId = builder.AddParameter("ProposalsApiAppId", secret: true);

var webAppId = builder.AddParameter("WebAppId", secret: true);
var webAppSecret = builder.AddParameter("WebAppSecret", secret: true);
#endregion

#region Key Vault configuration
var keyVault = builder.AddAzureKeyVault("kitt-keyvault")
    .ConfigureInfrastructure(infra =>
    {
        var keyVaultResource = infra.GetProvisionableResources()
            .OfType<KeyVaultService>()
            .Single();

        keyVaultResource.Location = AzureLocation.ItalyNorth;
        keyVaultResource.Properties.Sku = new()
        {
            Name = KeyVaultSkuName.Standard,
            Family = KeyVaultSkuFamily.A
        };
    })
    .RunAsEmulator();
#endregion

var kittDb = builder.AddConnectionString("KittDatabase");

#region CMS
var cmsApi = builder.AddProject<Projects.KITT_Cms_Web_Api>("cms-api")
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__Cms__AppId", cmsApiAppId);
#endregion

#region Proposals
var proposalsApi = builder.AddProject<Projects.KITT_Proposals_Web_Api>("proposals-api")
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__Proposals__AppId", proposalsApiAppId);
#endregion

var webApp = builder.AddProject<Projects.KITT_Web_App>("webapp")
    .WithReference(cmsApi)
    .WaitFor(cmsApi)
    .WithReference(proposalsApi)
    .WaitFor(proposalsApi)
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithReference(keyVault)
    .WaitFor(keyVault)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__DomainName", domainName)
    .WithEnvironment("Identity__WebApp__AppId", webAppId)
    .WithEnvironment("Identity__WebApp__AppSecret", webAppSecret)
    .WithEnvironment("Identity__Cms__AppId", cmsApiAppId)
    .WithEnvironment("Identity__Proposals__AppId", proposalsApiAppId)
    .WithExternalHttpEndpoints();


if (builder.Environment.IsDevelopment())
{
    builder.AddProject<Projects.KITT_Support_Seeder>("kitt-support-seeder")
        .WithReference(keyVault)
        .WaitFor(keyVault)
        .WithEnvironment("Identity__WebApp__AppSecret", webAppSecret);
}


builder.Build().Run();

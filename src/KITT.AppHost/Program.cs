using Microsoft.Extensions.Hosting;
using KITT.Services;
using KITT.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("kitt-env");

#region EntraID configuration
var tenantId = builder.AddParameter("EntraIdTenantId", secret: true);
var domainName = builder.AddParameter("EntraIdDomainName", secret: true);

var cmsApiAppId = builder.AddParameter("CmsApiAppId", secret: true);

var proposalsApiAppId = builder.AddParameter("ProposalsApiAppId", secret: true);

var webAppId = builder.AddParameter("WebAppId", secret: true);
var webAppSecret = builder.AddParameter("WebAppSecret", secret: true);
#endregion

#region Database
var kittDb = builder.AddKittDatabase();
#endregion

#region CMS
var cmsApi = builder.AddProject<Projects.KITT_Cms_Web_Api>(ServiceNames.CmsApi)
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__Cms__AppId", cmsApiAppId);
#endregion

#region Proposals
var proposalsApi = builder.AddProject<Projects.KITT_Proposals_Web_Api>(ServiceNames.ProposalsApi)
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__Proposals__AppId", proposalsApiAppId);
#endregion

var webApp = builder.AddProject<Projects.KITT_Web_App>(ServiceNames.WebApp)
    .WithReference(cmsApi)
    .WaitFor(cmsApi)
    .WithReference(proposalsApi)
    .WaitFor(proposalsApi)
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__DomainName", domainName)
    .WithEnvironment("Identity__WebApp__AppId", webAppId)
    .WithEnvironment("Identity__WebApp__AppSecret", webAppSecret)
    .WithEnvironment("Identity__Cms__AppId", cmsApiAppId)
    .WithEnvironment("Identity__Proposals__AppId", proposalsApiAppId)
    .WithExternalHttpEndpoints();


if (builder.Environment.IsDevelopment())
{
    var seeder = builder.AddProject<Projects.KITT_Support_Seeder>(ServiceNames.Seeder)
        .WithReference(kittDb)
        .WaitFor(kittDb);
}


builder.Build().Run();

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region EntraID configuration
var tenantId = builder.AddParameter("EntraIdTenantId", secret: true);
var domainName = builder.AddParameter("EntraIdDomainName", secret: true);

var cmsApiAppId = builder.AddParameter("CmsApiAppId", secret: true);

var proposalsApiAppId = builder.AddParameter("ProposalsApiAppId", secret: true);

var webAppId = builder.AddParameter("WebAppId", secret: true);
var webAppSecret = builder.AddParameter("WebAppSecret", secret: true);
#endregion

var kittDb = builder.AddConnectionString("KittDatabase");

#region CMS
var cmsApi = builder.AddProject<KITT_Cms_Web_Api>("cms-api")
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__Cms__AppId", cmsApiAppId);
#endregion

#region Proposals
var proposalsApi = builder.AddProject<KITT_Proposals_Web_Api>("proposals-api")
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("Identity__TenantId", tenantId)
    .WithEnvironment("Identity__Proposals__AppId", proposalsApiAppId);
#endregion

var webApp = builder.AddProject<KITT_Web_App>("webapp")
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

builder.Build().Run();

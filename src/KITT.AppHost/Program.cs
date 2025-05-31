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
    .WithEnvironment("TENANT_ID", tenantId)
    .WithEnvironment("CMS_APPID", cmsApiAppId);
#endregion

#region Proposals
var proposalsApi = builder.AddProject<KITT_Proposals_Web_Api>("proposals-api")
    .WithReference(kittDb)
    .WaitFor(kittDb)
    .WithEnvironment("TENANT_ID", tenantId)
    .WithEnvironment("PROPOSALS_APPID", proposalsApiAppId);
#endregion

var webApp = builder.AddProject<KITT_Web_App>("webapp")
    .WithReference(cmsApi)
    .WaitFor(cmsApi)
    .WithReference(proposalsApi)
    .WaitFor(proposalsApi)
    .WithEnvironment("TENANT_ID", tenantId)
    .WithEnvironment("DOMAIN_NAME", domainName)
    .WithEnvironment("WEB_APPID", webAppId)
    .WithEnvironment("WEB_APP_SECRET", webAppSecret)
    .WithEnvironment("CMS_APPID", cmsApiAppId)
    .WithEnvironment("PROPOSALS_APPID", proposalsApiAppId)
    .WithExternalHttpEndpoints();

builder.Build().Run();

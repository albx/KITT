using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region EntraID configuration
var tenantId = builder.AddParameter("EntraIdTenantId", secret: true);
var domainName = builder.AddParameter("EntraIdDomainName", secret: true);

var cmsApiAppId = builder.AddParameter("CmsApiAppId", secret: true);
var cmsApiScopes = builder.AddParameter("CmsApiScopes", secret: true);

var webAppId = builder.AddParameter("WebAppId", secret: true);
#endregion

var kittDb = builder.AddConnectionString("KittDatabase");

#region CMS
var cmsApi = builder.AddProject<KITT_Cms_Web_Api>("cms-api")
    .WithReference(kittDb)
    .WithEnvironment("TENANT_ID", tenantId)
    .WithEnvironment("CMS_APPID", cmsApiAppId);
#endregion

#region Proposals
var proposalsApi = builder.AddProject<KITT_Proposals_Web_Api>("proposals-api")
    .WithReference(kittDb);
#endregion

var webApp = builder.AddProject<KITT_Web_App>("webapp")
    .WithReference(cmsApi)
    .WithReference(proposalsApi)
    .WithReference(kittDb)
    .WithEnvironment("TENANT_ID", tenantId)
    .WithEnvironment("DOMAIN_NAME", domainName)
    .WithEnvironment("WEB_APPID", webAppId)
    .WithEnvironment("CMS_APPID", cmsApiAppId)
    .WithEnvironment("CMS_API_SCOPES", cmsApiScopes)
    .WithExternalHttpEndpoints();

builder.Build().Run();

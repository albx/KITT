using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var kittDb = builder.AddConnectionString("KittDatabase");

#region CMS
var cmsApi = builder.AddProject<KITT_Cms_Web_Api>("cms-api")
    .WithReference(kittDb);
#endregion

#region Proposals
var proposalsApi = builder.AddProject<KITT_Proposals_Web_Api>("proposals-api")
    .WithReference(kittDb);
#endregion

var webApp = builder.AddProject<KITT_Web_App>("webapp")
    .WithReference(cmsApi)
    .WithReference(proposalsApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();

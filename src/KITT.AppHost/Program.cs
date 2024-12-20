using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var kittDb = builder.AddConnectionString("KittDatabase");

var cmsApi = builder.AddProject<KITT_Cms_Web_Api>("cms-api")
    .WithReference(kittDb);

var webApp = builder.AddProject<KITT_Web_App>("webapp")
    .WithReference(cmsApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();

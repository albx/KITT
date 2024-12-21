using KITT.Web.App.UI;
using KITT.Cms.Web.App.Clients;
using KITT.Proposals.Web.App.Clients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddDefaultServices();
builder.Services.AddCmsClients(builder.HostEnvironment.BaseAddress);
builder.Services.AddProposalsClients(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();

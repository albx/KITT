using KITT.Web.App.UI;
using KITT.Cms.Web.App.Clients;
using KITT.Proposals.Web.App.Clients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KITT.Web.App.Client.Clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddDefaultServices();

builder.Services.AddGeneralClients(builder.HostEnvironment.BaseAddress);
builder.Services.AddCmsClients(builder.HostEnvironment.BaseAddress);
builder.Services.AddProposalsClients(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();

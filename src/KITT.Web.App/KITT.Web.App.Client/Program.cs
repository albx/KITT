using KITT.Web.App.UI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddDefaultServices();

await builder.Build().RunAsync();

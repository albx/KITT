using KITT.Web.App;
using KITT.Web.App.Clients;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddLocalization();

#region Fluent UI
builder.Services.AddFluentUIComponents(options =>
{
    options.ValidateClassNames = false;
});
#endregion

builder.Services
    .AddCmsClients(builder.Configuration["services__cms-api__https"]!);

await builder.Build().RunAsync();

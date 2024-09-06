using KITT.Web.App;
using KITT.Web.App.Clients;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
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

builder.Services.AddConsoleClients(builder.HostEnvironment.BaseAddress);

builder.Services
    .AddHttpClient("KITT.Web.App.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
    .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("KITT.Web.App.ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add($"https://{builder.Configuration["ApiApp:Tenant"]}/{builder.Configuration["ApiApp:AppId"]}/Api.Development");

    options.ProviderOptions.LoginMode = "redirect";
});

await builder.Build().RunAsync();

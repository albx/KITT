using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using KITT.Web.App.Clients;
using KITT.Web.App.Clients.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace KITT.Web.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.Services
                .AddHttpClient("KITT.Web.App.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler(provider =>
                {
                    var handler = provider.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(authorizedUrls: new[] { "https://localhost:5001/api/console" });

                    return handler;
                });

            builder.Services
                .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("KITT.Web.App.ServerAPI"));

            //builder.Services.AddScoped<IStreamingsClient, StreamingsHttpClient>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration["ScopeUri"]);

                options.ProviderOptions.LoginMode = "redirect";
            });

            #region IS4
            //builder.Services
            //    .AddHttpClient<IStreamingsClient, StreamingsHttpClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            //    .AddHttpMessageHandler(provider =>
            //    {
            //        var handler = provider.GetRequiredService<AuthorizationMessageHandler>()
            //            .ConfigureHandler(authorizedUrls: new[] { builder.HostEnvironment.BaseAddress });

            //        return handler;
            //    });

            //builder.Services
            //    .AddHttpClient("IdentityAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            //    .AddHttpMessageHandler(provider =>
            //    {
            //        var handler = provider.GetRequiredService<AuthorizationMessageHandler>()
            //            .ConfigureHandler(authorizedUrls: new[] { builder.HostEnvironment.BaseAddress });

            //        return handler;
            //    });

            //builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("IdentityAPI"));

            //builder.Services.AddApiAuthorization(options =>
            //{
            //    //options.AuthenticationPaths.LogInCallbackPath = "/console/authentication/login-callback";
            //    //options.AuthenticationPaths.LogOutCallbackPath = "/console/authentication/logout-callback";

            //    //options.AuthenticationPaths.LogInPath = "/Account/Login";
            //    //options.AuthenticationPaths.LogOutPath = "/Account/Logout";

            //    options.ProviderOptions.ConfigurationEndpoint = "/_configuration/KITT.Console";
            //});
            #endregion

            await builder.Build().RunAsync();
        }
    }
}

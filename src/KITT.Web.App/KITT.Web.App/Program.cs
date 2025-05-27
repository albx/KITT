using KITT.Core.DependencyInjection;
using KITT.Core.Persistence;
using KITT.Telegram.Messages;
using KITT.Web.App;
using KITT.Web.App.Components;
using KITT.Web.App.Endpoints;
using KITT.Web.App.Endpoints.Services;
using KITT.Web.App.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        options.CallbackPath = "/signin-oidc";
        options.ClientId = builder.Configuration["WEB_APPID"];
        options.Domain = $"{builder.Configuration["DOMAIN_NAME"]}.onmicrosoft.com";
        options.Instance = "https://login.microsoftonline.com/";
        options.ResponseType = "code";
        options.TenantId = builder.Configuration["TENANT_ID"];
        options.ClientSecret = builder.Configuration["WEB_APP_SECRET"];
        options.SaveTokens = true;
    })
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services
    .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
    .Configure(options =>
    {
        options.Scope.Add(OpenIdConnectScope.OfflineAccess);
        options.ClientSecret = builder.Configuration["WEB_APP_SECRET"];
        options.SaveTokens = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddMicrosoftIdentityConsentHandler();

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<KittDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("KittDatabase")));

builder.Services.AddKittCore();

builder.Services.AddDefaultServices();
builder.Services.AddHttpForwarderWithServiceDiscovery();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<SettingsEndpointsServices>();

//TOFIX this line will be fixed in future to use the correct service
builder.Services.AddSingleton<IMessageBus, LocalMessageBus>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(KITT.Web.App.Client._Imports).Assembly);

app
    .MapAuthenticationEndpoints()
    .MapSettingsEndpoints()
    .MapCmsEndpoints()
    .MapProposalEndpoints();

app.Run();

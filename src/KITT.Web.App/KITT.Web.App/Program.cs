using KITT.Web.App.Client;
using KITT.Web.App.Components;
using KITT.Web.App.Endpoints;
using KITT.Web.App.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        options.CallbackPath = "/signin-oidc";
        options.ClientId = builder.Configuration["Identity:WebApp:AppId"];
        options.Domain = $"{builder.Configuration["Identity:DomainName"]}.onmicrosoft.com";
        options.Instance = "https://login.microsoftonline.com/";
        options.ResponseType = "code";
        options.TenantId = builder.Configuration["Identity:TenantId"];
        options.ClientSecret = builder.Configuration["Identity:WebApp:AppSecret"];
        options.SaveTokens = true;

        options.TokenValidationParameters.NameClaimType = "name";
    })
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDistributedTokenCaches();

builder.Services.Configure<MsalDistributedTokenCacheAdapterOptions>(options =>
{
    options.Encrypt = !builder.Environment.IsDevelopment();
});

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("KittDatabase");
    options.SchemaName = "dbo";
    options.TableName = "SecurityCache";

    options.ExpiredItemsDeletionInterval = TimeSpan.FromMinutes(5);
});

builder.Services
    .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
    .Configure(options =>
    {
        options.Scope.Add(OpenIdConnectScope.OfflineAccess);
        options.ClientSecret = builder.Configuration["Identity:WebApp:AppSecret"];
        options.SaveTokens = true;

        options.TokenValidationParameters.NameClaimType = "name";
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddMicrosoftIdentityConsentHandler();

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDefaultServices();
builder.Services.AddHttpForwarderWithServiceDiscovery();
builder.Services.AddProblemDetails();

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
    .AddAdditionalAssemblies([typeof(KITT.Web.App.Client._Imports).Assembly, ..Modules.AdditionalAssemblies]);

app
    .MapAuthenticationEndpoints()
    .MapCmsEndpoints()
    .MapProposalEndpoints();

app.Run();

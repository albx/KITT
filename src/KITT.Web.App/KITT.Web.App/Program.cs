using KITT.Web.App.Components;
using KITT.Web.App.UI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDefaultServices();
builder.Services.AddHttpForwarderWithServiceDiscovery();

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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(KITT.Web.App.Client._Imports).Assembly);

app.MapForwarder("/api/cms/streamings", "https+http://cms-api", "/api/streamings");
app.MapForwarder("/api/cms/streamings/{id}", "https+http://cms-api", "/api/streamings/{id}");
app.MapForwarder("/api/cms/streamings/import", "https+http://cms-api", "/api/streamings/import");

app.Run();

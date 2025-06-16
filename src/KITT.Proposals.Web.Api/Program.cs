using KITT.Core.Persistence;
using KITT.Core.DependencyInjection;
using KITT.Proposals.Web.Api.Endpoints.Services;
using KITT.Proposals.Web.Api.Endpoints;
using KITT.Proposals.Web.Api;
using KITT.Telegram.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<KittDbContext>("KittDatabase");

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = $"https://sts.windows.net/{builder.Configuration["Identity:TenantId"]}/";
        options.Audience = $"api://{builder.Configuration["Identity:Proposals:AppId"]}";
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddKittCore();

//TOFIX this line will be fixed in future to use the correct service
builder.Services.AddSingleton<IMessageBus, LocalMessageBus>();

builder.Services.AddScoped<ProposalsEndpointsServices>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapDefaultEndpoints();
app.MapProposalsEndpoints();

app.Run();

using KITT.Cms.Web.Api.Endpoints;
using KITT.Core.Persistence;
using KITT.Core.DependencyInjection;
using KITT.Cms.Web.Api.Endpoints.Services;
using KITT.Cms.Web.Api;
using KITT.Telegram.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<KittDbContext>("KittDatabase");

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = $"https://sts.windows.net/{builder.Configuration["Identity:TenantId"]}/";
        options.Audience = $"api://{builder.Configuration["Identity:Cms:AppId"]}";
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddKittCore();

builder.Services
    .AddScoped<StreamingsEndpointsServices>()
    .AddScoped<SettingsEndpointsServices>();

builder.Services.AddProblemDetails();

builder.Services.Configure<MessageBusOptions>(
    options => options.ConnectionString = builder.Configuration["QueueClientOptions:ConnectionString"]!);

//TOFIX disable the message bus for now, it will be fixed in future to use the correct service
builder.Services.AddSingleton<IMessageBus, LocalMessageBus>();
//if (builder.Environment.IsDevelopment())
//{
//    builder.Services.AddSingleton<IMessageBus, LocalMessageBus>();
//}
//else
//{
//    builder.Services.AddSingleton<IMessageBus, QueueStorageMessageBus>();
//}

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

app
    .MapStreamingsEndpoints()
    .MapSettingsEndpoints();

app.Run();

#region Testing workaround
public partial class Program { } // This is a workaround for testing purposes, to allow the test project to reference the Program class.
#endregion
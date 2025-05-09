using KITT.Cms.Web.Api.Endpoints;
using KITT.Core.Persistence;
using KITT.Core.DependencyInjection;
using KITT.Cms.Web.Api.Endpoints.Services;
using KITT.Cms.Web.Api;
using KITT.Telegram.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<KittDbContext>("KittDatabase");

builder.Services.AddOpenApi();

builder.Services.AddKittCore();

builder.Services
    .AddScoped<StreamingsEndpointsServices>();

builder.Services.AddProblemDetails();

builder.Services.Configure<MessageBusOptions>(
    options => options.ConnectionString = builder.Configuration["QueueClientOptions:ConnectionString"]!);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IMessageBus, LocalMessageBus>();
}
else
{
    builder.Services.AddSingleton<IMessageBus, QueueStorageMessageBus>();
}

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
app.MapStreamingsEndpoints();

app.Run();

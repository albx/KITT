using KITT.Cms.Web.Api.Endpoints;
using KITT.Core.Persistence;
using KITT.Core.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using KITT.Cms.Web.Api.Endpoints.Services;
using KITT.Cms.Web.Api;
using KITT.Telegram.Messages;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<KittDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("KittDatabase")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options => options.MapType<TimeSpan>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("00:00:00")
    }));

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapStreamingsEndpoints();

app.Run();

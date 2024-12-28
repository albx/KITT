using KITT.Core.Persistence;
using KITT.Core.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using KITT.Proposals.Web.Api.Endpoints.Services;
using KITT.Proposals.Web.Api.Endpoints;
using KITT.Proposals.Web.Api;
using KITT.Telegram.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<KittDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("KittDatabase")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKittCore();

//TOFIX this line will be fixed in future to use the correct service
builder.Services.AddSingleton<IMessageBus, LocalMessageBus>();

builder.Services.AddScoped<ProposalsEndpointsServices>();
builder.Services.AddProblemDetails();

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

app.MapDefaultEndpoints();
app.MapProposalsEndpoints();

app.Run();

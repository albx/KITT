using KITT.Core.Persistence;
using KITT.Core.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using KITT.Proposals.Web.Api.Endpoints.Services;
using KITT.Proposals.Web.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<KittDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("KittDatabase")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKittCore();

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

using KITT.Support.Seeder;
using KITT.Web.Shared.Azure;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureKeyVaultClientWithEmulatorFallback("kitt-keyvault");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

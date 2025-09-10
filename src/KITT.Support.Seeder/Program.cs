using KITT.Core.Persistence;
using KITT.Services;
using KITT.Support.Seeder;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<KittDbContext>(ServiceNames.Database);
//builder.AddAzureKeyVaultClientWithEmulatorFallback("kitt-keyvault");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

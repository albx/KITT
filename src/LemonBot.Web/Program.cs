var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .Build()
    .Configure(builder.Environment);

app.Run();

public partial class Program { }
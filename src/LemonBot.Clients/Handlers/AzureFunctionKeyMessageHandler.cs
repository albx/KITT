using LemonBot.Clients.Configurations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LemonBot.Clients.Handlers;

public class AzureFunctionKeyMessageHandler : DelegatingHandler
{
    private readonly HubOptions _options;
    private readonly IHostEnvironment _environment;

    public AzureFunctionKeyMessageHandler(IOptions<HubOptions> options, IHostEnvironment environment)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            if (string.IsNullOrWhiteSpace(_options.FunctionKey))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            request.Headers.Add("x-functions-key", _options.FunctionKey);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

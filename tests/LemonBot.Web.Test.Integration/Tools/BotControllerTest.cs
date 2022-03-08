using LemonBot.Web.Areas.Tools.Services;
using LemonBot.Web.Test.Integration.Fixtures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Moq;
using System.Net;
using Xunit;

namespace LemonBot.Web.Test.Integration.Tools;

public class BotControllerTest : IClassFixture<KittWebApplicationFactory>
{
    private readonly KittWebApplicationFactory factory;

    public BotControllerTest(KittWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetJobDetails_Should_Call_Client_GetDetailAsync()
    {
        var botClientMock = new Mock<IBotHttpClient>();

        botClientMock.Setup(c => c.GetDetailAsync()).Returns(Task.FromResult(new KITT.Web.Models.Tools.BotJobDetail()));

        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

                    services.AddScoped(provider => botClientMock.Object);
                });
            })
            .CreateClient();

        var response = await client.GetAsync("/api/tools/bot");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        botClientMock.Verify(c => c.GetDetailAsync(), Times.Once);
    }

    [Fact]
    public async Task GetJobDetails_Should_Return_Not_Found_If_InvalidOperationException_Occured()
    {
        string errorMessage = "error from test";
        var botClientMock = new Mock<IBotHttpClient>();

        botClientMock
            .Setup(c => c.GetDetailAsync())
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                    services.AddScoped(provider => botClientMock.Object);
                });
            })
            .CreateClient();

        var response = await client.GetAsync("/api/tools/bot");
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(errorMessage, responseContent);
    }
}

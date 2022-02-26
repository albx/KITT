using KITT.Core.Models;
using KITT.Core.Persistence;
using KITT.Web.Models.Streamings;
using LemonBot.Web.Test.Integration.Fixtures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;
using Xunit;

namespace LemonBot.Web.Test.Integration.Console;

public class StreamingsControllerTest : 
    IClassFixture<KittWebApplicationFactory>
{
    private readonly KittWebApplicationFactory factory;

    public StreamingsControllerTest(KittWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetAllStreamings_Should_Return_Unauthorized_When_No_Access_Token_Is_Specified()
    {
        var client = this.factory.CreateClient();
        var response = await client.GetAsync("/api/console/streamings");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllStreamings_Should_Return_Ok_As_Expected()
    {
        var createdStreamingId = Guid.Empty;
        var scheduleDate = DateTime.Today.AddDays(1);
        var startingTime = TimeSpan.FromHours(16);
        var endingTime = TimeSpan.FromHours(18);
        var userId = TestAuthenticationHandler.UserId;

        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
                });

                builder.ConfigureServices(services =>
                {
                    var provider = services.BuildServiceProvider();

                    using (var scope = provider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;

                        var context = scopedServices.GetRequiredService<KittDbContext>();

                        var streaming = Streaming.Schedule("test", "test", "albx87", scheduleDate, startingTime, endingTime, "albx87", userId);
                        context.Streamings.Add(streaming);
                        context.SaveChanges();
                        createdStreamingId = streaming.Id;
                    }
                });
            })
            .CreateClient();

        var response = await client.GetAsync("/api/console/streamings");
        var responseContent = await response.Content.ReadAsStringAsync();

        var model = JsonConvert.DeserializeObject<StreamingsListModel>(responseContent);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(model);
        Assert.Equal(
            new StreamingsListModel.StreamingListItemModel { Id = createdStreamingId, EndingTime = scheduleDate.AddHours(18), StartingTime = scheduleDate.AddHours(16), HostingChannelUrl = "albx87", ScheduledOn = scheduleDate, Title = "test" },
            model.Items.First());
    }

    [Fact]
    public async Task ScheduleStreaming_Should_Return_Bad_Request_If_Model_Is_Invalid()
    {
        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
                });
            })
            .CreateClient();

        var model = new ScheduleStreamingModel();

        var response = await client.PostAsJsonAsync("/api/console/streamings", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStreamingDetail_Should_Return_Not_Found_If_Streaming_Does_Not_Exist()
    {
        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
                });
            })
            .CreateClient();

        var response = await client.GetAsync($"/api/console/streamings/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

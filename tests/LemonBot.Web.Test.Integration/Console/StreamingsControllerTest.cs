using KITT.Core.Models;
using KITT.Core.Persistence;
using KITT.Web.Models.Streamings;
using LemonBot.Web.Test.Integration.Fixtures;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Text.Json;
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

    #region GetAllStreamings tests
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
                    services.AddTestAuthentication();
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

        var model = await DeserializeFromResponseAsync<StreamingsListModel>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(model);
        Assert.Equal(
            new StreamingsListModel.StreamingListItemModel { Id = createdStreamingId, EndingTime = scheduleDate.AddHours(18), StartingTime = scheduleDate.AddHours(16), HostingChannelUrl = "albx87", ScheduledOn = scheduleDate, Title = "test" },
            model.Items.First());
    }
    #endregion

    #region GetStreamingDetail tests
    [Fact]
    public async Task GetStreamingDetail_Should_Return_Not_Found_If_Streaming_Does_Not_Exist()
    {
        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                });
            })
            .CreateClient();

        var response = await client.GetAsync($"/api/console/streamings/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetStreamingDetail_Should_Return_Streaming_Detail_Information()
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
                    services.AddTestAuthentication();
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

        var response = await client.GetAsync($"/api/console/streamings/{createdStreamingId}");

        var model = await DeserializeFromResponseAsync<StreamingDetailModel>(response);

        var expected = new StreamingDetailModel
        {
            Id = createdStreamingId,
            EndingTime = scheduleDate.Add(endingTime),
            HostingChannelUrl = "albx87",
            ScheduleDate = scheduleDate,
            Slug = "test",
            StartingTime = scheduleDate.Add(startingTime),
            Title = "test"
        };

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expected, model);
    }
    #endregion

    #region ScheduleStreaming tests
    [Fact]
    public async Task ScheduleStreaming_Should_Return_Bad_Request_If_Model_Is_Invalid()
    {
        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                });
            })
            .CreateClient();

        var model = new ScheduleStreamingModel();

        var response = await client.PostAsJsonAsync("/api/console/streamings", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ScheduleStreaming_Should_Return_Created_As_Expected()
    {
        var scheduleDate = DateTime.Today.AddDays(1);
        var startingTime = TimeSpan.FromHours(16);
        var endingTime = TimeSpan.FromHours(18);
        var userId = TestAuthenticationHandler.UserId;

        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                });

                builder.ConfigureServices(services =>
                {
                    var provider = services.BuildServiceProvider();

                    using (var scope = provider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;

                        var context = scopedServices.GetRequiredService<KittDbContext>();

                        if (context.Settings.Any())
                        {
                            context.Settings.RemoveRange(context.Settings);
                        }
                        context.Settings.Add(Settings.CreateNew(userId, "albx87"));
                        context.SaveChanges();
                    }
                });
            })
            .CreateClient();

        var model = new ScheduleStreamingModel
        {
            EndingTime = scheduleDate.Add(endingTime),
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-create",
            StartingTime = scheduleDate.Add(startingTime),
            StreamingAbstract = "test",
            Title = "test create"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings", model);
        var modelFromResponse = await DeserializeFromResponseAsync<ScheduleStreamingModel>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(model, modelFromResponse);
    }
    #endregion

    #region ImportStreaming tests
    [Fact]
    public async Task ImportStreaming_Should_Return_Bad_Request_If_Model_Is_Invalid()
    {
        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                });
            })
            .CreateClient();

        var model = new ImportStreamingModel();

        var response = await client.PostAsJsonAsync("/api/console/streamings/import", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ImportStreaming_Should_Return_Created_As_Expected()
    {
        var scheduleDate = DateTime.Today.AddDays(-1);
        var startingTime = TimeSpan.FromHours(16);
        var endingTime = TimeSpan.FromHours(18);
        var userId = TestAuthenticationHandler.UserId;

        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                });
            })
            .CreateClient();

        var model = new ImportStreamingModel
        {
            EndingTime = scheduleDate.Add(endingTime),
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-import",
            StartingTime = scheduleDate.Add(startingTime),
            StreamingAbstract = "test",
            Title = "test import",
            YoutubeVideoUrl = "youtube.com/test"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings/import", model);
        var modelFromResponse = await DeserializeFromResponseAsync<ImportStreamingModel>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(model, modelFromResponse);
    }
    #endregion

    #region UpdateStreaming tests
    #endregion

    #region DeleteStreaming tests
    #endregion

    #region Private helpers
    private async Task<TModel?> DeserializeFromResponseAsync<TModel>(HttpResponseMessage? response)
        where TModel : class
    {
        if (response is null)
        {
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TModel>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
    #endregion
}

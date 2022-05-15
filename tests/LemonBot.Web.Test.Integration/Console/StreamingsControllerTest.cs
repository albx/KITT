using KITT.Core.Models;
using KITT.Web.Models.Streamings;
using LemonBot.Web.Test.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
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
        var client = this.factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
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
                    DataHelper.PrepareDataForTest(
                        services,
                        context =>
                        {
                            var streaming = Streaming.Schedule("test", "test", "albx87", scheduleDate, startingTime, endingTime, "albx87", userId);
                            context.Streamings.Add(streaming);
                            context.SaveChanges();
                            createdStreamingId = streaming.Id;
                        });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/console/streamings");

        var model = await DeserializeFromResponseAsync<StreamingsListModel>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(model);
        Assert.Equal(
            new StreamingsListModel.StreamingListItemModel { Id = createdStreamingId, EndingTime = scheduleDate.AddHours(18), StartingTime = scheduleDate.AddHours(16), HostingChannelUrl = "albx87", ScheduledOn = scheduleDate, Title = "test" },
            model!.Items.First());
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
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

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
                    DataHelper.PrepareDataForTest(
                        services,
                        context =>
                        {
                            var streaming = Streaming.Schedule("test", "test", "albx87", scheduleDate, startingTime, endingTime, "albx87", userId);
                            context.Streamings.Add(streaming);
                            context.SaveChanges();
                            createdStreamingId = streaming.Id;
                        });
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
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

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

    [Fact]
    public async Task ScheduleStreaming_Should_Return_Bad_Request_If_Schedule_Date_Is_Previous_Than_Today()
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
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ScheduleStreaming_Should_Return_Bad_Request_If_Ending_Time_Is_Previous_Than_Starting_Time()
    {
        var scheduleDate = DateTime.Today.AddDays(1);
        var startingTime = TimeSpan.FromHours(18);
        var endingTime = TimeSpan.FromHours(16);
        var userId = TestAuthenticationHandler.UserId;

        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

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
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

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

    [Fact]
    public async Task ImportStreaming_Should_Return_Bad_Request_If_Ending_Time_Is_Previous_Than_Starting_Time()
    {
        var scheduleDate = DateTime.Today.AddDays(-1);
        var startingTime = TimeSpan.FromHours(18);
        var endingTime = TimeSpan.FromHours(16);
        var userId = TestAuthenticationHandler.UserId;

        var client = this.factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTestAuthentication();
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion

    #region UpdateStreaming tests
    [Fact]
    public async Task UpdateStreaming_Should_Return_Not_Found_If_Streaming_Id_Is_An_Empty_Guid()
    {
        var streamingId = Guid.Empty;
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
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new StreamingDetailModel
        {
            Id = streamingId,
            EndingTime = scheduleDate.Add(endingTime),
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            StartingTime = scheduleDate.Add(startingTime),
            Slug = "test",
            StreamingAbstract = "my abstract",
            Title = "test",
            YoutubeVideoUrl = "https://www.youtube.com/test"
        };

        var response = await client.PutAsJsonAsync($"/api/console/streamings/{streamingId}", model);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStreaming_Should_Return_Bad_Request_If_Model_Is_Invalid()
    {
        var streamingId = Guid.Empty;
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
                    DataHelper.PrepareDataForTest(
                        services,
                        context =>
                        {
                            var streaming = Streaming.Schedule("test", "test", "albx87", scheduleDate, startingTime, endingTime, "albx87", userId);
                            context.Streamings.Add(streaming);
                            streamingId = streaming.Id;

                            context.SaveChanges();
                        });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new StreamingDetailModel { Id = streamingId };

        var response = await client.PutAsJsonAsync($"/api/console/streamings/{streamingId}", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStreaming_Should_Return_Ok_Status_Code_As_Expected()
    {
        var streamingId = Guid.Empty;
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
                    DataHelper.PrepareDataForTest(
                        services,
                        context =>
                        {
                            var streaming = Streaming.Schedule("test", "test", "albx87", scheduleDate, startingTime, endingTime, "albx87", userId);
                            context.Streamings.Add(streaming);
                            streamingId = streaming.Id;

                            context.SaveChanges();
                        });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new StreamingDetailModel
        {
            Id = streamingId,
            EndingTime = scheduleDate.Add(endingTime),
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            StartingTime = scheduleDate.Add(startingTime),
            Slug = "test",
            StreamingAbstract = "my abstract",
            Title = "test",
            YoutubeVideoUrl = "https://www.youtube.com/test"
        };

        var response = await client.PutAsJsonAsync($"/api/console/streamings/{streamingId}", model);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    #endregion

    #region DeleteStreaming tests
    [Fact]
    public async Task DeleteStreaming_Should_Return_Ok_Status_Code_As_Expected()
    {
        var streamingId = Guid.Empty;
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
                    DataHelper.PrepareDataForTest(
                        services,
                        context =>
                        {
                            var streaming = Streaming.Schedule("test", "test", "albx87", scheduleDate, startingTime, endingTime, "albx87", userId);
                            context.Streamings.Add(streaming);
                            streamingId = streaming.Id;

                            context.SaveChanges();
                        });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.DeleteAsync($"/api/console/streamings/{streamingId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStreaming_Should_Return_Not_Found_If_Streaming_Id_Is_An_Empty_Guid()
    {
        var streamingId = Guid.Empty;
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
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.DeleteAsync($"/api/console/streamings/{streamingId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
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

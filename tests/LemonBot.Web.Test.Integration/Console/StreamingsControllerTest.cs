using KITT.Core.Models;
using KITT.Web.Models.Streamings;
using LemonBot.Web.Test.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net;
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
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        TimeOnly startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        TimeOnly endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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

        var model = await response.Content.ReadFromJsonAsync<StreamingsListModel>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(model);
        Assert.Equal(
            new StreamingsListModel.StreamingListItemModel { Id = createdStreamingId, EndingTime = new(18, 0), StartingTime = new(16, 0), HostingChannelUrl = "albx87", ScheduledOn = scheduleDate, Title = "test" },
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
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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

        var model = await response.Content.ReadFromJsonAsync<StreamingDetailModel>();

        var expected = new StreamingDetailModel
        {
            Id = createdStreamingId,
            EndingTime = endingTime,
            HostingChannelUrl = "albx87",
            ScheduleDate = scheduleDate,
            Slug = "test",
            StartingTime = startingTime,
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

                builder.ConfigureServices(services =>
                {
                    DataHelper.PrepareDataForTest(services);
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
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new ScheduleStreamingModel
        {
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-create",
            StartingTime = startingTime,
            StreamingAbstract = "test",
            Title = "test create"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings", model);
        var modelFromResponse = await response.Content.ReadFromJsonAsync<ScheduleStreamingModel>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(model, modelFromResponse);
    }

    [Fact]
    public async Task ScheduleStreaming_Should_Return_Bad_Request_If_Schedule_Date_Is_Previous_Than_Today()
    {
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new ScheduleStreamingModel
        {
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-create",
            StartingTime = startingTime,
            StreamingAbstract = "test",
            Title = "test create"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ScheduleStreaming_Should_Return_Bad_Request_If_Ending_Time_Is_Previous_Than_Starting_Time()
    {
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
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
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new ScheduleStreamingModel
        {
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-create",
            StartingTime = startingTime,
            StreamingAbstract = "test",
            Title = "test create"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ScheduleStreaming_Should_Save_Schedule_Time_Correctly()
    {
        var scheduleDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new ScheduleStreamingModel
        {
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-create",
            StartingTime = startingTime,
            StreamingAbstract = "test",
            Title = "test create"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings", model);
        var modelFromResponse = await response.Content.ReadFromJsonAsync<ScheduleStreamingModel>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.Equal(startingTime, modelFromResponse!.StartingTime);
        Assert.Equal(endingTime, modelFromResponse!.EndingTime);
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

                builder.ConfigureServices(services =>
                {
                    DataHelper.PrepareDataForTest(services);
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

                builder.ConfigureServices(services =>
                {
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new ImportStreamingModel
        {
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-import",
            StartingTime = startingTime,
            StreamingAbstract = "test",
            Title = "test import",
            YoutubeVideoUrl = "youtube.com/test"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings/import", model);
        var modelFromResponse = await response.Content.ReadFromJsonAsync<ImportStreamingModel>();

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

                builder.ConfigureServices(services =>
                {
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new ImportStreamingModel
        {
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-import",
            StartingTime = startingTime,
            StreamingAbstract = "test",
            Title = "test import",
            YoutubeVideoUrl = "youtube.com/test"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings/import", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ImportStreaming_Should_Save_Schedule_Time_Correctly()
    {
        var scheduleDate = DateTime.Now.AddDays(-1);
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
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new ImportStreamingModel
        {
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            Slug = "test-import",
            StartingTime = startingTime,
            StreamingAbstract = "test",
            Title = "test import",
            YoutubeVideoUrl = "youtube.com/test"
        };

        var response = await client.PostAsJsonAsync("/api/console/streamings/import", model);
        var modelFromResponse = await response.Content.ReadFromJsonAsync<ImportStreamingModel>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.Equal(startingTime, modelFromResponse!.StartingTime);
        Assert.Equal(endingTime, modelFromResponse!.EndingTime);
    }
    #endregion

    #region UpdateStreaming tests
    [Fact]
    public async Task UpdateStreaming_Should_Return_Not_Found_If_Streaming_Id_Is_An_Empty_Guid()
    {
        var streamingId = Guid.Empty;
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
                    DataHelper.PrepareDataForTest(services);
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var model = new StreamingDetailModel
        {
            Id = streamingId,
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            StartingTime = startingTime,
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
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
    public async Task UpdateStreaming_Should_Return_NoContent_Status_Code_As_Expected()
    {
        var streamingId = Guid.Empty;
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            StartingTime = startingTime,
            Slug = "test",
            StreamingAbstract = "my abstract",
            Title = "test",
            YoutubeVideoUrl = "https://www.youtube.com/test"
        };

        var response = await client.PutAsJsonAsync($"/api/console/streamings/{streamingId}", model);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStreaming_Should_Return_NoContent_Status_Code_Even_If_Streaming_Has_A_Past_Schedule_Date()
    {
        var streamingId = Guid.Empty;
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
                            var streaming = Streaming.Import("test", "test", "albx87", scheduleDate, startingTime, endingTime, "albx87", "youtube", "my abstract", userId);
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
            EndingTime = endingTime,
            HostingChannelUrl = "https://www.twitch.tv/albx87",
            ScheduleDate = scheduleDate,
            StartingTime = new(16, 30),
            Slug = "test",
            StreamingAbstract = "my abstract",
            Title = "test",
            YoutubeVideoUrl = "https://www.youtube.com/test"
        };

        var response = await client.PutAsJsonAsync($"/api/console/streamings/{streamingId}", model);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    #endregion

    #region DeleteStreaming tests
    [Fact]
    public async Task DeleteStreaming_Should_Return_NoContent_Status_Code_As_Expected()
    {
        var streamingId = Guid.Empty;
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
        var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
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
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStreaming_Should_Return_Not_Found_If_Streaming_Id_Is_An_Empty_Guid()
    {
        var streamingId = Guid.Empty;

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
}

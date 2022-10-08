using KITT.Core.Models;
using KITT.Core.Persistence;
using KITT.Core.ReadModels;
using LemonBot.Web.Test.Integration.Fixtures;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using Xunit;

namespace LemonBot.Web.Test.Integration.Tools;

public class StreamingsControllerTest : IClassFixture<KittWebApplicationFactory>
{
    private readonly KittWebApplicationFactory factory;

    public StreamingsControllerTest(KittWebApplicationFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [Fact]
    public async Task SaveStreamingStats_Should_Return_Not_Found_If_Specified_Streaming_Does_Not_Exist()
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

        var streamingId = Guid.NewGuid();
        var streamingStats = new KITT.Web.Models.Tools.StreamingStats(10, 2, 10, 20);

        var response = await client.PostAsJsonAsync($"/api/tools/streamings/{streamingId}/stats", streamingStats);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SaveStreamingStats_Should_Return_Ok_If_Stats_Has_Been_Saved_Correctly()
    {
        var streamingId = Guid.Empty;

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
                            var streaming = Streaming.Schedule("test", "test", "albx87", new DateTime(2022, 10, 15), new TimeSpan(16, 00, 00), new TimeSpan(18, 00, 00), "albx87", Guid.NewGuid().ToString());
                            context.Streamings.Add(streaming);
                            streamingId = streaming.Id;

                            context.SaveChanges();
                        });
                });
            })
            .CreateClient();

        var streamingStats = new KITT.Web.Models.Tools.StreamingStats(10, 2, 10, 20);
        var response = await client.PostAsJsonAsync($"/api/tools/streamings/{streamingId}/stats", streamingStats);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = this.factory.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<KittDbContext>();

        var streamingStatsFound = dataContext.StreamingStats.ByStreaming(streamingId).FirstOrDefault();

        Assert.Equal(streamingId, streamingStatsFound?.Streaming.Id);
        Assert.Equal(streamingStats.Viewers, streamingStatsFound?.Viewers);
        Assert.Equal(streamingStats.Subscribers, streamingStatsFound?.Subscribers);
        Assert.Equal(streamingStats.UserJoinedNumber, streamingStatsFound?.UserJoinedNumber);
        Assert.Equal(streamingStats.UserLeftNumber, streamingStatsFound?.UserLeftNumber);
    }
}

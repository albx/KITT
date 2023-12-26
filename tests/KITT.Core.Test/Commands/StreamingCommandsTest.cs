using FluentValidation;
using KITT.Core.Commands;
using KITT.Core.Models;
using KITT.Core.Persistence;
using KITT.Core.Test.Fixtures;
using KITT.Core.Validators;
using KITT.Telegram.Messages;
using KITT.Telegram.Messages.Streaming;
using Moq;
using Xunit;

namespace KITT.Core.Test.Commands
{
    public class StreamingCommandsTest : IClassFixture<StreamingCommandsFixture>
    {
        private readonly StreamingCommandsFixture _fixture;

        public StreamingCommandsTest(StreamingCommandsFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        #region Constructor tests
        [Fact]
        public void Ctor_Should_Throw_ArgumentNullException_If_Context_Is_Null()
        {
            KittDbContext context = null;
            var validator = new StreamingValidator(_fixture.Context);
            var messageBus = new Mock<IMessageBus>().Object;

            var ex = Assert.Throws<ArgumentNullException>(() => new StreamingCommands(context, validator, messageBus));
            Assert.Equal(nameof(context), ex.ParamName);
        }

        [Fact]
        public void Ctor_Should_Throw_ArgumentNullException_If_Validator_Is_Null()
        {
            KittDbContext context = _fixture.Context;
            StreamingValidator validator = null;
            var messageBus = new Mock<IMessageBus>().Object;

            var ex = Assert.Throws<ArgumentNullException>(() => new StreamingCommands(context, validator, messageBus));
            Assert.Equal(nameof(validator), ex.ParamName);
        }
        #endregion

        #region ScheduleStreamingAsync tests
        [Fact]
        public async Task ScheduleStreamingAsync_Should_Throw_ValidationException_If_Streaming_Slug_Already_Exists()
        {
            string userId = Guid.NewGuid().ToString();
            _fixture.PrepareData(context =>
            {
                var streaming = Streaming.Schedule(
                    "test1",
                    "test-slug",
                    "albx87",
                    DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                    TimeOnly.FromTimeSpan(TimeSpan.FromHours(16)),
                    TimeOnly.FromTimeSpan(TimeSpan.FromHours(18)),
                    "https://www.twitch.tv/albx87",
                    userId);

                context.Add(streaming);
                context.SaveChanges();
            });

            var validator = new StreamingValidator(_fixture.Context);
            var messageBus = new Mock<IMessageBus>().Object;
            var commands = new StreamingCommands(_fixture.Context, validator, messageBus);

            string twitchChannel = "albx87";
            string streamingTitle = "test";
            string streamingSlug = "test-slug";
            DateOnly scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
            TimeOnly startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(19));
            TimeOnly endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(20));
            string hostingChannelUrl = "https://www.twitch.tv/albx87";
            string streamingAbstract = "streaming abstract";
            var seo = new Content.SeoData();

            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => commands.ScheduleStreamingAsync(
                    userId,
                    twitchChannel,
                    streamingTitle,
                    streamingSlug,
                    scheduleDate,
                    startingTime,
                    endingTime,
                    hostingChannelUrl,
                    streamingAbstract,
                    seo));

            Assert.Contains(nameof(Streaming.Slug), ex.Errors.Select(e => e.PropertyName));
        }

        [Fact]
        public async Task ScheduleStreamingAsync_Should_Add_Streaming_With_Specified_Values()
        {
            var validator = new StreamingValidator(_fixture.Context);
            var messageBus = new Mock<IMessageBus>().Object;
            var commands = new StreamingCommands(_fixture.Context, validator, messageBus);

            string userId = Guid.NewGuid().ToString();
            string twitchChannel = "albx87";
            string streamingTitle = "test";
            string streamingSlug = "test-schedule-streaming-slug";
            DateOnly scheduleDate = DateOnly.FromDateTime(DateTime.Today);
            TimeOnly startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
            TimeOnly endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
            string hostingChannelUrl = "https://www.twitch.tv/albx87";
            string streamingAbstract = "streaming abstract";
            var seo = new Content.SeoData();

            var scheduledStreamingId = await commands.ScheduleStreamingAsync(
                userId,
                twitchChannel,
                streamingTitle,
                streamingSlug,
                scheduleDate,
                startingTime,
                endingTime,
                hostingChannelUrl,
                streamingAbstract,
                seo);

            var scheduledStreaming = _fixture.Context.Streamings.FirstOrDefault(s => s.Id == scheduledStreamingId);

            Assert.NotNull(scheduledStreaming);
            Assert.Equal(twitchChannel, scheduledStreaming.TwitchChannel);
            Assert.Equal(streamingTitle, scheduledStreaming.Title);
            Assert.Equal(streamingSlug, scheduledStreaming.Slug);
            Assert.Equal(scheduleDate, scheduledStreaming.ScheduleDate);
            Assert.Equal(startingTime, scheduledStreaming.StartingTime);
            Assert.Equal(endingTime, scheduledStreaming.EndingTime);
            Assert.Equal(hostingChannelUrl, scheduledStreaming.HostingChannelUrl);
            Assert.Equal(streamingAbstract, scheduledStreaming.Abstract);
        }

        [Fact]
        public async Task ScheduleStreamingAsync_Should_Send_ScheduledStreamingMessage_Correctly()
        {
            var messageBusMock = new Mock<IMessageBus>();

            var validator = new StreamingValidator(_fixture.Context);
            var messageBus = messageBusMock.Object;
            var commands = new StreamingCommands(_fixture.Context, validator, messageBus);

            string userId = Guid.NewGuid().ToString();
            string twitchChannel = "albx87";
            string streamingTitle = "test";
            string streamingSlug = "test-schedule-streaming-slug";
            DateOnly scheduleDate = DateOnly.FromDateTime(DateTime.Today);
            TimeOnly startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(16));
            TimeOnly endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
            string hostingChannelUrl = "https://www.twitch.tv/albx87";
            string streamingAbstract = "streaming abstract";
            var seo = new Content.SeoData();

            var scheduledStreamingId = await commands.ScheduleStreamingAsync(
                userId,
                twitchChannel,
                streamingTitle,
                streamingSlug,
                scheduleDate,
                startingTime,
                endingTime,
                hostingChannelUrl,
                streamingAbstract,
                seo);

            var expectedMessage = new StreamingScheduledMessage(
                scheduledStreamingId,
                streamingTitle,
                streamingSlug,
                scheduleDate,
                startingTime,
                endingTime,
                hostingChannelUrl);

            messageBusMock.Verify(m => m.SendAsync(expectedMessage));
        }
        #endregion

        #region UpdateStreamingAsync tests
        [Fact]
        public async Task UpdateStreamingAsync_Should_Update_Streaming_Information_Correctly()
        {
            var streamingId = Guid.Empty;

            var validator = new StreamingValidator(_fixture.Context);
            var messageBus = new Mock<IMessageBus>().Object;
            var commands = new StreamingCommands(_fixture.Context, validator, messageBus);

            _fixture.PrepareData(context =>
            {
                string userId = Guid.NewGuid().ToString();
                var newStreaming = Streaming.Schedule(
                    "title",
                    "slug",
                    "albx87",
                    DateOnly.FromDateTime(DateTime.Today),
                    TimeOnly.FromTimeSpan(TimeSpan.FromHours(19)),
                    TimeOnly.FromTimeSpan(TimeSpan.FromHours(20)),
                    "https://www.twitch.tv/albx87",
                    userId);

                context.Add(newStreaming);
                context.SaveChanges();

                streamingId = newStreaming.Id;
            });

            string streamingTitle = "new title";
            DateOnly scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
            TimeOnly startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(20));
            TimeOnly endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(21));
            string hostingChannelUrl = "https://www.twitch.tv/newfakechannel";
            string streamingAbstract = "new abstract";
            string youtubeRegistrationLink = "https://www.youtube.com";
            var seo = new Content.SeoData();

            await commands.UpdateStreamingAsync(
                streamingId,
                streamingTitle,
                scheduleDate,
                startingTime,
                endingTime,
                hostingChannelUrl,
                streamingAbstract,
                youtubeRegistrationLink,
                seo);

            var updatedStreaming = _fixture.Context.Streamings.SingleOrDefault(s => s.Id == streamingId);

            Assert.Equal(streamingTitle, updatedStreaming.Title);
            Assert.Equal(scheduleDate, updatedStreaming.ScheduleDate);
            Assert.Equal(startingTime, updatedStreaming.StartingTime);
            Assert.Equal(endingTime, updatedStreaming.EndingTime);
            Assert.Equal(hostingChannelUrl, updatedStreaming.HostingChannelUrl);
            Assert.Equal(streamingAbstract, updatedStreaming.Abstract);
            Assert.Equal(youtubeRegistrationLink, updatedStreaming.YouTubeVideoUrl);
        }

        [Fact]
        public async Task UpdateStreamingAsync_Should_Not_Update_Schedule_If_It_Is_Not_Changed()
        {
            var streamingId = Guid.Empty;
            var scheduleDate = new DateOnly(2022, 01, 01);
            var startingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
            var endingTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(20));

            var validator = new StreamingValidator(_fixture.Context);
            var messageBus = new Mock<IMessageBus>().Object;
            var commands = new StreamingCommands(_fixture.Context, validator, messageBus);

            _fixture.PrepareData(context =>
            {
                string userId = Guid.NewGuid().ToString();
                var newStreaming = Streaming.Import(
                    "title",
                    "slug",
                    "albx87",
                    scheduleDate,
                    startingTime,
                    endingTime,
                    "https://www.twitch.tv/albx87",
                    "",
                    "",
                    userId);

                context.Add(newStreaming);
                context.SaveChanges();

                streamingId = newStreaming.Id;
            });

            string streamingTitle = "new title";
            string hostingChannelUrl = "https://www.twitch.tv/newfakechannel";
            string streamingAbstract = "new abstract";
            string youtubeRegistrationLink = "https://www.youtube.com";
            var seo = new Content.SeoData();

            await commands.UpdateStreamingAsync(
                streamingId,
                streamingTitle,
                scheduleDate,
                startingTime,
                endingTime,
                hostingChannelUrl,
                streamingAbstract,
                youtubeRegistrationLink,
                seo);

            var updatedStreaming = _fixture.Context.Streamings.SingleOrDefault(s => s.Id == streamingId);

            Assert.Equal(scheduleDate, updatedStreaming.ScheduleDate);
            Assert.Equal(startingTime, updatedStreaming.StartingTime);
            Assert.Equal(endingTime, updatedStreaming.EndingTime);
        }
        #endregion

        #region DeleteStreamingAsync tests
        #endregion
    }
}

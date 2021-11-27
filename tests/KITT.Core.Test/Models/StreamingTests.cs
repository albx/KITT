using KITT.Core.Models;
using Xunit;

namespace KITT.Core.Test.Models
{
    public class StreamingTests
    {
        #region Schedule tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_Title_Is_Empty(string title)
        {
            string slug = "test";
            string twitchChannel = "albx87";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);
            string hostingChannelUrl = "albx87";
            string userId = Guid.NewGuid().ToString();

            var ex = Assert.Throws<ArgumentException>(
                () => Streaming.Schedule(title, slug, twitchChannel, scheduleDate, startingTime, endingTime, hostingChannelUrl, userId));

            Assert.Equal(nameof(title), ex.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_Slug_Is_Empty(string slug)
        {
            string title = "test";
            string twitchChannel = "albx87";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);
            string hostingChannelUrl = "albx87";
            string userId = Guid.NewGuid().ToString();

            var ex = Assert.Throws<ArgumentException>(
                () => Streaming.Schedule(title, slug, twitchChannel, scheduleDate, startingTime, endingTime, hostingChannelUrl, userId));

            Assert.Equal(nameof(slug), ex.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_Twitch_Channel_Is_Empty(string twitchChannel)
        {
            string title = "test";
            string slug = "test";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);
            string hostingChannelUrl = "albx87";
            string userId = Guid.NewGuid().ToString();

            var ex = Assert.Throws<ArgumentException>(
                () => Streaming.Schedule(title, slug, twitchChannel, scheduleDate, startingTime, endingTime, hostingChannelUrl, userId));

            Assert.Equal(nameof(twitchChannel), ex.ParamName);
        }

        [Fact]
        public void Schedule_Should_Throw_ArgumentException_If_Schedule_Date_Is_Past_Date()
        {
            string title = "test";
            string slug = "test";
            string twitchChannel = "albx87";
            DateTime scheduleDate = DateTime.Today.AddDays(-1);
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);
            string hostingChannelUrl = "albx87";
            string userId = Guid.NewGuid().ToString();

            var ex = Assert.Throws<ArgumentException>(
                () => Streaming.Schedule(title, slug, twitchChannel, scheduleDate, startingTime, endingTime, hostingChannelUrl, userId));

            Assert.Equal(nameof(scheduleDate), ex.ParamName);
        }

        [Fact]
        public void Schedule_Should_Throw_ArgumentException_If_Ending_Time_Is_Previous_Than_Starting_Time()
        {
            string title = "test";
            string slug = "test";
            string twitchChannel = "albx87";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(15);
            string hostingChannelUrl = "albx87";
            string userId = Guid.NewGuid().ToString();

            var ex = Assert.Throws<ArgumentException>(
                () => Streaming.Schedule(title, slug, twitchChannel, scheduleDate, startingTime, endingTime, hostingChannelUrl, userId));

            Assert.Equal(nameof(endingTime), ex.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_Hosting_Channel_Url_Is_Empty(string hostingChannelUrl)
        {
            string title = "test";
            string slug = "test";
            string twitchChannel = "albx87";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);
            string userId = Guid.NewGuid().ToString();

            var ex = Assert.Throws<ArgumentException>(
                () => Streaming.Schedule(title, slug, twitchChannel, scheduleDate, startingTime, endingTime, hostingChannelUrl, userId));

            Assert.Equal(nameof(hostingChannelUrl), ex.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_UserId_Is_Empty(string userId)
        {
            string title = "test";
            string slug = "test";
            string twitchChannel = "albx87";
            string hostingChannelUrl = "albx87";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);

            var ex = Assert.Throws<ArgumentException>(
                () => Streaming.Schedule(title, slug, twitchChannel, scheduleDate, startingTime, endingTime, hostingChannelUrl, userId));

            Assert.Equal(nameof(userId), ex.ParamName);
        }

        [Fact]
        public void Schedule_Should_Create_A_New_Streaming_With_Specified_Values()
        {
            string title = "test";
            string slug = "test";
            string twitchChannel = "albx87";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);
            string hostingChannelUrl = "albx87";
            string userId = Guid.NewGuid().ToString();

            var streaming = Streaming.Schedule(
                title, 
                slug, 
                twitchChannel, 
                scheduleDate, 
                startingTime, 
                endingTime, 
                hostingChannelUrl, 
                userId);

            Assert.Equal(title, streaming.Title);
            Assert.Equal(slug, streaming.Slug);
            Assert.Equal(twitchChannel, streaming.TwitchChannel);
            Assert.Equal(scheduleDate, streaming.ScheduleDate);
            Assert.Equal(startingTime, streaming.StartingTime);
            Assert.Equal(endingTime, streaming.EndingTime);
            Assert.Equal(hostingChannelUrl, streaming.HostingChannelUrl);
            Assert.NotEqual(Guid.Empty, streaming.Id);
            Assert.Equal(userId, streaming.UserId);
        }
        #endregion

        #region ChangeTitle tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeTitle_Should_Throw_ArgumentException_If_Title_Is_Empty(string title)
        {
            var streaming = CreateStreamingForTests();

            var ex = Assert.Throws<ArgumentException>(() => streaming.ChangeTitle(title));
            Assert.Equal(nameof(title), ex.ParamName);
        }

        [Fact]
        public void ChangeTitle_Should_Change_Streaming_Title_With_Specified_Value()
        {
            var streaming = CreateStreamingForTests();
            string title = "new title";

            streaming.ChangeTitle(title);
            Assert.Equal(title, streaming.Title);
        }
        #endregion

        #region ChangeSchedule tests
        [Fact]
        public void ChangeSchedule_Should_Throw_ArgumentException_If_Schedule_Date_Is_Past_Date()
        {
            var streaming = CreateStreamingForTests();
            DateTime scheduleDate = DateTime.Today.AddDays(-1);
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);

            var ex = Assert.Throws<ArgumentException>(() => streaming.ChangeSchedule(scheduleDate, startingTime, endingTime));
            Assert.Equal(nameof(scheduleDate), ex.ParamName);
        }

        [Fact]
        public void ChangeSchedule_Should_Throw_ArgumentException_If_Ending_Time_Is_Previous_Than_Starting_Time()
        {
            var streaming = CreateStreamingForTests();
            DateTime scheduleDate = DateTime.Today.AddDays(1);
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(15);

            var ex = Assert.Throws<ArgumentException>(() => streaming.ChangeSchedule(scheduleDate, startingTime, endingTime));
            Assert.Equal(nameof(endingTime), ex.ParamName);
        }

        [Fact]
        public void ChangeSchedule_Should_Change_Schedule_Date_And_Times_Correctly()
        {
            var streaming = CreateStreamingForTests();
            DateTime scheduleDate = DateTime.Today.AddDays(1);
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);

            streaming.ChangeSchedule(scheduleDate, startingTime, endingTime);

            Assert.Equal(scheduleDate, streaming.ScheduleDate);
            Assert.Equal(startingTime, streaming.StartingTime);
            Assert.Equal(endingTime, streaming.EndingTime);
        }
        #endregion

        #region SetAbstract tests
        [Fact]
        public void SetAbstract_Should_Set_Streaming_Abstract_With_Specified_Value()
        {
            var streaming = CreateStreamingForTests();
            string streamingAbstract = "test abstract";

            streaming.SetAbstract(streamingAbstract);
            Assert.Equal(streamingAbstract, streaming.Abstract);
        }
        #endregion

        #region SetRegistrationYoutubeUrl tests
        [Fact]
        public void SetRegistrationYoutubeUrl_Should_Set_Youtube_Registration_Url_With_Specified_Value()
        {
            var streaming = CreateStreamingForTests();
            string youtubeUrl = "https://www.youtube.com";

            streaming.SetRegistrationYoutubeUrl(youtubeUrl);
            Assert.Equal(youtubeUrl, streaming.YouTubeVideoUrl);
        }
        #endregion

        #region Helpers
        private Streaming CreateStreamingForTests()
        {
            string userId = Guid.NewGuid().ToString();
            string title = "test";
            string slug = "test";
            string twitchChannel = "albx87";
            DateTime scheduleDate = DateTime.Today;
            TimeSpan startingTime = TimeSpan.FromHours(16);
            TimeSpan endingTime = TimeSpan.FromHours(18);
            string hostingChannelUrl = "albx87";

            var streaming = Streaming.Schedule(
                title,
                slug,
                twitchChannel,
                scheduleDate,
                startingTime,
                endingTime,
                hostingChannelUrl,
                userId);

            return streaming;
        }
        #endregion
    }
}

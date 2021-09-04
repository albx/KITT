using System;
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
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_Slug_Is_Empty(string slug)
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_Twitch_Channel_Is_Empty(string twitchChannel)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Schedule_Should_Throw_ArgumentException_If_Schedule_Date_Is_Past_Date()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Schedule_Should_Throw_ArgumentException_If_Ending_Time_Is_Previous_Than_Starting_Time()
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Schedule_Should_Throw_ArgumentException_If_Hosting_Channel_Url_Is_Empty(string hostingChannelUrl)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Schedule_Should_Create_A_New_Streaming_With_Specified_Values()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ChangeSchedule tests
        #endregion

        #region SetAbstract tests
        #endregion

        #region SetRegistrationYoutubeUrl tests
        #endregion
    }
}

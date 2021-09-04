using System;

namespace KITT.Core.Models
{
    public class Streaming
    {
        public Guid Id { get; protected set; }

        public string TwitchChannel { get; protected set; }

        public string Title { get; protected set; }

        public string Slug { get; protected set; }

        public DateTime ScheduleDate { get; protected set; }

        public TimeSpan StartingTime { get; protected set; }

        public TimeSpan EndingTime { get; protected set; }

        public string HostingChannelUrl { get; protected set; }

        public string YouTubeVideoUrl { get; protected set; }

        public string Abstract { get; protected set; }

        #region Constructor
        protected Streaming() { }
        #endregion

        #region Behaviors
        public void ChangeTitle(string title)
        {
            throw new NotImplementedException();
        }

        public void ChangeSchedule(DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime)
        {
            throw new NotImplementedException();
        }

        public void SetAbstract(string streamingAbstract)
        {
            throw new NotImplementedException();
        }

        public void SetRegistrationYoutubeUrl(string youtubeUrl)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Factory
        public static Streaming Schedule(string title, string slug, string twitchChannel, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

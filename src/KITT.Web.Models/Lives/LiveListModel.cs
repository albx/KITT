using System;
using System.Collections.Generic;

namespace KITT.Web.Models.Lives
{
    public class LiveListModel
    {
        public IEnumerable<LiveListItemModel> Items { get; set; }

        public record LiveListItemModel
        {
            public Guid Id { get; set; }

            public string Title { get; set; }

            public DateTime ScheduledOn { get; set; }

            public TimeSpan StartingTime { get; set; }

            public TimeSpan EndingTime { get; set; }

            public string TwitchChannelUrl { get; set; }

            public string YouTubeVideoUrl { get; set; }
        }
    }
}

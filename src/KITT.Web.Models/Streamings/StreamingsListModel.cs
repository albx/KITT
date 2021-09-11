using System;
using System.Collections.Generic;

namespace KITT.Web.Models.Streamings
{
    public class StreamingsListModel
    {
        public IEnumerable<StreamingListItemModel> Items { get; set; }

        public record StreamingListItemModel
        {
            public Guid Id { get; set; }

            public string Title { get; set; }

            public DateTime ScheduledOn { get; set; }

            public DateTime StartingTime { get; set; }

            public DateTime EndingTime { get; set; }

            public string HostingChannelUrl { get; set; }

            public string YouTubeVideoUrl { get; set; }
        }
    }
}

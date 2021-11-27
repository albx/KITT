namespace KITT.Web.Models.Streamings;

public class StreamingsListModel
{
    public IEnumerable<StreamingListItemModel> Items { get; set; } = Array.Empty<StreamingListItemModel>();

    public record StreamingListItemModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime ScheduledOn { get; set; }

        public DateTime StartingTime { get; set; }

        public DateTime EndingTime { get; set; }

        public string HostingChannelUrl { get; set; } = string.Empty;

        public string? YouTubeVideoUrl { get; set; }
    }
}

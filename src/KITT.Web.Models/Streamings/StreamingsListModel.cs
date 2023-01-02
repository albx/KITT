namespace KITT.Web.Models.Streamings;

public class StreamingsListModel
{
    public int TotalItems { get; set; }

    public IEnumerable<StreamingListItemModel> Items { get; set; } = Array.Empty<StreamingListItemModel>();

    public record StreamingListItemModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime ScheduledOn { get; set; }

        public TimeSpan StartingTime { get; set; }

        public TimeSpan EndingTime { get; set; }

        public string HostingChannelUrl { get; set; } = string.Empty;

        public string? YouTubeVideoUrl { get; set; }
    }
}

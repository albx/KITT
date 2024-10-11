namespace KITT.Cms.Web.Models.Streamings;

public class StreamingsListModel
{
    public int TotalItems { get; set; }

    public IEnumerable<StreamingListItemModel> Items { get; set; } = Array.Empty<StreamingListItemModel>();

    public record StreamingListItemModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateOnly ScheduledOn { get; set; }

        public TimeOnly StartingTime { get; set; }

        public TimeOnly EndingTime { get; set; }

        public string HostingChannelUrl { get; set; } = string.Empty;

        public string? YouTubeVideoUrl { get; set; }
    }
}

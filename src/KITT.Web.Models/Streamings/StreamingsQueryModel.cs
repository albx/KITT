using System.Web;

namespace KITT.Web.Models.Streamings;

public record StreamingQueryModel
{
    public string Query { get; set; } = string.Empty;

    public SortDirection ScheduleSort { get; set; } = SortDirection.Ascending;

    public int Page { get; set; } = 0;

    public int Size { get; set; } = 10;

    public string ToQueryString()
    {
        var queryItems = new List<string>
        {
            $"p={Page}",
            $"s={Size}",
            $"sort={ScheduleSort}"
        };

        if (!string.IsNullOrWhiteSpace(Query))
        {
            queryItems.Add($"q={HttpUtility.UrlEncode(Query)}");
        }

        return string.Join("&", queryItems);
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}

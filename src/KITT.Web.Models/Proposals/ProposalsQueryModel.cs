using System.Web;

namespace KITT.Web.Models.Proposals;

public record ProposalsQueryModel
{
    public string Query { get; set; } = string.Empty;

    public SortDirection ScheduleSort { get; set; } = SortDirection.Descending;

    public int Size { get; set; } = 10;

    public string ToQueryString()
    {
        var queryItems = new List<string>
        {
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

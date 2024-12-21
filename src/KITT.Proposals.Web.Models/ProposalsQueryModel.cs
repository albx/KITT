using System.Web;

namespace KITT.Proposals.Web.Models;

public record ProposalsQueryModel
{
    public string Query { get; set; } = string.Empty;

    public SortDirection ScheduleSort { get; set; } = SortDirection.Descending;

    public ProposalStatus? Status { get; set; } = null;

    public int Size { get; set; } = 10;

    public string ToQueryString()
    {
        var queryItems = new List<string>
        {
            $"s={Size}",
            $"sort={ScheduleSort}"
        };

        if (Status is not null)
        {
            queryItems.Add($"st={Status}");
        }

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

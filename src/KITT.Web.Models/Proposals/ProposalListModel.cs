namespace KITT.Web.Models.Proposals;

public class ProposalListModel
{
    public int TotalItems { get; set; }

    public IEnumerable<ProposalListItemModel> Items { get; set; } = Array.Empty<ProposalListItemModel>();

    public record ProposalListItemModel
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public string? AuthorNickname { get; init; }

        public ProposalStatus Status { get; init; }

        public DateTime SubmittedAt { get; init; }
    }
}

namespace KITT.Proposals.Web.Models;

public class ProposalDetailModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string? AuthorNickname { get; init; }

    public ProposalStatus Status { get; init; }

    public DateTime SubmittedAt { get; init; }
}

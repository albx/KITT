namespace KITT.Core.Models;

public class Proposal
{
    public Guid Id { get; protected set; }

    public string AuthorNickname { get; protected set; }

    public string Title { get; protected set; }

    public string Description { get; protected set; }

    public DateTime SubmittedAt { get; protected set; }
}

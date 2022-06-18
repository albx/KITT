namespace KITT.Core.Models;

public class Proposal
{
    public Guid Id { get; protected set; }

    public string AuthorNickname { get; protected set; }

    public string Title { get; protected set; }

    public string Description { get; protected set; }

    public DateTime SubmittedAt { get; protected set; }

    public ProposalStatus Status { get; protected set; } = ProposalStatus.Moderating;

    #region Public methods
    public void MarkAsAccepted()
    {
        if (Status != ProposalStatus.Moderating)
        {
            throw new InvalidOperationException("Proposal already move to approval flow");
        }

        Status = ProposalStatus.WaitingForApproval;
    }
    #endregion

    #region Inner classes
    public enum ProposalStatus
    {
        Moderating,
        WaitingForApproval,
    }
    #endregion
}

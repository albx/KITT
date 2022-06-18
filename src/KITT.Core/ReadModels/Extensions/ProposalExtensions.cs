namespace KITT.Core.ReadModels;

public static class ProposalExtensions
{
    public static IQueryable<Proposal> Moderating(this IQueryable<Proposal> proposals)
        => proposals.Where(p => p.Status == Proposal.ProposalStatus.Moderating);

    public static IQueryable<Proposal> WaitingForApproval(this IQueryable<Proposal> proposals)
        => proposals.Where(p => p.Status == Proposal.ProposalStatus.WaitingForApproval);
}

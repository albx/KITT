namespace KITT.Core.ReadModels;

public static class ProposalExtensions
{
    extension(IQueryable<Proposal> proposals)
    {
        public IQueryable<Proposal> Moderating() => proposals.Where(p => p.Status == Proposal.ProposalStatus.Moderating);

        public IQueryable<Proposal> WaitingForApproval() => proposals.Where(p => p.Status == Proposal.ProposalStatus.WaitingForApproval);

        public IQueryable<Proposal> OrderedBySubmissionDate(bool ascending = true)
        {
            return ascending switch
            {
                false => proposals.OrderByDescending(p => p.SubmittedAt),
                _ => proposals.OrderBy(p => p.SubmittedAt)
            };
        }
    }
    
}

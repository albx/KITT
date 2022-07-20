namespace KITT.Core.Commands;

public class ProposalCommands : IProposalCommands
{
    private readonly KittDbContext _context;

    public ProposalCommands(KittDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task Accept(Guid proposalId)
    {
        if (proposalId == Guid.Empty)
        {
            throw new ArgumentException("value cannot be empty", nameof(proposalId));
        }

        var proposal = _context.Proposals.SingleOrDefault(p => p.Id == proposalId);
        if (proposal is null)
        {
            throw new ArgumentOutOfRangeException(nameof(proposalId));
        }

        proposal.MarkAsAccepted();
        return _context.SaveChangesAsync();
    }

    public Task Refuse(Guid proposalId)
    {
        if (proposalId == Guid.Empty)
        {
            throw new ArgumentException("value cannot be empty", nameof(proposalId));
        }

        var proposal = _context.Proposals.SingleOrDefault(p => p.Id == proposalId);
        if (proposal is null)
        {
            throw new ArgumentOutOfRangeException(nameof(proposalId));
        }

        if (proposal.Status != Proposal.ProposalStatus.Moderating)
        {
            throw new InvalidOperationException("Proposal already accepted");
        }

        _context.Proposals.Remove(proposal);
        return _context.SaveChangesAsync();
    }

    public Task Reject(Guid proposalId)
    {
        if (proposalId == Guid.Empty)
        {
            throw new ArgumentException("value cannot be empty", nameof(proposalId));
        }

        var proposal = _context.Proposals.SingleOrDefault(p => p.Id == proposalId);
        if (proposal is null)
        {
            throw new ArgumentOutOfRangeException(nameof(proposalId));
        }

        if (proposal.Status != Proposal.ProposalStatus.WaitingForApproval)
        {
            throw new InvalidOperationException("Proposal not accepted yet");
        }

        _context.Proposals.Remove(proposal);
        return _context.SaveChangesAsync();
    }
}

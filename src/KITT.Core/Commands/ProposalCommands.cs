using KITT.Core.Validators;

namespace KITT.Core.Commands;

public class ProposalCommands : IProposalCommands
{
    private readonly KittDbContext _context;
    private readonly StreamingValidator _validator;

    public ProposalCommands(KittDbContext context, StreamingValidator validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator;
    }

    public Task AcceptProposalAsync(Guid proposalId)
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

    public Task RefuseProposalAsync(Guid proposalId)
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

    public Task RejectProposalAsync(Guid proposalId)
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

    public Task ScheduleProposalAsync(Guid proposalId, string userId, string twitchChannel, string title, string slug, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string hostingChannelUrl, string streamingAbstract)
    {
        var proposal = _context.Proposals.SingleOrDefault(p => p.Id == proposalId);
        if (proposal is null)
        {
            throw new ArgumentOutOfRangeException(nameof(proposalId));
        }

        ScheduleStreaming(
            userId,
            twitchChannel,
            title,
            slug,
            scheduleDate,
            startingTime,
            endingTime,
            streamingAbstract,
            hostingChannelUrl);

        _context.Proposals.Remove(proposal);

        return _context.SaveChangesAsync();
    }

    private void ScheduleStreaming(string userId, string twitchChannel, string streamingTitle, string streamingSlug, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string streamingAbstract, string hostingChannelUrl)
    {
        var streaming = Streaming.Schedule(
            streamingTitle,
            streamingSlug,
            twitchChannel,
            scheduleDate,
            startingTime,
            endingTime,
            hostingChannelUrl,
            userId);

        if (!string.IsNullOrWhiteSpace(streamingAbstract))
        {
            streaming.SetAbstract(streamingAbstract);
        }

        _validator.ValidateForScheduleStreaming(streaming);

        _context.Streamings.Add(streaming);
    }
}

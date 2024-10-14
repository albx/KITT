namespace KITT.Web.App.Models;

public class DashboardModel
{
    public StreamingStatsDescriptor? StreamingStats { get; init; }

    public ProposalStatsDescriptor? ProposalStats { get; init; }

    public record StreamingStatsDescriptor(
        int NumberOfStreamingsDelivered,
        int NumberOfStreamingsScheduled);

    public record ProposalStatsDescriptor(
        int TotalNumberOfProposals,
        int NumberOfProposalAccepted);
}

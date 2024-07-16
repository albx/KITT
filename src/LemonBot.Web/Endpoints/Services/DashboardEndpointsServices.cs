using KITT.Core.ReadModels;
using KITT.Web.Models.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace LemonBot.Web.Endpoints.Services;

public class DashboardEndpointsServices
{
    public IDatabase Database { get; }

    public DashboardEndpointsServices(IDatabase database)
    {
        Database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<DashboardModel> GetDashboardAsync(string userId)
    {
        var streamingsQuery = Database.Streamings.ByUserId(userId).OrderedBySchedule();
        var deliveredStreamingsNumber = await streamingsQuery.DeliveredOnly().CountAsync();
        var scheduledStreamingsNumber = await streamingsQuery.Scheduled().CountAsync();

        var proposalsQuery = Database.Proposals;

        var totalNumberOfProposals = await proposalsQuery.CountAsync();
        var acceptedProposalsNumber = await proposalsQuery.WaitingForApproval().CountAsync();

        var model = new DashboardModel 
        {
            StreamingStats = new(deliveredStreamingsNumber, scheduledStreamingsNumber),
            ProposalStats = new(totalNumberOfProposals, acceptedProposalsNumber)
        };

        return model;
    }
}

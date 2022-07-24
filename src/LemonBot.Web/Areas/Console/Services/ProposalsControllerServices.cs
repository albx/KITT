using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Web.Models.Proposals;

namespace LemonBot.Web.Areas.Console.Services;

public class ProposalsControllerServices
{
    public IDatabase Database { get; }
    public IProposalCommands Commands { get; }

    public ProposalsControllerServices(IDatabase database, IProposalCommands commands)
    {
        Database = database ?? throw new ArgumentNullException(nameof(database));
        Commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    public ProposalListModel GetAllProposals(int size, ProposalsQueryModel.SortDirection sort, string? query, ProposalStatus? status)
    {
        var ascending = sort == ProposalsQueryModel.SortDirection.Ascending;

        var proposalsQuery = Database.Proposals;
        if (!string.IsNullOrWhiteSpace(query))
        {
            proposalsQuery = proposalsQuery.Where(p => p.Title.Contains(query) || p.Description.Contains(query));
        }

        if (status is not null)
        {
            proposalsQuery = status switch
            {
                ProposalStatus.WaitingForApproval => proposalsQuery.WaitingForApproval(),
                ProposalStatus.Moderating => proposalsQuery.Moderating(),
                _ => proposalsQuery
            };
        }

        var proposals = proposalsQuery
            .OrderedBySubmissionDate(ascending)
            .Select(p => new ProposalListModel.ProposalListItemModel
            {
                Id = p.Id,
                AuthorNickname = p.AuthorNickname,
                Description = p.Description,
                Title = p.Title,
                SubmittedAt = p.SubmittedAt,
                Status = Enum.Parse<ProposalStatus>(p.Status.ToString())
            }).Take(size).ToArray();

        var model = new ProposalListModel { TotalItems = proposalsQuery.Count(), Items = proposals };
        return model;
    }

    public ProposalDetailModel? GetProposalDetail(Guid proposalId)
    {
        var proposal = Database.Proposals.SingleOrDefault(p => p.Id == proposalId);
        if (proposal == null)
        {
            return null;
        }

        var model = new ProposalDetailModel
        {
            Id = proposal.Id,
            AuthorNickname = proposal.AuthorNickname,
            Description = proposal.Description,
            Status = (ProposalStatus)proposal.Status,
            SubmittedAt = proposal.SubmittedAt,
            Title = proposal.Title
        };

        return model;
    }

    public Task AcceptProposalAsync(Guid proposalId) => Commands.AcceptProposalAsync(proposalId);

    public Task RejectProposalAsync(Guid proposalId) => Commands.RejectProposalAsync(proposalId);

    public Task RefuseProposalAsync(Guid proposalId) => Commands.RefuseProposalAsync(proposalId);

    public Task ScheduleProposalAsync(Guid proposalId, ScheduleProposalModel model, string userId)
    {
        var settings = Database.Settings
            .ByUserId(userId)
            .FirstOrDefault();

        if (settings is null)
        {
            throw new InvalidOperationException("No settings configured");
        }

        return Commands.ScheduleProposalAsync(
            proposalId,
            userId,
            settings.TwitchChannel,
            model.Title,
            model.Slug,
            model.ScheduleDate,
            model.StartingTime.TimeOfDay,
            model.EndingTime.TimeOfDay,
            model.HostingChannelUrl,
            model.StreamingAbstract);
    }
}

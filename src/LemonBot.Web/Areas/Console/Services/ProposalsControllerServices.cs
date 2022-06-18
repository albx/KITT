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

    public ProposalListModel GetAllProposals()
    {
        var proposalsQuery = Database.Proposals;

        var proposals = proposalsQuery
            .OrderBy(p => p.SubmittedAt)
            .Select(p => new ProposalListModel.ProposalListItemModel
            {
                Id = p.Id,
                AuthorNickname = p.AuthorNickname,
                Description = p.Description,
                Title = p.Title,
                Status = (ProposalStatus)p.Status,
                SubmittedAt = p.SubmittedAt
            }).ToArray();

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

    public Task AcceptProposal(Guid proposalId) => Commands.Accept(proposalId);

    public Task DeleteProposal(Guid proposalId) => Commands.Delete(proposalId);

    public Task RefuseProposal(Guid proposalId) => Commands.Refuse(proposalId);
}

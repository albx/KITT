using KITT.Web.Models.Proposals;

namespace KITT.Web.App.Clients;

public interface IProposalsClient
{
    Task<ProposalListModel> GetAllProposalsAsync(ProposalsQueryModel query);

    Task AcceptProposalAsync(Guid proposalId);

    Task RejectProposalAsync(Guid proposalId);

    Task RefuseProposalAsync(Guid proposalId);

    Task<ProposalDetailModel?> GetProposalDetailAsync(Guid proposalId);

    Task ScheduleProposalAsync(Guid proposalId, ScheduleProposalModel model);
}

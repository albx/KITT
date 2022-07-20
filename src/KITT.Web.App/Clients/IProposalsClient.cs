using KITT.Web.Models.Proposals;

namespace KITT.Web.App.Clients;

public interface IProposalsClient
{
    Task<ProposalListModel> GetAllProposalsAsync(ProposalsQueryModel query);
}

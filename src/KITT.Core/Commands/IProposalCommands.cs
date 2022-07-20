namespace KITT.Core.Commands;

public interface IProposalCommands
{
    Task Accept(Guid proposalId);

    Task Refuse(Guid proposalId);

    Task Reject(Guid proposalId);
}

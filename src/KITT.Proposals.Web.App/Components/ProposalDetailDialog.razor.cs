using KITT.Proposals.Web.App.Clients;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Proposals.Web.App.Components;

public partial class ProposalDetailDialog
{
    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Parameter]
    public InputModel Content { get; set; } = default!;

    [Inject]
    public IProposalsClient Client { get; set; } = default!;

    public record InputModel(
        Guid ProposalId);
}

using KITT.Proposals.Web.App.Clients;
using KITT.Proposals.Web.Models;
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

    private ProposalDetailModel? model;

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        try
        {
            model = await Client.GetProposalDetailAsync(Content.ProposalId);
        }
        finally
        {
            loading = false;
        }
    }

    private async Task CloseAsync() => await Dialog.CancelAsync();

    public record InputModel(
        Guid ProposalId);
}

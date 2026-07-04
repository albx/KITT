using KITT.Proposals.Web.App.Clients;
using KITT.Proposals.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Proposals.Web.App.Components;

public partial class ProposalDetailDialog
{
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

    protected override void OnInitializeDialog(DialogOptionsHeader header, DialogOptionsFooter footer)
    {
        footer.PrimaryAction.Label = LocalizerFor[nameof(Resources.Components.ProposalDetailDialog.CloseButtonText)];
        footer.SecondaryAction.Visible = false;
    }

    protected override async Task OnActionClickedAsync(bool primary) => await CloseAsync();

    private async Task CloseAsync() => await DialogInstance.CloseAsync();

    public record InputModel(
        Guid ProposalId);
}

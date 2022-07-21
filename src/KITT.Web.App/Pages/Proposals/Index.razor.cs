using KITT.Web.App.Clients;
using KITT.Web.App.UI.Components;
using KITT.Web.Models.Proposals;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KITT.Web.App.Pages.Proposals;

public partial class Index
{
    private ProposalsQueryModel query = new();

    private ProposalListModel model = new();

    private bool isLoading = false;

    private int[] sizes = new[] { 10, 25, 50 };

    [Inject]
    public IProposalsClient Client { get; set; } = default!;

    [Inject]
    IDialogService Dialog { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    async Task Search()
    {
        await LoadProposalsAsync(query);
    }

    async void ClearSearch()
    {
        query = new();
        await LoadProposalsAsync(query);
        StateHasChanged();
    }

    async Task AcceptProposalAsync(ProposalListModel.ProposalListItemModel proposal)
    {
        var proposalTitle = proposal.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Proposals.Index.AcceptProposalConfirmText), proposalTitle];

        var confirm = await Dialog.Show<ConfirmDialog>(
            Localizer[nameof(Resources.Pages.Proposals.Index.AcceptProposalConfirmTitle)],
            new DialogParameters
            {
                [nameof(ConfirmDialog.ConfirmText)] = confirmText
            }).Result;

        if (!confirm.Cancelled)
        {
            try
            {
                await Client.AcceptProposalAsync(proposal.Id);
                Snackbar.Add(Localizer[nameof(Resources.Pages.Proposals.Index.AcceptProposalSuccessMessage), proposalTitle], Severity.Success);

                await LoadProposalsAsync(query);
            }
            catch
            {
                Snackbar.Add(Localizer[nameof(Resources.Pages.Proposals.Index.AcceptProposalErrorMessage), proposalTitle], Severity.Error);
            }
        }
    }

    async Task RejectProposalAsync(ProposalListModel.ProposalListItemModel proposal)
    {
        var proposalTitle = proposal.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Proposals.Index.RejectProposalConfirmText), proposalTitle];

        var confirm = await Dialog.Show<ConfirmDialog>(
            Localizer[nameof(Resources.Pages.Proposals.Index.RejectProposalConfirmTitle)],
            new DialogParameters
            {
                [nameof(ConfirmDialog.ConfirmText)] = confirmText
            }).Result;

        if (!confirm.Cancelled)
        {
            try
            {
                await Client.RejectProposalAsync(proposal.Id);
                Snackbar.Add(Localizer[nameof(Resources.Pages.Proposals.Index.RejectProposalSuccessMessage), proposalTitle], Severity.Success);

                await LoadProposalsAsync(query);
            }
            catch
            {
                Snackbar.Add(Localizer[nameof(Resources.Pages.Proposals.Index.RejectProposalErrorMessage), proposalTitle], Severity.Error);
            }
        }
    }

    async Task RefuseProposalAsync(ProposalListModel.ProposalListItemModel proposal)
    {
        var proposalTitle = proposal.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Proposals.Index.RefuseProposalConfirmText), proposalTitle];

        var confirm = await Dialog.Show<ConfirmDialog>(
            Localizer[nameof(Resources.Pages.Proposals.Index.RefuseProposalConfirmTitle)],
            new DialogParameters
            {
                [nameof(ConfirmDialog.ConfirmText)] = confirmText
            }).Result;

        if (!confirm.Cancelled)
        {
            try
            {
                await Client.RefuseProposalAsync(proposal.Id);
                Snackbar.Add(Localizer[nameof(Resources.Pages.Proposals.Index.RefuseProposalSuccessMessage), proposalTitle], Severity.Success);

                await LoadProposalsAsync(query);
            }
            catch
            {
                Snackbar.Add(Localizer[nameof(Resources.Pages.Proposals.Index.RefuseProposalErrorMessage), proposalTitle], Severity.Error);
            }
        }
    }

    private async Task LoadProposalsAsync(ProposalsQueryModel query)
    {
        try
        {
            isLoading = true;
            model = await Client.GetAllProposalsAsync(query);
        }
        finally
        {
            isLoading = false;
        }
    }
}

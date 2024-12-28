using KITT.Proposals.Web.App.Clients;
using KITT.Proposals.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UI = KITT.Web.App.UI;

namespace KITT.Proposals.Web.App.Pages;

public partial class Index
{
    [Inject]
    public IProposalsClient Client { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    private ProposalsQueryModel query = new();

    private ProposalListModel model = new();

    private IQueryable<ProposalListModel.ProposalListItemModel> proposals = new List<ProposalListModel.ProposalListItemModel>().AsQueryable();

    private bool loading = false;

    private Option<int>[] sizes = [
        new() { Value = 5, Text = "5" },
        new() { Value = 10, Text = "10" },
        new() { Value = 25, Text = "25" },
        new() { Value = 50, Text = "50" }
    ];

    private Option<ProposalsQueryModel.SortDirection>[] directions = [];

    private Option<ProposalStatus?>[] statuses = [];

    protected override void OnInitialized()
    {
        directions = Enum.GetValues<ProposalsQueryModel.SortDirection>()
            .Select(d => new Option<ProposalsQueryModel.SortDirection> { Value = d, Text = Localizer[d.ToString()] })
            .ToArray();

        var validStatuses = Enum.GetValues<ProposalStatus>()
            .Select(s => new Option<ProposalStatus?> { Value = s, Text = Localizer[s.ToString()] })
            .ToArray();

        statuses = [
            new Option<ProposalStatus?> { Value = null, Text = "" },
            ..validStatuses,
        ];
    }

    private async Task SearchAsync()
    {
        await LoadProposalsAsync(query);
    }

    private async Task ClearSearchAsync()
    {
        query = new();
        await LoadProposalsAsync(query);
    }

    private async Task AcceptProposalAsync(ProposalListModel.ProposalListItemModel proposal)
    {
        var proposalTitle = proposal.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Index.AcceptProposalConfirmText), proposalTitle];

        var confirmDialog = await DialogService.ShowConfirmationAsync(
            confirmText,
            primaryText: CommonLocalizer[UI.Resources.Common.Confirm],
            secondaryText: CommonLocalizer[UI.Resources.Common.Cancel],
            title: Localizer[nameof(Resources.Pages.Index.AcceptProposalConfirmTitle)]);

        var confirmResult = await confirmDialog.Result;
        if (!confirmResult.Cancelled)
        {
            try
            {
                await Client.AcceptProposalAsync(proposal.Id);
                ToastService.ShowSuccess(
                    Localizer[nameof(Resources.Pages.Index.AcceptProposalSuccessMessage), proposalTitle]);

                await LoadProposalsAsync(query);
            }
            catch
            {
                ToastService.ShowError(
                    Localizer[nameof(Resources.Pages.Index.AcceptProposalErrorMessage), proposalTitle]);
            }
        }
    }

    private async Task RejectProposalAsync(ProposalListModel.ProposalListItemModel proposal)
    {
        var proposalTitle = proposal.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Index.RejectProposalConfirmText), proposalTitle];

        var confirmDialog = await DialogService.ShowConfirmationAsync(
            confirmText,
            primaryText: CommonLocalizer[UI.Resources.Common.Confirm],
            secondaryText: CommonLocalizer[UI.Resources.Common.Cancel],
            title: Localizer[nameof(Resources.Pages.Index.RejectProposalConfirmTitle)]);

        var confirmResult = await confirmDialog.Result;
        if (!confirmResult.Cancelled)
        {
            try
            {
                await Client.RejectProposalAsync(proposal.Id);
                ToastService.ShowSuccess(
                    Localizer[nameof(Resources.Pages.Index.RejectProposalSuccessMessage), proposalTitle]);

                await LoadProposalsAsync(query);
            }
            catch
            {
                ToastService.ShowError(
                    Localizer[nameof(Resources.Pages.Index.RejectProposalErrorMessage), proposalTitle]);
            }
        }
    }

    private async Task RefuseProposalAsync(ProposalListModel.ProposalListItemModel proposal)
    {
        var proposalTitle = proposal.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Index.RefuseProposalConfirmText), proposalTitle];

        var confirmDialog = await DialogService.ShowConfirmationAsync(
            confirmText,
            primaryText: CommonLocalizer[UI.Resources.Common.Confirm],
            secondaryText: CommonLocalizer[UI.Resources.Common.Cancel],
            title: Localizer[nameof(Resources.Pages.Index.RefuseProposalConfirmTitle)]);

        var confirmResult = await confirmDialog.Result;
        if (!confirmResult.Cancelled)
        {
            try
            {
                await Client.RefuseProposalAsync(proposal.Id);
                ToastService.ShowSuccess(
                    Localizer[nameof(Resources.Pages.Index.RefuseProposalSuccessMessage), proposalTitle]);

                await LoadProposalsAsync(query);
            }
            catch
            {
                ToastService.ShowError(
                    Localizer[nameof(Resources.Pages.Index.RefuseProposalErrorMessage), proposalTitle]);
            }
        }
    }

    private void ScheduleProposal(ProposalListModel.ProposalListItemModel proposal)
    {
        Navigation.NavigateTo($"proposals/schedule/{proposal.Id}");
    }

    private async Task LoadProposalsAsync(ProposalsQueryModel query)
    {
        loading = true;

        try
        {
            model = await Client.GetAllProposalsAsync(query);
            proposals = model.Items.AsQueryable();
        }
        finally
        {
            loading = false;
        }
    }
}
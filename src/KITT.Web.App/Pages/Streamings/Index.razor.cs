using KITT.Web.App.Clients;
using KITT.Web.App.UI.Components;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KITT.Web.App.Pages.Streamings;

public partial class Index
{
    [Inject]
    public IStreamingsClient Client { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    IDialogService Dialog { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    private StreamingsListModel model = new();

    private StreamingQueryModel query = new();

    private MudTable<StreamingsListModel.StreamingListItemModel> table;

    private async Task LoadStreamings(StreamingQueryModel query) => model = await Client.GetAllStreamingsAsync(query);

    async Task<TableData<StreamingsListModel.StreamingListItemModel>> LoadStreamingsAsync(TableState state)
    {
        query.Page = state.Page;
        query.Size = state.PageSize;

        await LoadStreamings(query);
        return new() { TotalItems = model.TotalItems, Items = model.Items };
    }

    void OpenStreamingDetail(StreamingsListModel.StreamingListItemModel streaming)
        => Navigation.NavigateTo($"streamings/{streaming.Id}");

    async Task DeleteStreaming(StreamingsListModel.StreamingListItemModel streaming)
    {
        var streamingTitle = streaming.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmText), streamingTitle];

        var confirm = await Dialog.Show<ConfirmDialog>(
            Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmTitle), streamingTitle],
            new DialogParameters
            {
                [nameof(ConfirmDialog.ConfirmText)] = confirmText
            }).Result;

        if (!confirm.Canceled)
        {
            try
            {
                await Client.DeleteStreamingAsync(streaming.Id);
                Snackbar.Add(Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingSuccessMessage), streamingTitle], Severity.Success);

                await table.ReloadServerData();
            }
            catch
            {
                Snackbar.Add(Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingErrorMessage), streamingTitle], Severity.Error);
            }
            finally
            {
                StateHasChanged();
            }
        }
    }

    void Search() => table.ReloadServerData();

    void ClearSearch()
    {
        query = new();
        table.ReloadServerData();
    }
}

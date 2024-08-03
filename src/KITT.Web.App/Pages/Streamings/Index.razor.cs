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

    private int numberOfPages = 0;

    private bool loading = false;

    private readonly int[] sizes = new[] { 5, 10, 25, 50 };

    private async Task LoadStreamingsAsync(StreamingQueryModel query)
    {
        try
        {
            loading = true;

            model = await Client.GetAllStreamingsAsync(query);
            numberOfPages = (int)Math.Ceiling(model.TotalItems / (decimal)query.Size);
        }
        finally
        {
            loading = false;
        }
    }

    private async Task OnPageChangedAsync(int pageIndex)
    {
        query.Page = pageIndex;
        await LoadStreamingsAsync(query);
    }

    void OpenStreamingDetail(StreamingsListModel.StreamingListItemModel streaming)
        => Navigation.NavigateTo($"streamings/{streaming.Id}");

    async Task DeleteStreaming(StreamingsListModel.StreamingListItemModel streaming)
    {
        //var streamingTitle = streaming.Title;
        //string confirmText = Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmText), streamingTitle];

        //var confirm = await Dialog.Show<ConfirmDialog>(
        //    Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmTitle), streamingTitle],
        //    new DialogParameters
        //    {
        //        [nameof(ConfirmDialog.ConfirmText)] = confirmText
        //    }).Result;

        //if (!confirm.Canceled)
        //{
        //    try
        //    {
        //        await Client.DeleteStreamingAsync(streaming.Id);
        //        Snackbar.Add(Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingSuccessMessage), streamingTitle], Severity.Success);

        //        await LoadStreamingsAsync(query);
        //    }
        //    catch
        //    {
        //        Snackbar.Add(Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingErrorMessage), streamingTitle], Severity.Error);
        //    }
        //    finally
        //    {
        //        StateHasChanged();
        //    }
        //}
    }

    async Task SearchAsync() => await LoadStreamingsAsync(query);

    async Task ClearSearchAsync()
    {
        query = new();
        await LoadStreamingsAsync(query);
    }
}

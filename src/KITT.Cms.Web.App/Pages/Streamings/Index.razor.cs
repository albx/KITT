using KITT.Cms.Web.App.Client;
using KITT.Cms.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Streamings;

public partial class Index
{
    [Inject]
    public IStreamingsClient Client { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    private StreamingsListModel model = new();

    private IQueryable<StreamingsListModel.StreamingListItemModel> streamings = new List<StreamingsListModel.StreamingListItemModel>().AsQueryable();

    private StreamingQueryModel query = new();

    private int numberOfPages = 0;

    private bool loading = false;

    private PaginationState paginationState = new();

    private Option<StreamingQueryModel.SortDirection>[] directions = [];

    private readonly Option<int>[] sizes = [
        new() { Value = 5, Text = "5" },
        new() { Value = 10, Text = "10" },
        new() { Value = 25, Text = "25" },
        new() { Value = 50, Text = "50" }
    ];

    protected override void OnInitialized()
    {
        directions = Enum.GetValues<StreamingQueryModel.SortDirection>()
            .Select(v => new Option<StreamingQueryModel.SortDirection>() { Value = v, Text = Localizer[v.ToString()] })
            .ToArray();

        SetPaginationState();
    }

    private async Task LoadStreamingsAsync(StreamingQueryModel query)
    {
        loading = true;

        try
        {
            model = await Client.GetAllStreamingsAsync(query);
            numberOfPages = (int)Math.Ceiling(model.TotalItems / (decimal)query.Size);

            streamings = model.Items.AsQueryable();
            await paginationState.SetTotalItemCountAsync(model.TotalItems);
        }
        finally
        {
            loading = false;
        }
    }

    private async Task OnPageChangedAsync(int pageIndex)
    {
        query.Page = pageIndex + 1;
        await LoadStreamingsAsync(query);
    }

    private void SetPaginationState()
        => paginationState.ItemsPerPage = query.Size;

    private void OpenStreamingDetailPage(StreamingsListModel.StreamingListItemModel streaming)
        => NavigationManager.NavigateTo($"streamings/{streaming.Id}");

    private async Task DeleteStreaming(StreamingsListModel.StreamingListItemModel streaming)
    {
        var streamingTitle = streaming.Title;
        string confirmText = Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmText), streamingTitle];

        var confirm = await DialogService.ShowConfirmationAsync(
            confirmText,
            primaryText: CommonLocalizer[nameof(KITT.Web.App.UI.Resources.Common.Confirm)],
            secondaryText: CommonLocalizer[(nameof(KITT.Web.App.UI.Resources.Common.Cancel))],
            title: Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmTitle), streamingTitle]);

        var result = await confirm.Result;
        if (!result.Cancelled)
        {
            try
            {
                await Client.DeleteStreamingAsync(streaming.Id);
                ToastService.ShowSuccess(Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingSuccessMessage), streamingTitle]);

                await LoadStreamingsAsync(query);
            }
            catch
            {
                ToastService.ShowError(Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingErrorMessage), streamingTitle]);
            }
        }
    }

    private async Task SearchAsync() => await LoadStreamingsAsync(query);

    private async Task ClearSearchAsync()
    {
        query = new();
        SetPaginationState();

        await LoadStreamingsAsync(query);
    }
}


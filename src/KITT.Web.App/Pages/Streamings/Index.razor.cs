using KITT.Web.App.Clients;
using KITT.Web.App.Shared;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;

namespace KITT.Web.App.Pages.Streamings;

public partial class Index
{
    [Inject]
    public IStreamingsClient Client { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    [Inject]
    IDialogService Dialog { get; set; }

    [Inject]
    ISnackbar Snackbar { get; set; }

    private StreamingsListModel model = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            await LoadStreamings();
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    private async Task LoadStreamings() => model = await Client.GetAllStreamingsAsync();

    void OpenStreamingDetail(StreamingsListModel.StreamingListItemModel streaming) 
        => Navigation.NavigateTo($"streamings/{streaming.Id}");

    async Task DeleteStreaming(StreamingsListModel.StreamingListItemModel streaming)
    {
        var streamingTitle = streaming.Title;

        var confirm = await Dialog.Show<ConfirmDialog>(
            Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmTitle), streamingTitle], 
            new DialogParameters 
            {
                [nameof(ConfirmDialog.ConfirmText)] = Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingConfirmText), streamingTitle]
            }).Result;

        if (!confirm.Cancelled)
        {
            try
            {
                await Client.DeleteStreamingAsync(streaming.Id);
                await LoadStreamings();

                Snackbar.Add(Localizer[nameof(Resources.Pages.Streamings.Index.DeleteStreamingSuccessMessage), streamingTitle], Severity.Success);
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
}

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
        var confirm = await Dialog.Show<ConfirmDialog>(
            $"Deleting {streaming.Title}", 
            new DialogParameters 
            {
                [nameof(ConfirmDialog.ConfirmText)] = $"You are going to delete streaming {streaming.Title}. Are you sure?"
            }).Result;

        if (!confirm.Cancelled)
        {
            try
            {
                await Client.DeleteStreamingAsync(streaming.Id);
                await LoadStreamings();
            }
            catch (Exception ex)
            {
                //TODO
            }
            finally
            {
                StateHasChanged();
            }
        }
    }
}

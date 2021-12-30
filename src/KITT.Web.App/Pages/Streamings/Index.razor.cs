using KITT.Web.App.Clients;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Localization;

namespace KITT.Web.App.Pages.Streamings;

public partial class Index
{
    [Inject]
    public IStreamingsClient Client { get; set; }

    [Inject]
    internal IStringLocalizer<Resources.Common> CommonLocalizer { get; set; }

    [Inject]
    internal IStringLocalizer<Resources.Pages.Streamings.Index> Localizer { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    private StreamingsListModel model = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            model = await Client.GetAllStreamingsAsync();
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    void OpenStreamingDetail(StreamingsListModel.StreamingListItemModel streaming) 
        => Navigation.NavigateTo($"streamings/{streaming.Id}");
}

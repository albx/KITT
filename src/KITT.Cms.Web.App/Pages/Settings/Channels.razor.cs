using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.App.Components;
using KITT.Cms.Web.Models.Settings;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Settings;

public partial class Channels(IDialogService dialogService, IConnectedChannelsClient client)
{
    private ChannelModel[] channels = [];

    private IDialogReference? dialogReference;

    protected override async Task OnInitializedAsync()
    {
        channels = await client.GetConnectedChannelsAsync();
    }

    private async Task CreateNewChannelAsync(ChannelModel channel)
    {
        await client.CreateNewConnectedChannelAsync(channel);
    }

    private async Task OpenAddNewChannelPanelAsync()
    {
        var model = new ChannelModel();

        dialogReference = await dialogService.ShowPanelAsync<ChannelFormPanel>(
            model,
            new DialogParameters<ChannelModel>()
            {
                Content = model,
                Alignment = HorizontalAlignment.Right,
                Title = "Add new channel",
                Width = "50em",
            });

        await dialogReference.Result;
    }
}

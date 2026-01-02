using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.App.Components;
using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Settings;

public partial class Channels(IDialogService dialogService, IConnectedChannelsClient client)
{
    private ChannelModel[] channels = [];

    private IDialogReference? dialogReference;

    protected override async Task OnInitializedAsync()
    {
        await LoadConnectedChannelsAsync();
    }

    private async Task CreateNewChannelAsync(ChannelModel channel)
    {
        await client.CreateNewConnectedChannelAsync(channel);
    }

    private async Task LoadConnectedChannelsAsync() => channels = await client.GetConnectedChannelsAsync();

    private async Task OpenAddNewChannelPanelAsync()
    {
        var panelModel = new ChannelFormPanel.ViewModel
        {
            Model = new(),
            OnChannelSave = EventCallback.Factory.Create<ChannelModel>(this, CreateNewChannelAsync)
        };

        dialogReference = await dialogService.ShowPanelAsync<ChannelFormPanel>(
            panelModel,
            new DialogParameters<ChannelFormPanel.ViewModel>()
            {
                Content = panelModel,
                Alignment = HorizontalAlignment.Right,
                Title = "Add new channel",
                Width = "50em",
            });

        var result = await dialogReference.Result;
        if (!result.Cancelled)
        {
            await LoadConnectedChannelsAsync();
        }
    }
}

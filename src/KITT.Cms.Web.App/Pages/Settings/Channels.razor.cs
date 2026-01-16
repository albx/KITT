using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.App.Components;
using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Settings;

public partial class Channels(
    IDialogService dialogService, 
    IToastService toastService,
    IConnectedChannelsClient client)
{
    private ChannelModel[] channels = [];

    private bool loading = false;

    private IDialogReference? dialogReference;

    protected override async Task OnInitializedAsync()
    {
        await LoadConnectedChannelsAsync();
    }

    private async Task CreateNewChannelAsync(ChannelModel channel)
    {
        await client.CreateNewConnectedChannelAsync(channel);
    }

    private async Task LoadConnectedChannelsAsync()
    {
        loading = true;

        try
        {
            channels = await client.GetConnectedChannelsAsync();
        }
        finally
        {
            loading = false;
        }
    }

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

    private async Task OpenEditChannelPanelAsync(ChannelModel channel)
    {
        var channelCopy = new ChannelModel
        {
            Id = channel.Id,
            Name = channel.Name,
            Type = channel.Type,
            Url = channel.Url,
        };

        var panelModel = new ChannelFormPanel.ViewModel
        {
            Model = channelCopy,
            OnChannelSave = EventCallback.Factory.Create<ChannelModel>(this, UpdateChannelAsync)
        };

        dialogReference = await dialogService.ShowPanelAsync<ChannelFormPanel>(
            panelModel,
            new DialogParameters<ChannelFormPanel.ViewModel>()
            {
                Content = panelModel,
                Alignment = HorizontalAlignment.Right,
                Title = $"Edit channel {channel.Name}",
                Width = "50em",
            });

        var result = await dialogReference.Result;
        if (!result.Cancelled)
        {
            await LoadConnectedChannelsAsync();
        }
    }

    private async Task UpdateChannelAsync(ChannelModel channel)
    {
        await client.UpdateConnectedChannelAsync(channel);
    }

    private async Task DeleteChannelAsync(ChannelModel channel)
    {
        var channelName = channel.Name;

        var confirm = await dialogService.ShowConfirmationAsync(
            "You are going to delete this channel. Are you sure?",
            title: $"Delete channel {channelName}");

        var result = await confirm.Result;

        if (!result.Cancelled)
        {
            try
            {
                await client.DeleteConnectedChannelAsync(channel);
                toastService.ShowSuccess($"{channelName} deleted successfully!");

                await LoadConnectedChannelsAsync();
            }
            catch 
            {
                toastService.ShowError($"There was an error deleting channel {channelName}");
            }
        }
    }
}

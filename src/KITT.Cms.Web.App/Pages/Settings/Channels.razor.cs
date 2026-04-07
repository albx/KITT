using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.App.Components;
using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Settings;

public partial class Channels(
    IDialogService dialogService, 
    IToastService toastService,
    IConnectedChannelsClient client,
    IStringLocalizer<Resources.Pages.Settings.Channels> localizer)
{
    private ChannelModel[] channels = [];

    private bool loading = false;

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

        var dialog = await dialogService.ShowPanelAsync<ChannelFormPanel>(
            panelModel,
            new DialogParameters<ChannelFormPanel.ViewModel>()
            {
                Content = panelModel,
                Alignment = HorizontalAlignment.Right,
                Title = localizer[nameof(Resources.Pages.Settings.Channels.AddNewChannelPanelTitle)],
                Width = "50em",
            });

        var result = await dialog.Result;
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

        var dialog = await dialogService.ShowPanelAsync<ChannelFormPanel>(
            panelModel,
            new DialogParameters<ChannelFormPanel.ViewModel>()
            {
                Content = panelModel,
                Alignment = HorizontalAlignment.Right,
                Title = localizer[nameof(Resources.Pages.Settings.Channels.EditChannelPanelTitle), channel.Name],
                Width = "50em",
            });

        var result = await dialog.Result;
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
            localizer[nameof(Resources.Pages.Settings.Channels.DeleteChannelConfirmMessage)],
            title: localizer[nameof(Resources.Pages.Settings.Channels.DeleteChannelConfirmTitle), channelName],
            primaryText: localizer[nameof(Resources.Pages.Settings.Channels.DeleteChannelConfirmPrimaryButtonText)]);

        var result = await confirm.Result;

        if (!result.Cancelled)
        {
            try
            {
                await client.DeleteConnectedChannelAsync(channel);
                toastService.ShowSuccess(localizer[nameof(Resources.Pages.Settings.Channels.DeletedChannelSuccessMessage), channelName]);

                await LoadConnectedChannelsAsync();
            }
            catch 
            {
                toastService.ShowError(localizer[nameof(Resources.Pages.Settings.Channels.DeletedChannelErrorMessage), channelName]);
            }
        }
    }
}

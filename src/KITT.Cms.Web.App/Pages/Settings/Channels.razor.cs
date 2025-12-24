using KITT.Cms.Web.App.Components;
using KITT.Cms.Web.Models.Settings;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Settings;

public partial class Channels(IDialogService dialogService)
{
    private List<ChannelModel> channels = [];

    private IDialogReference? dialogReference;

    protected override void OnInitialized()
    {
        channels = [
            new ChannelModel { Id = Guid.NewGuid(), Type = ChannelType.Twitch, Name = "Twitch - albx87", Url = "https://www.twitch.tv/albx87" },
            new ChannelModel { Id = Guid.NewGuid(), Type = ChannelType.YouTube, Name = "YouTube - albx87", Url = "https://www.youtube.com/@albx87" },
        ];
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
                Width = "50em"
            });

        await dialogReference.Result;
    }
}

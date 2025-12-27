using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ChannelFormPanel : IDialogContentComponent<ChannelModel>
{
    [Parameter]
    public ChannelModel Content { get; set; } = new();

    [CascadingParameter]
    public DialogReference Dialog { get; set; } = default!;

    private readonly ChannelType[] channelTypes = Enum.GetValues<ChannelType>();

    private string UrlPlaceholder => Content.Type switch
    {
        ChannelType.Twitch => "https://www.twitch.tv/channelName",
        ChannelType.YouTube => "https://www.youtube.com/@channelName",
        _ => string.Empty
    };

    private async Task CloseAsync() => await Dialog.CloseAsync(DialogResult.Cancel());
}

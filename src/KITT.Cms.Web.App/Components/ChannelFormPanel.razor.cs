using KITT.Cms.Settings;
using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ChannelFormPanel : IDialogContentComponent<ChannelModel>
{
    [Parameter]
    public ChannelModel Content { get; set; } = new();

    [Parameter]
    public EventCallback<ChannelModel> OnSaveChannel { get; set; }

    [CascadingParameter]
    public DialogReference Dialog { get; set; } = default!;

    private EditContext context = default!;

    private bool saving = false;

    private readonly ChannelType[] channelTypes = Enum.GetValues<ChannelType>();

    private string UrlPlaceholder => Content.Type switch
    {
        ChannelType.Twitch => "https://www.twitch.tv/channelName",
        ChannelType.YouTube => "https://www.youtube.com/@channelName",
        _ => string.Empty
    };

    protected override void OnInitialized()
    {
        context = new(Content);
    }

    private async Task SaveAsync()
    {
        saving = true;

        try
        {
            if (!context.Validate())
            {
                return;
            }

            await OnSaveChannel.InvokeAsync(Content);
        }
        finally
        {
            saving = false;
        }
    }

    private async Task CloseAsync() => await Dialog.CloseAsync(DialogResult.Cancel());
}

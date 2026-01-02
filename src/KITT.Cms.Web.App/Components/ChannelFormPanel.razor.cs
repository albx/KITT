using KITT.Cms.Settings.Models;
using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ChannelFormPanel(IToastService toastService) : IDialogContentComponent<ChannelFormPanel.ViewModel>
{
    [Parameter]
    public ViewModel Content { get; set; } = new();

    [CascadingParameter]
    public DialogReference Dialog { get; set; } = default!;

    private EditContext context = default!;

    private bool saving = false;

    private readonly ChannelType[] channelTypes = Enum.GetValues<ChannelType>();

    private string UrlPlaceholder => Content.Model.Type switch
    {
        ChannelType.Twitch => "https://www.twitch.tv/channelName",
        ChannelType.YouTube => "https://www.youtube.com/@channelName",
        _ => string.Empty
    };

    protected override void OnInitialized()
    {
        context = new(Content.Model);
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

            await Content.OnChannelSave.InvokeAsync(Content.Model);

            toastService.ShowSuccess("Channel saved correctly");
            await Dialog.CloseAsync(DialogResult.Ok(true));
        }
        catch
        {
            toastService.ShowError("Error saving the channel");
        }
        finally
        {
            saving = false;
        }
    }

    private async Task CloseAsync() => await Dialog.CloseAsync(DialogResult.Cancel());

    public class ViewModel
    {
        public ChannelModel Model { get; set; } = new();

        public EventCallback<ChannelModel> OnChannelSave { get; set; }
    }
}

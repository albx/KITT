using KITT.Cms.Settings.Models;
using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ChannelFormPanel(
    INotificationService notificationService,
    IStringLocalizer<Resources.Components.ChannelFormPanel> localizer) : FluentDialogInstance
{
    [Parameter]
    public ViewModel Content { get; set; } = new();

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

    protected override void OnInitializeDialog(DialogOptionsHeader header, DialogOptionsFooter footer)
    {
        footer.PrimaryAction.Label = localizer[nameof(Resources.Components.ChannelFormPanel.SaveButtonText)];
        footer.SecondaryAction.Label = localizer[nameof(Resources.Components.ChannelFormPanel.CloseButtonText)];
    }

    protected override async Task OnActionClickedAsync(bool primary)
    {
        if (primary)
        {
            await SaveAsync();
        }
        else
        {
            await CloseAsync();
        }
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

            await notificationService.ShowSuccessToastAsync(localizer[nameof(Resources.Components.ChannelFormPanel.ChannelSavedSuccessMessage)]);

            await DialogInstance.CloseAsync(true);
        }
        catch
        {
            await notificationService.ShowErrorToastAsync(localizer[nameof(Resources.Components.ChannelFormPanel.ChannelSavedErrorMessage)]);
        }
        finally
        {
            saving = false;
        }
    }

    private async Task CloseAsync() => await DialogInstance.CloseAsync();

    public class ViewModel
    {
        public ChannelModel Model { get; set; } = new();

        public EventCallback<ChannelModel> OnChannelSave { get; set; }
    }
}

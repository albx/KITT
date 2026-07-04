using KITT.Web.Models.Messages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Web.App.Client.Components;

public partial class MessageEditorDialog
{
    [Inject]
    public INotificationService NotificationService { get; set; } = default!;

    //[Inject]
    //public IMessagesClient Client { get; set; } = default!;

    private SendMessageModel model = new();

    private EditContext context = default!;

    private bool sending = false;

    protected override void OnInitialized()
    {
        context = new EditContext(model);
    }

    protected override void OnInitializeDialog(DialogOptionsHeader header, DialogOptionsFooter footer)
    {
        footer.PrimaryAction.Label = LocalizerFor[nameof(Resources.Components.MessageEditorDialog.SendButtonText)];
        footer.SecondaryAction.Label = LocalizerFor[nameof(Resources.Components.MessageEditorDialog.CloseButtonText)];
    }

    protected override async Task OnActionClickedAsync(bool primary)
    {
        if (primary)
        {
            await SendMessageAsync();
        }
        else
        {
            await CloseAsync();
        }
    }

    private async Task CloseAsync() => await DialogInstance.CloseAsync();

    private async Task SendMessageAsync()
    {
        sending = true;

        try
        {
            if (!context.Validate())
            {
                return;
            }

            //await Client.SendMessageAsync(model);

            await NotificationService.ShowSuccessToastAsync(LocalizerFor[nameof(Resources.Components.MessageEditorDialog.MessageSentSuccessMessage)]);
            await DialogInstance.CloseAsync();
        }
        finally
        {
            sending = false;
        }
    }
}

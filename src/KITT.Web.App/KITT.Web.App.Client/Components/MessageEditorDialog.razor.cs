using KITT.Web.Models.Messages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Web.App.Client.Components;

public partial class MessageEditorDialog
{
    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    //[Inject]
    //public IMessagesClient Client { get; set; } = default!;

    private SendMessageModel model = new();

    private EditContext context = default!;

    private bool sending = false;

    protected override void OnInitialized()
    {
        context = new EditContext(model);
    }

    private async Task CloseAsync() => await Dialog.CloseAsync();

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

            ToastService.ShowSuccess(Localizer[nameof(Resources.Components.MessageEditorDialog.MessageSentSuccessMessage)]);
            await Dialog.CloseAsync();
        }
        finally
        {
            sending = false;
        }
    }
}

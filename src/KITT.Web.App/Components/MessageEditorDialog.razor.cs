using KITT.Web.App.Clients;
using KITT.Web.Models.Messages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace KITT.Web.App.Components;

public partial class MessageEditorDialog
{
    [CascadingParameter]
    public MudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    public IMessagesClient Client { get; set; } = default!;

    private SendMessageModel model = new();

    private EditContext? context;

    private bool sending = false;

    protected override void OnInitialized()
    {
        context = new EditContext(model);
    }

    void Close()
    {
        Dialog.Close(DialogResult.Cancel());
    }

    async Task SendMessageAsync()
    {
        sending = true;

        try
        {
            await Client.SendMessageAsync(model);

            Snackbar.Add(Localizer[nameof(Resources.Components.MessageEditorDialog.MessageSentSuccessMessage)], Severity.Success);
            Dialog.Close(DialogResult.Ok(true));
        }
        finally
        {
            sending = false;
        }
    }
}

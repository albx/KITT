using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models.Contents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class PublishContentDialog(
    IContentClient client,
    IToastService toastService) : IDialogContentComponent<PublishContentDialog.ContentModel>
{
    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    [Parameter]
    public ContentModel Content { get; set; } = default!;

    private PublishContentModel model = new();

    private EditContext context = default!;

    private bool publishing;

    protected override void OnInitialized()
    {
        context = new(model);
    }

    private async Task CloseAsync() => await Dialog.CloseAsync();

    private async Task PublishContentAsync()
    {
        publishing = true;

        if (!context.Validate())
        {
            return;
        }

        try
        {
            await client.PublishContentAsync(Content.Id, model);
            toastService.ShowSuccess("Content published successfully!");

            await Dialog.CloseAsync(model);
        }
        finally
        {
            publishing = false;
        }
    }

    public record ContentModel(Guid Id);
}

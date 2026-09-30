using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.Contents;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ContentStatusControl(IDialogService dialogService)
{
    [Parameter]
    public ViewModel Content { get; set; } = default!;

    [Parameter]
    public EventCallback<ContentPublishedModel> OnContentPublished { get; set; }

    private async Task OpenPublishContentDialogAsync()
    {
        var dialog = await dialogService.ShowDialogAsync<PublishContentDialog>(
            new PublishContentDialog.ContentModel(Content.Id),
            new()
            {
                Title = Localizer[nameof(Resources.Components.ContentStatusControl.PublishDialogTitle)]
            });

        var result = await dialog.Result;
        if (result.Cancelled || result.Data is not PublishContentModel { PublicationDate: not null } publishedModel)
        {
            return;
        }

        await OnContentPublished.InvokeAsync(new(publishedModel.PublicationDate!.Value));
    }

    public record ViewModel(
        Guid Id,
        ContentStatus Status);

    public record ContentPublishedModel(DateTime PublicationDate);
}

using KITT.Cms.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ContentStatusControl(IDialogService dialogService)
{
    [Parameter]
    public ViewModel Content { get; set; } = default!;

    private async Task OpenPublishContentDialogAsync()
    {
        await dialogService.ShowDialogAsync<PublishContentDialog>(new()
        {
            Title = "Publish Content"
        });
    }

    public record ViewModel(
        Guid Id,
        ContentStatus Status);
}

using KITT.Cms.Web.Models;
using Microsoft.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ContentStatusControl
{
    [Parameter]
    public ViewModel Content { get; set; } = default!;

    private string contentStatusLabel = string.Empty;

    protected override void OnParametersSet()
    {
        contentStatusLabel = Content.Status switch
        {
            ContentStatus.Draft => "Publish",
            ContentStatus.Published => "Remove from publish",
            _ => string.Empty
        };
    }

    public record ViewModel(
        Guid Id,
        ContentStatus Status);
}

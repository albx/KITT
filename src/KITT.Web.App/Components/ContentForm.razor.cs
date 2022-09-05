using Microsoft.AspNetCore.Components;

namespace KITT.Web.App.Components
{
    public partial class ContentForm<TContent>
    {
        [Parameter]
        public TContent Model { get; set; } = default!;

        [Parameter]
        public string ContentTabTitle { get; set; } = string.Empty;

        [Parameter]
        public bool ReadOnly { get; set; } = false;

        [Parameter]
        public RenderFragment<TContent> ContentInfo { get; set; } = default!;

        [Parameter]
        public EventCallback<TContent> OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        async Task SubmitAsync() => await OnSave.InvokeAsync(Model);

        async Task CancelAsync() => await OnCancel.InvokeAsync();
    }
}

using KITT.Web.App.UI.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

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

        [Inject]
        public IDialogService Dialog { get; set; } = default!;

        private EditContext? editContext;

        private bool saving = false;

        protected override void OnParametersSet()
        {
            editContext = new EditContext(Model);
        }

        async Task SubmitAsync()
        {
            try
            {
                saving = true;
                await OnSave.InvokeAsync(Model);
            }
            finally
            {
                saving = false;
            }
        }

        async Task CancelAsync() => await OnCancel.InvokeAsync();

        async Task HandleNavigationAsync(LocationChangingContext context)
        {
            if ((editContext?.IsModified() ?? false) && !saving)
            {
                string confirmText = Localizer[nameof(UI.Resources.Common.UnsavedChangesDialogContent)];

                var confirm = await Dialog.Show<ConfirmDialog>(
                    Localizer[nameof(UI.Resources.Common.UnsavedChangesDialogTitle)],
                    new DialogParameters
                    {
                        [nameof(ConfirmDialog.ConfirmText)] = confirmText
                    }).Result;

                if (confirm.Canceled)
                {
                    context.PreventNavigation();
                }
            }
        }
    }
}

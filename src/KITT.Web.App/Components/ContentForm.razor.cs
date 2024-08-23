using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Web.App.Components;

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
    public IDialogService DialogService { get; set; } = default!;

    private EditContext editContext = default!;

    private bool saving = false;

    protected override void OnParametersSet()
    {
        editContext = new EditContext(Model);
    }

    async Task SubmitAsync()
    {
        saving = true;

        try
        {
            await OnSave.InvokeAsync(Model);
        }
        finally
        {
            saving = false;
        }
    }

    private async Task CancelAsync() => await OnCancel.InvokeAsync();

    private async Task HandleNavigationAsync(LocationChangingContext context)
    {
        if ((editContext?.IsModified() ?? false) && !saving)
        {
            string confirmText = Localizer[nameof(UI.Resources.Common.UnsavedChangesDialogContent)];

            var confirmDialog = await DialogService.ShowConfirmationAsync(
                confirmText,
                primaryText: Localizer[nameof(UI.Resources.Common.Confirm)],
                secondaryText: Localizer[nameof(UI.Resources.Common.Cancel)],
                title: Localizer[nameof(UI.Resources.Common.UnsavedChangesDialogTitle)]);

            var confirmResult = await confirmDialog.Result;

            //var confirm = await Dialog.Show<ConfirmDialog>(
            //    Localizer[nameof(UI.Resources.Common.UnsavedChangesDialogTitle)],
            //    new DialogParameters
            //    {
            //        [nameof(ConfirmDialog.ConfirmText)] = confirmText
            //    }).Result;

            if (confirmResult.Cancelled)
            {
                context.PreventNavigation();
            }
        }
    }
}

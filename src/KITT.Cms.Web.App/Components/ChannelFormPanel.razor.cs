using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Components;

public partial class ChannelFormPanel : IDialogContentComponent<ChannelModel>
{
    [Parameter]
    public ChannelModel Content { get; set; } = new();

    [CascadingParameter]
    public DialogReference Dialog { get; set; } = default!;

    private readonly Option<ChannelType>[] channelTypes = Enum.GetValues<ChannelType>().Select(t => new Option<ChannelType>{ Value = t, Text = t.ToString() }).ToArray();

    private string urlPrefix = string.Empty;

    private Option<ChannelType> SelectedChannelType
    {
        get => new() { Value = Content.Type, Text = Content.Type.ToString() };
        set => Content.Type = value.Value;
    }

    private async Task CloseAsync() => await Dialog.CloseAsync(DialogResult.Cancel());
}

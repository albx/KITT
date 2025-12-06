using KITT.Cms.Web.Models.Settings;

namespace KITT.Cms.Web.App.Pages.Settings;

public partial class Channels
{
    private List<ChannelModel> channels = [];

    protected override void OnInitialized()
    {
        channels = [
            new ChannelModel { Id = Guid.NewGuid(), Name = "Twitch - albx87", Url = "https://www.twitch.tv/albx87" },
            new ChannelModel { Id = Guid.NewGuid(), Name = "YouTube - albx87", Url = "https://www.youtube.com/@albx87" },
        ];
    }
}

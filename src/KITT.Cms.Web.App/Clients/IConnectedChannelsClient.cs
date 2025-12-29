using KITT.Cms.Web.Models.Settings;

namespace KITT.Cms.Web.App.Clients;

public interface IConnectedChannelsClient
{
    Task<ChannelModel[]> GetConnectedChannelsAsync();

    Task<ChannelModel> CreateNewConnectedChannelAsync(ChannelModel model);

    Task UpdateConnectedChannelAsync(ChannelModel model);

    Task DeleteConnectedChannelAsync(ChannelModel model);
}

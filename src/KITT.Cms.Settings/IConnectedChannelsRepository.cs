using KITT.Cms.Settings.Models;

namespace KITT.Cms.Settings;

public interface IConnectedChannelsRepository
{
    Task<ConnectedChannel[]> GetConnectedChannelsAsync(string userId);

    Task<ConnectedChannel?> GetConnectedChannelAsync(Guid channelId, string userId);

    Task SaveAsync(ConnectedChannel channel);

    Task DeleteAsync(Guid channelId, string userId);
}

namespace KITT.Cms.Settings.Stores;

public interface IConnectedChannelStore
{
    Task<ConnectedChannel[]> GetConnectedChannelsAsync(string userId);

    Task<ConnectedChannel?> GetConnectedChannelAsync(Guid channelId, string userId);

    Task SaveAsync(ConnectedChannel channel);

    Task DeleteAsync(Guid channelId, string userId);
}

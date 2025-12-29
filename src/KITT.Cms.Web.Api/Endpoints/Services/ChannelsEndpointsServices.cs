using KITT.Cms.Settings;
using KITT.Cms.Settings.Stores;
using KITT.Cms.Web.Models.Settings;

namespace KITT.Cms.Web.Api.Endpoints.Services;

public class ChannelsEndpointsServices(IConnectedChannelStore connectedChannelStore)
{
    public async Task<ChannelModel[]> GetChannelsAsync(string userId)
    {
        var channels = await connectedChannelStore.GetConnectedChannelsAsync(userId);
        return channels.Select(c => new ChannelModel
        {
            Id = Guid.Parse(c.RowKey),
            Name = c.Name,
            Url = c.Url,
            Type = c.Type
        }).ToArray();
    }

    public async Task<ChannelModel?> GetChannelAsync(Guid channelId, string userId)
    {
        var channel = await connectedChannelStore.GetConnectedChannelAsync(channelId, userId);
        if (channel is null)
        {
            return null;
        }

        return new ChannelModel
        {
            Id = Guid.Parse(channel.RowKey),
            Name = channel.Name,
            Url = channel.Url,
            Type = channel.Type
        };
    }

    public async Task<ChannelModel> CreateChannelAsync(ChannelModel model, string userId)
    {
        model.Id = Guid.NewGuid();

        var channelEntity = MapToConnectedChannel(model, userId);
        await connectedChannelStore.SaveAsync(channelEntity);

        return model;
    }

    public async Task DeleteChannelAsync(Guid channelId, string userId)
    {
        await connectedChannelStore.DeleteAsync(channelId, userId);
    }

    public async Task UpdateChannelAsync(Guid channelId, ChannelModel model, string userId)
    {
        var connectedChannel = await connectedChannelStore.GetConnectedChannelAsync(channelId, userId);
        if (connectedChannel is null)
        {
            throw new ArgumentOutOfRangeException(nameof(channelId));
        }

        connectedChannel.Name = model.Name;
        connectedChannel.Url = model.Url;
        connectedChannel.Type = model.Type;

        await connectedChannelStore.SaveAsync(connectedChannel);
    }

    private static ConnectedChannel MapToConnectedChannel(ChannelModel model, string userId)
    {
        return new()
        {
            RowKey = model.Id.ToString(),
            PartitionKey = userId,
            Name = model.Name,
            Url = model.Url,
            Type = model.Type
        };
    }
}

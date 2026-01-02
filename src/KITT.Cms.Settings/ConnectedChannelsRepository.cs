using Azure.Data.Tables;
using KITT.Cms.Settings.Models;

namespace KITT.Cms.Settings;

public class ConnectedChannelsRepository : IConnectedChannelsRepository
{
    private readonly TableClient _tableClient;

    internal const string ConnectedChannelsTableName = "SettingsChannels";

    public ConnectedChannelsRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = CreateTableIfNotExist(tableServiceClient) ?? throw new ArgumentNullException("tableClient");
    }

    private TableClient? CreateTableIfNotExist(TableServiceClient tableServiceClient)
    {
        var tableClient = tableServiceClient.GetTableClient(ConnectedChannelsTableName);
        tableClient.CreateIfNotExists();

        return tableClient;
    }

    public async Task<ConnectedChannel[]> GetConnectedChannelsAsync(string userId)
    {
        var channelsQuery = _tableClient.QueryAsync<ConnectedChannel>(c => c.PartitionKey == userId);
        var channels = new List<ConnectedChannel>();

        await foreach (var channel in channelsQuery)
        {
            channels.Add(channel);
        }

        return channels.OrderBy(c => c.Name).ToArray();
    }

    public Task<ConnectedChannel?> GetConnectedChannelAsync(Guid channelId, string userId)
    {
        var channels = _tableClient.Query<ConnectedChannel>(c => c.PartitionKey == userId && c.RowKey == channelId.ToString());
        return Task.FromResult(channels.SingleOrDefault());
    }

    public async Task SaveAsync(ConnectedChannel channel)
    {
        await _tableClient.UpsertEntityAsync(channel, TableUpdateMode.Replace);
    }

    public async Task DeleteAsync(Guid channelId, string userId)
    {
        await _tableClient.DeleteEntityAsync(userId, channelId.ToString());
    }
}

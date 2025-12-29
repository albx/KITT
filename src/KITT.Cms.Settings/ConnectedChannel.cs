using Azure;
using Azure.Data.Tables;

namespace KITT.Cms.Settings;

public class ConnectedChannel : ITableEntity
{
    public required string PartitionKey { get; set; } = string.Empty;

    public required string RowKey { get; set; } = string.Empty;
    
    public DateTimeOffset? Timestamp { get; set; }
    
    public ETag ETag { get; set; }

    public required string Name { get; set; } = string.Empty;

    public required string Url { get; set; } = string.Empty;

    public required ChannelType Type { get; set; }
}

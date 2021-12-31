using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping;

internal class StreamingMapper : IEntityTypeConfiguration<Streaming>
{
    public void Configure(EntityTypeBuilder<Streaming> builder)
    {
        builder.ToTable($"{Defaults.TablePrefix}_Streamings");

        builder.Property(s => s.TwitchChannel).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.TwitchChannel);

        builder.Property(s => s.HostingChannelUrl).HasMaxLength(255).IsRequired();
        builder.Property(s => s.YouTubeVideoUrl).HasMaxLength(255);
    }
}

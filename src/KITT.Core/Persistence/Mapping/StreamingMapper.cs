using KITT.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping
{
    internal class StreamingMapper : IEntityTypeConfiguration<Streaming>
    {
        public void Configure(EntityTypeBuilder<Streaming> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedNever();

            builder.Property(s => s.TwitchChannel).HasMaxLength(50).IsRequired();
            builder.HasIndex(s => s.TwitchChannel);
            
            builder.Property(s => s.Title).HasMaxLength(255).IsRequired();
            builder.HasIndex(s => s.Title);
            
            builder.Property(s => s.Slug).HasMaxLength(255).IsRequired();
            builder.HasIndex(s => s.Slug).IsUnique();

            builder.Property(s => s.HostingChannelUrl).HasMaxLength(255).IsRequired();
            builder.Property(s => s.YouTubeVideoUrl).HasMaxLength(255);
        }
    }
}

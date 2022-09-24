using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping;

internal class StreamingStatsMapper : IEntityTypeConfiguration<StreamingStats>
{
    public void Configure(EntityTypeBuilder<StreamingStats> builder)
    {
        builder.ToTable($"{Defaults.TablePrefix}_StreamingStats");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasOne(s => s.Streaming).WithMany();
    }
}

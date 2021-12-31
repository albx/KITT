using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping;

internal class SettingsMapper : IEntityTypeConfiguration<Settings>
{
    public void Configure(EntityTypeBuilder<Settings> builder)
    {
        builder.ToTable($"{Defaults.TablePrefix}_Settings");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.UserId).HasMaxLength(36).IsRequired();
        builder.HasIndex(s => s.UserId);

        builder.Property(s => s.TwitchChannel).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.TwitchChannel);
    }
}

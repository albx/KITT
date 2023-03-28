using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping;

internal class RatingMapper : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable($"{Defaults.TablePrefix}_Ratings");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Website).HasMaxLength(255).IsRequired();
        builder.Property(r => r.PageUrl).HasMaxLength(100).IsRequired();
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping;

internal class ContentMapper : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.ToTable($"{Defaults.TablePrefix}_Contents");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.UserId).HasMaxLength(36).IsRequired();
        builder.HasIndex(c => c.UserId);

        builder.Property(c => c.Title).HasMaxLength(255).IsRequired();
        builder.HasIndex(c => c.Title);

        builder.Property(c => c.Slug).HasMaxLength(255).IsRequired();
        builder.HasIndex(c => c.Slug).IsUnique();

        builder.Property(c => c.Status).HasConversion<string>();

        builder.OwnsOne(c => c.Seo);
    }
}

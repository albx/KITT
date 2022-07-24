using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping;

internal class ProposalMapper : IEntityTypeConfiguration<Proposal>
{
    public void Configure(EntityTypeBuilder<Proposal> builder)
    {
        builder.ToTable($"{Defaults.TablePrefix}_Proposals");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.AuthorNickname).HasMaxLength(50);

        builder.Property(p => p.Title)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasDefaultValue(Proposal.ProposalStatus.Moderating);
    }
}

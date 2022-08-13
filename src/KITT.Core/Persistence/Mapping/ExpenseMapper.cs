using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KITT.Core.Persistence.Mapping;

internal class ExpenseMapper : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable($"{Defaults.TablePrefix}_Expenses");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ExpenseDate).IsRequired();

        builder.Property(e => e.TotalAmount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(e => e.Description).HasMaxLength(255);
        builder.Property(e => e.PaymentInfo).HasMaxLength(100);

        builder.Property(e => e.Method).HasConversion<string>();
    }
}

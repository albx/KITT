using KITT.Core.Persistence.Mapping;

namespace KITT.Core.Persistence;

public class KittDbContext : DbContext
{
    public KittDbContext(DbContextOptions<KittDbContext> options)
        : base(options)
    {
    }

    public DbSet<Content> Contents { get; set; }

    public DbSet<Streaming> Streamings { get; set; }

    public DbSet<Settings> Settings { get; set; }

    public DbSet<Proposal> Proposals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ContentMapper());
        modelBuilder.ApplyConfiguration(new StreamingMapper());
        modelBuilder.ApplyConfiguration(new SettingsMapper());
        modelBuilder.ApplyConfiguration(new ProposalMapper());
    }
}

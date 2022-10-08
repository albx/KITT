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

    public DbSet<Expense> Expenses { get; set; }

    public DbSet<StreamingStats> StreamingStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}

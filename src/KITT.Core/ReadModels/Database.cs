namespace KITT.Core.ReadModels;

public class Database : IDatabase
{
    private readonly KittDbContext _context;

    public Database(KittDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IQueryable<Streaming> Streamings => _context.Streamings.AsNoTracking();

    public IQueryable<Settings> Settings => _context.Settings.AsNoTracking();

    public IQueryable<Proposal> Proposals => _context.Proposals.AsNoTracking();
}

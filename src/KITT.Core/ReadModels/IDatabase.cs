namespace KITT.Core.ReadModels;

public interface IDatabase
{
    IQueryable<Streaming> Streamings { get; }

    IQueryable<Settings> Settings { get; }

    IQueryable<Proposal> Proposals { get; }
}

using KITT.Core.Models;
using System.Linq;

namespace KITT.Core.ReadModels
{
    public interface IDatabase
    {
        IQueryable<Streaming> Streamings { get; }

        IQueryable<Settings> Settings { get; }
    }
}

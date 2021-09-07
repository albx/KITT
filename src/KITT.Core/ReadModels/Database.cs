using KITT.Core.Models;
using KITT.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KITT.Core.ReadModels
{
    public class Database : IDatabase
    {
        private readonly KittDbContext _context;

        public Database(KittDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<Streaming> Streamings => _context.Streamings.AsNoTracking();
    }
}

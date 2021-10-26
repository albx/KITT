using KITT.Core.Models;
using KITT.Core.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KITT.Core.Persistence
{
    public class KittDbContext : DbContext
    {
        public KittDbContext(DbContextOptions<KittDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Streaming> Streamings { get; set; }

        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new SettingsMapper());
            modelBuilder.ApplyConfiguration(new StreamingMapper());
        }
    }
}

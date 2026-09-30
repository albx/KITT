using KITT.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KITT.Core.Test.Fixtures
{
    public class KittDbContextFixture : IDisposable
    {
        private bool disposedValue;

        public KittDbContext Context { get; }

        public KittDbContextFixture()
        {
            var options = new DbContextOptionsBuilder<KittDbContext>()
                .UseInMemoryDatabase(databaseName: $"Kitt-InMemory-{Guid.NewGuid()}")
                .Options;

            Context = new KittDbContext(options);
        }

        public void PrepareData(Action<KittDbContext> onDataPreparing) => onDataPreparing.Invoke(Context);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

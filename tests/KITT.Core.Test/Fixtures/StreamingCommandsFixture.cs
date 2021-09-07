using KITT.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace KITT.Core.Test.Fixtures
{
    public class StreamingCommandsFixture : IDisposable
    {
        private bool disposedValue;

        private DbContextOptions<KittDbContext> contextOptions;

        public KittDbContext Context { get; }

        public StreamingCommandsFixture()
        {
            BuildContextOptions();
            Context = new KittDbContext(this.contextOptions);
        }

        private void BuildContextOptions()
        {
            var options = new DbContextOptionsBuilder<KittDbContext>()
                .UseInMemoryDatabase(databaseName: "Kitt-InMemory")
                .Options;

            this.contextOptions = options;
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

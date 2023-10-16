using Microsoft.EntityFrameworkCore;

namespace RocketPDF.EntityFrameworkCore
{
    public sealed class ResilientTransaction
    {
        private readonly DbContext _context;

        private ResilientTransaction(DbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public static ResilientTransaction New(DbContext context) => new(context);

        public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            // Use of an EF Core resiliency strategy when using multiple DbContexts
            // within an explicit BeginTransaction():
            // https://docs.microsoft.com/ef/core/miscellaneous/connection-resiliency
            if (_context.Database.CurrentTransaction != null)
            {
                await action();
            }
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                await action();
                await transaction.CommitAsync(cancellationToken);
            });
        }

        public async ValueTask<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default)
        {
            // Use of an EF Core resiliency strategy when using multiple DbContexts
            // within an explicit BeginTransaction():
            // https://docs.microsoft.com/ef/core/miscellaneous/connection-resiliency
            if (_context.Database.CurrentTransaction != null)
            {
                return await action();
            }
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                var result = await action();
                await transaction.CommitAsync(cancellationToken);
                return result;
            });
        }
    }
}
namespace RocketPDF.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task ExecuteStrategyAsync(Func<Task> action, CancellationToken cancellationToken = default);

        ValueTask<TResult> ExecuteStrategyAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default);
    }
}
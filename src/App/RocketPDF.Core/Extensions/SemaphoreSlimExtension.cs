namespace RocketPDF.Core.Extensions;

public static class SemaphoreSlimExtension
{
    public static IDisposable Lock(this SemaphoreSlim semaphore)
    {
        if (semaphore == null)
            throw new ArgumentNullException(nameof(semaphore));

        var wrapper = new AutoReleaseSemaphoreWrapper(semaphore);
        semaphore.Wait();
        return wrapper;
    }

    public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphore)
    {
        if (semaphore == null)
            throw new ArgumentNullException(nameof(semaphore));

        var wrapper = new AutoReleaseSemaphoreWrapper(semaphore);
        await semaphore.WaitAsync();
        return wrapper;
    }
}

public class AutoReleaseSemaphoreWrapper : IAsyncDisposable, IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    private bool disposed = false;

    public AutoReleaseSemaphoreWrapper(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual ValueTask DisposeAsync()
    {
        try
        {
            Dispose();
            return default;
        }
        catch (Exception exception)
        {
            return new ValueTask(Task.FromException(exception));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            try
            {
                _semaphore.Release();
            }
            catch { }
        }

        disposed = true;
    }
}
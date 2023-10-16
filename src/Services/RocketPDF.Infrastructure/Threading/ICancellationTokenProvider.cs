namespace RocketPDF.Infrastructure.Threading
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
    }
}
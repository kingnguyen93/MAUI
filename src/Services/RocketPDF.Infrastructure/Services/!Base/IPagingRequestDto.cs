namespace RocketPDF.Infrastructure.Services
{
    public interface IPagingRequestDto
    {
        short PageIndex { get; set; }

        short PageSize { get; set; }
    }
}
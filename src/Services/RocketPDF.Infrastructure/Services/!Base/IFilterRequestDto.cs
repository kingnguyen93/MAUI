namespace RocketPDF.Infrastructure.Services
{
    public interface IFilterRequestDto
    {
        string? Sorting { get; set; }

        DateTime? FromDate { get; set; }

        DateTime? ToDate { get; set; }
    }
}
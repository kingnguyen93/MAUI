using RocketPDF.Infrastructure.Services;
using System.ComponentModel.DataAnnotations;

namespace RocketPDF.Application.Services;

[Serializable]
public class BaseRequestDto : IBaseRequestDto
{
    public virtual string? Sorting { get; set; }

    public virtual DateTime? FromDate { get; set; }

    public virtual DateTime? ToDate { get; set; }

    [Range(1, short.MaxValue)]
    public virtual short PageIndex { get; set; } = 1;

    [Range(1, short.MaxValue)]
    public virtual short PageSize { get; set; } = 1000;
}

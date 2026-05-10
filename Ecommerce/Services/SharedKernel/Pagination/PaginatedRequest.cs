using System.ComponentModel;

namespace SharedKernel.Pagination;

public record PaginatedRequest
{
    [DefaultValue(10)]
    public required int PageSize { get; init; } = 10;

    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
}

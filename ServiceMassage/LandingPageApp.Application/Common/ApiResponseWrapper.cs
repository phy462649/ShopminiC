namespace LandingPageApp.Application.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
public record ApiResponseWrapper<T>
{
    public bool Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public object? Meta { get; init; }

    public static ApiResponseWrapper<T> Success(T data, string message = "Success")
        => new() { Status = true, Message = message, Data = data };

    public static ApiResponseWrapper<T> Success(T data, object meta, string message = "Success")
        => new() { Status = true, Message = message, Data = data, Meta = meta };

    public static ApiResponseWrapper<T> Fail(string message)
        => new() { Status = false, Message = message };
}

/// <summary>
/// Pagination metadata
/// </summary>
public record PaginationMeta
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious { get; init; }
    public bool HasNext { get; init; }

    public static PaginationMeta FromPagedResult<T>(PagedResult<T> result)
        => new()
        {
            CurrentPage = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPrevious = result.HasPreviousPage,
            HasNext = result.HasNextPage
        };
}

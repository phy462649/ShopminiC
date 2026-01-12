namespace LandingPageApp.Application.Common;

/// <summary>
/// Result pattern for handling success/failure without exceptions
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public string ErrorCode { get; }

    protected Result(bool isSuccess, string error, string errorCode = "")
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error, string errorCode = "ERROR") => new(false, error, errorCode);
    
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error, string errorCode = "ERROR") => Result<T>.Failure(error, errorCode);
}

/// <summary>
/// Generic Result with value
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string error, string errorCode = "") 
        : base(isSuccess, error, errorCode)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public static new Result<T> Failure(string error, string errorCode = "ERROR") => new(false, default, error, errorCode);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        => IsSuccess ? onSuccess(Value!) : onFailure(Error);
}

/// <summary>
/// Paginated result for list queries
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int page, int pageSize)
        => new()
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
}

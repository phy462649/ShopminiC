namespace LandingPageApp.Application.Dtos;

public class ApiResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class ApiResponse<T>
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

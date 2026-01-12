namespace LandingPageApp.Application.Dtos;

public class ErrorResponse
{
    public bool Status { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public Dictionary<string, string[]> Errors { get; set; } = new();
}

namespace LandingPageApp.Application.Interfaces;

public interface ICloudinaryService
{
    Task<CloudinaryUploadResult> UploadImageAsync(Stream fileStream, string fileName, string folder = "uploads");
    Task<CloudinaryUploadResult> UploadImageAsync(byte[] fileBytes, string fileName, string folder = "uploads");
    Task<bool> DeleteImageAsync(string publicId);
    string GetImageUrl(string publicId, int? width = null, int? height = null);
}

public class CloudinaryUploadResult
{
    public bool Success { get; set; }
    public string? PublicId { get; set; }
    public string? Url { get; set; }
    public string? SecureUrl { get; set; }
    public string? Error { get; set; }
}

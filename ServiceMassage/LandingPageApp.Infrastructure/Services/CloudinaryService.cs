using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LandingPageApp.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace LandingPageApp.Infrastructure.Services;

public class CloudinarySettings
{
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> settings)
    {
        var account = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<CloudinaryUploadResult> UploadImageAsync(Stream fileStream, string fileName, string folder = "uploads")
    {
        try
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folder,
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                return new CloudinaryUploadResult { Success = false, Error = result.Error.Message };

            return new CloudinaryUploadResult
            {
                Success = true,
                PublicId = result.PublicId,
                Url = result.Url?.ToString(),
                SecureUrl = result.SecureUrl?.ToString()
            };
        }
        catch (Exception ex)
        {
            return new CloudinaryUploadResult { Success = false, Error = ex.Message };
        }
    }

    public async Task<CloudinaryUploadResult> UploadImageAsync(byte[] fileBytes, string fileName, string folder = "uploads")
    {
        using var stream = new MemoryStream(fileBytes);
        return await UploadImageAsync(stream, fileName, folder);
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
        catch { return false; }
    }

    public string GetImageUrl(string publicId, int? width = null, int? height = null)
    {
        var transformation = new Transformation();
        if (width.HasValue) transformation = transformation.Width(width.Value);
        if (height.HasValue) transformation = transformation.Height(height.Value);
        transformation = transformation.Crop("fill").Quality("auto").FetchFormat("auto");
        return _cloudinary.Api.UrlImgUp.Transform(transformation).BuildUrl(publicId);
    }
}

using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly long _maxFileSize = 5 * 1024 * 1024;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public UploadController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "uploads")
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "No file uploaded" });

        if (file.Length > _maxFileSize)
            return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return BadRequest(new { success = false, message = "Invalid file type" });

        using var stream = file.OpenReadStream();
        var result = await _cloudinaryService.UploadImageAsync(stream, file.FileName, folder);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Error });

        return Ok(new { success = true, data = new { publicId = result.PublicId, url = result.Url, secureUrl = result.SecureUrl } });
    }

    [HttpDelete("{publicId}")]
    public async Task<IActionResult> DeleteImage(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return BadRequest(new { success = false, message = "Public ID is required" });

        var result = await _cloudinaryService.DeleteImageAsync(publicId);
        return Ok(new { success = result, message = result ? "Image deleted" : "Failed to delete" });
    }
}

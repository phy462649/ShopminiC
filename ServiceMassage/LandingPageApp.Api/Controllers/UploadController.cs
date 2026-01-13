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
            return BadRequest(new { success = false, message = "Invalid file type. Allowed: jpg, jpeg, png, gif, webp" });

        using var stream = file.OpenReadStream();
        var result = await _cloudinaryService.UploadImageAsync(stream, file.FileName, folder);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Error });

        return Ok(new { success = true, data = new { publicId = result.PublicId, url = result.Url, secureUrl = result.SecureUrl } });
    }

    [HttpPost("images")]
    public async Task<IActionResult> UploadImages(List<IFormFile> files, [FromQuery] string folder = "uploads")
    {
        if (files == null || files.Count == 0)
            return BadRequest(new { success = false, message = "No files uploaded" });

        if (files.Count > 10)
            return BadRequest(new { success = false, message = "Maximum 10 files allowed" });

        var results = new List<object>();
        var errors = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > _maxFileSize) { errors.Add($"{file.FileName}: File size exceeds 5MB limit"); continue; }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension)) { errors.Add($"{file.FileName}: Invalid file type"); continue; }

            using var stream = file.OpenReadStream();
            var result = await _cloudinaryService.UploadImageAsync(stream, file.FileName, folder);

            if (result.Success)
                results.Add(new { fileName = file.FileName, publicId = result.PublicId, url = result.Url, secureUrl = result.SecureUrl });
            else
                errors.Add($"{file.FileName}: {result.Error}");
        }

        return Ok(new { success = true, data = results, errors = errors.Count > 0 ? errors : null });
    }

    [HttpDelete("{publicId}")]
    public async Task<IActionResult> DeleteImage(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return BadRequest(new { success = false, message = "Public ID is required" });

        var result = await _cloudinaryService.DeleteImageAsync(publicId);
        return Ok(new { success = result, message = result ? "Image deleted" : "Failed to delete image" });
    }
}

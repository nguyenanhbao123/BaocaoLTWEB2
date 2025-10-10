using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace BeverageShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadController> _logger;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("beverage-image")]
        public async Task<IActionResult> UploadBeverageImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file uploaded" });

                // Validate file size
                if (file.Length > MaxFileSize)
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });

                // Validate file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                    return BadRequest(new { success = false, message = "Invalid file type. Only images allowed" });

                // Create upload directory if not exists
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "beverages");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Save original image
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create thumbnail
                var thumbnailFileName = $"thumb_{fileName}";
                var thumbnailPath = Path.Combine(uploadPath, thumbnailFileName);
                await CreateThumbnail(filePath, thumbnailPath, 300, 300);

                var imageUrl = $"/uploads/animals/{fileName}";
                var thumbnailUrl = $"/uploads/animals/{thumbnailFileName}";

                _logger.LogInformation($"Image uploaded successfully: {fileName}");

                return Ok(new
                {
                    success = true,
                    imageUrl,
                    thumbnailUrl,
                    fileName,
                    fileSize = file.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, new { success = false, message = "Error uploading image", error = ex.Message });
            }
        }

        [HttpPost("beverage-images")]
        public async Task<IActionResult> UploadMultipleImages([FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest(new { success = false, message = "No files uploaded" });

                if (files.Count > 10)
                    return BadRequest(new { success = false, message = "Maximum 10 images allowed" });

                var uploadedImages = new List<object>();

                foreach (var file in files)
                {
                    if (file.Length > 0 && file.Length <= MaxFileSize)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (AllowedExtensions.Contains(extension))
                        {
                            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "beverages");
                            if (!Directory.Exists(uploadPath))
                                Directory.CreateDirectory(uploadPath);

                            var fileName = $"{Guid.NewGuid()}{extension}";
                            var filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Create thumbnail
                            var thumbnailFileName = $"thumb_{fileName}";
                            var thumbnailPath = Path.Combine(uploadPath, thumbnailFileName);
                            await CreateThumbnail(filePath, thumbnailPath, 300, 300);

                            uploadedImages.Add(new
                            {
                                imageUrl = $"/uploads/animals/{fileName}",
                                thumbnailUrl = $"/uploads/animals/{thumbnailFileName}",
                                fileName
                            });
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    count = uploadedImages.Count,
                    images = uploadedImages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading multiple images");
                return StatusCode(500, new { success = false, message = "Error uploading images", error = ex.Message });
            }
        }

        [HttpDelete("beverage-image")]
        public IActionResult DeleteImage([FromQuery] string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return BadRequest(new { success = false, message = "Filename required" });

                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "beverages");
                var filePath = Path.Combine(uploadPath, fileName);
                var thumbnailPath = Path.Combine(uploadPath, $"thumb_{fileName}");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                if (System.IO.File.Exists(thumbnailPath))
                {
                    System.IO.File.Delete(thumbnailPath);
                }

                return Ok(new { success = true, message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image");
                return StatusCode(500, new { success = false, message = "Error deleting image", error = ex.Message });
            }
        }

        private async Task CreateThumbnail(string sourcePath, string destinationPath, int width, int height)
        {
            try
            {
                using var image = Image.FromFile(sourcePath);
                var ratioX = (double)width / image.Width;
                var ratioY = (double)height / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                using var thumbnail = new Bitmap(newWidth, newHeight);
                using (var graphics = Graphics.FromImage(thumbnail))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                thumbnail.Save(destinationPath, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating thumbnail");
                // If thumbnail creation fails, just continue without it
            }
        }
    }
}


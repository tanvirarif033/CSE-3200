using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CSE3200.Application.Services
{
    public interface IImageService
    {
        Task<string> SaveProfileImageAsync(IFormFile imageFile, string userId);
        void DeleteProfileImage(string imagePath);
        string GetDefaultProfileImageUrl();
    }

    public class ImageService : IImageService
    {
        private readonly string _webRootPath;
        private readonly IConfiguration _configuration;

        public ImageService(IConfiguration configuration)
        {
            _configuration = configuration;
            // Get web root path from configuration or use a default
            _webRootPath = _configuration["WebRootPath"] ?? "wwwroot";
        }

        public async Task<string> SaveProfileImageAsync(IFormFile imageFile, string userId)
        {
            if (imageFile == null || imageFile.Length == 0)
                return GetDefaultProfileImageUrl();

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || Array.IndexOf(allowedExtensions, extension) == -1)
                throw new ArgumentException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");

            // Validate file size (max 5MB)
            if (imageFile.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File size cannot exceed 5MB.");

            // Create uploads directory if it doesn't exist
            var uploadsFolder = Path.Combine(_webRootPath, "uploads", "profiles");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var fileName = $"{userId}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return relative path for storage in database
            return $"/uploads/profiles/{fileName}";
        }

        public void DeleteProfileImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && !imagePath.Contains("default-profile"))
            {
                var fullPath = Path.Combine(_webRootPath, imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        public string GetDefaultProfileImageUrl()
        {
            return "/images/default-profile.png";
        }
    }
}
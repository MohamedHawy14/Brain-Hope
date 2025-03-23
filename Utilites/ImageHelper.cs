using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{
    public static class ImageHelper
    {
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxImageSize = 3 * 1024 * 1024; // 3MB

       

        public static async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file.");

            string extension = Path.GetExtension(imageFile.FileName).ToLower();

            // Validate extension
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException("Only .jpg, .jpeg, and .png files are allowed.");

            // Validate size
            if (imageFile.Length > _maxImageSize)
                throw new ArgumentException("Image size cannot exceed 3MB.");

            // Ensure directory exists
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique file name
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return full URL instead of just the path
            string baseUrl = "http://braincancer.runasp.net";  // Change this to your domain
            return $"{baseUrl}/uploads/{uniqueFileName}";
        }

    }


}

#region old
//public static async Task<string> SaveImageAsync(IFormFile imageFile)
//{
//    if (imageFile == null || imageFile.Length == 0)
//        throw new ArgumentException("Invalid image file.");

//    string extension = Path.GetExtension(imageFile.FileName).ToLower();

//    // Validate extension
//    if (!_allowedExtensions.Contains(extension))
//        throw new ArgumentException("Only .jpg, .jpeg, and .png files are allowed.");

//    // Validate size
//    if (imageFile.Length > _maxImageSize)
//        throw new ArgumentException("Image size cannot exceed 3MB.");

//    // Ensure directory exists
//    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
//    if (!Directory.Exists(uploadsFolder))
//        Directory.CreateDirectory(uploadsFolder);

//    // Generate unique file name
//    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
//    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//    // Save file
//    using (var stream = new FileStream(filePath, FileMode.Create))
//    {
//        await imageFile.CopyToAsync(stream);
//    }

//    // Return URL path
//    return $"/uploads/{uniqueFileName}";
//} 
#endregion

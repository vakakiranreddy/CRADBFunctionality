using Microsoft.AspNetCore.Http;

namespace ConferenceRoomBooking.Business.Helpers
{
    public static class ImageHelper
    {
        public static async Task<byte[]?> ConvertToByteArrayAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > 2 * 1024 * 1024) // 2MB limit
                throw new ArgumentException("File size must be less than 2MB");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static string? ConvertToBase64String(byte[]? imageBytes)
        {
            return imageBytes == null ? null : Convert.ToBase64String(imageBytes);
        }

        public static bool IsValidImageFile(IFormFile file)
        {
            if (file == null) return false;
            
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            return allowedTypes.Contains(file.ContentType?.ToLower()) && file.Length <= 2 * 1024 * 1024;
        }
    }
}









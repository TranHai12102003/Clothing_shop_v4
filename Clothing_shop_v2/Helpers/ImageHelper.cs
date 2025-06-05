using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Clothing_shop_v2.Helpers
{
    public class ImageHelper
    {
        private readonly Cloudinary _cloudinary;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public ImageHelper(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        //public async Task<(bool IsSuccess, string ImageUrl, string ErrorMessage)> UploadImageAsync(IFormFile imageFile, string folder, int width = 500, int height = 500)
        public async Task<(bool IsSuccess, string ImageUrl, string ErrorMessage)> UploadImageAsync(IFormFile imageFile, string folder)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return (false, string.Empty, "Vui lòng chọn một ảnh.");
            }

            // Kiểm tra kích thước file
            if (imageFile.Length > MaxFileSize)
            {
                return (false, string.Empty, "Ảnh vượt quá kích thước tối đa 5MB.");
            }

            // Kiểm tra định dạng file
            string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                return (false, string.Empty, $"Ảnh không hợp lệ. Chỉ cho phép ảnh thuộc: {string.Join(", ", _allowedExtensions)}");
            }

            try
            {
                // Upload ảnh lên Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                    //Transformation = new Transformation().Width(width).Height(height).Crop("fill"),
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    return (false, string.Empty, $"Đã xảy ra lỗi khi upload ảnh: {uploadResult.Error.Message}");
                }

                return (true, uploadResult.SecureUrl.ToString(), string.Empty);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"Lỗi hệ thống khi upload ảnh: {ex.Message}");
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl, string folder)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return true; // Không có ảnh để xóa
            }

            try
            {
                var uri = new Uri(imageUrl);
                var publicId = Path.GetFileNameWithoutExtension(uri.Segments.Last());
                var fullPublicId = $"{folder}/{publicId}";

                var deletionParams = new DeletionParams(fullPublicId);
                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                return deletionResult.Result == "ok";
            }
            catch
            {
                return false; // Xóa thất bại
            }
        }

        // Phương thức để thêm lỗi vào ModelState nếu cần
        public void AddModelError(ModelStateDictionary modelState, string key, string errorMessage)
        {
            modelState.AddModelError(key, errorMessage);
        }
    }
}

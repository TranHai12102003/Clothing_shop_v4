using System.Threading.Tasks;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Helpers;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class BannerController : Controller
    {
        private readonly IBannerService _bannerService;
        private readonly ClothingShopV3Context _context;
        private readonly Cloudinary _cloudinary;
        private readonly ImageHelper _imageHelper;
        public BannerController(IBannerService bannerService, Cloudinary cloudinary, ClothingShopV3Context context, ImageHelper imageHelper)
        {
            _bannerService = bannerService;
            _cloudinary = cloudinary;
            _context = context;
            _imageHelper = imageHelper;
        }
        public async Task<ActionResult<PaginationModel<BannerGetVModel>>> Index([FromQuery] BannerFilterParams parameters)
        {
            var response = await _bannerService.GetAll(parameters);
            return View(response.Value);
        }
        public async Task<ActionResult<BannerGetVModel>> Details(int id)
        {
            var response = await _bannerService.GetById(id);
            if (response.Value == null)
            {
                return NotFound();
            }
            return View(response.Value);
        }

        private async Task LoadViewBagData()
        {
            ViewBag.Categories = await _context.Categories.Select(c => new { c.Id, c.CategoryName }).ToListAsync();
            ViewBag.Products = await _context.Products.Select(p => new { p.Id, p.ProductName }).ToListAsync();
            ViewBag.Promotions = await _context.Promotions.Select(p => new { p.Id, p.PromotionName }).ToListAsync();
        }

        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerCreateVModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                // Log lỗi ModelState để debug
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                await LoadViewBagData();
                return View(vmodel);
            }

            string imageUrl = string.Empty;
            if (vmodel.ImageFile != null && vmodel.ImageFile.Length > 0)
            {
                var (isSuccess, uploadedImageUrl, errorMessage) = await _imageHelper.UploadImageAsync(
                    vmodel.ImageFile,
                    folder: "ClothingShop/Banner"
                );

                if (!isSuccess)
                {
                    _imageHelper.AddModelError(ModelState, "ImageFile", errorMessage);
                    return View(vmodel);
                }

                imageUrl = uploadedImageUrl;
            }

            var response = await _bannerService.Create(vmodel, imageUrl);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Thêm banner thất bại.";
            await LoadViewBagData();
            return View(vmodel);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, bool isActive = false)
        {
            var response = await _bannerService.ToggleActive(id, isActive);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
            }
            else
            {
                TempData["ErrorMessage"] = response.Message; // Lưu thông báo lỗi
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var response = await _bannerService.GetById(id);
            if (response.Value == null)
            {
                return NotFound();
            }
            await LoadViewBagData();
            return View(response.Value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BannerUpdateVModel vmodel)
        {
            if (id != vmodel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                await LoadViewBagData();
                return View(vmodel);
            }

            var existingBannerResponse = await _bannerService.GetById(id);
            if (existingBannerResponse.Value == null)
            {
                return NotFound();
            }

            string imageUrl = existingBannerResponse.Value.ImageUrl; // Giữ lại ảnh cũ
            if (vmodel.ImageFile != null && vmodel.ImageFile.Length > 0)
            {
                // Upload ảnh mới
                var (isSuccess, uploadedImageUrl, errorMessage) = await _imageHelper.UploadImageAsync(
                    vmodel.ImageFile,
                    folder: "ClothingShop/Banner"
                );

                if (!isSuccess)
                {
                    _imageHelper.AddModelError(ModelState, "ImageFile", errorMessage);
                    return View(vmodel);
                }

                // Xóa ảnh cũ nếu có
                await _imageHelper.DeleteImageAsync(existingBannerResponse.Value.ImageUrl, "ClothingShop/Banner");
                imageUrl = uploadedImageUrl;
            }

            // Cập nhật thông tin banner
            var response = await _bannerService.Update(vmodel, imageUrl);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Cập nhật banner thành công.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Cập nhật banner thất bại.";
            await LoadViewBagData();
            return View(vmodel);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _bannerService.Delete(id);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
            }
            else
            {
                TempData["ErrorMessage"] = response.Message; // Lưu thông báo lỗi
            }
            return RedirectToAction("Index");
        }
    }
}

using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clothing_shop_v2.Controllers
{
    public class SizeController : Controller
    {
        private readonly ILogger<SizeController> _logger;
        private readonly ClothingShopV3Context _context;
        private readonly ISizeService _sizeService;

        public SizeController(ILogger<SizeController> logger, ClothingShopV3Context context, ISizeService sizeService)
        {
            _logger = logger;
            _context = context;
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationModel<SizeGetVModel>>> Index([FromQuery] SizeFilterParams parameters)
        {
            var response = await _sizeService.GetAll(parameters);
            return View(response.Value);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SizeCreateVModel size)
        {
            if (ModelState.IsValid)
            {
                var response = await _sizeService.Create(size);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = $"Kích thước '{size.SizeName}' đã được thêm thành công.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", response.Message);
            }
            return View(size);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var size = await _sizeService.GetById(id);
            if (size == null)
            {
                TempData["ErrorMessage"] = "Kích thước không tồn tại.";
                return RedirectToAction("Index");
            }
            return View(size.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, SizeUpdateVModel sizeVModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _sizeService.Update(sizeVModel);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = response.Message;
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", response.Message);
            }
            return View(sizeVModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _sizeService.Delete(id);
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
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, bool isActive = false)
        {
            var response = await _sizeService.ToggleActive(id, isActive);
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
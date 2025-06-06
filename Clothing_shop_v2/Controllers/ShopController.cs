using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Clothing_shop_v2.Common.Constants;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Clothing_shop_v2.VModels.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ClothingShopV3Context _context;
        public ShopController(IProductService productService, ICategoryService categoryService, ClothingShopV3Context context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _context = context;
        }
        public async Task<IActionResult> Index([FromQuery] ProductFilterParams filterParams, string sortBy = null)
        {
            ViewData["CurrentFilter"] = filterParams;
            // Gọi action GetAll
            var productResult = await _productService.GetAll(filterParams); // GetAll trả về Task<ActionResult<PaginationModel<ProductGetVModel>>>

            // Kiểm tra phản hồi
            if (productResult.Result is NotFoundObjectResult)
            {
                return NotFound("No products found.");
            }

            // Truy xuất giá trị từ ActionResult
            var paginationModel = productResult.Value;
            if (paginationModel == null || !paginationModel.Records.Any())
            {
                return NotFound("No products found.");
            }

            // Lấy danh mục
            var categories = await _context.Categories
                .Select(c => CategoryMapping.EntityToVModel(c)).ToListAsync();

            // Lấy danh sách color
            var colors = await _context.Colors
                .Select(c => ColorMapping.EntityToVModel(c)).ToListAsync();

            // Lấy danh sách size
            var sizes = await _context.Sizes
                .Select(c => SizeMapping.EntityToVModel(c)).ToListAsync();

            // Tạo ShopVModel
            var model = new ShopVModel
            {
                Products = paginationModel,
                Categories = categories,
                Colors = colors,
                Sizes = sizes,
            };
            int cartCount = 0;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                cartCount = await _context.Carts
                    .Where(c => c.UserId == int.Parse(userId) && c.IsActive == true)
                    .CountAsync();
                ViewBag.CartCount = cartCount;
            }
            else
            {
                var cartCookie = Request.Cookies["Cart"];
                var cartSession = string.IsNullOrEmpty(cartCookie)
                    ? new List<CartCreateVModel>()
                    : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie) ?? new List<CartCreateVModel>();
                ViewBag.CartCount = cartSession.Count();
            }

            return View(model);
        }
        // Helper để tạo slug (nên đặt trong một class riêng hoặc extension method)
        private string ToSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = text.ToLower();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[èéẹẻẽêềếệểễ]", "e");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[òóọỏõôồốộổỗơờớợởỡ]", "o");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[ìíịỉĩ]", "i");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[ùúụủũưừứựửữ]", "u");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[ỳýỵỷỹ]", "y");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[đ]", "d");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\-]", "");
            return text;
        }
    }
}

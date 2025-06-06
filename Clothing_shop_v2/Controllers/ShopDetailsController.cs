using System.Security.Claims;
using System.Text.Json;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Controllers
{
    public class ShopDetailsController : Controller
    {
        private readonly ILogger<ShopDetailsController> _logger;
        private readonly IProductService _productService;
        private readonly ClothingShopV3Context _context;
        public ShopDetailsController(ILogger<ShopDetailsController> logger, IProductService productService, ClothingShopV3Context context)
        {
            _logger = logger;
            _productService = productService;
            _context = context;
        }
        public async Task<IActionResult> Index(int id)
        {
            var product = await _productService.GetProductDetail(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive == true)
                .Select(c => CategoryMapping.EntityToVModel(c)).ToListAsync();
            var relatedProducts = await _productService.RelatedProducts(id);
            if (relatedProducts == null || !relatedProducts.Any())
            {
                ViewBag.RelatedProducts = new List<ProductGetVModel>();
            }
            else
            {
                ViewBag.RelatedProducts = relatedProducts;
            }
            int cartCount = 0;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                cartCount = await _context.Carts
                    .Where(c => c.UserId ==  int.Parse(userId) && c.IsActive == true)
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
            return View(product.Value);
        }
        private async Task<List<ProductGetVModel>> RelatedProducts(int productId)
        {
            var relatedProducts = await _productService.RelatedProducts(productId);
            return relatedProducts;
        }
    }
}

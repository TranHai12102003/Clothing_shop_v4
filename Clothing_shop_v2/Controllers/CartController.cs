using System.Security.Claims;
using System.Text.Json;
using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.Utilities;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ClothingShopV3Context _context;
        public CartController(ICartService cartService, ClothingShopV3Context context)
        {
            _cartService = cartService;
            _context = context;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        //    var result = await _cartService.GetAll(userId);
        //    return View(result);
        //}

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var cartCookie = Request.Cookies["Cart"];
                var cartSession = string.IsNullOrEmpty(cartCookie)
                    ? new List<CartCreateVModel>()
                    : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie);

                var variantIds = cartSession.Select(c => c.VariantId).ToList();
                var variants = await _context.Variants
                    .Where(v => variantIds.Contains(v.Id))
                    .Include(v => v.Product)
                        .ThenInclude(p => p.ProductImages)
                    .Include(v => v.Size)
                    .Include(v => v.Color)
                    .ToListAsync();

                var cartItems = variants.Select(v => new CartGetVModel
                {
                    Id = 0,
                    VariantId = v.Id,
                    UserId = 0,
                    Quantity = cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0,
                    TotalPrice = v.Price * (cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0),
                    Variant = VariantMapping.EntityGetVModel(v)
                }).ToList();

                ViewBag.CartCount = cartSession.Count;
                ViewBag.Categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Select(c => CategoryMapping.EntityToVModel(c)).ToListAsync();
                return View(cartItems ?? new List<CartGetVModel>());
            }
            else
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _cartService.GetAll(userId) ?? new List<CartGetVModel>();
                ViewBag.CartCount = result.Count;
                ViewBag.Categories = await _context.Categories
                    .Include(c => c.ParentCategory)
                    .Select(c => CategoryMapping.EntityToVModel(c)).ToListAsync();
                return View(result);
            }

        }
        #region
        //[HttpPost]
        //public async Task<IActionResult> AddToCart(CartFormVModel formModel)
        //{
        //    if (!ModelState.IsValid || formModel.SizeId == 0 || formModel.ColorId == 0 || formModel.Quantity < 1)
        //    {
        //        TempData["ErrorMessage"] = "Vui lòng chọn kích cỡ, màu sắc và số lượng hợp lệ.";
        //        return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
        //    }

        //    // Tìm VariantId dựa trên ProductId, SizeId và ColorId
        //    var variant = await _context.Variants
        //        .FirstOrDefaultAsync(v => v.ProductId == formModel.ProductId &&
        //                                 v.SizeId == formModel.SizeId &&
        //                                 v.ColorId == formModel.ColorId);

        //    if (variant == null)
        //    {
        //        TempData["ErrorMessage"] = "Biến thể sản phẩm không tồn tại.";
        //        return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
        //    }

        //    // Tạo CartCreateVModel để gửi đến service
        //    var cartModel = new CartCreateVModel
        //    {
        //        UserId = formModel.UserId,
        //        VariantId = variant.Id,
        //        Quantity = formModel.Quantity
        //    };

        //    // Gọi service để thêm vào giỏ hàng
        //    var response = await _cartService.Create(cartModel);
        //    if (response.IsSuccess)
        //    {
        //        TempData["SuccessMessage"] = response.Message;
        //        return RedirectToAction("Index", "Cart");
        //    }

        //    TempData["ErrorMessage"] = response.Message;
        //    return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
        //}
        #endregion
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartFormVModel formModel)
        {
            if (!ModelState.IsValid || formModel.SizeId == 0 || formModel.ColorId == 0 || formModel.Quantity < 1)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn kích cỡ, màu sắc và số lượng hợp lệ.";
                return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
            }

            var variant = await _context.Variants
                .Include(v => v.Product) // Bao gồm Product nếu cần cho thông báo
                .FirstOrDefaultAsync(v => v.ProductId == formModel.ProductId &&
                                         v.SizeId == formModel.SizeId &&
                                         v.ColorId == formModel.ColorId);

            if (variant == null)
            {
                TempData["ErrorMessage"] = "Biến thể không tồn tại.";
                return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
            }

            if (!User.Identity.IsAuthenticated)
            {
                var cartCookie = Request.Cookies["Cart"] != null
                    ? JsonSerializer.Deserialize<List<CartCreateVModel>>(Request.Cookies["Cart"])
                    : new List<CartCreateVModel>();

                var existingItem = cartCookie.FirstOrDefault(c => c.VariantId == variant.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += formModel.Quantity;
                }
                else
                {
                    cartCookie.Add(new CartCreateVModel
                    {
                        VariantId = variant.Id,
                        Quantity = formModel.Quantity
                    });
                }
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                };
                Response.Cookies.Append("Cart", JsonSerializer.Serialize(cartCookie), cookieOptions);
                TempData["SuccessMessage"] = "Thêm sản phẩm vào giỏ hàng tạm thời thành công.";
            }
            else
            {
                var cartModel = new CartCreateVModel
                {
                    UserId = formModel.UserId,
                    VariantId = variant.Id,
                    Quantity = formModel.Quantity
                };
                var response = await _cartService.Create(cartModel);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = response.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message;
                }
            }
                return RedirectToAction("Index", "Cart");
        }
        #region
        //[HttpPost]
        //public async Task<IActionResult> Update(CartUpdateVModel cartVModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(cartVModel);
        //    }
        //    var response = await _cartService.Update(cartVModel);
        //    if (response.IsSuccess)
        //    {
        //        TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
        //    return View(cartVModel);
        //}
        #endregion
        [HttpPost]
        public async Task<IActionResult> Update(int Id, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { isSuccess = false, message = "Vui lòng đăng nhập để cập nhật giỏ hàng." });
            }

            if (quantity < 1)
            {
                return Json(new { isSuccess = false, message = "Số lượng không hợp lệ.", currentQuantity = 1 });
            }

            var cartVModel = new CartUpdateVModel
            {
                Id = Id,
                Quantity = quantity
            };
            var response = await _cartService.Update(cartVModel);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return Json(new { isSuccess = response.IsSuccess, message = response.Message, cartCount = await _context.Carts.CountAsync(c => c.UserId == userId && c.IsActive == true) });
        }
        #region
        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var response = await _cartService.Delete(id);
        //    if (response.IsSuccess)
        //    {
        //        TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
        //    return RedirectToAction("Index");
        //}
        #endregion

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var cartSession = HttpContext.Session.GetObjectFromJson<List<CartCreateVModel>>("Cart") ?? new List<CartCreateVModel>();
                var item = cartSession.FirstOrDefault(c => c.VariantId == id);
                if (item == null)
                {
                    return Json(new { isSuccess = false, message = "Mục giỏ hàng không tồn tại." });
                }

                cartSession.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cartSession);
                return Json(new { isSuccess = true, message = "Xóa sản phẩm thành công.", cartCount = cartSession.Count });
            }

            var response = await _cartService.Delete(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return Json(new { isSuccess = response.IsSuccess, message = response.Message, cartCount = await _context.Carts.CountAsync(c => c.UserId == userId && c.IsActive == true) });
        }
        #region
        //[HttpPost]
        //public async Task<IActionResult> DeleteByUserId()
        //{
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        //    var response = await _cartService.DeleteByUserId(userId);
        //    if (response.IsSuccess)
        //    {
        //        TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public async Task<IActionResult> DeleteByVariantId(int variantId)
        //{
        //    var response = await _cartService.DeleteByVariantId(variantId);
        //    if (response.IsSuccess)
        //    {
        //        TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
        //    return RedirectToAction("Index");
        //}
        #endregion
        [HttpPost]
        public async Task<IActionResult> DeleteByUserId()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _cartService.DeleteByUserId(userId);
            return Json(new { isSuccess = response.IsSuccess, message = response.Message, cartCount = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteByVariantId(int variantId)
        {
            var response = await _cartService.DeleteByVariantId(variantId);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return Json(new { isSuccess = response.IsSuccess, message = response.Message, cartCount = await _context.Carts.CountAsync(c => c.UserId == userId && c.IsActive == true) });
        }

        [HttpPost]
        public IActionResult UpdateCookie(int id, int quantity)
        {
            var cartCookie = Request.Cookies["Cart"];
            var cart = string.IsNullOrEmpty(cartCookie)
                ? new List<CartCreateVModel>()
                : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie);

            var item = cart.FirstOrDefault(c => c.VariantId == id);
            if (item == null)
            {
                return Json(new { isSuccess = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            if (quantity < 1)
            {
                return Json(new { isSuccess = false, message = "Số lượng phải lớn hơn 0.", currentQuantity = item.Quantity });
            }

            item.Quantity = quantity;
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true
            };
            Response.Cookies.Append("Cart", JsonSerializer.Serialize(cart), cookieOptions);

            return Json(new { isSuccess = true, message = "Cập nhật số lượng thành công.", cartCount = cart.Count });
        }

        [HttpPost]
        public IActionResult DeleteCookie(int id)
        {
            var cartCookie = Request.Cookies["Cart"];
            var cart = string.IsNullOrEmpty(cartCookie)
                ? new List<CartCreateVModel>()
                : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie);

            var item = cart.FirstOrDefault(c => c.VariantId == id);
            if (item == null)
            {
                return Json(new { isSuccess = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            cart.Remove(item);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true
            };
            Response.Cookies.Append("Cart", JsonSerializer.Serialize(cart), cookieOptions);

            return Json(new { isSuccess = true, message = "Xóa sản phẩm thành công.", cartCount = cart.Count });
        }
    }
}

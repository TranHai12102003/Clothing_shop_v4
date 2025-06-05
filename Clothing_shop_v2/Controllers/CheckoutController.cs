using Clothing_shop_v2.Mappings;
using System.Security.Claims;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Clothing_shop_v2.Services.ISerivce;
using Shopapp.Mappings;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Services;
using Microsoft.Extensions.Logging;

namespace Clothing_shop_v2.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ClothingShopV3Context _context;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IVnPayService _vnPayService;
        public CheckoutController(ClothingShopV3Context context, ICartService cartService, IOrderService orderService, IVnPayService vpnPayService)
        {
            _context = context;
            _cartService = cartService;
            _orderService = orderService;
            _vnPayService = vpnPayService;
        }
        //public async Task<IActionResult> Index()
        //{
        //    List<CartGetVModel> cartItems;

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        // Người dùng chưa đăng nhập: Lấy giỏ hàng từ cookie
        //        var cartCookie = Request.Cookies["Cart"];
        //        var cartSession = string.IsNullOrEmpty(cartCookie)
        //            ? new List<CartCreateVModel>()
        //            : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie);

        //        var variantIds = cartSession.Select(c => c.VariantId).ToList();
        //        var variants = await _context.Variants
        //            .Where(v => variantIds.Contains(v.Id))
        //            .Include(v => v.Product)
        //                .ThenInclude(p => p.ProductImages)
        //            .Include(v => v.Size)
        //            .Include(v => v.Color)
        //            .ToListAsync();

        //        cartItems = variants.Select(v => new CartGetVModel
        //        {
        //            Id = 0,
        //            VariantId = v.Id,
        //            UserId = 0,
        //            Quantity = cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0,
        //            TotalPrice = v.Price * (cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0),
        //            Variant = VariantMapping.EntityGetVModel(v)
        //        }).ToList();

        //        // Lấy danh mục
        //        ViewBag.Categories = await _context.Categories
        //            .Select(c => CategoryMapping.EntityToVModel(c))
        //            .ToListAsync();

        //        ViewBag.CartCount = cartSession.Count;
        //    }
        //    else
        //    {
        //        // Người dùng đã đăng nhập: Lấy giỏ hàng từ dịch vụ
        //        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        //        cartItems = await _cartService.GetAll(userId) ?? new List<CartGetVModel>();
        //        ViewBag.CartCount = cartItems.Count;
        //        // Lấy thông tin hồ sơ người dùng (nếu có)
        //        var userProfile = User.Identity.IsAuthenticated
        //            ? await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
        //            : null;
        //        ViewBag.UserProfile = userProfile;
        //    }
        //    return View(cartItems ?? new List<CartGetVModel>());
        //}
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
                return RedirectToAction("Login", "Home");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cartItems = await _cartService.GetAll(userId) ?? new List<CartGetVModel>();
            ViewBag.CartCount = cartItems.Count;

            var userProfile = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userProfile == null)
            {
                TempData["WarningMessage"] = "Không tìm thấy thông tin hồ sơ người dùng.";
            }

            var model = new CheckoutVModel
            {
                FullName = userProfile != null ? $"{userProfile.FullName}":"",
                Email = userProfile?.Email,
                PhoneNumber = userProfile?.PhoneNumber,
                Address = userProfile?.Address,
                ShippingFullName = userProfile != null ? $"{userProfile.FullName}" : "",
                ShippingEmail = userProfile?.Email,
                ShippingPhoneNumber = userProfile?.PhoneNumber,
                ShippingAddress = userProfile?.Address,
            };

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive == true)
                .Select(c => CategoryMapping.EntityToVModel(c))
                .ToListAsync();
            ViewBag.CartItems = cartItems;
            ViewBag.UserProfile = userProfile;

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutVModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng.";
                return RedirectToAction("Login", "Home");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ các thông tin bắt buộc.";
                var carts = await _cartService.GetAll(userId) ?? new List<CartGetVModel>();
                ViewBag.CartItems = carts;
                ViewBag.Categories = await _context.Categories
                    .Where(c => c.IsActive == true)
                    .Select(c => CategoryMapping.EntityToVModel(c))
                    .ToListAsync();
                ViewBag.UserProfile = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return View("Index", model);
            }

            var cartItems = await _cartService.GetAll(userId) ?? new List<CartGetVModel>();
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                ViewBag.CartItems = cartItems;
                ViewBag.Categories = await _context.Categories
                    .Where(c => c.IsActive == true)
                    .Select(c => CategoryMapping.EntityToVModel(c))
                    .ToListAsync();
                ViewBag.UserProfile = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return View("Index", model);
            }

            var shippingAddress = model.ShipToDifferentAddress ? model.ShippingAddress : model.Address;

            var orderVModel = new OrderCreateVModel
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(item => item.TotalPrice) + 10, // Phí vận chuyển $10
                Status = "Pending",
                ShippingAddress = shippingAddress,
                PaymentId = null,
                VoucherId = null,
                IsActive = true
            };

            var response = await _orderService.Create(orderVModel);
            if (response is ErrorResponseResult errorResponse)
            {
                TempData["ErrorMessage"] = errorResponse.Message;
                ViewBag.CartItems = cartItems;
                ViewBag.Categories = await _context.Categories
                    .Where(c => c.IsActive == true)
                    .Select(c => CategoryMapping.EntityToVModel(c))
                    .ToListAsync();
                ViewBag.UserProfile = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return View("Index", model);
            }

            var successResponse = (SuccessResponseResult)response;
            var order = (Order)successResponse.Data;
            int orderId = order.Id;

            var orderDetails = cartItems.Select(item => new OrderDetail
            {
                OrderId = orderId,
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                UnitPrice = item.Variant?.Price ?? 0
            }).ToList();

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            // Xử lý thanh toán VNPay
            if (model.PaymentMethod == "VNPay") // Ở đây sử dụng "Paypal" để đại diện cho VNPay
            {
                var paymentVModel = new PaymentCreateVModel
                {
                    PaymentGateway = "VNPay",
                    Amount = order.TotalAmount,
                    PaymentStatus = "Pending",
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "VNPay",
                    IsActive = true
                };

                var payment = PaymentMapping.VModelToEntity(paymentVModel);
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Cập nhật PaymentId cho Order
                order.PaymentId = payment.Id;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                // Tạo URL thanh toán VNPay
                var paymentUrl = _vnPayService.CreatePaymentUrl(order, HttpContext);
                return Redirect(paymentUrl);
            }
            if(model.PaymentMethod == "COD")
            {
                var payment = new Payment
                {
                    PaymentGateway = "COD",
                    Amount = order.TotalAmount,
                    PaymentMethod = "COD",
                    PaymentStatus = "UnPaid",
                    PaymentDate = DateTime.Now,
                    IsActive = true
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                // Cập nhật PaymentId cho Order
                order.PaymentId = payment.Id;
                order.Status = "Pending"; // Trạng thái đơn hàng khi thanh toán COD
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }

            // Xóa giỏ hàng sau khi thanh toán
            var cartItemsToDelete = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
            _context.Carts.RemoveRange(cartItemsToDelete);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            //return RedirectToAction("OrderConfirmation", new { orderId });
            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success)
            {
                // Cập nhật trạng thái thanh toán và đơn hàng
                //var payment = await _context.Payments
                //    .FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);
                var order = await _context.Orders
                    .Include(c => c.Payment)
                    .FirstOrDefaultAsync(o => o.Id == Convert.ToInt64(response.OrderId));

                if (order.Payment != null && order != null)
                {
                    order.Payment.PaymentStatus = response.VnPayResponseCode == "00" ? "Completed" : "Failed";
                    order.Payment.TransactionId = response.TransactionId;
                    order.Status = response.VnPayResponseCode == "00" ? "Confirmed" : "Pending";
                    _context.Update(order.Payment);
                    _context.Update(order);

                    // Xóa giỏ hàng nếu thanh toán thành công
                    if (response.VnPayResponseCode == "00")
                    {
                        var userId = order.UserId; // Lấy UserId từ Order
                        var cartItems = await _context.Carts
                            .Where(c => c.UserId == userId)
                            .ToListAsync();
                        if (cartItems.Any())
                        {
                            _context.Carts.RemoveRange(cartItems);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = response.VnPayResponseCode == "00"
                        ? "Thanh toán thành công!"
                        : "Thanh toán không thành công. Vui lòng thử lại.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin thanh toán hoặc đơn hàng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Xác thực chữ ký không hợp lệ.";
            }

            return RedirectToAction("Index", "Cart");
        }

        //public async Task<IActionResult> OrderConfirmation(int orderId)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var order = await _context.Orders
        //        .Include(o => o.OrderDetails)
        //            .ThenInclude(od => od.Variant)
        //                .ThenInclude(v => v.Product)
        //        .Include(o => o.OrderDetails)
        //            .ThenInclude(od => od.Variant)
        //                .ThenInclude(v => v.Size)
        //        .Include(o => o.OrderDetails)
        //            .ThenInclude(od => od.Variant)
        //                .ThenInclude(v => v.Color)
        //        .Include(o => o.User)
        //        .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId));

        //    if (order == null)
        //    {
        //        TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Categories = await _context.Categories
        //        .Where(c => c.IsActive == true)
        //        .Select(c => CategoryMapping.EntityToVModel(c))
        //        .ToListAsync();

        //    return View(order);
        //}
    }
}

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
        private readonly IEmailService _emailService;
        public CheckoutController(ClothingShopV3Context context, ICartService cartService, IOrderService orderService, IVnPayService vpnPayService, IEmailService emailService)
        {
            _context = context;
            _cartService = cartService;
            _orderService = orderService;
            _vnPayService = vpnPayService;
            _emailService = emailService;
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
            List<CartGetVModel> cartItems;
            User userProfile = null;
            var model = new CheckoutVModel();

            if (!User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập: Lấy giỏ hàng từ cookie
                var cartCookie = Request.Cookies["Cart"];
                var cartSession = string.IsNullOrEmpty(cartCookie)
                    ? new List<CartCreateVModel>()
                    : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie) ?? new List<CartCreateVModel>();

                if (cartSession.Any())
                {
                    var variantIds = cartSession.Select(c => c.VariantId).ToList();
                    var variants = await _context.Variants
                        .Where(v => variantIds.Contains(v.Id))
                        .Include(v => v.Product)
                            .ThenInclude(p => p.ProductImages)
                        .Include(v => v.Size)
                        .Include(v => v.Color)
                        .ToListAsync();

                    cartItems = variants.Select(v => new CartGetVModel
                    {
                        Id = 0, // Guest cart không có ID trong database
                        VariantId = v.Id,
                        UserId = 0, // Guest user
                        Quantity = cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0,
                        TotalPrice = v.Price * (cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0),
                        Variant = VariantMapping.EntityGetVModel(v)
                    }).ToList();
                }
                else
                {
                    cartItems = new List<CartGetVModel>();
                }

                // Kiểm tra giỏ hàng có trống không
                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                    return RedirectToAction("Index", "Cart");
                }

                ViewBag.CartCount = cartSession.Count;
                // Model trống cho guest user (họ sẽ phải nhập thông tin)
            }
            else
            {
                // Người dùng đã đăng nhập
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                cartItems = await _cartService.GetAll(userId) ?? new List<CartGetVModel>();

                // Kiểm tra giỏ hàng có trống không
                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                    return RedirectToAction("Index", "Cart");
                }

                ViewBag.CartCount = cartItems.Count;
                userProfile = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (userProfile == null)
                {
                    TempData["WarningMessage"] = "Không tìm thấy thông tin hồ sơ người dùng.";
                }

                // Điền sẵn thông tin cho user đã đăng nhập
                model = new CheckoutVModel
                {
                    FullName = userProfile?.FullName ?? "",
                    Email = userProfile?.Email,
                    PhoneNumber = userProfile?.PhoneNumber,
                    Address = userProfile?.Address,
                    ShippingFullName = userProfile?.FullName ?? "",
                    ShippingEmail = userProfile?.Email,
                    ShippingPhoneNumber = userProfile?.PhoneNumber,
                    ShippingAddress = userProfile?.Address,
                };
            }

            // Set ViewBag data chung cho cả 2 trường hợp
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive == true)
                .Select(c => CategoryMapping.EntityToVModel(c))
                .ToListAsync();
            ViewBag.CartItems = cartItems;
            ViewBag.UserProfile = userProfile; // null cho guest, có giá trị cho user đã đăng nhập
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutVModel model)
        {
            List<CartGetVModel> cartItems;
            int? userId = null;

            // Xác định userId và lấy giỏ hàng
            if (User.Identity.IsAuthenticated)
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                cartItems = await _cartService.GetAll(userId.Value) ?? new List<CartGetVModel>();
            }
            else
            {
                // Guest user - lấy giỏ hàng từ cookie
                var cartCookie = Request.Cookies["Cart"];
                var cartSession = string.IsNullOrEmpty(cartCookie)
                    ? new List<CartCreateVModel>()
                    : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie) ?? new List<CartCreateVModel>();

                if (cartSession.Any())
                {
                    var variantIds = cartSession.Select(c => c.VariantId).ToList();
                    var variants = await _context.Variants
                        .Where(v => variantIds.Contains(v.Id))
                        .Include(v => v.Product)
                            .ThenInclude(p => p.ProductImages)
                        .Include(v => v.Size)
                        .Include(v => v.Color)
                        .ToListAsync();

                    cartItems = variants.Select(v => new CartGetVModel
                    {
                        Id = 0,
                        VariantId = v.Id,
                        UserId = 0,
                        Quantity = cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0,
                        TotalPrice = v.Price * (cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0),
                        Variant = VariantMapping.EntityGetVModel(v)
                    }).ToList();
                }
                else
                {
                    cartItems = new List<CartGetVModel>();
                }
            }

            // Kiểm tra giỏ hàng có trống không
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            // Kiểm tra số lượng tồn kho
            foreach (var item in cartItems)
            {
                var variant = await _context.Variants.FindAsync(item.VariantId);
                if (variant == null || variant.QuantityInStock < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm {item.Variant?.Product?.ProductName} không đủ số lượng tồn kho.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Validate model
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ các thông tin bắt buộc.";

                // Prepare ViewBag for return view
                ViewBag.CartItems = cartItems;
                ViewBag.Categories = await _context.Categories
                    .Where(c => c.IsActive == true)
                    .Select(c => CategoryMapping.EntityToVModel(c))
                    .ToListAsync();
                ViewBag.UserProfile = userId.HasValue ?
                    await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value) : null;
                ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

                return View("Index", model);
            }

            //// Tính địa chỉ giao hàng
            //var shippingAddress = model.ShipToDifferentAddress ? model.ShippingAddress : model.Address;
            string shippingAddress;
            if (User.Identity.IsAuthenticated)
            {
                // User đã đăng nhập: có thể chọn địa chỉ khác
                shippingAddress = model.ShipToDifferentAddress ? model.ShippingAddress : model.Address;
            }
            else
            {
                // Guest user: chỉ có 1 địa chỉ
                shippingAddress = model.Address;
            }

            // Tạo đơn hàng với thông tin phù hợp cho cả guest và user đã đăng nhập
            var orderVModel = new OrderCreateVModel
            {
                UserId = userId, // null cho guest user
                GuestFullName = !User.Identity.IsAuthenticated ? model.FullName : null,
                GuestEmail = !User.Identity.IsAuthenticated ? model.Email : null,
                GuestPhoneNumber = !User.Identity.IsAuthenticated ? model.PhoneNumber : null,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(item => item.TotalPrice) + 10, // Phí vận chuyển $10
                Status = "Pending",
                ShippingAddress = shippingAddress,
                PaymentId = null,
                VoucherId = null,
                IsActive = true
            };

            // Tạo đơn hàng
            var response = await _orderService.Create(orderVModel);
            if (response is ErrorResponseResult errorResponse)
            {
                TempData["ErrorMessage"] = errorResponse.Message;

                // Prepare ViewBag for return view
                ViewBag.CartItems = cartItems;
                ViewBag.Categories = await _context.Categories
                    .Where(c => c.IsActive == true)
                    .Select(c => CategoryMapping.EntityToVModel(c))
                    .ToListAsync();
                ViewBag.UserProfile = userId.HasValue ?
                    await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value) : null;
                ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

                return View("Index", model);
            }

            var successResponse = (SuccessResponseResult)response;
            var order = (Order)successResponse.Data;
            int orderId = order.Id;

            // Tạo order details
            var orderDetails = cartItems.Select(item => new OrderDetail
            {
                OrderId = orderId,
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                UnitPrice = item.Variant?.Price ?? 0
            }).ToList();

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            // Xử lý thanh toán
            Payment payment = null;

            if (model.PaymentMethod == "VNPay")
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

                payment = PaymentMapping.VModelToEntity(paymentVModel);
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
            else if (model.PaymentMethod == "COD")
            {
                payment = new Payment
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
                order.Status = "Pending";
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                // Send order confirmation email
                await SendOrderConfirmationEmail(order, cartItems, model);
            }

            // Xóa giỏ hàng sau khi đặt hàng thành công
            await ClearCart(userId);

            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            //return RedirectToAction("OrderConfirmation", new { orderId });
            return RedirectToAction("Index", "Cart");
        }

        // Helper method để xóa giỏ hàng
        private async Task ClearCart(int? userId)
        {
            if (userId.HasValue)
            {
                // Xóa giỏ hàng từ database cho user đã đăng nhập
                var cartItemsToDelete = await _context.Carts.Where(c => c.UserId == userId.Value).ToListAsync();
                _context.Carts.RemoveRange(cartItemsToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Xóa giỏ hàng từ cookie cho guest user
                Response.Cookies.Delete("Cart");
            }
        }

        // Helper method để tạo tài khoản cho guest user (optional)
        private async Task CreateGuestAccount(CheckoutVModel model)
        {
            try
            {
                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    // Set other required fields
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["InfoMessage"] = "Tài khoản của bạn đã được tạo thành công!";
            }
            catch (Exception ex)
            {
                // Log error but don't fail the order process
                TempData["WarningMessage"] = "Đặt hàng thành công nhưng không thể tạo tài khoản. Vui lòng đăng ký thủ công.";
            }
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
                    order.Status = response.VnPayResponseCode == "00" ? "Confirmed" : "Cancelled";
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
                        // Lấy cartItems để gửi email
                        var orderDetails = await _context.OrderDetails
                            .Where(od => od.OrderId == order.Id)
                            .Include(od => od.Variant)
                                .ThenInclude(v => v.Product)
                                    .ThenInclude(p => p.ProductImages)
                                .Include(od => od.Variant.Size)
                                .Include(od => od.Variant.Color)
                            .ToListAsync();

                        var cartItemsForEmail = orderDetails.Select(od => new CartGetVModel
                        {
                            VariantId = od.VariantId,
                            Quantity = od.Quantity,
                            TotalPrice = od.UnitPrice * od.Quantity,
                            Variant = VariantMapping.EntityGetVModel(od.Variant)
                        }).ToList();

                        // Tạo model để gửi email
                        var checkoutModel = new CheckoutVModel
                        {
                            FullName = order.GuestFullName ?? (await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId))?.FullName,
                            Email = order.GuestEmail ?? (await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId))?.Email,
                            PhoneNumber = order.GuestPhoneNumber ?? (await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId))?.PhoneNumber,
                            Address = order.ShippingAddress,
                            ShippingFullName = order.GuestFullName ?? (await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId))?.FullName,
                            ShippingEmail = order.GuestEmail ?? (await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId))?.Email,
                            ShippingPhoneNumber = order.GuestPhoneNumber ?? (await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId))?.PhoneNumber,
                            ShippingAddress = order.ShippingAddress
                        };

                        // Gửi email xác nhận
                        await SendOrderConfirmationEmail(order, cartItemsForEmail, checkoutModel);
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

        private async Task<string> LoadEmailTemplateAsync(string templatePath)
        {
            // Ensure the template path is valid and accessible  
            if (!System.IO.File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template not found at path: {templatePath}");
            }

            // Read the email template content from the file  
            return await System.IO.File.ReadAllTextAsync(templatePath);
        }

        private async Task SendOrderConfirmationEmail(Order order, List<CartGetVModel> cartItems, CheckoutVModel model)
        {
            try
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Template", "sendmail.html");
                var emailTemplate = await LoadEmailTemplateAsync(templatePath);

                var fullName = User.Identity.IsAuthenticated ? model.FullName : model.ShippingFullName ?? "Khách hàng";
                emailTemplate = emailTemplate.Replace("{{fullName}}", fullName);
                emailTemplate = emailTemplate.Replace("{{orderId}}", $"#DH{order.Id.ToString("D8")}");
                emailTemplate = emailTemplate.Replace("{{orderDate}}", order.OrderDate.ToString("dd/MM/yyyy - HH:mm"));
                emailTemplate = emailTemplate.Replace("{{paymentMethod}}", order.Payment?.PaymentMethod ?? "COD");
                emailTemplate = emailTemplate.Replace("{{status}}", order.Status);
                emailTemplate = emailTemplate.Replace("{{shippingAddress}}", order.ShippingAddress);
                emailTemplate = emailTemplate.Replace("{{phoneNumber}}", model.PhoneNumber ?? "N/A");
                emailTemplate = emailTemplate.Replace("{{estimatedDelivery}}", "2-3 ngày làm việc");
                emailTemplate = emailTemplate.Replace("{{shippingProvider}}", "Giao hàng nhanh");
                emailTemplate = emailTemplate.Replace("{{trackOrderLink}}", $"https://yourshop.com/order/track/{order.Id}");

                var productHtml = "";
                foreach (var item in cartItems)
                {
                    string imageUrl = item.Variant?.Product?.PrimaryImageUrl ?? "https://via.placeholder.com/70";
                    productHtml += $@"
                        <div style=""border: 1px solid #ddd; border-radius: 6px; margin-bottom: 10px; background-color: #fafafa; padding: 12px; display: flex; align-items: center;"">
                            <img src=""{imageUrl}"" alt=""{item.Variant?.Product?.ProductName ?? "Sản phẩm"}"" style=""width: 70px; height: 70px; border-radius: 4px; object-fit: cover; border: 1px solid #ddd;"" />
                            <div style=""flex: 1; padding-left: 15px;"">
                                <div style=""font-weight: bold; margin-bottom: 5px; color: #333; font-size: 16px;"">{item.Variant?.Product?.ProductName ?? "Sản phẩm"}</div>
                                <div style=""color: #666; font-size: 14px;"">Kích thước: {item.Variant?.Size?.Name ?? "N/A"} | Màu: {item.Variant?.Color?.Name ?? "N/A"} | Số lượng: {item.Quantity}</div>
                            </div>
                            <div style=""width: 100px; text-align: right; font-weight: bold; color: #4a90e2; font-size: 16px;"">{(item.TotalPrice):N0}₫</div>
                        </div>";
                }
                emailTemplate = emailTemplate.Replace("{{productList}}", productHtml);

                var subtotal = cartItems.Sum(item => item.TotalPrice);
                emailTemplate = emailTemplate.Replace("{{subtotal}}", $"{subtotal:N0}₫");
                emailTemplate = emailTemplate.Replace("{{shippingFee}}", "10.000₫");
                emailTemplate = emailTemplate.Replace("{{discount}}", "0₫");
                emailTemplate = emailTemplate.Replace("{{totalAmount}}", $"{order.TotalAmount:N0}₫");

                var email = User.Identity.IsAuthenticated ? model.Email : model.ShippingEmail;
                await _emailService.SendEmailAsync(email, "Đặt hàng thành công - Mã đơn hàng #" + order.Id.ToString("D8"), emailTemplate);
            }
            catch (Exception ex)
            {
                TempData["WarningMessage"] = "Đặt hàng thành công nhưng không thể gửi email xác nhận.";
            }
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

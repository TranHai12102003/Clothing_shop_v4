using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Clothing_shop_v2.Common.Constants;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.Utilities;
using Clothing_shop_v2.VModels;
using Clothing_shop_v2.VModels.Home;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClothingShopV3Context _context;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public HomeController(ILogger<HomeController> logger, ClothingShopV3Context context, 
            IEmailService emailService,IUserService userService,
            ICartService cartService, IOrderService orderService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _userService = userService;
            _cartService = cartService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            var model = new HomeVModel
            {
                //CÓ thể gọi hàm ở Product Service (sẽ dùng sau)
                Products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Variants)
                .AsEnumerable()
                .Select(p => ProductMapping.EntityToVModel(p))
                .ToList(),

                Categories = _context.Categories
                    //.Include(c=>c.ParentCategory)
                    .Select(c => new CategoryGetVModel
                    {
                        Id = c.Id,
                        CategoryName = c.CategoryName,
                        ImageUrl = c.ImageUrl,
                        ParentCategoryId = c.ParentCategoryId,
                        IsActive = c.IsActive,
                    }).ToList(),
                Banners = _context.Banners
                    .Select(b => new BannerGetVModel
                    {
                        Id = b.Id,
                        ImageUrl = b.ImageUrl,
                        LinkUrl = b.LinkUrl,
                        IsActive = b.IsActive,
                    }).ToList(),
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Categories = _context.Categories
            .Include(c => c.ParentCategory)
            .Select(c => CategoryMapping.EntityToVModel(c)).ToList();
            return View(new RegisterVModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVModel user)
        {
            if (ModelState.IsValid)
            {
                //// Kiểm tra xem tên người dùng đã tồn tại chưa
                //var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email);
                //if (existingUser != null)
                //{
                //    ModelState.AddModelError("", "Tên người dùng hoặc email đã tồn tại.");
                //    return View(user);
                //}
                //// Tạo người dùng mới
                //var savedUser =RegisterMapping.Register(user);
                //_context.Users.Add(savedUser);
                //_context.SaveChanges();
                //// Gửi email xác nhận
                //var emailContent = $"Chào {user.FullName},\n\nCảm ơn bạn đã đăng ký tài khoản trên trang web của chúng tôi. Vui lòng xác nhận địa chỉ email của bạn để hoàn tất quá trình đăng ký.\n\nTrân trọng,\nĐội ngũ hỗ trợ khách hàng.";
                //_emailService.SendEmailAsync(user.Email, "Xác nhận đăng ký tài khoản", emailContent);
                var result = _userService.RegisterUser(user);
                if (result.Result.Success)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", result.Result.Message);
                }
                //return RedirectToAction("Login");
            }
            return View(user);
        }

        // API kích hoạt tài khoản
        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery] string token)
        {
            var result = await _userService.ActivateAccount(token);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }

        public IActionResult Login()
        {
            ViewBag.Categories = _context.Categories
            .Include(c => c.ParentCategory)
            .Select(c => CategoryMapping.EntityToVModel(c))
            .ToList();
            return View(new LoginVModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Login(model);
                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                if (result.Token != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.UserID.ToString()),
                        new Claim(ClaimTypes.Name, result.FullName),
                        new Claim(ClaimTypes.Email, model.Email),
                        new Claim(ClaimTypes.Role, result.Role) // Role: Admin, Customer...
                    };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyCookieAuth", principal);
                    //Đồng bộ giỏ hàng
                    try
                    {
                        var cartCookie = Request.Cookies["Cart"];
                        if (!string.IsNullOrEmpty(cartCookie))
                        {
                            var cart = JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie);
                            foreach (var item in cart)
                            {
                                var cartModel = new CartCreateVModel
                                {
                                    UserId = result.UserID,
                                    VariantId = item.VariantId,
                                    Quantity = item.Quantity
                                };
                                await _cartService.Create(cartModel);
                            }
                            // Xóa cookie sau khi đồng bộ
                            Response.Cookies.Delete("Cart");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Đã xảy ra lỗi khi đồng bộ giỏ hàng. Vui lòng kiểm tra lại.");
                    }
                    // Điều hướng sau khi login
                    if (result.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (result.Role == "Staff")
                    {
                        return RedirectToAction("Index", "Staff");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // Đây là cái scheme mà bạn đặt lúc SignInAsync
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccountManagement() 
        { 
            return View();
        }

        public async Task<IActionResult> Profile([FromQuery] OrderFilterParams filterParams)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            filterParams.UserId = userId;
            filterParams.PageSize = 3; // Giới hạn số lượng đơn hàng hiển thị trên trang

            var result = await _orderService.GetAllByUser(filterParams);
            ViewBag.Status = filterParams.Status;
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive == true)
                .Select(c => CategoryMapping.EntityToVModel(c)).ToListAsync();
            ViewBag.UserInfo = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserUpdateVModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUser(model);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Profile");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction("Profile");
                }
            }

            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateUserAjax(UserUpdateVModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            model.Id = userId; // Đảm bảo Id của người dùng được cập nhật
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUser(model);
                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Message
                });
            }

            return Json(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ."
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

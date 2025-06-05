using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Controllers
{
    public class StaffController : Controller
    {
        [Authorize(Roles = "Staff")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

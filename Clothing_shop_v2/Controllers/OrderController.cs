using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<ActionResult<PaginationModel<OrderGetVModel>>> Index([FromQuery] OrderFilterParams parameters)
        {
            var response = await _orderService.GetAll(parameters);
            return View(response.Value);
        }
        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateVModel order)
        {
            var response = await _orderService.Create(order);
            return View();
        }
        [HttpGet]
        public async Task<ActionResult<OrderGetVModel>> Details(int id)
        {
            var response = await _orderService.GetById(id);
            if (response.Value == null)
            {
                return NotFound();
            }
            return View(response.Value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var response = await _orderService.UpdateStatus(id, status);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return BadRequest(response.Message);
        }
    }
}

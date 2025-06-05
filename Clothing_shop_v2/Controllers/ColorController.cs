﻿using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class ColorController : Controller
    {
        private readonly ILogger<ColorController> _logger;
        private readonly ClothingShopV3Context _context;
        private readonly IColorService _colorService;
        public ColorController(ILogger<ColorController> logger, ClothingShopV3Context context, IColorService colorService)
        {
            _logger = logger;
            _context = context;
            _colorService = colorService;
        }
        #region
        //public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        //{
        //    var query = _context.Colors.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        query = query.Where(s => s.ColorName.Contains(searchString) || s.ColorName.Contains(searchString));
        //    }

        //    var totalItems = await query.CountAsync();
        //    var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    pageNumber = Math.Max(1, pageNumber);
        //    pageNumber = Math.Min(pageNumber, totalPages > 0 ? totalPages : 1);

        //    var colors = await query
        //        .OrderBy(s => s.Id)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var viewModel = new ColorListViewModel
        //    {
        //        Colors = colors,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalPages = totalPages,
        //        TotalItems = totalItems,
        //        SearchString = searchString
        //    };
        //    return View(viewModel);
        //}
        #endregion
        public async Task<ActionResult<PaginationModel<ColorGetVModel>>> Index([FromQuery] ColorFilterParams parameters)
        {
            var response = await _colorService.GetAll(parameters);
            return View(response.Value);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ColorCreateVModel model)
        {
            #region
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        var newColor = ColorMapping.VModelToEntity(model);
            //        _context.Colors.Add(newColor);
            //        await _context.SaveChangesAsync();
            //        TempData["SuccessMessage"] = $"Kích thước '{model.ColorName}' đã được tạo thành công.";
            //        return RedirectToAction("Index");
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Lỗi khi tạo kích thước");
            //        TempData["ErrorMessage"] = "Không thể tạo kích thước. Vui lòng thử lại.";
            //    }
            //}
            //return View(model);
            #endregion
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var response = await _colorService.Create(model);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            var model = ColorMapping.EntityToVModel(color);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ColorUpdateVModel model)
        {
            #region
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        var color = await _context.Colors.FindAsync(model.Id);
            //        if (color == null)
            //        {
            //            return NotFound();
            //        }
            //        ColorMapping.VModelToEntity(model, color);
            //        _context.Colors.Update(color);
            //        await _context.SaveChangesAsync();
            //        TempData["SuccessMessage"] = $"Màu '{model.ColorName}' đã được cập nhật thành công.";
            //        return RedirectToAction("Index");
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Lỗi khi cập nhật màu sắc");
            //        TempData["ErrorMessage"] = "Không thể cập nhật màu sắc. Vui lòng thử lại.";
            //    }
            //}
            //return View(model);
            #endregion
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var response = await _colorService.Update(model);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            #region
            //var color = await _context.Colors.FindAsync(id);
            //if (color == null)
            //{
            //    return NotFound();
            //}
            //try
            //{
            //    _context.Colors.Remove(color);
            //    await _context.SaveChangesAsync();
            //    TempData["SuccessMessage"] = $"Màu '{color.ColorName}' đã được xóa thành công.";
            //    return RedirectToAction("Index");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Lỗi khi xóa màu sắc");
            //    TempData["ErrorMessage"] = "Không thể xóa màu sắc. Vui lòng thử lại.";
            //}
            //return View(color);
            #endregion
            var response = await _colorService.Delete(id);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, bool isActive =false)
        {
            var response = await _colorService.ToggleActive(id, isActive);
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

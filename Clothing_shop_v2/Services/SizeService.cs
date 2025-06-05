using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Services
{
    public class SizeService : ISizeService
    {
        private readonly ClothingShopV3Context _context;
        public SizeService(ClothingShopV3Context context)
        {
            _context = context;
        }
        public async Task<ResponseResult> Create(SizeCreateVModel size)
        {
            var response = new ResponseResult();
            try
            {
                var newSize = SizeMapping.VModelToEntity(size);
                _context.Sizes.Add(newSize);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(newSize, "Thêm kích thước mới thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }
        public async Task<ResponseResult> Delete(int id)
        {
            try
            {
                var size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);
                if (size == null)
                {
                    return new ErrorResponseResult("Không tìm thấy kích thước");
                }
                _context.Sizes.Remove(size);
                await _context.SaveChangesAsync();
                return new SuccessResponseResult(size, "Xóa kích thước thành công");
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Update(SizeUpdateVModel size)
        {
            var response = new ResponseResult();
            try
            {
                var entity = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == size.Id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy kích thước");
                }
                var updatedSize = SizeMapping.VModelToEntity(size,entity);
                _context.Sizes.Update(updatedSize);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Cập nhật kích thước thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<SizeGetVModel>> GetById(int id)
        {
            var size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);
            if (size == null)
            {
                return new NotFoundObjectResult("Không tìm thấy kích thước");
            }
            var sizeVModel = SizeMapping.EntityToVModel(size);
            return sizeVModel;
        }

        public async Task<ActionResult<PaginationModel<SizeGetVModel>>> GetAll(SizeFilterParams parameters)
        {
            IQueryable<Size> query = _context.Sizes.Where(BuildQueryable(parameters));
            var sizes = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => SizeMapping.EntityToVModel(x))
                .ToListAsync();
            var totalRecords = await query.CountAsync();
            var pagination = new PaginationModel<SizeGetVModel>
            {
                Records = sizes,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
            };
            return pagination;
        }

        private static Expression<Func<Size, bool>> BuildQueryable(SizeFilterParams fParams)
        {
            return x =>
                (fParams.SearchString == null || (x.SizeName != null && x.SizeName.Contains(fParams.SearchString))) &&
                (fParams.IsActive == null || x.IsActive == fParams.IsActive);
        }

        public async Task<ResponseResult> ToggleActive(int id, bool isActive = false)
        {
            var response = new ResponseResult();
            try
            {
                var size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);
                if (size == null)
                {
                    return new ErrorResponseResult("Không tìm thấy kích thước");
                }
                size.IsActive = isActive;
                _context.Sizes.Update(size);
                _context.SaveChanges();
                response = new SuccessResponseResult(size, "Cập nhật trạng thái kích thước thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }
    }
}

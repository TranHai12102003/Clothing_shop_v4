using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface ISizeService
    {
        Task<ActionResult<PaginationModel<SizeGetVModel>>> GetAll(SizeFilterParams parameters);
        Task<ActionResult<SizeGetVModel>> GetById(int id);
        Task<ResponseResult> Create(SizeCreateVModel size);
        Task<ResponseResult> Update(SizeUpdateVModel size);
        Task<ResponseResult> Delete(int id);
        Task<ResponseResult> ToggleActive(int id, bool isActive = false);
    }
}

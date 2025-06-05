using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IOrderService
    {
        public Task<ActionResult<PaginationModel<OrderGetVModel>>> GetAll(OrderFilterParams parameters);
        public Task<ActionResult<PaginationModel<OrderGetVModel>>> GetAllByUser(OrderFilterParams parameters);
        public Task<ActionResult<OrderGetVModel>> GetById(int id);
        public Task<ResponseResult> Create(OrderCreateVModel vModel);
        public Task<ResponseResult> Update(OrderUpdateVModel vModel);
        public Task<ResponseResult> Delete(int id);
        public Task<ResponseResult> UpdateStatus(int id, string status);
    }
}

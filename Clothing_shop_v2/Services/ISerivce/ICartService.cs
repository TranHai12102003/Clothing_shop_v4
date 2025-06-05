using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface ICartService
    {
        Task<List<CartGetVModel>> GetAll(int userId);
        Task<ResponseResult> Create(CartCreateVModel cartVModel);
        Task<ResponseResult> Update(CartUpdateVModel cartVModel);
        Task<ResponseResult> Delete(int id);
        Task<ResponseResult> DeleteByUserId(int userId);
        Task<ResponseResult> DeleteByVariantId(int variantId);
    }
}

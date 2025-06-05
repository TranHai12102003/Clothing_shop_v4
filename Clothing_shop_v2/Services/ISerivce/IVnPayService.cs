using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(Order model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}

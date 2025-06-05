using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class CartMapping
    {
        public static Cart VModelToEntity(CartCreateVModel cartVModel)
        {
            return new Cart
            {
                UserId = cartVModel.UserId,
                VariantId = cartVModel.VariantId,
                Quantity = cartVModel.Quantity,
                IsActive = true
            };
        }
        public static Cart VModelToEntity(CartUpdateVModel cartVModel)
        {
            return new Cart
            {
                Id = cartVModel.Id,
                UserId = cartVModel.UserId,
                VariantId = cartVModel.VariantId,
                Quantity = cartVModel.Quantity,
                UpdatedDate = DateTime.Now
            };
        }
        public static CartGetVModel EntityToGetVModel(Cart cart)
        {
            var model =  new CartGetVModel
            {
                Id = cart.Id,
                UserId = cart.UserId,
                VariantId = cart.VariantId,
                Quantity = cart.Quantity,
                TotalPrice = cart.Quantity * cart.Variant.Price,
            };
            if(cart.Variant != null)
            {
                model.Variant = VariantMapping.EntityGetVModel(cart.Variant);
            }
            return model;
        }
    }
}

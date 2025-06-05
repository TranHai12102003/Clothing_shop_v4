using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class OrderDetailMapping
    {
        public static OrderDetailGetVModel EntityToVModel(OrderDetail orderDetail)
        {
            var vmodel = new OrderDetailGetVModel
            {
                Id = orderDetail.Id,
                OrderId = orderDetail.OrderId,
                VariantId = orderDetail.VariantId,
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
                CreatedBy = orderDetail.CreatedBy,
                UpdatedBy = orderDetail.UpdatedBy,
                CreatedDate = orderDetail.CreatedDate,
                UpdatedDate = orderDetail.UpdatedDate
            };
            if(orderDetail.Variant != null)
            {
                vmodel.Variant = VariantMapping.EntityGetVModel(orderDetail.Variant);
            }
            return vmodel;
        }
        public static OrderDetail VModelToEntity(this OrderDetailCreateVModel vModel)
        {
            return new OrderDetail
            {
                OrderId = vModel.OrderId,
                VariantId = vModel.VariantId,
                Quantity = vModel.Quantity,
                UnitPrice = vModel.UnitPrice
            };
        }
        public static OrderDetail VModelToEntity(OrderDetailUpdateVModel vModel, OrderDetail orderDetail)
        {
            orderDetail.OrderId = vModel.OrderId;
            orderDetail.VariantId = vModel.VariantId;
            orderDetail.Quantity = vModel.Quantity;
            orderDetail.UnitPrice = vModel.UnitPrice;
            return orderDetail;
        }
    }
}

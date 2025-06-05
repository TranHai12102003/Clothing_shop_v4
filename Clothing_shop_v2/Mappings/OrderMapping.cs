using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class OrderMapping
    {
        public static OrderGetVModel EntityGetVModel(Order order)
        {
            var model = new OrderGetVModel
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                PaymentId = order.PaymentId,
                VoucherId = order.VoucherId,
                CreatedBy = order.CreatedBy,
                UpdatedBy = order.UpdatedBy,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                IsActive = order.IsActive
            };
            if(order.OrderDetails != null && order.OrderDetails.Count > 0)
            {
                model.OrderDetailGetVModel = order.OrderDetails.Select(x => OrderDetailMapping.EntityToVModel(x)).ToList();
            }
            if(order.User != null)
            {
                model.User = new UserGetVModel
                {
                    FullName = order.User.FullName,
                    Email = order.User.Email,
                    PhoneNumber = order.User.PhoneNumber,
                };
            }
            return model;
        }
        public static Order VModelToEntity(OrderCreateVModel orderCreateVModel)
        {
            var order = new Order
            {
                UserId = orderCreateVModel.UserId,
                GuestFullName = orderCreateVModel.GuestFullName,
                GuestEmail = orderCreateVModel.GuestEmail,
                GuestPhoneNumber = orderCreateVModel.GuestPhoneNumber,
                OrderDate = orderCreateVModel.OrderDate,
                TotalAmount = orderCreateVModel.TotalAmount,
                Status = orderCreateVModel.Status,
                ShippingAddress = orderCreateVModel.ShippingAddress,
                PaymentId = orderCreateVModel.PaymentId,
                VoucherId = orderCreateVModel.VoucherId,
                IsActive = orderCreateVModel.IsActive
            };
            return order;
        }
        public static Order VModelToEntity(OrderUpdateVModel orderUpdateVModel, Order order)
        {
            order.UserId = orderUpdateVModel.UserId;
            order.OrderDate = orderUpdateVModel.OrderDate;
            order.TotalAmount = orderUpdateVModel.TotalAmount;
            order.Status = orderUpdateVModel.Status;
            order.ShippingAddress = orderUpdateVModel.ShippingAddress;
            order.PaymentId = orderUpdateVModel.PaymentId;
            order.VoucherId = orderUpdateVModel.VoucherId;
            order.IsActive = orderUpdateVModel.IsActive;
            return order;
        }
    }
}

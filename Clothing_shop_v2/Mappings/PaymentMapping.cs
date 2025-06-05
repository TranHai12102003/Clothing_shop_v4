using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class PaymentMapping
    {
        public static PaymentGetVModel EntityToVModel(Payment payment)
        {
            return new PaymentGetVModel
            {
                Id = payment.Id,
                PaymentGateway = payment.PaymentGateway,
                TransactionId = payment.TransactionId,
                Amount = payment.Amount,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                CreatedBy = payment.CreatedBy,
                UpdatedBy = payment.UpdatedBy,
                CreatedDate = payment.CreatedDate,
                UpdatedDate = payment.UpdatedDate,
                IsActive = payment.IsActive
            };
        }
        public static Payment VModelToEntity(PaymentCreateVModel paymentVModel)
        {
            return new Payment
            {
                PaymentGateway = paymentVModel.PaymentGateway,
                TransactionId = paymentVModel.TransactionId,
                Amount = paymentVModel.Amount,
                PaymentStatus = paymentVModel.PaymentStatus,
                PaymentDate = paymentVModel.PaymentDate,
                PaymentMethod = paymentVModel.PaymentMethod,
                IsActive = paymentVModel.IsActive
            };
        }
        public static Payment VModelToEntity(PaymentUpdateVModel paymentVModel, Payment existingPayment)
        {
            if (existingPayment == null)
            {
                throw new ArgumentNullException(nameof(existingPayment));
            }
            existingPayment.PaymentGateway = paymentVModel.PaymentGateway;
            existingPayment.TransactionId = paymentVModel.TransactionId;
            existingPayment.Amount = paymentVModel.Amount;
            existingPayment.PaymentStatus = paymentVModel.PaymentStatus;
            existingPayment.PaymentDate = paymentVModel.PaymentDate;
            existingPayment.PaymentMethod = paymentVModel.PaymentMethod;
            existingPayment.IsActive = paymentVModel.IsActive;
            return existingPayment;
        }
    }
}

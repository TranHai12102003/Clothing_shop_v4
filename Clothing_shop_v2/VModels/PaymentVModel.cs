namespace Clothing_shop_v2.VModels
{
    public class PaymentCreateVModel
    {
        public string PaymentGateway { get; set; } = null!;

        public string? TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = null!;

        public DateTime PaymentDate { get; set; }

        public string? PaymentMethod { get; set; }
        public bool? IsActive { get; set; }
    }
    public class PaymentUpdateVModel : PaymentCreateVModel
    {
        public int Id { get; set; }
    }
    public class PaymentGetVModel : PaymentUpdateVModel
    {
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

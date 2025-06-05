namespace Clothing_shop_v2.VModels
{
    public class OrderDetailCreateVModel
    {
        public int OrderId { get; set; }

        public int VariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
    public class OrderDetailUpdateVModel : OrderDetailCreateVModel
    {
        public int Id { get; set; }
    }
    public class OrderDetailGetVModel : OrderDetailUpdateVModel
    {
        public VariantGetVModel Variant { get; set; } 
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

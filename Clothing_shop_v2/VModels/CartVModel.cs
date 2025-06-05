namespace Clothing_shop_v2.VModels
{
    public class CartCreateVModel
    {
        public int UserId { get; set; }

        public int VariantId { get; set; }

        public int Quantity { get; set; }
    }
    public class CartUpdateVModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VariantId { get; set; }
        public int Quantity { get; set; }
    }
    public class CartGetVModel : CartUpdateVModel
    {
        public decimal TotalPrice { get; set; }
        public VariantGetVModel Variant { get; set; }
    }

    public class CartFormVModel
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; } // Nếu người dùng đã đăng nhập
    }

}

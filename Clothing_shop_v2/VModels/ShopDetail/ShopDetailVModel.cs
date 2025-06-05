namespace Clothing_shop_v2.VModels.ShopDetail
{
    public class ShopDetailVModel
    {
        public ProductDetailVModel Product { get; set; } = null!;
        public List<ProductImageGetVModel> ProductImages { get; set; } = new List<ProductImageGetVModel>();
        public List<VariantGetVModel> Variants { get; set; } = new List<VariantGetVModel>();
        //public List<ProductGetVModel> RelatedProducts { get; set; } = new List<ProductGetVModel>();
        public List<CategoryGetVModel> Categories { get; set; }

    }
}

using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels.ShopDetail;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Mappings
{
    public static class ShopDetailMapping
    {
        public static ShopDetailVModel ToShopDetailVModel( Product product, List<ProductImage> productImages, List<Variant> variants, List<Product> relatedProducts, List<Category> categories)
        {
            return new ShopDetailVModel
            {
                Product = ProductMapping.EntityToDetailVModel(product),
                ProductImages = productImages.Select(pi => ProductImageMapping.EntityToVModel(pi)).ToList(),
                Variants = variants.Select(v => VariantMapping.EntityGetVModel(v)).ToList(),
                //RelatedProducts = relatedProducts.Select(rp => rp.ToProductGetVModel()).ToList(),
                Categories = categories.Select(c => CategoryMapping.EntityToVModel(c)).ToList()
            };
        }
    }
}

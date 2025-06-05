using Clothing_shop_v2.Common.Models;

namespace Clothing_shop_v2.VModels.Shop
{
    public class ShopVModel
    {
        public PaginationModel<ProductGetVModel> Products { get; set; }
        public List<CategoryGetVModel> Categories { get; set; }
        public List<ColorGetVModel> Colors { get; set; }
        public List<SizeGetVModel> Sizes { get; set; }
    }
}

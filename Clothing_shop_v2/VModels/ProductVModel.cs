using Clothing_shop_v2.Common.Constants;
using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.VModels
{
    public class ProductCreateVModel
    {
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public List<IFormFile>? imageFiles { get; set; }
        //public List<int> ColorIds { get; set; } = new List<int>();
        //public List<int> SizeIds { get; set; } = new List<int>();
    }
    public class ProductUpdateVModel : ProductCreateVModel
    {
        public int Id { get; set; }
    }
    public class ProductGetVModel : ProductUpdateVModel
    {
        public CategoryGetVModel Category { get; set; } = null!;
        public IdNameVModel Variant { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string PrimaryImageUrl { get; set; } // URL của ảnh chính
        public decimal Price { get; set; } // Giá sản phẩm
    }

    public class ProductDetailVModel : ProductGetVModel
    {
        public List<ProductImageGetVModel> ProductImages { get; set; } = new List<ProductImageGetVModel>();
        public List<VariantGetVModel> Variants { get; set; } = new List<VariantGetVModel>();
        //public List<CategoryGetVModel> Categories { get; set; } = new List<CategoryGetVModel>();
    }

    public class ProductListViewModel
    {
        public IEnumerable<ProductGetVModel> Products { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? SearchString { get; set; }
    }

    public class ProductFilterParams
    {
        public string? SearchString { get; set; }
        public bool? IsActive { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int[] SizeIds { get; set; } // Để lọc theo kích thước
        public int[] ColorIds { get; set; } // Để lọc theo màu sắc
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}

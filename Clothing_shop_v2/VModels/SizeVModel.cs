using Clothing_shop_v2.Common.Constants;
using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.VModels
{
    public class SizeCreateVModel
    {
        public string SizeName { get; set; } = null!;
    }
    public class SizeUpdateVModel : SizeCreateVModel
    {
        public int Id { get; set; }
        //public DateTime UpdatedDate { get; set; }

    }
    public class SizeGetVModel : SizeUpdateVModel
    {
        public List<IdNameVModel> Variants { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
    public class SizeListViewModel
    {
        public IEnumerable<Size> Sizes { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? SearchString { get; set; }
    }
    public class SizeFilterParams
    {
        public string? SearchString { get; set; }
        public bool? IsActive { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}

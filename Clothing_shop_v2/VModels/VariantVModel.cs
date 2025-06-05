﻿using System.ComponentModel.DataAnnotations;
using Clothing_shop_v2.Common.Constants;
using Clothing_shop_v2.Common.Contansts;

namespace Clothing_shop_v2.VModels
{
    public class VariantCreateVModel
    {
        public int ProductId { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn kích thước.")]
        public int SizeId { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn màu sắc.")]
        public int ColorId { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập giá.")]
        //[Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }
        //[Range(0, double.MaxValue, ErrorMessage = "Giá khuyến mãi phải lớn hơn hoặc bằng 0.")]
        public decimal? SalePrice { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập số lượng tồn.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0.")]
        public int QuantityInStock { get; set; }
    }
    public class VariantUpdateVModel : VariantCreateVModel
    {
        public int Id { get; set; }
    }

    public class VariantGetVModel : VariantUpdateVModel
    {
        public IdNameVModel Size { get; set; } = null!;
        public IdNameVModel Color { get; set; } = null!;
        public ProductGetVModel Product { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }


    public class VariantListViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int? ProductId { get; set; } // Để lọc theo sản phẩm
    }
    public class VariantFilterParams
    {
        //public string? SearchString { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? ProductId { get; set; } // Để lọc theo sản phẩm
        public int? SizeId { get; set; } // Để lọc theo kích thước
        public int? ColorId { get; set; } // Để lọc theo màu sắc
        public bool? IsActive { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}

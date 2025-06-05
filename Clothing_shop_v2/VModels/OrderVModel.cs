using Clothing_shop_v2.Common.Constants;

namespace Clothing_shop_v2.VModels
{
    public class OrderCreateVModel
    {
        public int? UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public string? ShippingAddress { get; set; }

        public int? PaymentId { get; set; }

        public int? VoucherId { get; set; }
        public bool? IsActive { get; set; }
    }
    public class  OrderUpdateVModel : OrderCreateVModel
    {
        public int Id { get; set; }
    }
    public class OrderGetVModel : OrderUpdateVModel
    {
        public UserGetVModel User { get; set; } = null!;    
        public List<OrderDetailGetVModel> OrderDetailGetVModel { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class OrderFilterParams
    {
        //public string? SearchString { get; set; }
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}


namespace Clothing_shop_v2.Common.Models
{
    public class AuditEntity
    {
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class BaseEntity : AuditEntity
    {
        public int Id { get; set; }
    }
}

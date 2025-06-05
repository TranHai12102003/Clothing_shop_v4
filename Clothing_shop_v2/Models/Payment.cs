using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string PaymentGateway { get; set; } = null!;

    public string? TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

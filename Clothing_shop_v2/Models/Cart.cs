using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}

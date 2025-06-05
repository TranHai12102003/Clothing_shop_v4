using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class VariantPromotion
{
    public int Id { get; set; }

    public int VariantId { get; set; }

    public int PromotionId { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Promotion Promotion { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Color
{
    public int Id { get; set; }

    public string ColorName { get; set; } = null!;

    public string? ColorCode { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}

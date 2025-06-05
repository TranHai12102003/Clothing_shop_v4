using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Size
{
    public int Id { get; set; }

    public string SizeName { get; set; } = null!;

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}

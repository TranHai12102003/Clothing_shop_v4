﻿using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? GuestFullName { get; set; }

    public string? GuestEmail { get; set; }

    public string? GuestPhoneNumber { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? ShippingAddress { get; set; }

    public int? PaymentId { get; set; }

    public int? VoucherId { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Payment? Payment { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}

using System;
using System.Collections.Generic;

namespace PR1.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime DeliveryTime { get; set; }

    public string DeliveryStreet { get; set; } = null!;

    public string DeliveryHouse { get; set; } = null!;

    public int? DeliveryApartment { get; set; }

    public decimal TotalCost { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}

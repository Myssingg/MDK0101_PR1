using System;
using System.Collections.Generic;

namespace PR1.Models;

public partial class Bouquet
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

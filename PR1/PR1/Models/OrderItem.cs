using System;
using System.Collections.Generic;

namespace PR1.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int BouquetId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Bouquet Bouquet { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}

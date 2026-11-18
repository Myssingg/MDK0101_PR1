using System;
using System.Collections.Generic;

namespace PR1.Models;

public partial class Basket
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BouquetId { get; set; }

    public int Quantity { get; set; }

    public virtual Bouquet Bouquet { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

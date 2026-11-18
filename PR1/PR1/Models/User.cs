using System;
using System.Collections.Generic;

namespace PR1.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public int? Apartment { get; set; }

    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

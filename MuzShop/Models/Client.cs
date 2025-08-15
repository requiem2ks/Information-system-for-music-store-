using MuzShop.Models;
using System;
using System.Collections.Generic;

namespace MuzShop;

public partial class Client
{
    public int Clientid { get; set; }

    public string? Fio { get; set; }

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Personaldiscount> Personaldiscounts { get; set; } = new List<Personaldiscount>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

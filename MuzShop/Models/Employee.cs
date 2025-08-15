using MuzShop.Models;
using System;
using System.Collections.Generic;

namespace MuzShop;

public partial class Employee
{
    public int Employeeid { get; set; }

    public int Jobtitle { get; set; }

    public string Fio { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual Jobtitle JobtitleNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

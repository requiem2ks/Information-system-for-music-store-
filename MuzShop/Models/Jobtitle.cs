using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Jobtitle
{
    public int Jobtitleid { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

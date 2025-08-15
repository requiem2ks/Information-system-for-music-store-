using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class EmployeeDTO
{
    public int Jobtitle { get; set; }

    public string Fio { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;
}

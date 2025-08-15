using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class InstrumentsetupDTO
{

    public int Productid { get; set; }

    public string Settingtype { get; set; } = null!;

    public string? Descriptionsetting { get; set; }

    public decimal Setupcost { get; set; }
}

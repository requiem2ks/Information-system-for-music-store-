using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public class RegisterClientDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Fio { get; set; }
    public string phone { get; set; }
}

using System;
using System.Collections.Generic;

namespace Exchange_appl.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Exchangeoffer> Exchangeoffers { get; set; } = new List<Exchangeoffer>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

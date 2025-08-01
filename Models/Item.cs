using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;

namespace Exchange_appl.Models;

public partial class Item
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Image { get; set; }
    public string? ExchangeCategory { get; set; }
    public string? AuthorPhone { get; set; }

    public virtual ICollection<Exchangeoffer> ExchangeofferItemOffereds { get; set; } = new List<Exchangeoffer>();

    public virtual ICollection<Exchangeoffer> ExchangeofferItemRequesteds { get; set; } = new List<Exchangeoffer>();

    public virtual User User { get; set; } = null!;
}

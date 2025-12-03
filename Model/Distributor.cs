using System;
using System.Collections.Generic;

namespace Model;

public partial class Distributor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ContactInfo { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}

using System;
using System.Collections.Generic;

namespace Model;

public partial class ShipmentItem
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public int BatchId { get; set; }

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public virtual Batch Batch { get; set; } = null!;

    public virtual Shipment Shipment { get; set; } = null!;
}

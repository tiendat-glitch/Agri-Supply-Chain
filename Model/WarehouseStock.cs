using System;
using System.Collections.Generic;

namespace Model;

public partial class WarehouseStock
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public int BatchId { get; set; }

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual Batch Batch { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}

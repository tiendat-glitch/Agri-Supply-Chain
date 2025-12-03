using System;
using System.Collections.Generic;

namespace Model;

public partial class RetailerStock
{
    public int Id { get; set; }

    public int RetailerId { get; set; }

    public int BatchId { get; set; }

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual Batch Batch { get; set; } = null!;

    public virtual Retailer Retailer { get; set; } = null!;
}

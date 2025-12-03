using System;
using System.Collections.Generic;

namespace Model;

public partial class Batch
{
    public int Id { get; set; }

    public string BatchCode { get; set; } = null!;

    public int ProductId { get; set; }

    public int FarmId { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateOnly? HarvestDate { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual Farm Farm { get; set; } = null!;

    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

    public virtual Product Product { get; set; } = null!;

    public virtual QrCode? QrCode { get; set; }

    public virtual ICollection<RetailerStock> RetailerStocks { get; set; } = new List<RetailerStock>();

    public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();

    public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
}

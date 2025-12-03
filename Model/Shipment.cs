using System;
using System.Collections.Generic;

namespace Model;

public partial class Shipment
{
    public int Id { get; set; }

    public string ShipmentCode { get; set; } = null!;

    public int? PlannedByUserId { get; set; }

    public int? DistributorId { get; set; }

    public string FromType { get; set; } = null!;

    public int? FromId { get; set; }

    public string ToType { get; set; } = null!;

    public int? ToId { get; set; }

    public string? VehicleInfo { get; set; }

    public string? DriverInfo { get; set; }

    public DateTime? DepartureTime { get; set; }

    public DateTime? ArrivalTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? Notes { get; set; }

    public virtual Distributor? Distributor { get; set; }

    public virtual User? PlannedByUser { get; set; }

    public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();
}

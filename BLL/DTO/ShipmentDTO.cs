namespace BLL.DTO
{
    public class ShipmentDTO
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
    }

    public class CreateShipmentDto
    {
        public string ShipmentCode { get; set; } = null!;
        public int? DistributorId { get; set; }
        public string FromType { get; set; } = null!;
        public int? FromId { get; set; }
        public string ToType { get; set; } = null!;
        public int? ToId { get; set; }
        public string? VehicleInfo { get; set; }
        public string? DriverInfo { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateShipmentDto
    {
        public int? DistributorId { get; set; }
        public string? VehicleInfo { get; set; }
        public string? DriverInfo { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string? Notes { get; set; }
    }

    public class ShipmentItemDTO
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public int BatchId { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
    }

    public class CreateShipmentItemDto
    {
        public int BatchId { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
    }
}


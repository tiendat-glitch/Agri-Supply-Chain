using System.ComponentModel.DataAnnotations;

namespace API_Adm.DTO
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateWarehouseDto
    {
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public bool IsActive { get; set; } = true;
    }


}

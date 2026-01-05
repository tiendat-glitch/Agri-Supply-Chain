namespace BLL.DTO
{
    public class FarmDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? OwnerName { get; set; }
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public string? Certifications { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFarmDto
    {
        public string Name { get; set; } = null!;
        public string? OwnerName { get; set; }
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public string? Certifications { get; set; }
    }

    public class UpdateFarmDto
    {
        public string Name { get; set; } = null!;
        public string? OwnerName { get; set; }
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public string? Certifications { get; set; }
    }
}



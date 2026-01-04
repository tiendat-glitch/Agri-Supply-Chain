namespace API_Adm.Models
{
    public class Farm
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? OwnerName { get; set; }

        public string? Location { get; set; }

        public string? ContactInfo { get; set; }

        public string? Certifications { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

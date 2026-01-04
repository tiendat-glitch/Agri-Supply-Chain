using System.ComponentModel.DataAnnotations;

namespace API_Adm.DTO
{
    public class CreateFarmRequest
    {

        [Required(ErrorMessage = "Tên farm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên farm tối đa 200 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? OwnerName { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string? ContactInfo { get; set; }

        [StringLength(200)]
        public string? Certifications { get; set; }

        }
}

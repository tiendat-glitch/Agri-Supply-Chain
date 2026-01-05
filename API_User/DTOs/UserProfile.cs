namespace API_User.DTOs
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
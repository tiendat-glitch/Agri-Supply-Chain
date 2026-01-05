namespace API_Auth.DTOs
{
    public class ResetPasswordRequest
    {
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
    }
}

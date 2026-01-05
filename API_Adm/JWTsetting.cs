namespace API_Adm.Settings
{
    /// <summary>
    /// Cấu hình JWT cho API_Adm – phải trùng với API_Auth / API_User về Key, Issuer, Audience.
    /// </summary>
    public class JWTsetting
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}



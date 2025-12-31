namespace QL_chuoi_cung_ung_nong_san
{
    public class JWTsetting
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireMinutes { get; set; }
    }
}

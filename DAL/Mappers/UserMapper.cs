using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Mappers
{
    public static class UserMapper
    {
        public static User Map(SqlDataReader reader, bool includePassword = true)
        {
            string passwordHash = null;
            if (includePassword)
                passwordHash = reader.GetString(reader.GetOrdinal("password_hash")); // chắc chắn NOT NULL

            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                PasswordHash = passwordHash,
                FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString(reader.GetOrdinal("full_name")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                PasswordResetToken = reader.IsDBNull(reader.GetOrdinal("password_reset_token")) ? null : reader.GetString(reader.GetOrdinal("password_reset_token")),
                PasswordResetExpiry = reader.IsDBNull(reader.GetOrdinal("password_reset_expiry")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("password_reset_expiry")),
                Role = reader.GetString(reader.GetOrdinal("role")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
}

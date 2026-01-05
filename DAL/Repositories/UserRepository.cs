using DAL.Helper;
using DAL.Mappers;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseHelper _dbHelper;
        public UserRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection conn = _dbHelper.GetConnection())
            using (SqlDataReader reader = _dbHelper.ExecuteStoredProcedure("GetAllUsers", conn, null))
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        PasswordHash = reader.GetString(2),
                        FullName = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Phone = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Role = reader.GetString(6),
                        CreatedAt = reader.GetDateTime(7)
                    });
                }
            }

            return users;
        }
        //Lấy người dùng theo username
        public User? GetUserByUsername(string username)
        {
            using var conn = _dbHelper.GetConnection();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Username", username)
            };

            using var reader = _dbHelper.ExecuteStoredProcedure("SP_Login", conn, parameters);
            if (!reader.Read()) return null;

            var user = UserMapper.Map(reader, includePassword: true);

            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new Exception("Mật khẩu chưa được đặt cho user này");

            return user;
        }

        //Đăng ký
        public int Register(User user)
        {
            using var conn = _dbHelper.GetConnection();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@FullName", user.FullName ?? (object)DBNull.Value),
                new SqlParameter("@Email", user.Email ?? (object)DBNull.Value),
                new SqlParameter("@Phone", user.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Role", user.Role)
            };

            return _dbHelper.ExecuteNonQueryStoredProcedure(
                "SP_RegisterUser", conn, parameters);
        }
        //
        public User? GetUserByEmail(string email)
        {
            using var conn = _dbHelper.GetConnection();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", email)
            };

            using var reader = _dbHelper.ExecuteStoredProcedure(
                "SP_GetUserByEmail", conn, parameters);

            if (!reader.Read()) return null;

            return UserMapper.Map(reader);
        }

        //Lấy thông tin người dùng theo Id
        public User? GetUserById(int userId, bool includePassword = true)
        {
            using var conn = _dbHelper.GetConnection();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            using var reader = _dbHelper.ExecuteStoredProcedure("GetUserById", conn, parameters);

            if (!reader.Read()) return null;

            return UserMapper.Map(reader, includePassword);
        }

        //Đổi mk
        public int UpdatePassword(int userId, string newPasswordHash)
        {
            using var conn = _dbHelper.GetConnection();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@NewPasswordHash", newPasswordHash)
            };

            return _dbHelper.ExecuteNonQueryStoredProcedure(
                "SP_ChangePassword", conn, parameters);
        }
        //Quên mk
        public void SetPasswordResetToken(int userId, string token, DateTime expiry)
        {
            using var conn = _dbHelper.GetConnection();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@PasswordResetToken", token),
                new SqlParameter("@PasswordResetExpiry", expiry)
            };
            _dbHelper.ExecuteStoredProcedure("SP_UpdateUser_ResetToken", conn, parameters);
        }

        //Reset mật khẩu = token
        public int ResetPassword(string token, string newPasswordHash)
        {
            using var conn = _dbHelper.GetConnection();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ResetToken", token),
                new SqlParameter("@NewPasswordHash", newPasswordHash)
            };
            return _dbHelper.ExecuteNonQueryStoredProcedure("SP_ResetPassword", conn, parameters);
        }
    }
}

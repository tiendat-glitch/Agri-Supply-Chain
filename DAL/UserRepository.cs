using DAL.Helper;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DAL
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


    }
}

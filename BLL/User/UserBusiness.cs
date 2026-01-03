using DAL.Helper;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UserBusiness
    {
        private readonly UserRepository dal;

        public UserBusiness(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection");
            var helper = new DatabaseHelper(connectionString);
            dal = new UserRepository(helper);
        }

        public List<User> GetAllUsers()
        {
            return dal.GetAllUsers();
        }
    }
}

using DAL;
using DAL.Helper;
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

        public UserBusiness(string connectionString)
        {
            DatabaseHelper dataHelper = new DatabaseHelper(connectionString);
            dal = new UserRepository(dataHelper);
        }

        public List<User> GetAllUsers()
        {
            return dal.GetAllUsers();
        }
    }
}

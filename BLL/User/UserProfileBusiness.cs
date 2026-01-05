using DAL.Helper;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Model;

namespace BLL
{
    public class UserProfileBusiness
    {
        private readonly UserRepository _userRepo;

        public UserProfileBusiness(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection");
            _userRepo = new UserRepository(new DatabaseHelper(cs));
        }

        public User GetProfile(int userId)
        {
            return _userRepo.GetUserById(userId, includePassword: false)
                   ?? throw new Exception("User không tồn tại");
        }
    }
}
using DAL.Helper;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Model;

namespace BLL
{
    public class UserBusiness
    {
        private readonly UserRepository _userRepo;

        public UserBusiness(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection");
            var helper = new DatabaseHelper(connectionString);
            _userRepo = new UserRepository(helper);
        }

        // login
        public User Login(string username, string password)
        {
            var user = _userRepo.GetUserByUsername(username)
                       ?? throw new Exception("User không tồn tại");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Sai mật khẩu");

            return user; // API sẽ tạo JWT
        }

        // đăng ký
        public void Register(User user, string password)
        {
            if (_userRepo.GetUserByUsername(user.Username) != null)
                throw new Exception("Username đã tồn tại");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;

            _userRepo.Register(user);
        }

        // đổi mk
        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userRepo.GetUserById(userId)
                       ?? throw new Exception("User không tồn tại");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                throw new Exception("Mật khẩu cũ không đúng");

            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _userRepo.UpdatePassword(userId, newHash);
        }

        // quên mk
        public void ForgotPassword(string email)
        {
            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddMinutes(15);

            _userRepo.SetPasswordResetToken(email, token, expiry);
        }

        // reset = token
        public void ResetPassword(string resetToken, string newPassword)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            int result = _userRepo.ResetPassword(resetToken, hash);

            if (result == 0)
                throw new Exception("Token không hợp lệ hoặc đã hết hạn");
        }

        // thông tin người dùng
        public User GetProfile(int userId)
        {
            return _userRepo.GetUserById(userId)
                   ?? throw new Exception("User không tồn tại");
        }
    }
}

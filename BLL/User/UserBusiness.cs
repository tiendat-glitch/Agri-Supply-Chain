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

            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new Exception("Mật khẩu chưa được đặt");
          
            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!valid)
                throw new Exception("Sai mật khẩu");

            return user;
        }

        // đăng ký
        public void Register(User user, string password)
        {
            if (_userRepo.GetUserByUsername(user.Username) != null) 
                throw new Exception("Username đã tồn tại"); 
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.Role = null;
            _userRepo.Register(user);
        }

        // đổi mk
        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userRepo.GetUserById(userId, includePassword: true)
                ?? throw new Exception("User không tồn tại");

            if (!string.IsNullOrEmpty(user.PasswordResetToken))
                Console.WriteLine("Token reset tồn tại, sẽ check logic reset password");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                throw new Exception("Mật khẩu cũ không đúng");

            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _userRepo.UpdatePassword(userId, newHash);
        }


        // quên mk
        public string ForgotPassword(string email)
        {
            // Lấy user theo email
            var user = _userRepo.GetUserByEmail(email)
                       ?? throw new Exception("Email không tồn tại");

            // Tạo token mới
            string token = Guid.NewGuid().ToString("N");
            DateTime expiry = DateTime.UtcNow.AddMinutes(15);

            // Lưu token vào DB
            _userRepo.SetPasswordResetToken(user.Id, token, expiry);

            // Trả token ra controller
            return token;
        }

        // reset = token
        public void ResetPassword(string token, string newPassword)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            int rowsAffected = _userRepo.ResetPassword(token, hash);

            if (rowsAffected == 0)
                throw new Exception("Token không hợp lệ hoặc đã hết hạn");
        }

        // thông tin người dùng
        public User GetUser(int userId, bool includePassword = false)
        {
            return _userRepo.GetUserById(userId, includePassword)
                   ?? throw new Exception("User không tồn tại");
        }
    }
}

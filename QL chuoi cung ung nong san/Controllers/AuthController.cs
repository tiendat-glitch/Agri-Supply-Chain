using API_Auth.DTOs;
using API_Auth.Services;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Auth.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserBusiness _userBiz;
        private readonly JwtService _jwtService;

        public AuthController(UserBusiness userBiz, JwtService jwtService)
        {
            _userBiz = userBiz;
            _jwtService = jwtService;
        }

        // ===================== LOGIN =====================
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _userBiz.Login(req.Username, req.Password);
            if (user == null)
                return Unauthorized("Sai username hoặc password");

            var token = _jwtService.Generate(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Role
                }
            });
        }

        // ===================== REGISTER =====================
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            _userBiz.Register(req);
            return Ok("Đăng ký thành công");
        }

        // ===================== CHANGE PASSWORD (LOGIN) =====================
        [Authorize]
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest req)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var success = _userBiz.ChangePasswordWithOldPassword(
                userId, req.OldPassword, req.NewPassword);

            if (!success)
                return BadRequest("Mật khẩu cũ không đúng");

            return Ok("Đổi mật khẩu thành công");
        }

        // ===================== FORGOT PASSWORD =====================
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            var token = _userBiz.CreateResetTokenByUsername(req.Username);
            if (token == null)
                return NotFound("User không tồn tại");

            // Demo: trả token luôn (thực tế gửi email)
            return Ok(new { resetToken = token });
        }

        // ===================== RESET PASSWORD =====================
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest req)
        {
            var success = _userBiz.ResetPasswordByToken(
                req.Token, req.NewPassword);

            if (!success)
                return BadRequest("Token không hợp lệ hoặc đã hết hạn");

            return Ok("Reset mật khẩu thành công");
        }
    }
}

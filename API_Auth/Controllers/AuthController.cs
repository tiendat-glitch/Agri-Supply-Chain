using API_Auth.DTOs;
using API_Auth.Services;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Security.Claims;

namespace API_Auth.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserBusiness _userBusiness;
        private readonly JwtTokenService _jwtService;

        public AuthController(
            UserBusiness userBusiness,
            JwtTokenService jwtService)
        {
            _userBusiness = userBusiness;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username))
                    throw new Exception("Username không được để trống");
                if (string.IsNullOrEmpty(request.Password))
                    throw new Exception("Password không được để trống");

                var user = _userBusiness.Login(request.Username, request.Password);

                var token = _jwtService.GenerateToken(user);

                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập thành công",
                    data = new
                    {
                        token,
                        user = new
                        {
                            user.Id,
                            user.Username,
                            user.FullName,
                            user.Email,
                            user.Phone,
                            user.Role
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.ToString() // stack trace đầy đủ
                });
            }
        }


        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            try
            {
                var user = new User
                {
                    Username = request.Username,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone
                };

                _userBusiness.Register(user, request.Password);

                return Ok(new
                {
                    success = true,
                    message = "Đăng ký thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var user = _userBusiness.GetUser(userId);

                _userBusiness.ChangePassword(userId, request.OldPassword, request.NewPassword);

                return Ok(new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var token = _userBusiness.ForgotPassword(request.Email);
                return Ok(new
                {
                    success = true,
                    message = "Đã tạo token reset mật khẩu",
                    data = new
                    {
                        reset_token = token
                    }
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                _userBusiness.ResetPassword(
                    request.ResetToken,
                    request.NewPassword);

                return Ok(new
                {
                    success = true,
                    message = "Reset mật khẩu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}

using API_User.DTOs;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_User.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize] // áp dụng cho toàn controller
    public class UserController : ControllerBase
    {
        private readonly UserProfileBusiness _profileBusiness;

        public UserController(UserProfileBusiness profileBusiness)
        {
            _profileBusiness = profileBusiness;
        }

        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Token không hợp lệ");

            int userId = int.Parse(userIdClaim);

            var user = _profileBusiness.GetProfile(userId);

            var dto = new UserProfile
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return Ok(dto);
        }
    }
}
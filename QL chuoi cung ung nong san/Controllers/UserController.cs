using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using static BLL.UserBusiness;

namespace QL_chuoi_cung_ung_nong_san.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserBusiness bll;
        public UserController(UserBusiness service)
        {
            bll = service;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(bll.GetAllUsers());
        }
    }
}

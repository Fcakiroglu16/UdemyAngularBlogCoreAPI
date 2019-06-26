using Microsoft.AspNetCore.Mvc;
using UdemyAngularBlogCore.API.Models;

namespace UdemyAngularBlogCore.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult IsAuthenticated(AdminUser adminUser)
        {
            bool status = false;

            if (adminUser.Email == "f@outlook.com" && adminUser.Password == "1234")
            {
                status = true;
            }

            var result = new
            {
                status = status
            };
            return Ok(result);
        }
    }
}
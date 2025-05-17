using Microsoft.AspNetCore.Mvc;
using OCR.BusinessService;
using OCR.Model;
using static Paket.CredentialProviderResult;

namespace OCR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserBusinessService _businessService;

        public UserController(UserBusinessService businessService)
        {
            _businessService = businessService;
        }

        [HttpPost("userregister")]
        public IActionResult Register([FromBody] UserModel user)
        {
            int result = _businessService.UserRegister(user);
            //if (result==)

            return Ok(new { success = true });

            /*if (_businessService.UserRegister(user))
                //return Ok(Success = true);
                return Ok("User registered successfully");
            else
                return BadRequest("Registration failed");*/
        }

        [HttpPost("userlogin")]
        public IActionResult Login([FromBody] UserModel user)
        {
            var result = _businessService.UserLogin(user.Email, user.Password);
            if (result != null)
                return Ok(new { result.UserId});
            else
                return Unauthorized("Invalid credentials");
        }
    }

}

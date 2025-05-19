using Microsoft.AspNetCore.Identity.Data;
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
        public IActionResult Register([FromBody] LoginRequestModel request)
        {
            var result = _businessService.UserRegister(request);
            

            if (result.StatusCode == 1)
                return Ok(new { success = true, message = "User registered successfully" });
            else
                return BadRequest(new { success = false, message = result.ErrorMessage });

            /*if (_businessService.UserRegister(user))
                //return Ok(Success = true);
                return Ok("User registered successfully");
            else
                return BadRequest("Registration failed");*/
        }

        [HttpPost("userlogin")]
        public IActionResult Login([FromBody] LoginRequestModel LoginRequest)
        {
            var result = _businessService.UserLogin(LoginRequest.Email, LoginRequest.Password);
            if (result != null)
                return Ok(new { result.UserId, result.StatusCode, result.ErrorMessage});
            else
                return Unauthorized("Invalid credentials");
        }
    }

}

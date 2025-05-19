using OCR.Model;
using OCR.Service;

namespace OCR.BusinessService
{
    public class UserBusinessService
    {
        private readonly UserService _service;

        public UserBusinessService(UserService service)
        {
            _service = service;
        }
        public LoginResponseModel UserRegister(LoginRequestModel request)
        {
            var user = new UserModel
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var result = _service.UserRegister(user);

            return new LoginResponseModel
            {
                UserId = result.UserId,
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage
            };
        }

        public UserModel UserRegister(UserModel user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password); // Hashing
            return _service.UserRegister(user);
        }
        public UserModel UserLogin(string email,string password)
        {
            var user = _service.UserLogin(email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
    }

}

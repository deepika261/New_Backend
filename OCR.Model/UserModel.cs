using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Model
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Plain password for signup/login only
        public string PasswordHash { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } // Store hashed
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Model
{
    public class LoginResponseModel
    {
        public int UserId { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}

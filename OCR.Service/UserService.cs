using OCR.Model;
using OCR.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Service
{
    public class UserService
    {
        private readonly UserProvider _provider;

        public UserService(UserProvider provider)
        {
            _provider = provider;
        }

        public UserModel UserRegister(UserModel user) => _provider.UserRegister(user);


        public UserModel UserLogin(string email) => _provider.UserLogin(email);


    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Profile
{
    public class UserAccount
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Person { get; set; }
        public string PersonPassword { get; set; }

        public UserAccount()
        {
        }

        public UserAccount(string userName, string password, string person, string personPassword)
            : this()
        {
            this.UserName = userName;
            this.Password = password;
            this.Person = person;
            this.PersonPassword = personPassword;
        }
    }
}

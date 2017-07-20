using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.IO;

namespace FMWW.Entity
{
    public class UserAccount
    {
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "person")]
        public string Person { get; set; }

        [DataMember(Name = "personPassword")]
        public string PersonPassword { get; set; }
    }
}

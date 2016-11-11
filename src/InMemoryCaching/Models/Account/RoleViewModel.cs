using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.Models.Account
{
    public class RoleViewModel
    {
        public string UserId { get; set; }

        public string EmailAddress { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Role> Roles { get; set; }
        public List<LoginInfo> users { get; set; }

        //public int RoleId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.Models.Account
{
    public class Role
    {

        [Key]
        public int RoleId { get; set; }
        public string Rolename { get; set; }
        public LoginInfo LoginInfo { get; set; }

    }
}

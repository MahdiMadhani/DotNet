using InMemoryCaching.App_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        public string ReturnUrl { get; set; } = null;
       // public string Rolename { get; set; } = "user";
        internal async Task<ResultBundle> Login(Models.DALContext context)
        {
            ResultBundle r = ResultBundle.Success();
            r.UserData = context.Login(EmailAddress, Password);
            r.IsSuccessful = r.UserData != null;
            
            return r;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.Models.Account
{
    [Table("tblLogins")]
    public class LoginInfo
    {
        public LoginInfo()
        {
        }

        [Key]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [StringLength(80)]
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [StringLength(80)]
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }


        [StringLength(50)]
        [Required(AllowEmptyStrings = true)]
        public string EmailConfirmationKey { get; set; } = "";
        
        public bool? EmailConfirmed { get; set; } = false;

        public virtual List<Role> Role { get; set; }
        public int RoleId { get; set; }

    }
}

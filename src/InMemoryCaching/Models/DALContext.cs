using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using InMemoryCaching.App_Code;
using Microsoft.Data.Entity.Metadata;
using InMemoryCaching.Models.Account;

namespace InMemoryCaching.Models
{
    public class DALContext:DbContext
    {
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // optionsBuilder.UseSqlServer(@"Server = localhost; Database = img; Trusted_Connection = True; MultipleActiveResultSets = True;");

            // optionsBuilder.UseSqlServer(@"Server = localhost; Database = img; Trusted_Connection = True; MultipleActiveResultSets = True");
            // "Server = localhost; Database = img; Trusted_Connection = True; MultipleActiveResultSets = True;");

        }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photoes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Models.Account.LoginInfo> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        #region Helpers
        internal Models.Account.LoginInfo SearchForUserByEmailAddress(string emailAddress)
        {
            return (from u in Users
                    where u.EmailAddress == emailAddress.Trim()
                    select u).FirstOrDefault();
        }
        internal Models.Account.LoginInfo Login(string emailAddress, string password)
        {
            var user = (from u in Users
                        where u.EmailAddress == emailAddress && Utils.VerifyPassword(u.Password, password) && u.EmailConfirmed==true
                        select u).FirstOrDefault();
            if (user != null)
            {
               
                return user;
            }

            return null;
        }
        internal Models.Account.LoginInfo SearchForUserByConfirmationKey(string key)
        {
            var user = (from u in Users
                        where u.EmailConfirmationKey == key
                        select u).FirstOrDefault();
            return user;
        }
        #endregion
    }
}

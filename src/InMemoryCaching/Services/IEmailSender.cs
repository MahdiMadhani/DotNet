using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.Services
{
   public interface IEmailSender
    {
        Task<bool> SendEmailConfirmationForm(string emailAddress, string linkUrl);
        Task<bool> SendRegisterConfirmation(string emailAddress);
    }
}

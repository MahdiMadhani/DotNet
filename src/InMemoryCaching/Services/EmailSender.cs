using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InMemoryCaching.Services
{
    public class EmailSender:IEmailSender
    {
        public async Task<bool> SendEmailConfirmationForm(string emailAddress, string linkUrl)
        {
            var credential = new NetworkCredential
            {
                UserName = "ensw@ensw.servicefars.ir", 
                Password = "2008815132asd~"// 
            };

            string body = string.Format(@"<div style='border:solid 1px #DDDDDD;width:90%;background:#ffef8c;font-family:Tahoma;'>
                                        <div lang='fa' dir='rtl' align='right'>
                                            <p>
                                                سلام،
                                            </p>
                                            <p>
                                                از عضویت شما متشکریم
                                                <br />
                                                لطفا جهت فعال سازی اشتراک خود و استفاده از کلیه امکانات سایت، با کلیک برروی لینک زیر اکانت خود را فعال نمایید.
                                            </p>
                                            <div lang='en' dir='ltr' align='left'>
                                                <a href='{0}' target='_blank'>{0}</a>
                                            </div>
                                            <hr />
                                            <p><span lang='fa' dir='rtl' style='color:#600;'> این ایمیل بصورت خودکار برای شما ارسال شده است. لطفا به آن پاسخ ندهید. </span></p>
                                        </div>
                                    </div>", linkUrl);

            return await SendEmailAsync("ensw@ensw.servicefars.ir", credential, new string[] { emailAddress }, "Noreplay, Subscription confirmation", body);
        }

        public async Task<bool> SendRegisterConfirmation(string emailAddress)
        {
            var credential = new NetworkCredential
            {
                UserName = "ensw@ensw.servicefars.ir", 
                Password = "2008815132asd~"
            };

            string body = string.Format(@"<div style='border:solid 1px #DDDDDD;width:90%;background:#ffef8c;font-family:Tahoma;'>
                                        <div lang='fa' dir='rtl' align='right'>
                                            <p>
                                                سلام،
                                            </p>
                                            <p>
                                                از عضویت شما متشکریم
                                                <br />
                                                اکانت شما فعال شد
                                            </p>

                                            <hr />
                                            <p><span lang='fa' dir='rtl' style='color:#600;'> این ایمیل بصورت خودکار برای شما ارسال شده است. لطفا به آن پاسخ ندهید. </span></p>
                                        </div>
                                    </div>");

            return await SendEmailAsync("ensw@ensw.servicefars.ir", credential, new string[] { emailAddress }, "Noreplay, Subscription confirmation", body);
        }
        private async Task<bool> SendEmailAsync(string from, NetworkCredential credential, string[] to, string subject, string body)
        {

            var message = new MailMessage();
            to.ToList().ForEach(x => message.To.Add(new MailAddress(x)));
            message.From = new MailAddress(from);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Credentials = credential;
                smtp.Host = "mx1.servicefars.ir";
                smtp.Port =  587;    // or 25
                smtp.EnableSsl = false;
                try
                {
                    await smtp.SendMailAsync(message);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

    }
}

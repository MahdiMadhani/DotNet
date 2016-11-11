using InMemoryCaching.App_Code;
using InMemoryCaching.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InMemoryCaching.Models.Account
{
    public class RegisterationViewModel
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }
        [Required]
        public string CaptchaCode { get; set; }
        
      //  private int RoleId { get; set; } 
        internal async static Task<bool> ValidateUserReuest(string key, Models.DALContext context, IEmailSender emailSender)
        {
            Guid g;
            if (key != null && Guid.TryParse(key, out g))
            {
                var user = context.SearchForUserByConfirmationKey(key);
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    context.SaveChanges();
                    await emailSender.SendRegisterConfirmation(user.EmailAddress);
                    return true;
                }
            }
            return false;
        }
        internal string GenerateCaptchaCode(out string imgSrcData)
        {
            Bitmap objBMP = new System.Drawing.Bitmap(200, 60);
            Graphics objGraphics = System.Drawing.Graphics.FromImage(objBMP);

            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            //' Configure font to use for text
            Font objFont = new Font("Arial", 32, FontStyle.Bold);
            string randomStr = "";
            int[] myIntArray = new int[5];
            int x;

            //That is to create the random # and add it to our string
            Random autoRand = new Random();

            for (x = 0; x < 5; x++)
            {
                myIntArray[x] = System.Convert.ToInt32(autoRand.Next(0, 9));
                randomStr += (myIntArray[x].ToString());
            }

            //This is to add the string to session cookie, to be compared later
            //Session.Add("randomStr", randomStr);

            //' Write out the text
            objGraphics.Clear(Color.LightGray);
            HatchBrush hb = new HatchBrush(HatchStyle.Sphere, Color.Black, Color.LightGray);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            // objGraphics.RotateTransform(10);
            objGraphics.DrawString(randomStr, objFont, hb, new RectangleF(0, 0, objBMP.Width, objBMP.Height), sf);
            objGraphics.DrawLine(Pens.Black, 0, 0, objBMP.Width, objBMP.Height - 1);
            objGraphics.DrawRectangle(Pens.Black, 0, 0, objBMP.Width - 1, objBMP.Height - 1);

            //' Set the content type and return the image
            MemoryStream ms = new MemoryStream();
            objBMP.Save(ms, ImageFormat.Gif);
            var base64Data = Convert.ToBase64String(ms.ToArray());
            imgSrcData = "data:image/gif;base64," + base64Data;

            ms.Dispose();
            objFont.Dispose();
            objGraphics.Dispose();
            objBMP.Dispose();

            return randomStr;
        }

        internal ResultBundle Validate(string originalCaptchaCode)
        {
            var result = ResultBundle.Success();

            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                result.AddMessage("فیلد نام و نام خانوادگی نمیتواند خالی باشد.");
                result.IsSuccessful = false;
            }

            if (string.IsNullOrEmpty(Password) || (Password != PasswordConfirm))
            {
                result.AddMessage("رمز عبور و تایید آن باید مشابه یکدیگر باشند. لطفا مجددا رمز ورود را وارد نمایید.");
                result.IsSuccessful = false;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(EmailAddress);
            if (!match.Success)
            {
                result.AddMessage("آدرس ایمیل وارد شده معتبر نمی باشد. لطفا آدرس ایمیل را مجددا بررسی نمایید.");
                result.IsSuccessful = false;
            }

            if (originalCaptchaCode != null && originalCaptchaCode != CaptchaCode)
            {
                result.AddMessage("کد تصویری صحیح نمی باشد لطفا مجددا تلاش نمایید");
                result.IsSuccessful = false;
            }

            return result;
        }

        internal async Task<ResultBundle> RegisterUser(string originalCaptcha, DALContext context, IEmailSender _emailSender, string host)
        {

            var r = Validate(originalCaptcha);
            //  have to del session for Captcha   http://www.dotnettips.info/post/1809/%d8%a7%db%8c%d8%ac%d8%a7%d8%af-%da%a9%d9%be%da%86%d8%a7%db%8c%db%8c-captcha-%d8%b3%d8%b1%db%8c%d8%b9-%d9%88-%d8%b3%d8%a7%d8%af%d9%87-%d8%af%d8%b1-asp-net-mvc-5
            if (r.IsSuccessful)
            {
                var user = context.SearchForUserByEmailAddress(EmailAddress);

                if (user != null)
                {
                    r.AddMessage("آدرس ایمیل وارد شده قبلا برای کاربر دیگری ثبت شده است. اگر رمز عبور خود را فراموش کرده اید از طریق صفحه (ورود) اقدام به بازیابی آن نمایید.");
                    r.IsSuccessful = false;
                }
                else
                {

                    string key = Guid.NewGuid().ToString();

                    string keyUrl = string.Format("{0}/Account/ConfirmEmailAddress/{1}", host, key);
                    bool emailSent = await _emailSender.SendEmailConfirmationForm(EmailAddress, keyUrl);
                    if (!emailSent)
                    {
                        r.AddMessage("آدرس ایمیل وارد شده معتبر نمی باشد. امکان ثبت نام با این ایمیل وجود ندارد.");
                        r.IsSuccessful = false;

                    }
                    else
                    {
                        string hashPassword = Utils.GenerateHashPassword(Password);
                        if (!await AddLogin(context, hashPassword, key))
                        {
                            r.AddMessage("خطایی در هنگام ذخیره اطلاعات شما در دیتابیس رخ داده است لطفا مجددا تلاش کنید.");
                            r.IsSuccessful = false;
                        }
                        else {
                            // Registeration is accepted
                            r.UserData = context.Login(EmailAddress, hashPassword);
                        }
                    }

                }
            }

            return r;

        }

        private async Task<bool> AddLogin(DALContext context, string password, string confirmationKey)
        {
            try
            {
                if (context.SearchForUserByEmailAddress(EmailAddress) != null)
                    return false;
                int RoleId = context.Roles.FirstOrDefault(x => x.Rolename == "user").RoleId;
                // add login
                var user = new Models.Account.LoginInfo()
                {
                    UserId = Guid.NewGuid().ToString(),                  
                    EmailAddress = EmailAddress,
                    Password = password,
                    FirstName = FirstName,
                    LastName = LastName,
                    EmailConfirmationKey = confirmationKey,
                    EmailConfirmed = false,
                    RoleId=RoleId,
                  
                };
                context.Users.Add(user);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}

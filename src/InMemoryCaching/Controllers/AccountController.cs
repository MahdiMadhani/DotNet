using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using InMemoryCaching.Services;
using InMemoryCaching.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using InMemoryCaching.App_Code;
using InMemoryCaching.Models.Account;
using System.Collections;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace InMemoryCaching.Controllers
{
    public class AccountController : Controller
    {
        private readonly SessionManager _session;
        private readonly DALContext _db;
        private readonly Services.IEmailSender _emailSender;

        public AccountController(SessionManager session, DALContext db, Services.IEmailSender emailSender)
        {
            _session = session;
            _db = db;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Index()
        {

            return View();

        }
        public IActionResult Registeration()
        {
            try
            {
                Models.Account.RegisterationViewModel model = new Models.Account.RegisterationViewModel();
                string imgUrl;
                _session.CaptchaCode = model.GenerateCaptchaCode(out imgUrl);
                ViewBag.CaptchaImageAccountController = imgUrl;
               // ViewBag.RoleId = _db.Roles.FirstOrDefault(x => x.Rolename == "user").RoleId;
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registeration(RegisterationViewModel model)
        {
            try
            {
                ViewBag.ErrorOccured = true;

                if (ModelState.IsValid)
               {
                    string originalCaptcha;
                    originalCaptcha = _session.CaptchaCode;
                    var registerResult = await model.RegisterUser(originalCaptcha, _db, _emailSender, ((Microsoft.AspNet.Http.HttpRequest)Request).Host.ToString());
                    if (registerResult.IsSuccessful)
                    {
                        ViewBag.ErrorOccured = false;
                    }
                    else
                    {
                        ViewBag.HtmlFormattedMessage = registerResult.FormattedMessages;
                    }


                    if (ViewBag.ErrorOccured)
                    {
                        string imgUrl;
                        _session.CaptchaCode = model.GenerateCaptchaCode(out imgUrl);
                        ViewBag.CaptchaImageAccountController = imgUrl;
                        model.CaptchaCode = "";
                        return View(model);
                    }
                    else
                        return RedirectToAction("RegisterationCallback", "Account");
                }

                else
                {
                    string imgUrl;
                    _session.CaptchaCode = model.GenerateCaptchaCode(out imgUrl);
                    ViewBag.CaptchaImageAccountController = imgUrl;
                    model.CaptchaCode = "";
                    ViewBag.RoleId = _db.Roles.FirstOrDefault(x => x.Rolename == "user").RoleId;

                    ViewBag.HtmlFormattedMessage = "لطفا تمام قسمت ها را وارد نماید";
                    return View(model);
                }
               // return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        // this function is called according to that link in registration email
        [HttpGet("[controller]/[action]/{key}")]
        public async Task<IActionResult> ConfirmEmailAddress(string key)
        {
            try
            {
                if (await Models.Account.RegisterationViewModel.ValidateUserReuest(key, _db, _emailSender))
                    return RedirectToAction("RegisterationSuccessful", "Account");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }
        [HttpGet]
        public IActionResult RegisterationCallback()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegisterationSuccessful()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            try
            {
                var model = new Models.Account.LoginViewModel { ReturnUrl = returnUrl };
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Models.Account.LoginViewModel model)
        {
            try
            {
                ResultBundle result = ResultBundle.Failed();

                if (ModelState.IsValid)
                {
                    result = await model.Login(_db);

                    if (result.IsSuccessful)
                    {

                        var selectedProperty = from property in result.UserData.GetType().GetProperties()
                                               where property.Name == "RoleId"
                                               select property.GetValue(result.UserData, null);
                        int s = 0;
                        foreach (var file in selectedProperty)
                        {
                            s = (Int32)file;
                        }


                        string Rolename = _db.Roles.FirstOrDefault(x => x.RoleId == s).Rolename;




                        var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.EmailAddress),
                        new Claim(ClaimTypes.Role, Rolename)
                    };

                        var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "local"));

                        await HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance4", principal);

                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index", "Home");
                        }

                    }
                    else
                    {
                        result.AddMessage("نام کاربری یا رمز عبور صحیح نمی باشد، لطفا مجددا تلاش نمایید");
                    }
                }
                else
                {
                    result.AddMessage("اطاعات را وارد نمایید");
                }


                ViewBag.errorMessage = result.FormattedMessages;

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }
        public async Task<IActionResult> Sinout()
        {
            try
            {

                await HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance4");

              

                return RedirectToAction("index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }
        public IActionResult AccessDenied()
        {
            //    return RedirectToAction("AccessDenied", "Account");
            return View();
        }



    }
}

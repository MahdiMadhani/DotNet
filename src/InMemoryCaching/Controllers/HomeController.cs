using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using InMemoryCaching.Models;
using InMemoryCaching.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Xml;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Microsoft.Data.Entity;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using System.Net;
using System.Net.Mail;
using InMemoryCaching;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace InMemoryCaching.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly DALContext _db;
        private readonly IMemoryCache _memoryCache;
        private readonly SessionManager _session;
        private readonly Services.IEmailSender _emailSender;
        public HomeController(DALContext db,IMemoryCache memoryCache,SessionManager session, Services.IEmailSender emailSender)
        {
            _db = db;
            _memoryCache = memoryCache;
            _session = session;
            _emailSender  = emailSender;
        }
        [HttpGet("[controller]/[action]/{key}")]
        public IActionResult ConfirmEmailAddress(string key)
        {
           // if (await Models.Account.RegisterationViewModel.ValidateUserReuest(key, _db, _emailSender))
               // return RedirectToAction("RegisterationCallback", "Account");

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                SetGetMemoryCache();

                PhotosAlbumViewModel v = new PhotosAlbumViewModel();
                v.Album = _db.Albums.ToList();
                v.Photo = _db.Photoes.Take(3).ToList();
                foreach (var item in v.Photo)
                {
                    item.Src = InMemoryCaching.App_Code.Utils.GenerateHttpResponseImageOntheFly(item.ImgData, item.ImgType);

                }
                foreach (var item in v.Album)
                {
                    item.Src = InMemoryCaching.App_Code.Utils.GenerateHttpResponseImageOntheFly(item.AssetData, item.AssetType);

                }

                return View(v);
            }
            catch (Exception ex)
            {


                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }
        
        [HttpGet]
        public IActionResult Error()
        {

            return View();
        }

        

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.ShowPhotosAlbum), "Home");
            }
        }

        
        [HttpGet]
        public IActionResult ShowPhotosAlbum(int? id)
        {
            try
            {
                PhotosAlbumViewModel vm = new PhotosAlbumViewModel();
            vm.Photo = _db.Photoes
                .Where(p => p.Album.Id == id)
                .Include(a => a.Album)
                .ToList();
            var i = vm.Photo;
            vm.CommentLst= _db.Comments.ToList();
            vm.IdRoute = id;

            CommentViewModel model = new CommentViewModel();
            string imgUrl;
            _session.CaptchaCodeInShowPhotosAlbum = model.GenerateCaptchaCode(out imgUrl);
            ViewBag.CaptchaImageInShowPhotosAlbum = imgUrl;

            return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpGet]
        public IActionResult ShowPhotosComment(int? id)
        {
            try
            {
                PhotosCommentViewModel vm = new PhotosCommentViewModel();
            vm.PhotoId = _db.Photoes.FirstOrDefault(x => x.Id == id);

           // var i = vm.Photo;

            vm.CommentLst = _db.Comments
                .Where(p => p.PhotoId ==vm.PhotoId.Id && p.status==true)
              
                .ToList();
            vm.IdRoute = id;

            CommentViewModel model = new CommentViewModel();
            string imgUrl;
            _session.CaptchaCodeInPhotosComment = model.GenerateCaptchaCode(out imgUrl);
            ViewBag.CaptchaImageInPhotosComment = imgUrl;

            return View(vm);
        }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
}
     
   
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertCommentInComment(PhotosCommentViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
            {
                string originalCaptcha;
                originalCaptcha = _session.CaptchaCodeInPhotosComment;
                var r= model.SubmitReview(originalCaptcha, _db,model);
                
              
                return RedirectToAction("ShowPhotosAlbum", new { id = model.IdRoute });

                
            }

            return RedirectToAction("ShowPhotosAlbum", new { id = model.IdRoute });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertCommentInPhotosAlbum(PhotosAlbumViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    string originalCaptcha;
                    originalCaptcha = _session.CaptchaCodeInShowPhotosAlbum;
                    var r = model.SubmitReview(originalCaptcha, _db, model);


                    return RedirectToAction("ShowPhotosAlbum", new { id = model.IdRoute });

                }


                return RedirectToAction("ShowPhotosAlbum", new { id = model.IdRoute });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }


        string i;
        public JsonResult MoreImage(string id)
        {

            try
            {
                if (id != null) {
                    List<string> result8 = id.Split(',').ToList();

                    var results = _db.Photoes.Where(r => !result8.Contains(r.Id.ToString())).Take(3).ToList();

                    if (results.Count > 0)
                    {
                        foreach (var item in results)
                        {
                            item.Src = InMemoryCaching.App_Code.Utils.GenerateHttpResponseImageOntheFly(item.ImgData, item.ImgType);
                            i = i + "," + item.Id;
                            item.idd = i;
                        }
                        return Json(results);
                    }
                    else
                    {
                        return Json(null);

                    }
                }


                return Json(null);
            }
            
            catch (Exception ex)
            {
                return Json(null);

            }
        }


        [HttpGet]
        public IActionResult About()
        {            
            return View();
        }


     
        private List<Yjc> SetGetMemoryCache()
        {
            try
            {
                var cacheKey = "YjcKey";
            List<Yjc> yjc;
            if (!_memoryCache.TryGetValue(cacheKey, out yjc))
            {
                ViewBag.FBFeed = GetFeed().ToList();            

                _memoryCache.Set(cacheKey, ViewBag.FBFeed,
                   new MemoryCacheEntryOptions()
                   .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
                
            }
            else
            {
                ViewBag.FBFeed = _memoryCache.Get(cacheKey) as List<Yjc>;
                

            }
            return yjc;
            }
            catch (Exception ex)
            {
                return (null);

            }
        }


        public IEnumerable<Yjc> GetFeed()
        {

            try
            {
                // string url =("http://www.yjc.ir/fa/rss/allnews");

                XDocument xml = XDocument.Load(@"http://www.yjc.ir/fa/rss/2");


                var FBUpdates = (from story in xml.Descendants("item")

                                 select new Yjc

                                 {

                                     Title = ((string)story.Element("title")),

                                     Link = ((string)story.Element("link")),

                                     Description = ((string)story.Element("description")),

                                     PubDate = ((string)story.Element("pubDate"))

                                 }).Take(5);
                return FBUpdates;
            }
            catch (Exception ex)
            {
                return (null);

            }

        }


        

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using InMemoryCaching.Models;
using System.IO;
using InMemoryCaching.App_Code;
using System.Drawing;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Data.Common;
using System.Threading;
using InMemoryCaching.Models.Account;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace InMemoryCaching.Controllers
{
    [Authorize(Roles = "admin")]
    public class ManageController : Controller
    {
        // GET: /<controller>/
        private readonly DALContext _db;
        public ManageController(DALContext db)
        {
            _db = db;

        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAlbum()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAlbum(ICollection<Microsoft.AspNet.Http.IFormFile> Productphoto_file, Album model)
        {
            try
            {
                //  ResultBundle result = ResultBundle.Failed();
                if (ModelState.IsValid)
                {
                    if (Productphoto_file.Count > 0)
                    {

                        foreach (var file in Productphoto_file)
                        {
                            if (file.Length > 1024 * 1024)
                            {
                                TempData["ErrorMessage"] = "حجم فایل انتخاب شده بیش از یک مگابایت است. لطفا فایل دیگری انتخاب نمایید";
                                break;
                            }
                            else if (file.Length == 0)
                            {
                                TempData["ErrorMessage"] = "حجم فایل انتخاب شده صفر بایت است. لطفا فایل دیگری انتخاب نمایید";
                                break;
                            }
                            else
                            {
                                var fileName = Microsoft.Net.Http.Headers.ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');


                                using (Stream sr = file.OpenReadStream())
                                {
                                    byte[] fileData = null;
                                    using (MemoryStream msOrig = Utils.LoadToMemoryStream(sr))
                                    {
                                        // resize it
                                        // todo: we always resize profile image to 600x500
                                        Image img = Bitmap.FromStream(msOrig);
                                        // Bitmap bmp = new Bitmap(img, new Size(600, 500));

                                        Bitmap bmp = new Bitmap(img, new Size(340, 280));
                                        MemoryStream ms = new MemoryStream();
                                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);



                                        // read content instead
                                        fileData = Utils.ConvertMemoryStreamToBytes(ms);

                                        ms.Dispose();
                                        bmp.Dispose();
                                        img.Dispose();
                                    }


                                    // _db = new DALContext();
                                    Models.Album Album = new Models.Album();
                                    Album.AssetData = new Byte[fileData.Length];
                                    Album.AssetType = "jpg";
                                    Buffer.BlockCopy(fileData, 0, Album.AssetData, 0, fileData.Length);
                                    //imgc.Id = 2;
                                    ViewBag.imgusedatabaseController = Album.AssetData;//for img usedatabaseController
                                    Album.Titel = model.Titel;
                                    _db.Albums.Add(Album);
                                    _db.SaveChanges();
                                    TempData["ErrorMessage"] = ("درج با موفقیت انجام شد");

                                }
                            }
                        }

                    }
                    else
                    {
                        TempData["ErrorMessage"] = ("لطفا یک عکس انتخاب نمایید ");
                        // result.AddMessage("لطفا یک عکس انتخاب نمایید ");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = ("لطفا یک عکس انتخاب کنید و عنوان را وارد نمایید ");
                    // result.AddMessage("لطفا یک عکس انتخاب کنید و عنوان را وارد نمایید ");
                }
                //  ViewBag.errorMessage = result.FormattedMessages;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }


        [HttpGet]
        public IActionResult ShowAlbums()
        {
            try
            {
                //_db = new DALContext();
                var v = _db.Albums.ToList();
                if (v.Count == 0)
                    return RedirectToAction("CreateAlbum");

                foreach (var item in v)
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

        [HttpPost]
        public JsonResult DeleteAlbums(int? id)
        {
            try
            {
                if (id != null)
                //_db = new DALContext();
                {

                    var album = _db.Albums.FirstOrDefault(x => x.Id == id);
                    if (album != null)
                    {

                        List<Photo> Photoes = (from ph in _db.Photoes
                                               where ph.AlbumID == album.Id
                                               select ph).ToList();

                        if (Photoes.Count > 0)
                        {
                            foreach (var Photoe in Photoes)
                            {

                                var comments = (from s in _db.Comments
                                                where s.Photo.Id == Photoe.Id
                                                select s).ToList();
                                foreach (var com in comments)
                                {
                                    _db.Comments.Remove(com);
                                }

                                _db.Photoes.Remove(Photoe);
                            }
                        }
                        _db.Albums.Remove(album);

                        _db.SaveChanges();

                    }

                }
                return Json("Record deleted successfully!");
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id != null)
                {
                    //_db = new DALContext();
                    var album = _db.Albums.FirstOrDefault(x => x.Id == id);
                    if (album != null)
                    {
                        return View(album);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ICollection<Microsoft.AspNet.Http.IFormFile> Productphoto_file, Album model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (Productphoto_file.Count > 0)
                    {

                        foreach (var file in Productphoto_file)
                        {
                            if (file.Length > 1024 * 1024)
                            {
                                TempData["ErrorMessage"] = "حجم فایل انتخاب شده بیش از یک مگابایت است. لطفا فایل دیگری انتخاب نمایید";

                                break;
                            }
                            else if (file.Length == 0)
                            {
                                TempData["ErrorMessage"] = "حجم فایل انتخاب شده صفر بایت است. لطفا فایل دیگری انتخاب نمایید";

                                break;
                            }
                            else
                            {
                                var fileName = Microsoft.Net.Http.Headers.ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                                using (Stream sr = file.OpenReadStream())
                                {
                                    byte[] fileData = null;
                                    using (MemoryStream msOrig = Utils.LoadToMemoryStream(sr))
                                    {
                                        // resize it
                                        // todo: we always resize profile image to 140x140
                                        Image img = Bitmap.FromStream(msOrig);
                                        Bitmap bmp = new Bitmap(img, new Size(340, 280));
                                        MemoryStream ms = new MemoryStream();
                                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);



                                        // read content instead
                                        fileData = Utils.ConvertMemoryStreamToBytes(ms);

                                        ms.Dispose();
                                        bmp.Dispose();
                                        img.Dispose();
                                    }
                                    /*
                                    // remove error flags
                                    //TempData.Remove("ErrorMessage");
                                    AssetData = new Byte[fileData.Length];

                                    AssetType= "jpg";
                                    Buffer.BlockCopy(fileData, 0, AssetData, 0, fileData.Length);


                                    ViewBag.up = Utils.GenerateHttpResponseImageOntheFly(AssetData, "jpg");

                                    // just one file!
                                    // break;
                                    */

                                    //_db = new DALContext();
                                    Models.Album Album = new Models.Album();
                                    Album.AssetData = new Byte[fileData.Length];
                                    Album.AssetType = "jpg";
                                    Buffer.BlockCopy(fileData, 0, Album.AssetData, 0, fileData.Length);

                                    ViewBag.imgusedatabaseController = Album.AssetData;//for img usedatabaseController

                                    Album.Titel = model.Titel;

                                    Album.Id = model.Id;
                                    _db.Entry(Album).State = Microsoft.Data.Entity.EntityState.Modified;
                                    _db.SaveChanges();

                                    TempData["ErrorMessage"] = " ویرایش انجام شد";

                                }
                            }
                        }

                    }
                    else
                    {
                        //_db = new DALContext();
                        Models.Album a = _db.Albums.FirstOrDefault(x => x.Id == model.Id);


                        a.Titel = model.Titel;
                        _db.SaveChanges();
                        TempData["ErrorMessage"] = "ویرایش انجام شد";

                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "لطفا یک عکس انتخاب کنید و عنوان را وارد نمایید ";
                }

                //return RedirectToAction("ShowAlbums");
                //return View();
                return RedirectToAction("Edit", model.Id);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpGet]
        public IActionResult CreatePhotosAlbum(int? id)
        {
            try
            {
                if (id != null)
                {
                    var album = _db.Albums.FirstOrDefault(x => x.Id == id);
                    ViewBag.AlbumTitel = album.Titel;
                    ViewBag.id = id;
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePhotosAlbum(ICollection<Microsoft.AspNet.Http.IFormFile> Productphoto_file, Photo model, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Productphoto_file.Count > 0)
                    {

                        foreach (var file in Productphoto_file)
                        {
                            if (file.Length > 1024 * 1024)
                            {
                                TempData["ErrorMessage"] = "حجم فایل انتخاب شده بیش از یک مگابایت است. لطفا فایل دیگری انتخاب نمایید";
                                break;
                            }
                            else if (file.Length == 0)
                            {
                                TempData["ErrorMessage"] = "حجم فایل انتخاب شده صفر بایت است. لطفا فایل دیگری انتخاب نمایید";
                                break;
                            }
                            else
                            {
                                var fileName = Microsoft.Net.Http.Headers.ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');


                                // file.SaveAsAsync(Path.Combine(uploads, fileName));
                                // read content instead
                                using (Stream sr = file.OpenReadStream())
                                {
                                    byte[] fileData = null;
                                    using (MemoryStream msOrig = Utils.LoadToMemoryStream(sr))
                                    {
                                        // resize it
                                        // todo: we always resize profile image to 140x140
                                        Image img = Bitmap.FromStream(msOrig);
                                        Bitmap bmp = new Bitmap(img, new Size(340, 340));
                                        MemoryStream ms = new MemoryStream();
                                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);



                                        // read content instead
                                        fileData = Utils.ConvertMemoryStreamToBytes(ms);

                                        ms.Dispose();
                                        bmp.Dispose();
                                        img.Dispose();
                                    }


                                    //_db = new DALContext();
                                    Models.Photo Photo = new Models.Photo();
                                    var SerchAlbumId = _db.Albums.FirstOrDefault(x => x.Id == id);
                                    if (SerchAlbumId != null)
                                    {
                                        Photo.ImgData = new Byte[fileData.Length];
                                        Photo.ImgType = "jpg";
                                        Buffer.BlockCopy(fileData, 0, Photo.ImgData, 0, fileData.Length);
                                        Photo.Album = SerchAlbumId;

                                        //ViewBag.AlbumTitell = SerchAlbumId.Titel;
                                        Photo.Titel = model.Titel;
                                        _db.Photoes.Add(Photo);
                                        _db.SaveChanges();
                                        TempData["ErrorMessage"] = "درج با موفقیت انجام شد";
                                    }

                                }
                            }
                        }

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "لطفا یک عکس انتخاب نمایید ";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "لطفا یک عکس انتخاب کنید و عنوان را وارد نمایید ";
                }
                ViewBag.id = id;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpGet]
        public IActionResult ShowPhotosAlbum(int? id)
        {
            try
            {
                if (id != null)
                {
                    //_db = new DALContext();
                    var v = _db.Photoes
                        .Where(p => p.Album.Id == id)
                        .Include(a => a.Album)
                        .ToList();

                    var album = _db.Albums.FirstOrDefault(x => x.Id == id);

                    if (album != null)
                    {
                        ViewBag.AlbumTitell = album.Titel;
                        ViewBag.RouteShowPhotosAlbumAlbumId = id;
                    }


                    //var vb = _db.Photoes.ToList();
                    //foreach (var item in vb)
                    //{
                    //    ViewBag.Src = InMemoryCaching.App_Code.Utils.GenerateHttpResponseImageOntheFly(item.ImgData, item.ImgType);

                    //}
                    return View(v);
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpGet]
        public IActionResult CommentStatus()
        {
            try
            {
                CommentViewModel v = new CommentViewModel();

                List<dynamic> MyList = CollectionFromSql();

                List<Comment> lc = new List<Comment>();
                foreach (var item in MyList.ToArray())
                {
                    Comment p = new Comment()
                    {
                        Id = item.Id,
                        Content = item.Content,
                        status = item.status,
                        WriterEmail = item.WriterEmail,
                        WriterName = item.WriterName,

                    };
                    lc.Add(p);
                }
                v.Comments = lc;

                return View(v);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentStatus(CommentViewModel model)
        {
            try
            {
                foreach (var item in model.Comments)
                {
                    var Comment = _db.Comments.Where(p => p.Id == item.Id).FirstOrDefault();
                    Comment.status = item.status;
                }
                // Album.Id = model.Id;

                //_db.Entry(Album).State = Microsoft.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("CommentStatus");
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        private List<dynamic> CollectionFromSql()
        {
            try
            {
                using (var cmd = _db.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "CommentList";
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;
                            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                                dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);

                            retObject.Add((ExpandoObject)dataRow);
                        }
                    }

                    return retObject;
                }
            }
            catch (Exception ex)
            {
                return null;

            }

        }

        [HttpGet]
        public IActionResult PermissionUsers()
        {
            try
            {
                List<RoleViewModel> lIL = new List<RoleViewModel>();


                var R = _db.Roles.ToList();
                var u = _db.Users.Where(e=>e.EmailAddress!= "gmahdi1388@gmail.com").ToList();

                RoleViewModel vm = new RoleViewModel();
                vm.users = u;
                vm.Roles = R;
                foreach (var item in u)
                {

                    RoleViewModel LI = new RoleViewModel()
                    {
                        Roles = R,
                        users = u,
                
                    };
                    lIL.Add(LI);
                }
                // ViewBag.Count = lIL.Count();
                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PermissionUsers(RoleViewModel model)
        {
            try
            {

                foreach (var item in model.users)
                {
                    var lo = _db.Users.Where(p => p.UserId == item.UserId).FirstOrDefault();
                    lo.RoleId = item.RoleId;
                }

                // Album.Id = model.Id;

                //_db.Entry(Album).State = Microsoft.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("PermissionUsers");
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");

            }
        }


    }
}

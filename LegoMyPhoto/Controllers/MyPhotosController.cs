using LegoMyPhoto.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;

namespace LegoMyPhoto.Controllers
{
    [Authorize]
    public class MyPhotosController : Controller
    {
        LegoMyPhotoDb _db = new LegoMyPhotoDb();
        //
        // GET: /MyPhotos/
        public string uploadedPath = @"Images\TEMP\UploadedImages\";
        public string pixelPath = @"Images\TEMP\UploadedImages\pixel\";
        public string displayPath = @"Images\TEMP\UploadedImages\pixel\display\";
        public string userOrigPath = @"Images\UserImages\original\";
        public string userLegoPath = @"Images\UserImages\lego\";

        public ActionResult Index(string message, int page = 1)
        {
            int currentuserID = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            UserProfile currentUser = _db.UserProfiles.Find(currentuserID);
            ICollection<PhotoPack> currentuserPhotos = new List<PhotoPack>();
            PhotoPack Ppack =  new PhotoPack();

            var model =
                _db.PhotoPacks
                .OrderBy(r => r.Id)
                .Where(r => r.UserID == currentuserID)
                //.Where(r => searchTerm == null || r.ModelNum.StartsWith(searchTerm))
                .Select(r => new SearchPhotoPack
                {
                    Id = r.Id,
                    newFileName = r.newFileName,
                    Original = r.Original,
                    PhotoName = r.PhotoName,
                    savedPath = r.savedPath,
                    SelectedSize = r.SelectedSize,
                    ShareThisPhoto = r.ShareThisPhoto,
                    fullPath = @"..\" + r.savedPath + r.newFileName

                }).ToPagedList(page, 2);
            ViewBag.delete = message;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Page", model);
            }

            return View(model);
        }

        public ActionResult ShowAll(string message)
        {
            int currentuserID = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            UserProfile currentUser = _db.UserProfiles.Find(currentuserID);
            ICollection<PhotoPack> currentuserPhotos = new List<PhotoPack>();
            PhotoPack Ppack = new PhotoPack();

            var model =
                _db.PhotoPacks
                .OrderBy(r => r.Id)
                .Where(r => r.UserID == currentuserID)
                //.Where(r => searchTerm == null || r.ModelNum.StartsWith(searchTerm))
                .Select(r => new SearchPhotoPack
                {
                    Id = r.Id,
                    newFileName = r.newFileName,
                    Original = r.Original,
                    PhotoName = r.PhotoName,
                    savedPath = r.savedPath,
                    SelectedSize = r.SelectedSize,
                    ShareThisPhoto = r.ShareThisPhoto,
                    fullPath = @"..\" + r.savedPath + r.newFileName

                });
            ViewBag.delete = message;

            return View(model);
        }

        public ActionResult Download(int downloadId, string returnUrl)
        {
            PhotoPack Ppack = _db.PhotoPacks.Find(downloadId);

            SearchPhotoPack Spack = new SearchPhotoPack
            {
                Id = Ppack.Id,
                newFileName = Ppack.newFileName,
                Original = Ppack.Original,
                PhotoName = Ppack.PhotoName,
                savedPath = Ppack.savedPath,
                SelectedSize = Ppack.SelectedSize,
                ShareThisPhoto = Ppack.ShareThisPhoto
            };

            ViewBag.fileName = Ppack.savedPath + Ppack.newFileName;
            ViewBag.returnDownloadUrl = "Download?downloadId=" + downloadId;
            ViewBag.returnUrl = returnUrl;

            return View(Spack);
        }


        public ActionResult Rename(int renameId, string returnUrl)
        {
            PhotoPack Ppack = _db.PhotoPacks.Find(renameId);

            SearchPhotoPack Spack = new SearchPhotoPack
            {
                Id = Ppack.Id,
                newFileName = Ppack.newFileName,
                Original = Ppack.Original,
                PhotoName = Ppack.PhotoName,
                savedPath = Ppack.savedPath,
                SelectedSize = Ppack.SelectedSize,
                ShareThisPhoto = Ppack.ShareThisPhoto
            };

            ViewBag.fileName = Ppack.savedPath + Ppack.newFileName;
            ViewBag.returnUrl = returnUrl;
            return View(Spack);
        }

        //
        // POST: /MyPhotos/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rename(SearchPhotoPack Spack, FormCollection collection)
        {
            PhotoPack Ppack = _db.PhotoPacks.Find(Spack.Id);

            Ppack.PhotoName = Spack.PhotoName;

            _db.Entry(Ppack).State = EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Index");
            
        }

        public string DeleteMethod(string path, string fileName)
        {
            string deleteResult = "Delete Error";
            string fullpath = Server.MapPath(path);
            FileInfo fullFile = new FileInfo(fullpath + fileName);
            if (fullFile.Exists)
            {
                fullFile.Delete();
                deleteResult = @"Deleted: " + fileName;
            }

            DirectoryInfo d = new DirectoryInfo(Server.MapPath(@"~\" + uploadedPath));
            foreach (FileInfo f in d.GetFiles())
            {
                if (f.CreationTime.AddHours(2) < DateTime.Now)
                    f.Delete();
            }
            d = new DirectoryInfo(Server.MapPath(@"~\" + pixelPath));
            foreach (FileInfo f in d.GetFiles())
            {
                if (f.CreationTime.AddHours(2) < DateTime.Now)
                    f.Delete();
            }
            d = new DirectoryInfo(Server.MapPath(@"~\" + displayPath));
            foreach (FileInfo f in d.GetFiles())
            {
                if (f.CreationTime.AddHours(2) < DateTime.Now)
                    f.Delete();
            }
            return deleteResult;
        }

        public ActionResult Delete(int deletedid, string returnUrl)
        {
            PhotoPack Ppack = _db.PhotoPacks.Find(deletedid);

            SearchPhotoPack Spack = new SearchPhotoPack
            {
                Id = Ppack.Id,
                newFileName = Ppack.newFileName,
                Original = Ppack.Original,
                PhotoName = Ppack.PhotoName,
                savedPath = Ppack.savedPath,
                SelectedSize = Ppack.SelectedSize,
                ShareThisPhoto = Ppack.ShareThisPhoto
            };

            ViewBag.fileName = Ppack.savedPath + Ppack.newFileName;
            ViewBag.returnUrl = returnUrl;
            return View(Spack);
        }

        //
        // POST: /MyPhotos/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int deletedid)
        {
            PhotoPack Ppack = _db.PhotoPacks.Find(deletedid);

            _db.PhotoPacks.Remove(Ppack);
            _db.SaveChanges();

             string deleteMessage = DeleteMethod(@"~\" + Ppack.savedPath, Ppack.newFileName);

             return RedirectToAction("Index", new { message = deleteMessage } );
        }
    }
}

using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using LegoMyPhoto.Models;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;
using System.Security;
using System.Reflection;
using System.Web.Security;

namespace LegoMyPhoto.Controllers
{

    public class UploadController : Controller
    {
        
        LegoMyPhotoDb _db = new LegoMyPhotoDb();

        //[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        //public class MultipleButtonAttribute : ActionNameSelectorAttribute
        //{
        //    public string Name { get; set; }
        //    public string Argument { get; set; }

        //    public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        //    {
        //        var isValidName = false;
        //        var keyValue = string.Format("{0}:{1}", Name, Argument);
        //        var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

        //        if (value != null)
        //        {
        //            controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
        //            isValidName = true;
        //        }

        //        return isValidName;
        //    }
        //}
        public string uploadedPath = @"Images\TEMP\UploadedImages\";
        public string pixelPath = @"Images\TEMP\UploadedImages\pixel\";
        public string displayPath = @"Images\TEMP\UploadedImages\pixel\display\";
        public string userOrigPath = @"Images\UserImages\original\";
        public string userLegoPath = @"Images\UserImages\lego\";

        public ActionResult Index(string file = null, string name = null)
        {
            PhotoPack Ppack = new PhotoPack { newFileName = file, PhotoName = name, ShareThisPhoto = false };

            ViewBag.file = file;
            return View(Ppack);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PhotoPack Ppack, HttpPostedFileBase image)
        {
            string filepath = HttpRuntime.AppDomainAppPath;
            //@"C:\Users\Richard\Documents\Visual Studio 2013\Projects\LegoMyPhoto\LegoMyPhoto\UploadedImages\";


            if (image == null)
            {
                ViewBag.Error = "Please select a Photo";
                return View(Ppack);
            }
            if (Request.ContentLength > 1900 * 1600 * 2)
            {
                ViewBag.Error = "Photo size too big. Maximum file size is 3MB.";
                return View(Ppack);
            }

            if (Request.Files.Count == 0)
            {
                ViewBag.Error = "Failed uploading photo";
                return View(Ppack);
            }
            if (!FileIsWebFriendlyImage(image.InputStream))
            {
                ViewBag.Error = "File is not an photo, Please only upload photo ( jpg, jpeg, gif, bmp, png, tif)";
                return View(Ppack);
            }
            
            image = Request.Files[0];

            string imgfilename = Path.GetFileName(image.FileName);
            Ppack.PhotoName = Path.GetFileNameWithoutExtension(image.FileName);
            if (100 < imgfilename.Length)
            {
                imgfilename = "FileNameTooLong.jpg";
            }

            Bitmap bmpPostedImage = new Bitmap(image.InputStream);
            Ppack.newFileName = Guid.NewGuid().ToString() + "_" + imgfilename;
            bmpPostedImage.Save(filepath + uploadedPath + Ppack.newFileName, ImageFormat.Jpeg);
            Ppack.savedPath = uploadedPath;

            ViewBag.file = uploadedPath + Ppack.newFileName;          

            return View(Ppack);
            
            
        }

        
        public static bool FileIsWebFriendlyImage(Stream stream)
        {
            try
            {
                //Read an image from the stream...
                var i = Image.FromStream(stream);

                //Move the pointer back to the beginning of the stream
                stream.Seek(0, SeekOrigin.Begin);

                if (ImageFormat.Jpeg.Equals(i.RawFormat))
                    return true;
                return ImageFormat.Png.Equals(i.RawFormat) || ImageFormat.Gif.Equals(i.RawFormat);
            }
            catch
            {
                return false;
            }
        }

        public void LegoWaterMark(PhotoPack Ppack, int legoSize)
        {
            WebImage filePhoto = new WebImage(@"~\" + pixelPath + Ppack.newFileName);

            int h = filePhoto.Height;
            int w = filePhoto.Width;
            WebImage WatermarkPhoto = new WebImage(@"~\Images\" + "Lego" + legoSize + "_4096X4096.png");

            WatermarkPhoto.Crop(0, 0, WatermarkPhoto.Height - h, WatermarkPhoto.Width - w);

            filePhoto.AddImageWatermark(WatermarkPhoto, width: w-1, height: h-1,
            horizontalAlign: "Left", verticalAlign: "Top",
            opacity: 60, padding: 0);

            filePhoto.Save(@"~\" + displayPath + Ppack.newFileName);
            
            return;
        }

        private static Bitmap Pixelate(Bitmap image, Rectangle rectangle, Int32 pixelateSize)
        {
            Bitmap pixelated = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(pixelated))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // look at every pixel in the rectangle while making sure we're within the image bounds
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < image.Width; xx += pixelateSize)
            {
                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < image.Height; yy += pixelateSize)
                {
                    Int32 offsetX = pixelateSize / 2;
                    Int32 offsetY = pixelateSize / 2;

                    // make sure that the offset is within the boundry of the image
                    while (xx + offsetX >= image.Width) offsetX--;
                    while (yy + offsetY >= image.Height) offsetY--;

                    // get the pixel color in the center of the soon to be pixelated area
                    Color pixel = pixelated.GetPixel(xx + offsetX, yy + offsetY);

                    // for each pixel in the pixelate size, set it to the center color
                    for (Int32 x = xx; x < xx + pixelateSize && x < image.Width; x++)
                        for (Int32 y = yy; y < yy + pixelateSize && y < image.Height; y++)
                            pixelated.SetPixel(x, y, pixel);
                }
            }

            return pixelated;
        }

        public WebImage ResizeImage(WebImage photo, string fileName, int h, int w, bool aspect)
        {

            photo.Resize(width: w, height: h, preserveAspectRatio: aspect,
               preventEnlarge: false);

            return photo;
        }

        public void FitLegoSize(WebImage photo, string fileName, int targetH, int targetW, int legoSize)
        {
            int intH = 0;
            int intW = 0;
            WebImage background = new WebImage(@"~\Images\blackbackground.jpg");

            if (targetH != photo.Height || targetW != photo.Width)
            {
                background.Crop(0, 0, background.Height - targetH, background.Width - targetW);

                background.AddImageWatermark(photo, width: photo.Width - 1 , height: photo.Height - 1,
                horizontalAlign: "Center", verticalAlign: "Middle",
                opacity: 100, padding: 0);
                photo = background;
            }
            //if (targetW != photo.Width)
            //{

            //}

            decimal w = photo.Width / legoSize;
            decimal h = photo.Height / legoSize;
            w = Math.Round(w) * legoSize;
            h = Math.Round(h) * legoSize;
            intH = Convert.ToInt32(h);
            intW = Convert.ToInt32(w);

            photo = ResizeImage(photo, fileName, intH, intW, false);

            photo.Save(@"~\" + displayPath + fileName);
            return;
        }
        //
        // GET: /Upload/Details/5

        public string Delete(string path, string fileName)
        {
            string deleteResult = "Delete Error";
            string fullpath = Server.MapPath(path);
            FileInfo fullFile = new FileInfo(fullpath + fileName);
            if (fullFile.Exists) { 
                fullFile.Delete();
                deleteResult = @"Deleted: "+ fileName;
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

        [Authorize]
        public ActionResult Custom(string file, string name, string path, bool saved)
        {
            
            int currentuserID = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            UserProfile currentUser = _db.UserProfiles.Find(currentuserID);

            PhotoPack Ppack = new PhotoPack { newFileName = file, PhotoName = name, savedPath = userOrigPath, ShareThisPhoto = false, Original = true };

            if (saved == false)
            {
                WebImage photo = new WebImage(@"~\" + uploadedPath + Ppack.newFileName);
                photo.Save(@"~\" + userOrigPath + Ppack.newFileName);

                currentUser.MyPhotos.Add(Ppack);

                _db.Entry(currentUser).State = EntityState.Modified;
                _db.SaveChanges();

                Delete(@"~\" + uploadedPath, Ppack.newFileName);

            }        
            

            ViewBag.file = Ppack.savedPath + Ppack.newFileName;
            return View(Ppack);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Custom(PhotoPack Ppack, FormCollection form)
        {
            int legoSize = Convert.ToInt32(Request.Form["legosize"].ToString());
            string file = Request.Form["newFileName"].ToString();
            int h = 0;
            int w = 0;

            if (Request.Form["SelectedSize"] != "Keep")
            {
                h = Convert.ToInt32(Request.Form["SelectedSize"].ToString());

                if (h == 480) { w = 640; }
                if (h == 600) { w = 800; }
                if (h == 768) { w = 1024; }
                if (h == 960 || h == 1024 || h == 720 || h == 800) { w = 1280; }
                if (h == 1200) { w = 1600; }
                if (h == 900) { w = 1440; }
                if (h == 1050) { w = 1680; }
                if (h == 1080 || h == 1201) 
                { 
                    w = 1920;
                    if (h == 1201) { h = 1200; }
                }
                
                //Full Screen:        Widescreen:

                //800 x 600           1280 x 720
                //1024 x 768          1280 x 800
                //1280 x 960          1440 x 900
                //1280 x 1024         1680 x 1050
                //1600 x 1200         1920 x 1080
                //                    1920 x 1200


                return RedirectToAction("Display", new { file = file, name = Ppack.PhotoName, sized = true, savedFile = true, legoSize, savedFilePath = userOrigPath, w, h });
            }
            else
            {

                return RedirectToAction("Display", new { file = file, name = Ppack.PhotoName, sized = false, savedFile = true, legoSize, savedFilePath = userOrigPath });
            }

            
        }

        public ActionResult Display(string file, bool sized, string name, bool savedFile, int legoSize = 0, string savedFilePath = null, int w = 0, int h = 0)
        {
            PhotoPack Ppack = new PhotoPack { newFileName = Guid.NewGuid().ToString() + "_" + name + ".jpg", PhotoName = name, ShareThisPhoto = false };
            
            string filepath = HttpRuntime.AppDomainAppPath; 
            Bitmap newImage;
            Bitmap bmpPostedImage;

            if (savedFilePath == null) { savedFilePath = uploadedPath; }

            WebImage photo = new WebImage(filepath + savedFilePath + file);

            int currentH = photo.Height;
            int currentW = photo.Width;

            if (sized == false)
            {
                FitLegoSize(photo, file, currentH, currentW, legoSize);
            }
            else
            {

                photo.Resize(width: w, height: h, preserveAspectRatio: true,
                   preventEnlarge: false);

                FitLegoSize(photo, file, h, w, legoSize);
            }


            bmpPostedImage = new Bitmap(filepath + displayPath + file);
            newImage = Pixelate(bmpPostedImage, new Rectangle(0, 0, bmpPostedImage.Width, bmpPostedImage.Height), legoSize);
            bmpPostedImage.Dispose();
            newImage.Save(filepath + pixelPath + Ppack.newFileName, ImageFormat.Jpeg);

            newImage.Dispose();
            ViewBag.filepath = displayPath + Ppack.newFileName;
            LegoWaterMark(Ppack, legoSize);

            SearchPhotoPack Spack = new SearchPhotoPack
            {
                Id = Ppack.Id,
                newFileName = Ppack.newFileName,
                oldFileName = file,
                Original = Ppack.Original,
                PhotoName = Ppack.PhotoName,
                savedPath = Ppack.savedPath,
                savedFile = savedFile,
                SelectedSize = Ppack.SelectedSize,
                ShareThisPhoto = Ppack.ShareThisPhoto
            };

            return View(Spack);
        }

        //
        // POST: /Upload/Edit/5

        [Authorize]
        public ActionResult SaveMe(string file, string name)
        {
            int currentuserID = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            UserProfile currentUser = _db.UserProfiles.Find(currentuserID);

            PhotoPack Ppack = new PhotoPack { newFileName = file, savedPath = userLegoPath, PhotoName = name, ShareThisPhoto = false, Original = false };
            WebImage Fphoto = new WebImage(@"~\" + displayPath + Ppack.newFileName);

            Fphoto.Save(@"~\" + userLegoPath + Ppack.newFileName);

            currentUser.MyPhotos.Add(Ppack);

            _db.Entry(currentUser).State = EntityState.Modified;
            _db.SaveChanges();

            string message = Delete(@"~\" + displayPath, Ppack.newFileName);
            string message1 = Delete(@"~\" + pixelPath, Ppack.newFileName);

            return RedirectToAction("Index");
        }

        public ActionResult Return(string returnUrl)
        {
            if (returnUrl != null)
            {
                string newUrl = returnUrl.Remove(0, 14);
                string whichUrl = returnUrl.Remove(0, 8);
                string preUrl;

                if (whichUrl[0] == 'S')
                {
                    preUrl = @"/Upload/Display";
                    newUrl = newUrl + @"&sized=false&legoSize=16";
                }
                else { preUrl = @"/Upload/Index"; }

                if (Url.IsLocalUrl(preUrl + newUrl))
                {
                    return Redirect(preUrl + newUrl);
                }
            }
            
            return RedirectToAction("Index", "Home");

        }

        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

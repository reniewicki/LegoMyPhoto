using LegoMyPhoto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LegoMyPhoto.Controllers
{
    public class HomeController : Controller
    {
        LegoMyPhotoDb _db = new LegoMyPhotoDb();

        public ActionResult Index()
        {
            //var model = _db.PhotoPacks.ToList();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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

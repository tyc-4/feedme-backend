using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using feedme_backend;

namespace feedme_backend.Controllers
{
    public class RestaurantsController : Controller
    {
        private FeedmeEntities db = new FeedmeEntities();

        public RestaurantsController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        // GET: Login
        public ActionResult Login(string username, string password)
        {
            var user = from x in db.Restaurants
                       where x.Username == username && x.Password == password
                       select x;
            if (user.Count() == 1)
            {
                var authenticated_user = user.FirstOrDefault();
                return Json(authenticated_user, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string username, string password, string authkey)
        {
            if (authkey == "SUPERSECRETKEY" && username != "" && password != "")
            {
                var user = from x in db.Restaurants
                           where x.Username == username
                           select x;
                if (user.Count() == 1)
                {
                    var userToUpdate = user.FirstOrDefault();
                    userToUpdate.Password = password;
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: UpdateDetails      **For Restaurant Status: 1 = Open, 0 = Closed
        [HttpPost]
        public ActionResult UpdateDetails(string resname, int status, string username, string password, string imageURL, string authkey)
        {
            if (authkey == "SUPERSECRETKEY" && username != "" && password != "" && resname != "")
            {
                var user = from x in db.Restaurants
                           where x.Username == username
                           select x;
                if (user.Count() == 1)
                {
                    var userToUpdate = user.FirstOrDefault();
                    userToUpdate.StoreName = resname;
                    userToUpdate.Status = status;
                    userToUpdate.Username = username;
                    userToUpdate.Password = password;
                    userToUpdate.Image = imageURL;

                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: AllProductByRes
        public ActionResult AllProductByRes(int resId)
        {
            var products = (from x in db.Items
                            where x.RestaurantFK == resId
                            select x);
            if (products.Any())
            {
                return Json(products, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: AllRes
        public ActionResult AllRes()
        {
            return Json(db.Restaurants, JsonRequestBehavior.AllowGet);
        }

        // GET: AllResOpen
        public ActionResult AllResOpen()
        {
            return Json(db.Restaurants.Where(x => x.Status == 1), JsonRequestBehavior.AllowGet);
        }

        // GET: GetSpecResInfo
        public ActionResult GetSpecResInfo(int resId)
        {
            var res = (from x in db.Restaurants
                       where x.ID == resId
                       select new { x.StoreName, x.Image, x.Descriptiom }).ToList();

            if (res.Any())
            {
                return Json(res.First(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        //GET: GetResNameById
        public ActionResult GetResNameById(int resId)
        {
            return Content(db.Restaurants.Where(x => x.ID == resId).FirstOrDefault().StoreName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Web.Mvc;

namespace feedme_backend.Controllers
{
    public class UsersController : Controller
    {
        private FeedmeEntities db = new FeedmeEntities();

        public UsersController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        // GET: Login
        public ActionResult Login(string username, string password)
        {
            var user = from x in db.Users
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

        // POST: RegisterUser
        public ActionResult RegisterUser(string firstname, string lastname, string email, string username, string password)
        {
            // Check if username is valid
            if (CheckUsername(username) && username != "" && firstname != "" && password != "")
            {
                var newUser = db.Users.Add(new User { Email = email, FirstName = firstname, LastName = lastname, Password = password, Username = username });
                db.SaveChanges();
                return Json(newUser, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        // POST: CheckValidUsername
        [HttpPost]
        public ActionResult CheckValidUsername(string username)
        {
            if (CheckUsername(username))
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
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
                var user = from x in db.Users
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

        // POST: UpdateDetails
        [HttpPost]
        public ActionResult UpdateDetails(string firstname, string lastname, string email, string username, string password, string authkey)
        {
            if (authkey == "SUPERSECRETKEY" && username != "" && password != "" && firstname != "")
            {
                var user = from x in db.Users
                           where x.Username == username
                           select x;
                if (user.Count() == 1)
                {
                    var userToUpdate = user.FirstOrDefault();
                    userToUpdate.FirstName = firstname;
                    userToUpdate.LastName = lastname;
                    userToUpdate.Email = email;
                    userToUpdate.Username = username;
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

        // GET: GetName
        public ActionResult GetName(int userId)
        {
            return Content(db.Users.Where(x=>x.ID == userId).First().FirstName);
        }

        // Username Availablity Checking Algo
        public bool CheckUsername(string username)
        {
            var userCount = from x in db.Users
                            where x.Username == username
                            select x;
            return userCount.Count() == 0;
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

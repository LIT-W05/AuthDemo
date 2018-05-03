using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AuthDemo.Data;
using AuthDemo.Models;

namespace AuthDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new HomepageViewModel
            {
                IsAuthenticated = User.Identity.IsAuthenticated
            };

            return View(vm);
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user, string password)
        {
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            db.AddUser(user, password);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Secret()
        {
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            var vm = new SecretViewModel
            {
                User = db.GetByEmail(User.Identity.Name)
            };
            return View(vm);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            var user = db.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            FormsAuthentication.SetAuthCookie(email, true);
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
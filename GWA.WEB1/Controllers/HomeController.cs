using GWA.Data.Context;
using GWA.Domaine.Entities;

using System;
using System.Web.Mvc;
using System.Globalization;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net.Mime;
using static IdentitySample.Controllers.ManageController;
using GWA.Data.Context;
using Microsoft.AspNet.Identity.EntityFramework;
using GWA.Domaine.Entities;
using GWA.Service.UserService.Service;
using GWA.Service.Pattern;
using System.Net;
using System.Collections.Generic;
using GWA.WEB1.Helpers;
using GWA.WEB1.Models.Product;
using System.Collections;
using GWA.WEB1.Models;

namespace IdentitySample.Controllers
{
    [RequireHttps]
  
    public class HomeController : Controller
    {
        GWAContext context = new GWAContext();
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var UserManager = new UserManager<User>(new UserStore<User>(context));
                User user = UserManager.FindById(userId);

                if (isAdminUser())
                {

                    return RedirectToAction("Index", "UsersAdmin");
                }

                if (isExpertUser())
                {

                    return RedirectToAction("Index", "Expert");
                }
                if (isBookeperUser())
                {

                    return RedirectToAction("Index", "Bookeper");
                }
                if (isSellerUser())
                {

                    return RedirectToAction("Index", "Seller");
                }

            }
            return View();
        }



        public Boolean isAdminUser()
        {
            var user = User.Identity;

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var s = UserManager.GetRoles(user.GetUserId());
           
            if (s[0].ToString() == "Admin")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean isExpertUser()
        {

            var user = User.Identity;

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var s = UserManager.GetRoles(user.GetUserId());

            if (s[0].ToString() == "Expert")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean isBookeperUser()
        {

            var user = User.Identity;

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var s = UserManager.GetRoles(user.GetUserId());

            if (s[0].ToString() == "Bookeper")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean isSellerUser()
        {

            var user = User.Identity;

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var s = UserManager.GetRoles(user.GetUserId());

            if (s[0].ToString() == "Seller")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        //   

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

using GWA.Domaine.Entities;
using GWA.WEB1.Models.Product;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GWA.WEB1.Controllers
{

    [Authorize(Roles = "Expert")]
    public class ExpertController : Controller
    {

        //// GET: Seller
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ExpertController()
        {
        }

        public ExpertController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Roles/
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<User>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        //public ActionResult Profil()
        //{
        //    var userId = User.Identity.GetUserId();
        //    User user = service.GetUser(userId);
        //    List<string> listRoles = service.GetUserRoles(user.Id);

        //    List<Seller> listuserSuiv = service.GetListSuivis(user.Id);

        //    var nbrSellerSuivre = service.GetListSuivisnbr(user.Id);
        //    ViewBag.nbrSellerSuivre = nbrSellerSuivre;
        //    var listProduct = service.GetListProduitAuctionInscriBayresN(user.Id);

        //    var listProductWin = service.GetListProduitAuctionWinBayres(user.Id);
        //    var listProductNOWin = service.GetListProduitAuctionNoWinBayres(user.Id);


        //    //List<User> listuserAbon = service.GetListAbonne(user.Id);

        //    var nbrUserAccAuction = service.GetnbrUserAcepterAuction(user.Id, 1);




        //    //
        //    //var listProduct = service.GetListProduitAuctionSellSeller(user.Id);

        //    //var listProduct = service.GetListProduitAuctionNoSellSeller(user.Id);




        //    //ViewBag.GetListSuivisnbr = service.GetListSuivisnbr(user.Id);



        //    //ViewBag.GetListNotification = service.GetListNotification(user.Id);
        //    //ViewBag.GetnbrUserAcepterAuction = service.GetListNotification(user.Id);
        //    //ViewBag.GetListProduitAuctionInscriBayres = service.GetListNotification(user.Id);
        //    //ViewBag.GetListProduitAuctionInscriBayres = service.GetListNotification(user.Id);

        //    //ViewBag.GetListProduitAuctionWinBayres = service.GetListNotification(user.Id);
        //    //ViewBag.GetListProduitAuctionNoWinBayres = service.GetListNotification(user.Id);

        //    //ViewBag.GetListAttendSubAuctionBayres = service.GetListNotification(user.Id);
        //    //ViewBag.GetListAccepterSubAuctionBayres = service.GetListNotification(user.Id);
        //    //ViewBag.GetListProductFavories = service.GetListNotification(user.Id);



        //    //list de suivis des utilisateur pour seller
        //    List<RegisterViewModel> listuserSuivModel = new List<RegisterViewModel>();

        //    foreach (Seller item in listuserSuiv)
        //    {
        //        List<string> listRolesSuivis = service.GetUserRoles(item.Id);
        //        listuserSuivModel.Add(

        //            new RegisterViewModel
        //            {

        //                id = item.Id,
        //                ConfirmPassword = item.ConfirmPassword,
        //                HomeTown = item.HomeTown,
        //                Password = item.Password,
        //                RegisterEmail = item.RegisterEmail,
        //                RegisterUserName = item.RegisterUserName,
        //                UserRolesS = listRolesSuivis,
        //                UserName = item.UserName,
        //                Email = item.Email,
        //                FirstName = item.FirstName,
        //                LastName = item.LastName,
        //                Country = item.Country,
        //                BirthDate = item.BirthDate,
        //                JoinDate = item.JoinDate
        //                    ,
        //                EmailLinkDate = item.EmailLinkDate,
        //                LAstLoginDate = item.LAstLoginDate,
        //                PersonnalDescription = item.PersonnalDescription,


        //            }
        //            );
        //    }



        //    //List<RegisterViewModel> listuserSuivAbonn = new List<RegisterViewModel>();

        //    //foreach (User item in listuserAbon)
        //    //{
        //    //    List<string> listRolesSuivis = service.GetUserRoles(item.Id);
        //    //    listuserSuivAbonn.Add(

        //    //        new RegisterViewModel
        //    //        {
        //    //            id = item.Id,
        //    //            ConfirmPassword = item.ConfirmPassword,
        //    //            HomeTown = item.HomeTown,
        //    //            Password = item.Password,
        //    //            RegisterEmail = item.RegisterEmail,
        //    //            RegisterUserName = item.RegisterUserName,
        //    //            UserRolesS = listRolesSuivis,

        //    //            Email = item.Email,
        //    //            FirstName = item.FirstName,
        //    //            LastName = item.LastName,
        //    //            Country = item.Country,
        //    //            BirthDate = item.BirthDate,
        //    //            JoinDate = item.JoinDate
        //    //                ,
        //    //            EmailLinkDate = item.EmailLinkDate,
        //    //            LAstLoginDate = item.LAstLoginDate,


        //    //        }
        //    //        );
        //    //}



        //    //List<ProductViewModel> listuserProductModel = new List<ProductViewModel>();
        //    List<ProductViewModel> pvm = new List<ProductViewModel>();
        //    foreach (var item in listProduct)
        //    {
        //        pvm.Add(
        //            new ProductViewModel
        //            {
        //                Id = item.Id,
        //                Name = item.Name,
        //                CreationDate = item.CreationDate,
        //                CategoryId = item.IdCategory,
        //                CurrentPrice = item.CurrentPrice,
        //                reference = item.reference,
        //                status = item.status,
        //                UpdateDate = item.UpdateDate,
        //                ImageUrl = item.ImageUrl,
        //                Description = item.Description
        //                //IdUser = item.IdUser
        //                //BestSeller = u
        //            });
        //    }
        //    List<ProductViewModel> pvmWin = new List<ProductViewModel>();
        //    foreach (var item in listProductWin)
        //    {
        //        pvmWin.Add(
        //            new ProductViewModel
        //            {
        //                Id = item.Id,
        //                Name = item.Name,
        //                CreationDate = item.CreationDate,
        //                CategoryId = item.IdCategory,
        //                CurrentPrice = item.CurrentPrice,
        //                reference = item.reference,
        //                status = item.status,
        //                UpdateDate = item.UpdateDate,
        //                ImageUrl = item.ImageUrl,
        //                Description = item.Description
        //                //IdUser = item.IdUser
        //                //BestSeller = u
        //            });
        //    }
        //    List<ProductViewModel> pvmNowin = new List<ProductViewModel>();
        //    foreach (var item in listProductNOWin)
        //    {
        //        pvmNowin.Add(
        //            new ProductViewModel
        //            {
        //                Id = item.Id,
        //                Name = item.Name,
        //                CreationDate = item.CreationDate,
        //                CategoryId = item.IdCategory,
        //                CurrentPrice = item.CurrentPrice,
        //                reference = item.reference,
        //                status = item.status,
        //                UpdateDate = item.UpdateDate,
        //                ImageUrl = item.ImageUrl,
        //                Description = item.Description
        //                //IdUser = item.IdUser
        //                //BestSeller = u
        //            });
        //    }
        //    RegisterViewModel uvmk = new RegisterViewModel
        //    {
        //        id = userId,
        //        ConfirmPassword = user.ConfirmPassword,
        //        HomeTown = user.HomeTown,
        //        Password = user.Password,
        //        RegisterEmail = user.RegisterEmail,
        //        RegisterUserName = user.RegisterUserName,
        //        UserRolesS = listRoles,

        //        Email = user.Email,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Country = user.Country,
        //        BirthDate = user.BirthDate,
        //        JoinDate = user.JoinDate,
        //        PhoneNumber = user.PhoneNumber,
        //        EmailLinkDate = user.EmailLinkDate,
        //        LAstLoginDate = user.LAstLoginDate,
        //        UserSuivre = listuserSuivModel,
        //        //UserAbonne= listuserSuivAbonn,
        //        nbrUserAccAuction = nbrUserAccAuction,
        //        //ProductView = pvm,
        //        //ProductViewWin = pvmWin,
        //        //ProductViewNoWin = pvmNowin


        //    };



        //    return View(uvmk);

        //}
    }
}
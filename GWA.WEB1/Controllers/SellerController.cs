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

namespace GWA.WEB1.Controllers
{
   
      
 
        [Authorize(Roles = "Seller")]
        public class SellerController : Controller
        {


        GWAContext context;

        ServiceUser service = null;
       
        public SellerController()
            {

            context = new GWAContext();
            service = new ServiceUser();
        }

            public SellerController(ApplicationUserManager userManager,
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


        //  [AllowAnonymous]

        public ActionResult Profil()
        {
            var userId = User.Identity.GetUserId();
            User user = service.GetUser(userId);
            List<string> listRoles = service.GetUserRoles(user.Id);
            List<User> listuserAbon = service.GetListAbonne(user.Id);


            ViewBag.GetListAbonnenbr = service.GetListAbonnenbr(user.Id);


           

            var nbrUserAccAuction = service.GetnbrUserAcepterAuction(user.Id);

            var listUserAccAuction = service.GetUserAcepterAuction(user.Id);




           var listProduct = service.GetListProduitAuctionSellSeller(user.Id);

            var listProductNoSellSeller = service.GetListProduitAuctionNoSellSeller(user.Id);


            

 List<RegisterViewModel> listUserAccAuctionM = new List<RegisterViewModel>();

            foreach (User item in listUserAccAuction)
            {
                List<string> listRolesSuivis = service.GetUserRoles(item.Id);
                listUserAccAuctionM.Add(

                    new RegisterViewModel
                    {
                        id = item.Id,
                        ConfirmPassword = item.ConfirmPassword,
                        HomeTown = item.HomeTown,
                        Password = item.Password,
                        RegisterEmail = item.RegisterEmail,
                        RegisterUserName = item.RegisterUserName,
                        UserRolesS = listRolesSuivis,
                        PersonnalDescription = item.PersonnalDescription,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Country = item.Country,
                        BirthDate = item.BirthDate,
                        JoinDate = item.JoinDate,
                        EmailLinkDate = item.EmailLinkDate,
                        LAstLoginDate = item.LAstLoginDate,


                    }
                    );
            }







            List<RegisterViewModel> listuserSuivAbonn = new List<RegisterViewModel>();

            foreach (User item in listuserAbon)
            {
                List<string> listRolesSuivis = service.GetUserRoles(item.Id);
                listuserSuivAbonn.Add(

                    new RegisterViewModel
                    {
                        id = item.Id,
                        ConfirmPassword = item.ConfirmPassword,
                        HomeTown = item.HomeTown,
                        Password = item.Password,
                        RegisterEmail = item.RegisterEmail,
                        RegisterUserName = item.RegisterUserName,
                        UserRolesS = listRolesSuivis,
                        PersonnalDescription=item.PersonnalDescription,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Country = item.Country,
                        BirthDate = item.BirthDate,
                        JoinDate = item.JoinDate,
                        EmailLinkDate = item.EmailLinkDate,
                        LAstLoginDate = item.LAstLoginDate,


                    }
                    );
            }



           
          

            List<ProductViewModel> pvmSelOK = new List<ProductViewModel>();
            foreach (var item in listProduct)
            {
                pvmSelOK.Add(
                    new ProductViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreationDate = item.CreationDate,
                        CategoryId = item.IdCategory,
                        CurrentPrice = item.CurrentPrice,
                        reference = item.reference,
                        status = item.status,
                        UpdateDate = item.UpdateDate,
                        ImageUrl = item.ImageUrl,
                        Description = item.Description
                        
                    });
            }
            List<ProductViewModel> pvmSelNO = new List<ProductViewModel>();
            foreach (var item in listProductNoSellSeller)
            {
                pvmSelNO.Add(
                    new ProductViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreationDate = item.CreationDate,
                        CategoryId = item.IdCategory,
                        CurrentPrice = item.CurrentPrice,
                        reference = item.reference,
                        status = item.status,
                        UpdateDate = item.UpdateDate,
                        ImageUrl = item.ImageUrl,
                        Description = item.Description
                       
                    });
            }
          
            RegisterViewModel uvmk = new RegisterViewModel
            {
                id = userId,
                ConfirmPassword = user.ConfirmPassword,
                HomeTown = user.HomeTown,
                Password = user.Password,
                RegisterEmail = user.RegisterEmail,
                RegisterUserName = user.RegisterUserName,
                UserRolesS = listRoles,

                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.Country,
                BirthDate = user.BirthDate,
                JoinDate = user.JoinDate,
                PhoneNumber = user.PhoneNumber,
                EmailLinkDate = user.EmailLinkDate,
                LAstLoginDate = user.LAstLoginDate,
              
                UserAbonne= listuserSuivAbonn,
                nbrUserAccAuction = nbrUserAccAuction,
                //ProductView = pvm,
                //ProductViewWin = pvmWin,
                //ProductViewNoWin = pvmNowin
                ProductNoSe =pvmSelNO,
                ProductAcc =  pvmSelOK,
                listUserAccAuctionM= listUserAccAuctionM,

            };


            //    LoginViewModel lvm = new LoginViewModel
            //    {
            //        uvm = uvmk,
            //        role = service.GetUserRoles(user.Id)[0] 

            //};

            return View(uvmk);

        }



        // GET: Account/Edit
        [AllowAnonymous]
        public ActionResult EditSeller()
        {


            var user = UserManager.FindById(User.Identity.GetUserId());

            RegisterModel uvmk = new RegisterModel
            {
                id = user.Id,
                HomeTown = user.HomeTown,
                RegisterEmail = user.RegisterEmail,
                RegisterUserName = user.RegisterUserName,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.Country,
                BirthDate = user.BirthDate,






            };

          
            return View(uvmk);



        }

        // POST : Account/Edit
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> EditSeller(RegisterModel model, HttpPostedFileBase Image, string returnUrl)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(User.Identity.GetUserId());


            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, true, shouldLockout: false);

            if (result == SignInStatus.Success)
            {
                user.UserName = model.Email;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Country = model.Country;
                user.BirthDate = model.BirthDate;
                user.JoinDate = DateTime.Now;

                user.EmailLinkDate = DateTime.Now;
                user.LAstLoginDate = DateTime.Now;

               
                return Login(returnUrl);
               
            }
            else
                return View();
        }
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

    }
}

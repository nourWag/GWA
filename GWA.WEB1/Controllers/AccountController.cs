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
using System.IO;

namespace IdentitySample.Controllers
{


   

    [Authorize]
    public class AccountController : Controller
    {
        GWAContext context;

        ServiceUser service = null;
        


        public AccountController()
        {
            context = new GWAContext();
            service = new ServiceUser();
        }
        
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
      
        // GET: /Account/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two factor provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "The phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var model = new IndexViewModel
            {
                //HasPassword = HasPassword(),

                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId()),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
                Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId())
            };
            return View(model);
        }

        private bool HasPassword()
        {
           
            throw new NotImplementedException();
        }

        // GET: /Account/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        // POST: /Account/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Generate the token 
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(
                                       User.Identity.GetUserId(), model.AddPhoneNumberViewModel.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.AddPhoneNumberViewModel.Number,
                    Body = "Your security code is: " + code
                };
                // Send token
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.AddPhoneNumberViewModel.Number });
        }

        // POST: /Account/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }
        private async Task SignInAsync(User user, bool isPersistent)
        {
            return;
            // Clear the temporary cookies used for external and two factor sign ins
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie,
               DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = isPersistent
            },
               await user.GenerateUserIdentityAsync(UserManager));
        }




        /// <summary>
        /// ///////////////
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //  public static string EConfUser { get; set; }
        //  public static string connection = GetConnectionString("DefaultConnection");

        //  public static string command = null;
        //  public static string parametreName = null;
        //  public static string methodName = null;
        //string codeType = null;
        //
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

        ////
        //// POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // Require the user to have a confirmed email before they can log on.
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user != null)
        //    {
        //        if (!await UserManager.IsEmailConfirmedAsync(user.Id))
        //        {
        //            string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");
        //            ViewBag.errorMessage = "You must have a confirmed email to log on.";
        //            return View("Error");
        //        }
        //    }

        //    // This doen't count login failures towards lockout only two factor authentication
        //    // To enable password failures to trigger lockout, change to shouldLockout: true

        //   // var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
        //   var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,true);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //            //return View("index");
        //        case SignInStatus.LockedOut:
        //        //return View("index");
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //           // return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
        //    if (user != null)
        //    {
        //        ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}


        // POST: /Account/Login   
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout   
            // To enable password failures to trigger account lockout, change to shouldLockout: true   
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            User user = UserManager.FindByName(model.UserName);
            //var user =  service.GetUser(userId);
            switch (result)
            {
                case SignInStatus.Success:
                    {

                        if (isAdminUser(user))
                        {

                            return RedirectToAction("Index", "UsersAdmin");
                        }

                        if (isExpertUser(user))
                        {

                            return RedirectToAction("Index", "Expert");
                        }
                        if (isBookeperUser(user))
                        {

                            return RedirectToAction("Index", "Bookeper");
                        }
                        if (isSellerUser(user))
                        {

                            return RedirectToAction("Index", "Seller");
                        }
                        return RedirectToLocal(returnUrl);

                    }
                    
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        public Boolean isAdminUser(User user)
        {
           // var user = User.Identity;
            //GWAContext context = new GWAContext();
            //var UserManager = new UserManager<User>(new UserStore<User>(context));
            var s = UserManager.GetRoles(user.Id);
            if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
          
        }

        public Boolean isExpertUser(User user)
        {
           
                //var user = User.Identity;
                //GWAContext context = new GWAContext();
                //var UserManager = new UserManager<User>(new UserStore<User>(context));
                var s = UserManager.GetRoles(user.Id);
                if (s[0].ToString() == "Expert")
                {
                    return true;
                }
                else
                {
                    return false;
                }
           
        }

        public Boolean isBookeperUser(User user)
        {
           
                //var user = User.Identity;
                //GWAContext context = new GWAContext();
                //var UserManager = new UserManager<User>(new UserStore<User>(context));
                var s = UserManager.GetRoles(user.Id);
                if (s[0].ToString() == "bookkeeper")
                {
                    return true;
                }
                else
                {
                    return false;
                }
           
        }

        public Boolean isSellerUser(User user)
        {
           
                //var user = User.Identity;
                //GWAContext context = new GWAContext();
                //var UserManager = new UserManager<User>(new UserStore<User>(context));

                var s = UserManager.GetRoles(user.Id);
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
        // GET: /Account/VerifyCode   
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login   
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            
             ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains("Manager"))
                                            .ToList(), "Name", "Name");
          
            return View();
        }

        //
        // POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // var custEmail = FindEmail(model.ConfirmPassword);
        //        // var custUserName = FindUserName(model.);
        //        var user = new User
        //        {
        //            UserName = model.UserName,
        //            Email = model.Email,
        //            FirstName = model.FirstName,
        //            LastName = model.LastName,
        //            Country = model.Country,
        //            BirthDate = model.BirthDate,
        //            JoinDate = DateTime.Now
        //            ,
        //            EmailLinkDate = DateTime.Now,
        //            LAstLoginDate = DateTime.Now,





        //        };

        //        ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains("Manager"))
        //                                   .ToList(), "Name", "Name");
        //        //var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new GWAContext()));
        //        //manager.Create(user);

        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {

        //            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            //UserManager.AddToRole(user.Id, "Manager");
        //            //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            //var callbackUrl = Url.Action(
        //            //   "ConfirmEmail", "Account",
        //            //   new { userId = user.Id, code = code },
        //            //   protocol: Request.Url.Scheme);

        //            //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
        //            //// ViewBag.Link = callbackUrl;   // Used only for initial demo.


        //            //await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);

        //            //UserManager.Create(user);
        //            //context.SaveChanges();

        //            //return View("DisplayEmail");





        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // await UserManager.AddToRoleAsync(user.Id, model.UserRoles);
        //            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);




        //            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //            await UserManager.AddToRoleAsync(user.Id, model.UserRoles);
        //            return View("DisplayEmail");
        //            // return RedirectToAction("Index", "Home");



        //        }
        //        ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains("Manager"))
        //                                 .ToList(), "Name", "Name");
        //        AddErrors(result);
        //    }
        //    ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains("Manager"))
        //                                  .ToList(), "Name", "Name");
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


        // POST: /Account/Register   



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModelInsert model)
        {


            if (ModelState.IsValid)
            {
                User user = null;
                if(model.UserRoles== "Admin")
                {
                     user = new Manager
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Country = model.Country,
                        BirthDate = model.BirthDate,
                        JoinDate = DateTime.Now,
                        EmailLinkDate = DateTime.Now,
                        LAstLoginDate = DateTime.Now,
                    };

                }

                if (model.UserRoles == "Buyer")
                {
                    user = new Buyer
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Country = model.Country,
                        BirthDate = model.BirthDate,
                        JoinDate = DateTime.Now,
                        EmailLinkDate = DateTime.Now,
                        LAstLoginDate = DateTime.Now,
                    };

                }
                if (model.UserRoles == "Seller")
                {
                    user = new Seller
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Country = model.Country,
                        BirthDate = model.BirthDate,
                        JoinDate = DateTime.Now,
                        EmailLinkDate = DateTime.Now,
                        LAstLoginDate = DateTime.Now,
                    };
                }
                    if (model.UserRoles == "Expert")
                    {
                         user = new Expert
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Country = model.Country,
                            BirthDate = model.BirthDate,
                            JoinDate = DateTime.Now,
                            EmailLinkDate = DateTime.Now,
                            LAstLoginDate = DateTime.Now,
                        };

                    }
                if (model.UserRoles == "bookkeeper")
                {
                     user = new Bookkeeper
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Country = model.Country,
                        BirthDate = model.BirthDate,
                        JoinDate = DateTime.Now,
                        EmailLinkDate = DateTime.Now,
                        LAstLoginDate = DateTime.Now,
                    };

                }
               


                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771   
                    // Send an email with this link   
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);   
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);   
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");   
                    //Assign Role to user Here      
                   
                    //Ends Here   
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                    return View("DisplayEmail");
                    // return RedirectToAction("Index", "Users");
                }
                ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains("Admin"))
                                          .ToList(), "Name", "Name");
                AddErrors(result);
            }
            ViewBag.Name = new SelectList(context.Roles.Where(u => !u.Name.Contains("Admin"))
                                         .ToList(), "Name", "Name");
            // If we got this far, something failed, redisplay form   
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");

                //var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                //ViewBag.Link = callbackUrl;
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new User {

                    UserName = model.Email, Email = model.Email,
                    BirthDate = model.BirthDate,
                    HomeTown = model.HomeTown,
                   // UserName = model.Email,
                   // Email = model.Email,
                    //FirstName = model.FirstName,
                    //LastName = model.LastName,
                    //Country = model.Country,
                   // BirthDate = model.BirthDate,
                    JoinDate = DateTime.Now
                    ,
                    EmailLinkDate = DateTime.Now,
                    LAstLoginDate = DateTime.Now

                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Buyer");
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    await this.UserManager.AddToRoleAsync(user.Id, "Buyer");
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public static bool IsLiveMode { get; private set; }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }

           
        }
        #endregion

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {


            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject, "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            //string code1 = code.GeneratePasswordResetToken(userID);
            //string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(code1, Request);

            //bool emailSent = ForgotPassword(" khawaja Atteeq", Email.Text, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");

            return callbackUrl;
        }

        public static bool ForgotPassword(string fromAccount, string toAccount, string subject, string msg)
        {
            var ConfirmationMail = new MailMessage();
            if (IsLiveMode)
            {
                ConfirmationMail = new MailMessage("abc@abc.com", toAccount, subject, msg);
            }
            else
            {
                ConfirmationMail = new MailMessage("abc@abc.com", toAccount, subject, msg);
            }
            ConfirmationMail.Priority = MailPriority.High;
            ConfirmationMail.IsBodyHtml = true;
            SmtpClient objSMTPClient = new SmtpClient();
            try
            {
                objSMTPClient.Send(ConfirmationMail);
                return true;
            }
            catch
            {
                return false;
            }
        }
        void sendMail(IdentityMessage message)
        {
            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
            string html = "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a><br/>";

            html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("nourelimen.ouaganouni@esprit.tn");
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("nourelimen.ouaganouni@esprit.tn", "XXXXXX");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }

        // GET: /Account/Profil
       
      //  [AllowAnonymous]
        
        public ActionResult Profil()
        {
            var userId = User.Identity.GetUserId();
            User user = service.GetUser(userId);
            List<string> listRoles = service.GetUserRoles(user.Id);

            List<Seller> listuserSuiv= service.GetListSuivis(user.Id);

            var nbrSellerSuivre = service.GetListSuivisnbr(user.Id);
            ViewBag.nbrSellerSuivre = nbrSellerSuivre;
            var listProduct = service.GetListProduitAuctionInscriBayresN(user.Id);

            var listProductWin = service.GetListProduitAuctionWinBayres(user.Id);
            var listProductNOWin = service.GetListProduitAuctionNoWinBayres(user.Id);


            //List<User> listuserAbon = service.GetListAbonne(user.Id);

            var nbrUserAccAuction = service.GetnbrUserAcepterAuction(user.Id);

            
            

            //
            //var listProduct = service.GetListProduitAuctionSellSeller(user.Id);

            //var listProduct = service.GetListProduitAuctionNoSellSeller(user.Id);


         

            //ViewBag.GetListSuivisnbr = service.GetListSuivisnbr(user.Id);



            //ViewBag.GetListNotification = service.GetListNotification(user.Id);
            //ViewBag.GetnbrUserAcepterAuction = service.GetListNotification(user.Id);
            //ViewBag.GetListProduitAuctionInscriBayres = service.GetListNotification(user.Id);
            //ViewBag.GetListProduitAuctionInscriBayres = service.GetListNotification(user.Id);

            //ViewBag.GetListProduitAuctionWinBayres = service.GetListNotification(user.Id);
            //ViewBag.GetListProduitAuctionNoWinBayres = service.GetListNotification(user.Id);

            //ViewBag.GetListAttendSubAuctionBayres = service.GetListNotification(user.Id);
            //ViewBag.GetListAccepterSubAuctionBayres = service.GetListNotification(user.Id);
            //ViewBag.GetListProductFavories = service.GetListNotification(user.Id);



            //list de suivis des utilisateur pour seller
            List<RegisterViewModel> listuserSuivModel = new List<RegisterViewModel>();
            
            foreach (Seller item in listuserSuiv)
            {
               List<string> listRolesSuivis = service.GetUserRoles(item.Id);
                listuserSuivModel.Add(

                    new RegisterViewModel
                    {
                        
                        id = item.Id,
                        ConfirmPassword = item.ConfirmPassword,
                        HomeTown = item.HomeTown,
                        Password = item.Password,
                        RegisterEmail = item.RegisterEmail,
                        RegisterUserName = item.RegisterUserName,
                        UserRolesS =listRolesSuivis,
                        UserName=item.UserName,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Country = item.Country,
                        BirthDate = item.BirthDate,
                        JoinDate = item.JoinDate
                            ,
                        EmailLinkDate = item.EmailLinkDate,
                        LAstLoginDate = item.LAstLoginDate,
                        PersonnalDescription=item.PersonnalDescription,


                    }
                    );
            }



            //List<RegisterViewModel> listuserSuivAbonn = new List<RegisterViewModel>();

            //foreach (User item in listuserAbon)
            //{
            //    List<string> listRolesSuivis = service.GetUserRoles(item.Id);
            //    listuserSuivAbonn.Add(

            //        new RegisterViewModel
            //        {
            //            id = item.Id,
            //            ConfirmPassword = item.ConfirmPassword,
            //            HomeTown = item.HomeTown,
            //            Password = item.Password,
            //            RegisterEmail = item.RegisterEmail,
            //            RegisterUserName = item.RegisterUserName,
            //            UserRolesS = listRolesSuivis,

            //            Email = item.Email,
            //            FirstName = item.FirstName,
            //            LastName = item.LastName,
            //            Country = item.Country,
            //            BirthDate = item.BirthDate,
            //            JoinDate = item.JoinDate
            //                ,
            //            EmailLinkDate = item.EmailLinkDate,
            //            LAstLoginDate = item.LAstLoginDate,


            //        }
            //        );
            //}



            //List<ProductViewModel> listuserProductModel = new List<ProductViewModel>();
            List<ProductViewModel> pvm = new List<ProductViewModel>();
            foreach (var item in listProduct)
            {
                pvm.Add(
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
                        Description=item.Description
                        //IdUser = item.IdUser
                        //BestSeller = u
                    });
            }
            List<ProductViewModel> pvmWin = new List<ProductViewModel>();
            foreach (var item in listProductWin)
            {
                pvmWin.Add(
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
                        //IdUser = item.IdUser
                        //BestSeller = u
                    });
            }
            List<ProductViewModel> pvmNowin = new List<ProductViewModel>();
            foreach (var item in listProductNOWin)
            {
                pvmNowin.Add(
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
                        //IdUser = item.IdUser
                        //BestSeller = u
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
                JoinDate = user.JoinDate ,
                PhoneNumber=user.PhoneNumber,
                EmailLinkDate = user.EmailLinkDate,
                LAstLoginDate = user.LAstLoginDate,
                UserSuivre = listuserSuivModel,
                //UserAbonne= listuserSuivAbonn,
                nbrUserAccAuction=nbrUserAccAuction,
                ProductView= pvm,
                ProductViewWin=pvmWin,
                ProductViewNoWin=pvmNowin,
                ImageUrl =user.ImageUrl


            };


        //    LoginViewModel lvm = new LoginViewModel
        //    {
        //        uvm = uvmk,
        //        role = service.GetUserRoles(user.Id)[0] 

        //};
                
                return View(uvmk);
           
        }
        // Post : Account/Profil
        [HttpPost]
        public ActionResult Profil(FormCollection collection)
        {
            //try
            //{
            List<RegisterViewModel> list = new List<RegisterViewModel>();
            if (collection["searchStringC"] != null)
            {
                var Country = collection["searchStringC"];


                var result = service.GetListUserByCountry(Country);
               // List<RegisterViewModel> list = new List<RegisterViewModel>();
                //string nom=null;
                foreach (var item in result)
                {

                    List<string> listRolesSuivis = service.GetUserRoles(item.Id);
                    list.Add(

                        new RegisterViewModel
                        {
                            id = item.Id,
                            ConfirmPassword = item.ConfirmPassword,
                            HomeTown = item.HomeTown,
                            Password = item.Password,
                            RegisterEmail = item.RegisterEmail,
                            RegisterUserName = item.RegisterUserName,
                            UserRolesS = listRolesSuivis,
                            UserName = item.UserName,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Country = item.Country,
                            BirthDate = item.BirthDate,
                            JoinDate = item.JoinDate
                                ,
                            EmailLinkDate = item.EmailLinkDate,
                            LAstLoginDate = item.LAstLoginDate,
                            PersonnalDescription = item.PersonnalDescription,


                        }
                        );

                }
            }
            if (collection["searchStringN"] != null) {
                var FirstName = collection["searchStringN"];


                var result = service.GetListUserByFirstName(FirstName);
                
                //string nom=null;
                foreach (var item in result)
                {

                    List<string> listRolesSuivis = service.GetUserRoles(item.Id);
                    list.Add(

                        new RegisterViewModel
                        {
                            id = item.Id,
                            ConfirmPassword = item.ConfirmPassword,
                            HomeTown = item.HomeTown,
                            Password = item.Password,
                            RegisterEmail = item.RegisterEmail,
                            RegisterUserName = item.RegisterUserName,
                            UserRolesS = listRolesSuivis,
                            UserName = item.UserName,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Country = item.Country,
                            BirthDate = item.BirthDate,
                            JoinDate = item.JoinDate
                                ,
                            EmailLinkDate = item.EmailLinkDate,
                            LAstLoginDate = item.LAstLoginDate,
                            PersonnalDescription = item.PersonnalDescription,


                        }
                        );

                }
           

            }

            //RegisterViewModel m = new RegisterViewModel();
            //m.Email = nom;
            //list.Add(m);
            return PartialView("_SellerRechercher", list);
            //}
            //catch
            //{
            //    return View();
            //}


        }


        // GET: Account/Edit
        [AllowAnonymous]
        public ActionResult Edit()
        {
       
          
          var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.ImageUrl = user.ImageUrl;
            RegisterModel uvmk = new RegisterModel
            {
                id = user.Id,
                HomeTown = user.HomeTown,
                RegisterEmail = user.RegisterEmail,
                RegisterUserName = user.RegisterUserName,
              UserName=user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.Country,
                BirthDate = user.BirthDate,
              ImageUrl=user.ImageUrl
                    
               
              


            };

            //LoginViewModel lvm = new LoginViewModel
            //{
            //    uvm = uvmk

            //};
            //return View(lvm);

            return View(uvmk);


           
        }

        // POST : Account/Edit
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Edit(RegisterModel model, HttpPostedFileBase Image, string returnUrl)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(User.Identity.GetUserId());

            ViewBag.ImageUrl = Image.FileName;
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, true, shouldLockout: false);

            if (result == SignInStatus.Success)
            {
                model.ImageUrl = Image.FileName;
               
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Country = model.Country;
                user.BirthDate = model.BirthDate;
                user.JoinDate = DateTime.Now;
                user.ImageUrl = model.ImageUrl;
                user.EmailLinkDate = DateTime.Now;
                user.LAstLoginDate = DateTime.Now;

                var result1 = await UserManager.UpdateSecurityStampAsync(user.Id);
             
               
                var path = Path.Combine(Server.MapPath("~/Content/Upload/"), Image.FileName);
                Image.SaveAs(path);
                return View(model);
                //return Login(returnUrl);
                //return RedirectToAction("Index", "Home");
                //return View("index");
            }
            else
                return View();
            }
            //switch (result)
            //{


            //    case SignInStatus.Success:
            //        {
            //            user.UserName = model.uvm.Email;
            //            user.Email = model.uvm.Email;
            //            user.FirstName = model.uvm.FirstName;
            //            user.LastName = model.uvm.LastName;
            //            user.Country = model.uvm.Country;
            //            user.BirthDate = model.uvm.BirthDate;
            //            user.JoinDate = DateTime.Now;

            //            user.EmailLinkDate = DateTime.Now;
            //            user.LAstLoginDate = DateTime.Now;

            //            var result1 = await UserManager.UpdateSecurityStampAsync(user.Id);
            //            if (result1.Succeeded)
            //            {

            //                return RedirectToAction("Index", "Home");
            //            }
            //        AddErrors(result1);

            //        return RedirectToLocal(returnUrl);

            //        ///return View("index");

           



     
    


        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Desactiver(LoginViewModel model, HttpPostedFileBase Image, string returnUrl)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(User.Identity.GetUserId());


            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            if (result == SignInStatus.Success)
            {
                user.UserName = model.uvm.Email;
                user.Email = model.uvm.Email;
                user.FirstName = model.uvm.FirstName;
                user.LastName = model.uvm.LastName;
                user.Country = model.uvm.Country;
                user.BirthDate = model.uvm.BirthDate;
                user.JoinDate = DateTime.Now;

                user.EmailLinkDate = DateTime.Now;
                user.LAstLoginDate = DateTime.Now;

                var result1 = await UserManager.UpdateSecurityStampAsync(user.Id);
                if (result1.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
              
                }
                AddErrors(result1);

                return Login(returnUrl);
           
            }
            else
                return View();
          

        }




        // GET: Account/Edit
        [AllowAnonymous]
        public ActionResult test()
        {
            return View();
        }
        public ActionResult nour()
        {
            return View();
        }
    }
    }
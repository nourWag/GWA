using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Net;
using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Mail;
using Exceptions;
using System.Text;
using static IdentitySample.Models.EmailService;
using Twilio;
using GWA.WEB1.App_Start;
using GWA.Data.Context;
using GWA.Domaine.Entities;

namespace IdentitySample.Models
{
    
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> store)
            : base(store)
        {
        }
        public ApplicationUserManager(IUserStore<User> store, IIdentityMessageService emailService) : base(store)
        {
            this.EmailService = emailService;
            // the rest of configuration
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<User>(context.Get<GWAContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<User>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<User>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<GWAContext>()));
        }
    }
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // return configSendGridasync(message);
            // convert IdentityMessage to a MailMessage
            var email =
               new MailMessage(new MailAddress("nour12ouaganouni@gmail.com", "GWA"),
               new MailAddress(message.Destination))
               {
                   Subject = message.Subject,
                   Body = message.Body,
                   IsBodyHtml = true
               };
            var client = new SmtpClient();
            client.SendCompleted += (s, e) => {
                client.Dispose();
            };
            NetworkCredential basicCredential =
    new NetworkCredential("nour12ouaganouni@gmail.com", "123456rita*");
            MailMessage message1 = new MailMessage();
            MailAddress fromAddress = new MailAddress("nour12ouaganouni@gmail.com");

            client.Host = "smtp.gmail.com";
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential;

            message1.From = fromAddress;
            message1.Subject = "your subject";
            //Set IsBodyHtml to true means you can send HTML email.
            message1.IsBodyHtml = true;
            message1.Body = "<h1>your message body</h1>";
            message1.To.Add("to@anydomain.com");

               // client.Send(message1);

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("nour12ouaganouni@gmail.com");
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(mail.From.Address, "123456rita*");
            smtp.Host = "smtp.gmail.com";

            //recipient
            mail.To.Add(new MailAddress("nour12ouaganouni@gmail.com"));

            mail.IsBodyHtml = true;
            string st = "Test";

            mail.Body = st;
            //smtp.Send(mail);

            SmtpClient SmtpServer = new SmtpClient("smtp.live.com", 587);
            SmtpServer.Credentials = new NetworkCredential("nourelimen.ouaganouni@esprit.tn", "123456nour");
            SmtpServer.EnableSsl = true;
            SmtpServer.UseDefaultCredentials = false;
            //SmtpServer.Send(email);
            return client.SendMailAsync(email);
            //using (var client = new SmtpClient()) // SmtpClient configuration comes from config file
            //{
            //    return client.SendMailAsync(email);
            //}
        }

        private Task configSendGridasync(IdentityMessage message)
        {


            var myMessage = new SendGridMessage();
          
            //System.Console.Out.Write(message.Destination);
            myMessage.From = new MailAddress(
                                "nourelimen.ouaganouni@esprit.tn", "Nour wag");

            myMessage.AddTo(message.Destination);
           /// message.Subject = "this is the subject";
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;
            //myMessage.Subject=
           
            var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["nourelimen.ouaganouni@esprit.tn"],
                       ConfigurationManager.AppSettings["123456nour*"]
                       );

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            if (transportWeb != null)
            {
                return transportWeb.DeliverAsync(myMessage);
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        //void sendMail(Message message)
        //{
        //    #region formatter
        //    string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
        //    string html = "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a><br/>";

        //    html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
        //    #endregion

        //    MailMessage msg = new MailMessage();
        //    msg.From = new MailAddress("joe@contoso.com");
        //    msg.To.Add(new MailAddress(message.Destination));
        //    msg.Subject = message.Subject;
        //    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
        //    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

        //    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
        //    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("joe@contoso.com", "XXXXXX");
        //    smtpClient.Credentials = credentials;
        //    smtpClient.EnableSsl = true;
        //    smtpClient.Send(msg);
        //}
    }
    //public class EmailService : IIdentityMessageService
    //{
    //    public async Task SendAsync(IdentityMessage message)
    //    {
    //        await configSendGridasync(message);
    //        // return configSendGridasync(message);
    //    }

    //    //    private async Task configSendGridasync(IdentityMessage message)
    //    //    {
    //    //        var myMessage = new SendGridMessage();
    //    //        myMessage.AddTo(message.Destination);
    //    //        myMessage.From = new System.Net.Mail.MailAddress(
    //    //                            "Joe@contoso.com", "Joe S.");
    //    //        //myMessage.Subject = message.Subject;
    //    //        myMessage.Text = message.Body;
    //    //        myMessage.Html = message.Body;
    //    //        myMessage.Subject = "this is the subject";
    //    //        var credentials = new NetworkCredential(
    //    //                   ConfigurationManager.AppSettings["mailAccount"],
    //    //                   ConfigurationManager.AppSettings["mailPassword"]
    //    //                   );

    //    //        // Create a Web transport for sending email.
    //    //        var transportWeb = new Web(credentials);

    //    //        // Send the email.
    //    //        if (transportWeb != null)
    //    //        {
    //    //            // await transportWeb.DeliverAsync(myMessage);
    //    //            // await transportWeb.DeliverAsync(myMessage);
    //    //            try
    //    //            {
    //    //                await transportWeb.DeliverAsync(myMessage);
    //    //            }
    //    //            catch (InvalidApiRequestException ex)
    //    //            {
    //    //                var detalle = new StringBuilder();

    //    //                detalle.Append("ResponseStatusCode: " + ex.ResponseStatusCode + ".   ");
    //    //                for (int i = 0; i < ex.Errors.Count(); i++)
    //    //                {
    //    //                    detalle.Append(" -- Error #" + i.ToString() + " : " + ex.Errors[i]);
    //    //                }

    //    //                throw new ApplicationException(detalle.ToString(), ex);
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            Trace.TraceError("Failed to create Web transport.");
    //    //            await Task.FromResult(0);
    //    //        }
    //    //    }
    //    //}


    //    private async Task configSendGridasync(IdentityMessage message)
    //    {
    //        var smtp = new SmtpClient(GWA.WEB1.Properties.Resources.SendGridURL, 587);

    //        var creds = new NetworkCredential(GWA.WEB1.Properties.Resources.SendGridUser, GWA.WEB1.Properties.Resources.SendGridPassword);

    //        smtp.UseDefaultCredentials = false;
    //        smtp.Credentials = creds;
    //        smtp.EnableSsl = false;

    //        var to = new MailAddress(message.Destination);
    //        var from = new MailAddress("info@ycc.com", "Your Contractor Connection");

    //        var msg = new MailMessage();

    //        msg.To.Add(to);
    //        msg.From = from;
    //        msg.IsBodyHtml = true;
    //        msg.Subject = message.Subject;
    //        msg.Body = message.Body;

    //        await smtp.SendMailAsync(msg);
    //    }

    //    public class SmsService : IIdentityMessageService
    //    {
    //        public Task SendAsync(IdentityMessage message)
    //        {
    //            // Plug in your sms service here to send a text message.
    //            return Task.FromResult(0);
    //        }




    //    }
    //}

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes


    //public class SmsService : IIdentityMessageService
    //{
    //    //public Task SendAsync(IdentityMessage message)
    //    //{
    //    //    // // Twilio Begin
    //    //    // var Twilio = new TwilioRestClient(
    //    //    // Keys.SMSAccountIdentification,
    //    //    //   Keys.SMSAccountPassword);
    //    //    // var result = Twilio.SendMessage(
    //    //    //   Keys.SMSAccountFrom,
    //    //    //   message.Destination, message.Body
    //    //    // );
    //    //    // //Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
    //    //    //  Trace.TraceInformation(result.Status);
    //    //    //// Twilio doesn't currently have an async API, so return success.
    //    //    //  return Task.FromResult(0);
    //    //    // Twilio End

    //    //    // ASPSMS Begin 
    //    //    // var soapSms = new WebApplication1.ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
    //    //    // soapSms.SendSimpleTextSMS(
    //    //    //   Keys.SMSAccountIdentification,
    //    //    //   Keys.SMSAccountPassword,
    //    //    //   message.Destination,
    //    //    //   Keys.SMSAccountFrom,
    //    //    //   message.Body);
    //    //    // soapSms.Close();
    //    //    // return Task.FromResult(0);
    //    //    // ASPSMS End
           
    //    //}
       
    //}

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<GWAContext> 
    {
        protected override void Seed(GWAContext context) {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(GWAContext db) {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null) {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null) {
                user = new User { UserName = name, Email = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name)) {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }

    public class ApplicationSignInManager : SignInManager<User, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

       
    }
}
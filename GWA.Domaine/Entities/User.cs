using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
    public class User : IdentityUser
    {

        public DateTime AcountCreationDate { get; set; }
       // public String Email { get; set; }
        public String ConfirmPassword { get; set; }
        public string Password { get; set; }
      //public int PhoneNumber { get; set; }


        [DataType(DataType.ImageUrl), Display(Name = "Image")]
        public string ImageUrl { get; set; }


        public string RegisterEmail { get; set; }
        public string RegisterUserName { get; set; }
       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime EmailLinkDate { get; set; }
        public DateTime LAstLoginDate { get; set; }
        public string HomeTown { get; set; }
        public System.DateTime? BirthDate { get; set; }
        public virtual Session Session { get; set; }
        public virtual ICollection<Product> UserProducts { get; set; }
        public bool activer { get; set; }
        

        public virtual ICollection<Seller> SellerSuiv { get; set; }

        public virtual ICollection<Subscription> Subscription { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<Auction> SellerAuctions { get; set; }

        public virtual ICollection<Token> Tokens { get; set; }
        public String PersonnalDescription { get; set; }



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;


namespace IdentitySample.Models
{
    //You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    public string RegisterEmail { get; set; }
    //    public string RegisterUserName { get; set; }

    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Country { get; set; }
    //    public DateTime JoinDate { get; set; }
    //    public DateTime EmailLinkDate { get; set; }
    //    public DateTime LAstLoginDate { get; set; }
    //    public string HomeTown { get; set; }
    //    public System.DateTime? BirthDate { get; set; }
    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //        // Add custom user claims here

    //        return userIdentity;
    //    }
    //}
    ////[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    ////public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    ////{
    ////    public ApplicationDbContext()
    ////        : base("GWADB", throwIfV1Schema: false)
    ////    {
    ////    }

    ////    //static ApplicationDbContext()
    ////    //{
    ////    //    // Set the database intializer which is run once during application start
    ////    //    // This seeds the database with admin user credentials and admin role
    ////    //    Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
    ////    //}

    ////    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}
}
using GWA.Data.Context;
using GWA.Domaine.Entities;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using System;
using System.Threading.Tasks;


namespace IdentitySample
{
    public partial class Startup
    {

        
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

        }
        private void createRolesandUsers()
        {

         GWAContext context = new GWAContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<User>(new UserStore<User>(context));



            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                //var user = new User();
                //user.UserName = "shanu";
                //user.Email = "syedshanumcain@gmail.com";

                //string userPWD = "A@Z200711";

                //var chkUser = UserManager.Create(user, userPWD);

                ////Add default User to Role Admin   
                //if (chkUser.Succeeded)
                //{
                //    var result1 = UserManager.AddToRole(user.Id, "Manager");

                //}
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Buyer"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Buyer";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Seller"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Seller";
                roleManager.Create(role);

            }
            // creating Creating Manager role    
            if (!roleManager.RoleExists("bookkeeper"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "bookkeeper";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Expert"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Expert";
                roleManager.Create(role);

            }
        }

    }
}

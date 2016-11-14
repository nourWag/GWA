using GWA.Data.Infrastructure;
using GWA.Domaine.Entities;
using GWA.Service.Pattern;
using Microsoft.AspNet.Identity;

using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using GWA.Data.Context;
using System.Data.Linq.SqlClient;
using System.Linq.Expressions;
using GWA.Service.Helpers;
using System.Data.SqlClient;
using System.Data;

namespace GWA.Service.UserService.Service
{
   public class ServiceUser : Service<User>
    {
        private static IUnitOfWork utwk = new UnitOfWork(new DatabaseFactory());

        public ServiceUser() : base(utwk)
        {

        }
        GWAContext context = new GWAContext();


      

        public List<string> GetUserRoles(string username)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new Microsoft.AspNet.Identity.UserManager<User>(new UserStore<User>(context));
            List<string> ListOfRoleNames = new List<string>();
            var ListOfRoleIds =UserManager.FindById(username).Roles.Select(x => x.RoleId).ToList();
            foreach (string id in ListOfRoleIds)
            {
                string rolename = roleManager.FindById(id).Name;
                ListOfRoleNames.Add(rolename);
            }

            return ListOfRoleNames;
           
        }

        // Seller

        public List<User> GetListAbonne(string username)
        {
            var UserManager = new UserManager<Seller>(new UserStore<Seller>(context));


            var user = UserManager.FindById(username);
            var query = from users in user.BayersAbonnées
                        where users.SellerSuiv.Any(c => c.Id == username)
                        select users;
            return query.ToList();

        }
        //Seller

        public int GetListAbonnenbr(string username)
        {
            var UserManager = new UserManager<Seller>(new UserStore<Seller>(context));


            var user = UserManager.FindById(username);
            var query = from users in user.BayersAbonnées
                        where users.SellerSuiv.Any(c => c.Id == username)
                        select users;
            return query.ToList().Count();

        }

        public List<Seller> GetListSuivis(string username)
        {
            var UserManager = new UserManager<Seller>(new UserStore<Seller>(context));
            var UserManagerBuyer = new UserManager<Buyer>(new UserStore<Buyer>(context));

            var user = UserManagerBuyer.FindById(username);
            var query = from users in user.SellerSuiv
                        where users.BayersAbonnées.Any(c => c.Id == username)
                        select users;
            return query.ToList();


        }

        public int GetListSuivisnbr(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            

            var user = UserManager.FindById(username);
            var query = from users in user.SellerSuiv
                        where users.BayersAbonnées.Any(c => c.Id == username)
                        select users;
            return query.ToList().Count();
        }

        //Seller trouver nombre de user accepter son auction
       
        public int GetnbrUserAcepterAuction(string username)
        {
           
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(username);
            Subscription subsc = new Subscription();
            List<Auction> listAuction = user.SellerAuctions.ToList();
            Auction auction = new Auction();
           
            var categorizedProducts =
      from u in context.auctions
      join sub in context.subscription on u.Id equals sub.Auction.Id 
      where sub.AccepterUser==true 
      select new
      {
          UserId = u.Currentwinner.Id, // or pc.ProdId
         
        
      }
      ;

            return categorizedProducts.Count();

        }
        public List<User> GetUserAcepterAuction(string username)
        {

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(username);
            Subscription subsc = new Subscription();
            List<Auction> listAuction = user.SellerAuctions.ToList();
            Auction auction = new Auction();

            var listUser =
      from u in context.auctions
      join sub in context.subscription on u.Id equals sub.Auction.Id
      join product in context.products on u.product.Id equals product.Id
      where sub.AccepterUser == true && product.User.Id == user.Id
      select product.User;
            List<User> list = listUser.ToList();
            return listUser.ToList();

        }

        // pour bayres 
        public List<Product> GetListProduitAuctionInscriBayresN(string username)
        {

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(username);
            Subscription subsc = new Subscription();
            List<Auction> listAuction = user.SellerAuctions.ToList();
            Auction auction = new Auction();
       
            var Products =
                    from p in context.products
                    join au in context.auctions on p.Id equals au.product.Id
                    join sub in context.subscription on au.Id equals sub.Auction.Id
                    where sub.User.Id == user.Id
                    select p;

            
            List<Product> list = Products.ToList();
            return Products.ToList();

        }

        public List<Product> GetListProduitAuctionWinBayres(string username)
        {

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(username);
            Subscription subsc = new Subscription();
            List<Auction> listAuction = user.SellerAuctions.ToList();
            Auction auction = new Auction();

            var Products =
                    from p in context.products
                    join au in context.auctions on p.Id equals au.IdProduct
                    join sub in context.subscription on au.Id equals sub.Auction.Id
                    where sub.User.Id == user.Id && sub.Win==true
                    select p;


            List<Product> list = Products.ToList();
            return list;





        }


        public List<Product> GetListProduitAuctionNoWinBayres(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(username);
            Subscription subsc = new Subscription();
            List<Auction> listAuction = user.SellerAuctions.ToList();
            Auction auction = new Auction();

            var Products =
                    from p in context.products
                    join au in context.auctions on p.Id equals au.IdProduct
                    join sub in context.subscription on au.Id equals sub.Auction.Id
                    where sub.User.Id == user.Id && sub.Win == false
                    select p;


            List<Product> list = Products.ToList();
            return Products.ToList();


        }


        public List<Product> GetListProduitAuctionSellSeller(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(username);
           

            var Products =
                    from a in context.auctions
                    join au in context.products on a.product.Id equals au.Id
                    where a.validator==1 && au.User.Id==user.Id
                    select au;

            List<Product> listP= Products.ToList();
            return Products.ToList();
       


        }

        public List<Product> GetListProduitAuctionNoSellSeller(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(username);
           
            var Products =
                    from a in context.auctions
                    join au in context.products on a.product.Id equals au.Id 
                    where a.validator == 0 && au.User.Id == user.Id
                    select au;


            return Products.ToList();




        }


        public IEnumerable<Seller> GetListUserByCountry(string Country)
        {

            //        var query = from users in context.Users
            //                    where users.Country.Equals(Country) && users is Seller
            //                    select users ;
            //        var result2 = context.Users
            //.Select(b => b is Seller && b.Country.Equals(Country))
            //.ToList();
            var result3 = utwk.getRepository<Seller>();
            var query4 = from users in result3.GetAll()
                        where users.Country.Equals(Country) 
                        select users;
            //.GetMany().Where(k => k.Country.Equals(Country)).ToList();
            //}
            List<Seller> list = query4.ToList();
            return list;

        }

        public List<Seller> GetListUserByFirstName(string FirstName)
        {


            var result3 = utwk.getRepository<Seller>();

            var query = from users in result3.GetAll()
                        where users.UserName.Contains(FirstName)
                          orderby users.Note descending
                          select users;

            List<Seller> listS = new List<Seller>();
            foreach (var item in query)
            {
                if (item is Seller)
                    listS.Add((Seller)item);

            }
         
            return listS;



        }

        public List<Notification> GetListNotification(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            //List<string> ListOfRoleNames = new List<string>();
            //string s;
            //s = UserManager.FindByName(username).Roles.Select(x => x.RoleId).First();

            return UserManager.FindByName(username).Notifications.ToList();

        }

      


       
       


      


      



      
 



        public List<Subscription> GetListAttendSubAuctionBayres(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindByName(username);

            return UserManager.FindByName(username).Subscription.Where(c => c.AccepterUser==false).ToList();




        }


        public List<Subscription> GetListAccepterSubAuctionBayres(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindByName(username);

            return UserManager.FindByName(username).Subscription.Where(c => c.AccepterUser == true).ToList();

        }


        public List<Product> GetListProductFavories(string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindByName(username);

            return UserManager.FindByName(username).UserProducts.Where(c=>c.Favorie==true).ToList();

        }


        public int GetvalueAuctionTokenBayres(int idAuction,string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));

           // var utwk.getRepository<User>().
            var user = UserManager.FindByName(username);

         var subs = UserManager.FindByName(username).Subscription.Where(c=>c.Auction.Id==idAuction);
            var auction = subs.First().Auction;
            return auction.valueAuctionToken;

        }



        public int GetvalueAuctionTokenNeedBayres(int idAuction, string username)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));

            // var utwk.getRepository<User>().
            var user = UserManager.FindByName(username);

            var subs = UserManager.FindByName(username).Subscription.Where(c => c.Auction.Id == idAuction);
            var auction = subs.First().Auction;
            int valueBayres = 0;
            foreach (var item in user.Tokens)
            {
                valueBayres = item.valueToken + valueBayres;
            }
            return auction.valueAuctionToken-valueBayres;

        }




        public Product GetListAuction(string username, int idAuction)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));

            var auction = UserManager.FindByName(username).Subscription.Where(c => c.Auction.Id == idAuction && c.AccepterUser == true).First();


            return auction.Auction.product;

        }
        public User SuivreSaller(string username,string usernameSuivre)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));

            var SellerManager = new UserManager<Seller>(new UserStore<Seller>(context));


            var user = UserManager.FindByName(username);
            var userSuivre = SellerManager.FindByName(usernameSuivre);
            userSuivre.BayersAbonnées.Add(user);
            user.SellerSuiv.Add(userSuivre);
            return (userSuivre);

        }


        //public User AbonnéeAjouter(string username, string usernameSuivre)
        //{
        //    var UserManager = new UserManager<User>(new UserStore<User>(context));

        //    var SellerManager = new UserManager<Seller>(new UserStore<Seller>(context));
        //    var user = UserManager.FindByName(username);
        //    var userSuivre = SellerManager.FindByName(usernameSuivre);
        //    user.BayersAbonnées.Add(userSuivre);
        //    return (userSuivre);

        //}


        public bool SuivreSeller(string username, string usernameSuivre)
        {

            var UserManager = new UserManager<Seller>(new UserStore<Seller>(context));


            var user = UserManager.FindByName(username);
            var userSuiv = UserManager.FindByName(usernameSuivre);
            user.SellerSuiv.Add(userSuiv);
            userSuiv.BayersAbonnées.Add(user);
            UserManager.Update(userSuiv);
            UserManager.Update(user);

            return true;
           

        }
        public User GetUser(string id)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
           

            return UserManager.FindById(id);
        }

        //public IEnumerable<User> GetUserByRole(string Role)
        //{
        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        //    var userStore = new Mock<IUserStore<ApplicationUser>>();
        //    var userManager = new UserManager(userStore.Object);
        //    return utwk.getRepository<User>().GetAll().Where(c => c == Role).ToList();
        //}


        //public IEnumerable<User> GetUserAuction(int idAuction)
        //{
        //    return utwk.getRepository<User>().GetAll().Where(k => k.actions.Where(c => c.Id == idAuction).ToList() != null).ToList();
        //}


        //public int numberUserAuction(int idAuction)
        //{
        //    return utwk.getRepository<User>().GetAll().Where(k => k.actions.Where(c => c.Id == idAuction).ToList() != null).ToList().Count();
        //}

        //public void participateUserAuction(int idAuction, int idBayer)
        //{
        //    Subscription sub = new Subscription();
        //    sub.DateOfSubscription = DateTime.Now;
        //    //sub.IdAuction = idAuction;
        //    //sub.IdBayer = idBayer;
        //    //utwk.getRepository<Subscription>().Add(sub);

        //}


    }
}

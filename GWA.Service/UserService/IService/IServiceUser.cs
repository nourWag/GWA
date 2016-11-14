using GWA.Domaine.Entities;
using GWA.Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Service.UserService.IService
{
    public interface IServiceUser : IService<User>
    {
       
        List<string> GetUserRoles(string username);
        List<User> GetListAbonne(string username);
        int GetListAbonnenbr(string username);
        List<User> GetListSuivis(string username);
        int GetListSuivisnbr(string username);
        int GetnbrUserAcepterAuction(string username, int idAuction);

        List<Product> GetListProduitAuctionInscriBayresN(string username);
        List<Product> GetListProduitAuctionWinBayres(string username);
        List<Product> GetListProduitAuctionNoWinBayres(string username);
        List<Product> GetListProduitAuctionSellSeller(string username);
        List<Product> GetListProduitAuctionNoSellSeller(string username);
        List<User> GetListUserByCountry(string Country);
        List<User> GetListUserByFirstName(string FirstName);
    }
}

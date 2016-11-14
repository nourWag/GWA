using GWA.Domaine.Entities;
using GWA.Service.UserService.Service;
using GWA.WEB1.Models;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GWA.API.Controllers
{
    public class UsersController : ApiController
    {

        ServiceUser us = null;

        public UsersController()
        {
            us = new ServiceUser();
        }
        // GET: api/Users
        public IEnumerable<RegisterViewModel> Get(string idUser)
        {
            IEnumerable<Seller> p = us.GetListSuivis(idUser);
           

            return null;
        }

        // GET: api/Users/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Users
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Users/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }
    }
}

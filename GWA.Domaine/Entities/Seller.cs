using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
   public  class Seller : User
    {
        public int Note { get; set; }
        public virtual ICollection<User> BayersAbonnées { get; set; }
    }
}

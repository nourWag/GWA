using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
    public class Subscription
    {
       
        [Key, Column(Order = 0)]
        public DateTime DateOfSubscription { get; set; }
        [Key,Column(Order = 1)]
        public virtual User User { get; set; }

        [Key, Column(Order = 0)]
        public virtual Auction Auction { get; set; }

        public bool AccepterUser { get; set; }

        public bool Win { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
    public enum Style
    {
        English,
        Yankee,
        AggregateDemand,
        NegociatedPrice,
        Dutch,

    }
    public class Auction
    {
        public int Id { get; set; }
        public int AuctionValue { get; set; }
        public DateTime AuctionTimer { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndtDate { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public User Currentwinner { get; set; }
        public int FinalPrice { get; set; }
        public int validator { get; set; }
        [ForeignKey("product")]
        public int IdProduct { get; set; }
        public virtual Product product { get; set; }
        public Style Style { get; set; }

        public virtual ICollection<Subscription> Subscription { get; set; }

        public int valueAuctionToken { get; set; }

        public bool status { get; set; }

    }


}

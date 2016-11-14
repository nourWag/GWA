using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public virtual User User { get; set; }
        public virtual Auction Auction { get; set; }
    }
}

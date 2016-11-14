using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
    public class Buyer : User
    {
        public String Type { get; set; }
       
        public int NbrSignal { get; set; }
    }
}

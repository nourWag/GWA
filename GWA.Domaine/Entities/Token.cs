using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
    public class Token
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public virtual User User { get; set; }
        public int valueToken { get; set; }
    }
}

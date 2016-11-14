using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Domaine.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public bool status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string reference { get; set; }
        public float CurrentPrice { get; set; }

        
        public string ImageUrl { get; set; }



        //[ForeignKey("IdentityUser")]
        //public int IdUser { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Category")]
        public int IdCategory { get; set; }
        public virtual Category Category { get; set; }

        public string Description { get; set; }

        public bool Favorie { get; set; }
    }
}

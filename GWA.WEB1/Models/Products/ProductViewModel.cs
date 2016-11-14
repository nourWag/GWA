using GWA.Domaine.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GWA.WEB1.Models.Product
{
    public class ProductViewModel
    {
        
        public int Id { get; set; }
        public String Name { get; set; }
        public bool status { get; set; }

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        
        public string reference { get; set; }
        [DataType(DataType.Currency)]
        public float CurrentPrice { get; set; }

        [DataType(DataType.ImageUrl), Display(Name = "Image")]
        public string ImageUrl { get; set; }


        public string IdUser { get; set; }

        [Display(Name = "Categories")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Category { get; set; }


        public User BestSeller { get; set; }
        public string Description { get; set; }

    }
}
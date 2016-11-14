using GWA.WEB1.Models.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace GWA.WEB1.Models
{
    public class RegisterModel 
    {
        [Key]
        public string id { get; set; }
        [Required]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }

        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string RegisterUserName { get; set; }
        public string RegisterEmail { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime EmailLinkDate { get; set; }
        public DateTime LAstLoginDate { get; set; }
        public string HomeTown { get; set; }
        public System.DateTime? BirthDate { get; set; }
        public IEnumerable<SelectListItem> UserRoless { get; set; }
     

        public int nbrUserAccAuction { get; set; }
        public string PersonnalDescription { get; set; }

        [DataType(DataType.ImageUrl), Display(Name = "Image")]
        public string ImageUrl { get; set; }




    }
}
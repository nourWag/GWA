using GWA.WEB1.Models.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections;

namespace IdentitySample.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string HomeTown { get; set; }
        public System.DateTime? BirthDate { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
        public bool RememberMe { get; internal set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {

        [Required]
        [Display(Name = "UserName")]

        public string UserName { get; set; }
        //[Required]
        //[Display(Name = "Email")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public RegisterViewModel uvm { get; set; }
        public IndexViewModel IndexViewModel { get; set; }
        public AddPhoneNumberViewModel AddPhoneNumberViewModel { get; set; }
        public VerifyPhoneNumberViewModel VerifyPhoneNumberViewModel { get; set; }
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
        public SetPasswordViewModel SetPasswordViewModel { get; set; }
        public ManageLoginsViewModel ManageLoginsViewModel { get; set; }
        public string role { get; set; }







    }

    public class RegisterViewModel 
    {
        [Key]
        public string id { get; set; }
         [DataType(DataType.ImageUrl), Display(Name = "Image")]
        public string ImageUrl { get; set; }


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
        public List<string> UserRolesS { get; set; }
        public List<RegisterViewModel> UserSuivre { get; set; }
        public List<RegisterViewModel> UserAbonne { get; set; }
        public List<ProductViewModel> ProductView { get; set; }
      
        public int nbrUserAccAuction { get; set; }
        public string PersonnalDescription { get; set; }
        public List<ProductViewModel> ProductViewWin { get; set; }
        public List<ProductViewModel> ProductViewNoWin { get; set; }
        public string PhoneNumber { get; set; }
        public List<ProductViewModel> ProductAcc { get; set; }

        public List<ProductViewModel> ProductNoSe { get; set; }

        public List<RegisterViewModel> listUserAccAuctionM { get; set; }
        


    }


    public class RegisterViewModelInsert
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
      
      
    }

    public class ResetPasswordViewModel
    {
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

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
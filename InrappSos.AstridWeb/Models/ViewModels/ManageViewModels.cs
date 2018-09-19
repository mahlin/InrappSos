using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
        public string Namn { get; set; }
        public List<RegisterInfo> RegisterList { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Fältet lösenord är obligatoriskt.")]
        [StringLength(100, ErrorMessage = "{0} måset vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password, ErrorMessage = "Ogiltigt lösenord.")]
        [Display(Name = "Nytt lösenord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("NewPassword", ErrorMessage = "Lösenordet och verifieringen av lösenordet stämmer inte.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Fältet lösenord är obligatoriskt.")]
        [DataType(DataType.Password, ErrorMessage = "Ogiltigt lösenord.")]
        [Display(Name = "Nuvarande lösenord")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Fältet lösenord är obligatoriskt.")]
        [StringLength(100, ErrorMessage = "{0} måset vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password, ErrorMessage = "Ogiltigt lösenord.")]
        [Display(Name = "Nytt lösenord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("NewPassword", ErrorMessage = "Lösenordet och verifieringen av lösenordet stämmer inte.")]
        public string ConfirmPassword { get; set; }
    }

   
    public class ChangeNameViewModel
    {
        [Required(ErrorMessage = "Fältet Namn är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "Namnet kan inte vara längre än 60 tecken.")]
        [Display(Name = "Namn")]
        public string Name { get; set; }
    }

    
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}
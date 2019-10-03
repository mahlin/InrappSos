using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace InrappSos.FilipWeb.Models.ViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactNumber { get; set; }
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
        [Required(ErrorMessage = "Fältet pinkod är obligatoriskt.")]
        [StringLength(4, ErrorMessage = "{0} måste vara {2} tecken långt.", MinimumLength = 4)]
        [RegularExpression("([0-9]+)", ErrorMessage = "Pinkoden måste vara numerisk.")]
        [DataType(DataType.Password, ErrorMessage = "Ogiltig pinkod.")]
        [Display(Name = "Ny pinkod")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta pinkod")]
        [Compare("NewPassword", ErrorMessage = "Pinkoden och verifieringen av pinkoden stämmer inte.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Fältet pinkod är obligatoriskt.")]
        [DataType(DataType.Password, ErrorMessage = "Ogiltig pinkod.")]
        [Display(Name = "Nuvarande pinkod")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Fältet pinkod är obligatoriskt.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Pinkoden måste vara numerisk")]
        [StringLength(4, ErrorMessage = "{0} måste vara {2} tecken långt.", MinimumLength = 4)]
        [DataType(DataType.Password, ErrorMessage = "Ogiltig pinkod.")]
        [Display(Name = "Ny pinkod")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta pinkod")]
        [Compare("NewPassword", ErrorMessage = "Pinkoden och verifieringen av pinkoden stämmer inte.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Fältet Mobilnummer är obligatoriskt.")]
        [Phone(ErrorMessage = "Inte ett giltigt mobilnummer")]
        [Display(Name = "Mobilnummer")]
        public string Number { get; set; }
    }

    public class AddContactNumberViewModel
    {
        [Required(ErrorMessage = "Fältet Telefonnummer är obligatoriskt.")]
        [Phone(ErrorMessage = "Inte ett giltigt telefonnummer")]
        [Display(Name = "Telefonnummer")]
        public string Number { get; set; }
    }

    public class ChangeNameViewModel
    {
        [Required(ErrorMessage = "Fältet Namn är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "Namnet kan inte vara längre än 60 tecken.")]
        [Display(Name = "Namn")]
        public string Name { get; set; }
    }


    public class VerifyPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Fältet Kod är obligatoriskt.")]
        [Display(Name = "Kod")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Fältet Mobilnummer är obligatoriskt.")]
        [Phone (ErrorMessage = "Inte ett giltigt mobilnummer")]
        [Display(Name = "Mobilnummer")]
        public string PhoneNumber { get; set; }
        public string PhoneNumberMasked { get; set; }
    }

    public class SubdirViewModel
    {
        public int Id { get; set; }
        public int RegisterId { get; set; }
        public string Delregisternamn { get; set; }
        public string Kortnamn { get; set; }
        public string Beskrivning { get; set; }
        public List<OrgUnitViewModel> OrgUnitList{ get; set; }
    }

    public class OrgUnitViewModel
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public string Enhetsnamn { get; set; }
        public string Enhetskod { get; set; }
        public bool Selected { get; set; } = false;
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}
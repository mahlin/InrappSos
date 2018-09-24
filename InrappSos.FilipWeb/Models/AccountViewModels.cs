using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using InrappSos.DomainModel;

namespace InrappSos.FilipWeb.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }
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
        public bool RememberMe { get; set; }
        public string UserEmail { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required (ErrorMessage = "Fältet Provider är obligatoriskt.")]
        public string Provider { get; set; }

        [Required(ErrorMessage = "Fältet Kod är obligatoriskt.")]
        [Display(Name = "Kod")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Kom ihåg den här webbläsaren?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }

        public string UserEmail { get; set; }

        public string PhoneNumberMasked { get; set; }
    }


    public class RegisterVerifyPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Fältet Kod är obligatoriskt.")]
        [Display(Name = "Kod")]
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public string Id { get; set; }
    }

    public class RegisterPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Fältet Mobilnummer är obligatoriskt.")]
        [Phone(ErrorMessage = "Inte ett giltigt mobilnummer")]
        [Display(Name = "Mobilnummer")]
        public string Number { get; set; }
        public string Id { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "E-postadressen kan inte vara längre än 60 tecken.")]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "E-postadressen kan inte vara längre än 60 tecken.")]
        [Display(Name = "E-postadress")]
        [EmailAddress(ErrorMessage = "Fältet E-postadress är inte en giltig e-postadress.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Fältet pinkod är obligatoriskt.")]
        [DataType(DataType.Password, ErrorMessage = "Ogiltig pinkod.")]
        [Display(Name = "Pinkod")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }

        public bool DisabledAccount { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Fältet Namn är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "Namnet kan inte vara längre än 60 tecken.")]
        [Display(Name = "Namn")]
        public string Namn { get; set; }

        [Phone(ErrorMessage = "Inte ett giltigt telefonnummer")]
        [Display(Name = "Telefon arbete")]
        public string Telefonnummer { get; set; }

        [Required (ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "E-postadressen kan inte vara längre än 60 tecken.")]
        [EmailAddress(ErrorMessage = "Fältet E-postadress är inte en giltig e-postadress.")]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }

        [Required (ErrorMessage = "Fältet pinkod är obligatoriskt.")]
        [StringLength(4, ErrorMessage = "{0} måste vara {2} tecken långt.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [RegularExpression("([0-9]+)", ErrorMessage = "Pinkoden måste vara numerisk.")]
        [Display(Name = "Ange egen pinkod")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta pinkod")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Pinkoden och verifieringen av pinkoden stämmer inte.")]
        public string ConfirmPassword { get; set; }
        public List<RegisterInfo> RegisterList { get; set; }
    }


    public class ResetPasswordViewModel
    {
        [Required (ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "E-postadressen kan inte vara längre än 60 tecken.")]
        [EmailAddress(ErrorMessage = "Fältet E-postadress är inte en giltig e-postadress.")]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Fältet pinkod är obligatoriskt.")]
        [StringLength(4, ErrorMessage = "{0} måste vara {2} tecken långt.", MinimumLength = 4)]
        [RegularExpression("([0-9]+)", ErrorMessage = "Pinkoden måste vara numerisk.")]
        [DataType(DataType.Password, ErrorMessage = "Ogiltig pinkod.")]
        [Display(Name = "Pinkod")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta pinkod")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Pinkoden och verifieringen av pinkoden stämmer inte.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required (ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "E-postadressen kan inte vara längre än 60 tecken.")]
        [EmailAddress(ErrorMessage = "Fältet E-postadress är inte en giltig e-postadress.")]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using InrappSos.DomainModel;

namespace InrappSos.AstridWeb.Models
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

        [Required(ErrorMessage = "Fältet lösenord är obligatoriskt.")]
        [DataType(DataType.Password, ErrorMessage = "Ogiltigt lösenord.")]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Required (ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
        [StringLength(60, ErrorMessage = "E-postadressen kan inte vara längre än 60 tecken.")]
        [EmailAddress(ErrorMessage = "Fältet E-postadress är inte en giltig e-postadress.")]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }

        [Required (ErrorMessage = "Fältet lösenord är obligatoriskt.")]
        [StringLength(100, ErrorMessage = "{0} måset vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Lösenordet och verifieringen av lösenordet stämmer inte.")]
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

        [Required(ErrorMessage = "Fältet lösenord är obligatoriskt.")]
        [StringLength(100, ErrorMessage = "{0} måset vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password, ErrorMessage = "Ogiltigt lösenord.")]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Lösenordet och verifieringen av lösenordet stämmer inte.")]
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InrappSos.FilipWeb.Models;
using InrappSos.DataAccess;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Helpers;
using InrappSos.ApplicationService.Interface;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace InrappSos.FilipWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private CustomIdentityResultErrorDescriber _errorDecsriber;
        private readonly IPortalSosService _portalService;

        public AccountController()
        {
            _errorDecsriber = new CustomIdentityResultErrorDescriber();
            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _errorDecsriber = new CustomIdentityResultErrorDescriber();
            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //Kolla om öppet, annars visa stängt-sida
            if (!_portalService.IsOpen())
            {
                ViewBag.Text = _portalService.HamtaInfoText("Stangtsida").Text;
                return View("Closed");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                //Kolla om öppet, annars visa stängt-sida
                if (!_portalService.IsOpen())
                {
                    ViewBag.Text = _portalService.HamtaInfoText("Stangtsida").Text;
                    return View("Closed");
                }
                //Add this to check if the email was confirmed.
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Felaktigt användarnamn eller pinkod.");
                    return View(model);
                }
                //Check if users organisation or user has been disabled
                var userOrg = _portalService.HamtaOrgForAnvandare(user.Id);
                if (userOrg.AktivTom <= DateTime.Now)
                {
                    ModelState.AddModelError("", "Organisationen " + userOrg.Organisationsnamn + " är inaktiv, ta kontakt med " + ConfigurationManager.AppSettings["MailSender"]);
                    return View(model);
                }
                else if (user.AktivTom <= DateTime.Now)
                {
                    model.DisabledAccount = true;
                    ModelState.AddModelError("", "Kontot är inaktiverat. För att återaktivera ditt konto behöver du verifiera din e-postadress.");
                    return View(model);
                }
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    ModelState.AddModelError("", "Du behöver bekräfta din epostadress. Se mejl från inrapportering@socialstyrelsen.se");
                    return View(model);
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                    shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        //var user = UserManager.FindByEmail(model.Email);
                        _portalService.SaveToLoginLog(user.Id, user.UserName);
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        if (!await UserManager.IsPhoneNumberConfirmedAsync(user.Id))
                        {
                            var phoneNumber = UserManager.GetPhoneNumberAsync(user.Id);
                            //Skicka användaren till AddPhoneNumber
                            var phoneNumberModel = new RegisterPhoneNumberViewModel()
                            {
                                Id = user.Id,
                                Number = phoneNumber.Result
                            };
                            if (phoneNumberModel.Number == null)
                            {
                                return View("AddPhoneNumber", phoneNumberModel);
                            }
                            else
                            {
                                return await this.AddPhoneNumber(phoneNumberModel);
                            }
                            
                        }
                        else
                        {
                            return RedirectToAction("SendCode", new SendCodeViewModel
                            {
                                Providers = null,
                                ReturnUrl = returnUrl,
                                RememberMe = model.RememberMe,
                                SelectedProvider = "Phone Code",
                                UserEmail = model.Email
                            });
                        }
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Felaktigt användarnamn eller pinkod.");
                        return View(model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "Login", e.ToString(), e.HResult, model.Email);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid inloggningen",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }

        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe, string userEmail = "")
        {
            try
            {
                var user = UserManager.FindByEmail(userEmail);
                var phoneNumber = _portalService.HamtaAnvandaresMobilnummer(user.Id);

                if (phoneNumber == null)
                {
                    var errorModel = new CustomErrorPageModel();
                    errorModel.Information = "Telefonnummer saknas.";
                    return View("CustomError", errorModel);
                }
                else
                {
                    var model = new VerifyCodeViewModel();
                    model.PhoneNumberMasked = _portalService.MaskPhoneNumber(phoneNumber);
                    return View(model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "VerifyCode", e.ToString(), e.HResult, userEmail);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid inloggningen",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }


            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe, UserEmail = userEmail});
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            try { 
                var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
                switch (result)
                {
                    case SignInStatus.Success:
                        var user = UserManager.FindByEmail(model.UserEmail);
                        _portalService.SaveToLoginLog(user.Id, user.UserName);
                        return RedirectToLocal(model.ReturnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Felaktig verifieringskod.");
                        return View(model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "VerifyCode", e.ToString(), e.HResult, model.UserEmail);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid kontroll av verifieringskod.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new RegisterViewModel();
            //Hämta info om valbara register
            //var registerInfoList = _portalService.HamtaAllRegisterInformation().ToList();
            //model.RegisterList = registerInfoList;
            ////model.RegisterChoices = new List<SelectListItem>{ tmpSelItem, tmpSelItem2 };
            //this.ViewBag.RegisterList = CreateRegisterDropDownList(registerInfoList);

            //Get the culture info of the language code
            CultureInfo culture = CultureInfo.GetCultureInfo("sv");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var organisation = GetOrganisationForEmailDomain(model.Email);
                    if (organisation == null)
                    {
                        ModelState.AddModelError("",
                            "Epostdomänen saknas i vårt register. Kontakta Socialstyrelsen för mer information. Support, epost: " +
                            ConfigurationManager.AppSettings["ContactEmail"]);
                    }
                    else
                    {
                        var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                        user.OrganisationId = organisation.Id;
                        user.SkapadAv = model.Email;
                        user.SkapadDatum = DateTime.Now;
                        user.AndradAv = model.Email;
                        user.AndradDatum = DateTime.Now;
                        user.Namn = model.Namn;
                        user.Kontaktnummer = model.Telefonnummer;

                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            await UserManager.SetTwoFactorEnabledAsync(user.Id, true);
                            //Spara valda register
                            //_portalService.SparaValdaRegistersForAnvandare(user.Id, user.UserName, model.RegisterList);
                            //Verifiera epostadress
                            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code = code},
                                protocol: Request.Url.Scheme);
                            //TODO mail/utvecklingsmiljön
                            await UserManager.SendEmailAsync(user.Id, "Bekräfta e-postadress", "Bekräfta din e-postadress i Socialstyrelsens inrapporteringsportal genom att klicka <a href=\"" + callbackUrl + "\">här</a>");
                            ViewBag.Email = model.Email;
                            return View("DisplayEmail");
                        }
                        AddErrors(result);
                    }
                }
                catch (System.Net.Mail.SmtpException e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("AccountController", "Register", e.ToString(), e.HResult, model.Email);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid registreringen. Mail kunde ej skickas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("AccountController", "Register", e.ToString(), e.HResult, model.Email);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid registreringen.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> EnableAccount(string email)
        {
            try
            {
                var model = new RegisterViewModel();
                var user = UserManager.FindByEmail(email);
                //var emailText = _portalService.HamtaInformationsText()
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("EnableAccountConfirmEmail", "Account", new {userId = user.Id, code = code},
                    protocol: Request.Url.Scheme);
                var body = "Hej, </br>";
                body = body + "Du får detta mejl för att du har valt att återaktivera ditt konto i Socialstyrelsens inrapporteringsportal, Filip. </br>";
                body = body + "Klicka <a href='" + callbackUrl + "'>här</a> för att aktivera ditt konto.</br></br>";
                body = body + "Om du inte har valt att återaktivera ditt konto kan du bortse från detta mejl.";

                await UserManager.SendEmailAsync(user.Id, "Bekräfta e-postadress",body);
                //await UserManager.SendEmailAsync(user.Id, "Bekräfta e-postadress",
                //    "Bekräfta din e-postadress i Socialstyrelsens inrapporteringsportal genom att klicka <a href='" + callbackUrl + "'>här</a>");
                ViewBag.Email = email;
                return View("DisplayEmail");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "EnableAccount", e.ToString(), e.HResult, email);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid återaktivering av konto för " + email,
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        private Organisation GetOrganisationForEmailDomain(string modelEmail)
        {
                var organisation = _portalService.HamtaOrgForEmailDomain(modelEmail);
                return organisation;
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                var user = UserManager.FindById(userId);
                var model = new RegisterPhoneNumberViewModel();
                model.Id = userId;
                return View("ConfirmEmail", model);
            }
            else
            {
                var str = String.Empty;
                foreach (var error in result.Errors)
                {
                    str = str + error + ", ";
                }
                Console.WriteLine(str);
                ErrorManager.WriteToErrorLog("AccountController", "ConfirmEmail", str, 0, userId);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid verifieringen av epostadressen.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        // GET: /Account/EnableAccountConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> EnableAccountConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                //Enable account
                _portalService.AktiveraKontaktperson(userId);
                return View("EnableAccountConfirmEmail");
            }
            return View("Error");
        }

        // GET: AddPhoneNumber
        [AllowAnonymous]
        public ActionResult AddPhoneNumber(string id = "")
        {
            var model = new RegisterPhoneNumberViewModel();
            model.Id = id;
            return View(model);
        }

        // GET: AddPhoneNumber
        [AllowAnonymous]
        public ActionResult AreYouSurePhoneNumber(RegisterPhoneNumberViewModel model)
        {
            return View("AreYouSurePhoneNumber",model);
        }

        //
        // POST: /AddPhoneNumber
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(RegisterPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(model.Id, model.Number);

                if (UserManager.SmsService != null)
                {
                    var message = new IdentityMessage
                    {
                        Destination = model.Number,
                        Body = "Välkommen till Socialstyrelsens InrappSos. För att registrera dig ange följande verifieringskod på webbsidan: " + code

                    };
                    await UserManager.SmsService.SendAsync(message);
                }
                return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number, Id = model.Id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "Register", e.ToString(), e.HResult, model.Id);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid registreringen.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }

        }

        // GET: /Manage/VerifyPhoneNumber
        [AllowAnonymous]
        public async Task<ActionResult> GetNewCode(string phoneNumber, string id)
        {
            try
            {
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(id, phoneNumber);

                if (UserManager.SmsService != null)
                {
                    var message = new IdentityMessage
                    {
                        Destination = phoneNumber,
                        Body = "Välkommen till Socialstyrelsens InrappSos. För att registrera dig ange följande verifieringskod på webbsidan: " + code

                    };
                    await UserManager.SmsService.SendAsync(message);
                }
                return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = phoneNumber, Id = id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "GetNewCode", e.ToString(), e.HResult, id);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när en ny verifieringskod skulle skickas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [AllowAnonymous]
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber, string id)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(id, phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new RegisterVerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Id = id });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(RegisterVerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var result =
                    await UserManager.ChangePhoneNumberAsync(model.Id, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        //Set users activFrom date
                        _portalService.UppdateraAktivFromForAnvandare(user.Id);

                        return View("ConfirmRegistration");
                    }
                    else
                    {
                        ErrorManager.WriteToErrorLog("AccountController", "VerifyPhoneNumber", "Verifiering av telefonnummer gick bra, men user saknas i databasen.", 0, "");
                        var errorModel = new CustomErrorPageModel
                        {
                            Information = "Ett fel inträffade vid verifiering av mobilnummer.",
                            ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                        };
                        return View("CustomError", errorModel);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "VerifyPhoneNumber", e.ToString(), e.HResult, User.Identity.GetUserName());
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid verifiering av mobilnummer.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Misslyckades att bekräfta mobilnummer");
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                 string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                 var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //TODO - mail
                 await UserManager.SendEmailAsync(user.Id, "Återställ pinkod", "Vänligen återställ din pinkod genom att klicka <a href=\"" + callbackUrl + "\">här</a>");

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userId)
        {
            if (code == null)
            {
                return View("Error");
            }
            try
            {
                var user = UserManager.FindById(userId);
                var model = new ResetPasswordViewModel
                {
                    Email = user.Email
                };
                return View(model);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "ResetPassword", e.ToString(), e.HResult, userId);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när pinkod skulle bytas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                user.AndradAv = user.Email;
                user.AndradDatum = DateTime.Now;
                _portalService.UppdateraAnvandarInfo(user);

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginConfirmation", "Account", new { ReturnUrl = returnUrl }));
        }


        [AllowAnonymous]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                // Generate the token and send it
                if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
                {
                    return View("Error");
                }
                return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe, UserEmail = model.UserEmail });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "SendCode", e.ToString(), e.HResult, User.Identity.GetUserName());
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när sms-kod skulle skickas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                //ModelState.AddModelError("", error);
                ModelState.AddModelError("", _errorDecsriber.LocalizeErrorMessage(error));
            }
        }

        /// <summary>  
        /// Create list for register-dropdown  
        /// </summary>  
        /// <returns>Return register for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateRegisterDropDownList(IEnumerable<RegisterInfo> registerInfoList)
        {
            SelectList lstobj = null;

            var list = registerInfoList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Namn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Index", "FileUpload");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
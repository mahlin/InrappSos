using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InrappSos.DataAccess;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Helpers;
using InrappSos.ApplicationService.Interface;
using InrappSos.DomainModel;
using InrappSos.FilipWeb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using InrappSos.FilipWeb.Models.ViewModels;

namespace InrappSos.FilipWeb.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly IPortalSosService _portalService;
        private CustomIdentityResultErrorDescriber _errorDecsriber;

        public ManageController()
        {
            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
            _errorDecsriber = new CustomIdentityResultErrorDescriber();

        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
            _errorDecsriber = new CustomIdentityResultErrorDescriber();

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Din pinkod har ändrats."
                : message == ManageMessageId.SetPasswordSuccess ? "Din pinkod är sparad."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Din två-faktor-autentiseringsleverantör är sparad."
                : message == ManageMessageId.Error ? "Ett fel har uppstått."
                : message == ManageMessageId.AddPhoneSuccess ? "Ditt mobilnummer har sparats."
                : message == ManageMessageId.RemovePhoneSuccess ? "Ditt mobilnummer har tagits bort."
                : message == ManageMessageId.ChangeChosenRegister ? "Valda register har registrerats."
                : message == ManageMessageId.ChangeChosenOrgUnits ? "Valda organisationsenheter har registrerats."
                : message == ManageMessageId.ChangeNameSuccess ? "Ditt namn har ändrats."
                : message == ManageMessageId.AddContactNumberSuccess ? "Ditt telefonnummer har ändrats."
                : "";

            var userId = User.Identity.GetUserId();
            var orgId = _portalService.HamtaUserOrganisationId(userId);

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                Namn = _portalService.HamtaAnvandaresNamn(userId),
                ContactNumber = _portalService.HamtaAnvandaresKontaktnummer(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                RegisterList = _portalService.HamtaRegistersMedAnvandaresVal(userId, orgId).ToList()
            };

            //model.RegisterList = _portalService.HamtaOrgenheter(model.RegisterList, orgId);
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }



        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Välkommen till Socialstyrelsens InrappSos. För att registrera dig ange följande verifieringskod på webbsidan: " + code
                           
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        // GET: /Manage/AddPhoneNumber
        public ActionResult AddContactNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddContactNumber(AddContactNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                _portalService.UppdateraKontaktnummerForAnvandare(User.Identity.GetUserId(), model.Number);
                var userId = User.Identity.GetUserId();
                var user = UserManager.Users.SingleOrDefault(x => x.Id == userId);
                user.AndradAv = user.Email;
                user.AndradDatum = DateTime.Now;
                _portalService.UppdateraAnvandarInfo(user);
                return RedirectToAction("Index", new { Message = ManageMessageId.AddContactNumberSuccess });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("ManageController", "AddContactNumber", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid byte av kontaktnummer.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }



        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            if (phoneNumber == null)
            {
                var errorModel = new CustomErrorPageModel();
                errorModel.Information = "Telefonnummer saknas.";
                return View("CustomError", errorModel);
            }
            else
            {
                var model = new VerifyPhoneNumberViewModel();
                model.PhoneNumber = phoneNumber;
                model.PhoneNumberMasked = _portalService.MaskPhoneNumber(phoneNumber);
                return View(model);
            }
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var result =
                    await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        user.AndradAv = user.Email;
                        user.AndradDatum = DateTime.Now;
                        _portalService.UppdateraAnvandarInfo(user);
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        _portalService.SaveToLoginLog(user.Id, user.UserName);
                    }
                    return RedirectToAction("Index", new {Message = ManageMessageId.AddPhoneSuccess});
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("ManageController", "VerifyPhoneNumber", e.ToString(), e.HResult, User.Identity.Name);
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


        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    user.AndradAv = user.Email;
                    user.AndradDatum = DateTime.Now;
                    _portalService.UppdateraAnvandarInfo(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/Name
        public ActionResult ChangeName()
        {
            return View();
        }


        //
        // POST: /Manage/ChangeName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeName(ChangeNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                _portalService.UppdateraNamnForAnvandare(User.Identity.GetUserId(), model.Name);
                var userId = User.Identity.GetUserId();
                var user = UserManager.Users.SingleOrDefault(x => x.Id == userId);
                user.AndradAv = user.Email;
                user.AndradDatum = DateTime.Now;
                _portalService.UppdateraAnvandarInfo(user);
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangeNameSuccess });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("ManageController", "ChangeName", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid byte av namn.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Den externa inloggningen togs bort."
                : message == ManageMessageId.Error ? "Ett fel har uppstått."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // GET: /Manage/Name
        public ActionResult ChangeChosenRegisters()
        {
            var model = new IndexViewModel();
            var userId = User.Identity.GetUserId();
            var orgId = _portalService.HamtaUserOrganisationId(userId);
            model.RegisterList = _portalService.HamtaRegistersMedAnvandaresVal(userId, orgId).ToList();
            return View(model);
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeChosenRegisters(IndexViewModel model)
        {
            try
            {
                //Uppdatera valda register
                _portalService.UppdateraValdaRegistersForAnvandare(User.Identity.GetUserId(), User.Identity.GetUserName(), model.RegisterList);
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangeChosenRegister });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("ManageController", "ChangeChosenRegisters", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid byte av valda register.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        // GET: /Manage/Name
        public ActionResult ChangeChosenOrgUnits(int selectedSubdirId = 0)
        {
            var model = new SubdirViewModel();
            var userId = User.Identity.GetUserId();
            var subdir = _portalService.HamtaDelregister(selectedSubdirId);
            var orgId = _portalService.HamtaUserOrganisationId(userId);
            var availableOrgUnits = _portalService.HamtaDelregistersAktuellaEnheter(selectedSubdirId, orgId).ToList();
            var usersOrgUnits = _portalService.HamtaAnvandarensValdaEnheterForDelreg(userId, selectedSubdirId).ToList();
            model.Id = selectedSubdirId;
            model.Delregisternamn = subdir.Delregisternamn;
            model.OrgUnitList = ConvertOrgUnitsToVM(availableOrgUnits, usersOrgUnits);
            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeChosenOrgUnits(SubdirViewModel model)
        {
            try
            {
                //Uppdatera valda organisationsenheter
                var orgUnitsDTO = ConvertVMOrgUnits(model.OrgUnitList);
                var org = _portalService.HamtaOrgForAnvandare(User.Identity.GetUserId());
                _portalService.UppdateraValdaOrganisationsenheterForAnvandareOchDelreg(User.Identity.GetUserId(), User.Identity.GetUserName(), orgUnitsDTO, model.Id, org.Id);
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangeChosenOrgUnits });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("ManageController", "ChangeChosenOrgUnits", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid byte av valda organisationsenheter.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [Authorize]
        public ActionResult DisableAccount()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _portalService.InaktiveraKontaktperson(userId);
                //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("ManageController", "DisableAccount", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när ditt konto skulle inaktiveras.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("DisabledAccount");

        }

        // GET
        public ActionResult DisabledAccount()
        {
            ViewBag.Text = _portalService.HamtaInfoText("Inaktiveringssida").Text;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return View("DisabledAccount");
        }




        [HttpPost]
        [Authorize]
        public ActionResult DeleteFAQ(int faqId, int faqCatId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _portalService.InaktiveraKontaktperson(userId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "DeleteFAQ", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när FAQ skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("About","Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }


        private List<InrappSos.FilipWeb.Models.ViewModels.OrgUnitViewModel> ConvertOrgUnitsToVM(List<Organisationsenhet> availableOrgUnits, List<Organisationsenhet> usersOrgUnits)
        {
            var orgUnitsVMList = new List<InrappSos.FilipWeb.Models.ViewModels.OrgUnitViewModel>();
            foreach (var availableOrgUnit in availableOrgUnits)
            {
                var orgUnitVM = new InrappSos.FilipWeb.Models.ViewModels.OrgUnitViewModel()
                {
                    Id = availableOrgUnit.Id,
                    OrganisationsId = availableOrgUnit.OrganisationsId,
                    Enhetsnamn = availableOrgUnit.Enhetsnamn,
                    Enhetskod = availableOrgUnit.Enhetskod
                };
                var chosenOrgUnit = usersOrgUnits.Where(x => x.Id == availableOrgUnit.Id);
                if (chosenOrgUnit.Any())
                {
                    orgUnitVM.Selected = true;
                }
                orgUnitsVMList.Add(orgUnitVM);
            }
            return orgUnitsVMList;
        }

        private List<OrganisationsenhetDTO> ConvertVMOrgUnits(List<InrappSos.FilipWeb.Models.ViewModels.OrgUnitViewModel> orgUnitsVM)
        {
            var orgUnitsDTOList = new List<OrganisationsenhetDTO>();
            foreach (var orgUnitVM in orgUnitsVM)
            {
                var orgUnitDTO = new OrganisationsenhetDTO
                {
                    Id = orgUnitVM.Id,
                    Enhetsnamn = orgUnitVM.Enhetsnamn,
                    Enhetskod = orgUnitVM.Enhetskod,
                    Selected = orgUnitVM.Selected
                };
                orgUnitsDTOList.Add(orgUnitDTO);
            }
            return orgUnitsDTOList;
        }


        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ChangeChosenRegister,
            ChangeChosenOrgUnits,
            ChangeNameSuccess,
            AddContactNumberSuccess,
            Error
        }

#endregion
    }
}
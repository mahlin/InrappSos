using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using InrappSos.AstridWeb.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using InrappSos.AstridWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.AstridWeb.Controllers
{
   [Authorize]
   public class AccountController : Controller
   {
        private ApplicationUserManager _userManager;
        private CustomIdentityResultErrorDescriber _errorDecsriber;
        private readonly IPortalSosService _portalSosService;
        private ApplicationRoleManager _roleManager;


        public AccountController()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));

            _errorDecsriber = new CustomIdentityResultErrorDescriber();

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
      {
          _errorDecsriber = new CustomIdentityResultErrorDescriber();
          UserManager = userManager;
          SignInManager = signInManager;
          RoleManager = roleManager;
          _portalSosService =
              new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
          _errorDecsriber = new CustomIdentityResultErrorDescriber();

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

      private ApplicationSignInManager _signInManager;

      public ApplicationSignInManager SignInManager
      {
         get
         {
            return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
         }
         private set { _signInManager = value; }
      }

       public ApplicationRoleManager RoleManager
       {
           get
           {
               return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
           }
           private set { _roleManager = value; }
       }

       //
       // GET: /Account/Login
        [AllowAnonymous]
       public ActionResult Login(string returnUrl)
       {
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
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, change to shouldLockout: true
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,shouldLockout: false);

                    //if (UserManager.IsInRole(user.Id, "Admin"))
                    //{
                    //    ViewBag.displayRegister = "Yes";
                    //}

                    //var x = User.Identity.Name;
                    //var y = User.Identity.GetUserName();

                    switch (result)
                    {
                        case SignInStatus.Success:
                            return RedirectToLocal(returnUrl, true );
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Felaktigt användarnamn eller lösenord.");
                            return View(model);
                    }
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
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new RegisterViewModel();
            try
            {
                //Skapa lista över astrid-roller 
                //model.UserRoleList = _portalSosService.HamtaRoller();
                model.ChosenRolesStr = "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AccountController", "Register", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av registeringsformulär.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }

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
                    var user = new AppUserAdmin { UserName = model.Email, Email = model.Email };
                    user.SkapadAv = model.Email;
                    user.SkapadDatum = DateTime.Now;
                    user.AndradAv = model.Email;
                    user.AndradDatum = DateTime.Now;
                    user.EmailConfirmed = true;
                    var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //var _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new IdentityDbContext()));

                    //    var role = new IdentityRole();
                    //role.Name = "Admin";
                    //await _roleManager.CreateAsync(role);
                    //    UserManager.AddToRole(user.Id, "Admin");
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                     //return RedirectToAction("Index", "Home");
                    return RedirectToAction("Index", "Home", new { Message = AccountMessageId.AddUserSuccess });
                    }
                  AddErrors(result);
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

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Återställ lösenord", "Vänligen återställ ditt lösenord genom att klicka <a href=\"" + callbackUrl + "\">här</a>");

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
            ViewBag.Link = TempData["ViewBagLink"];
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
                    Information = "Ett fel inträffade när lösenord skulle bytas.",
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
                _portalSosService.UppdateraAnvandarInfo(user, User.Identity.GetUserName());
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
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
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
            //foreach (var errorMsg in result.Errors)
            //{
            //    //Om flera felmdedelanden i samma error - splitta på '.'
            //    string[] errors = errorMsg.Split('.');

            //    foreach (var error in errors)
            //    {
            //        ModelState.AddModelError("", _errorDecsriber.LocalizeErrorMessage(error + "."));
            //    }
            //}
        }

        private ActionResult RedirectToLocal(string returnUrl, bool admin)
        {
            if (admin)
            {
                ViewBag.displayRegister = "Yes";
            }
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
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

       public enum AccountMessageId
       {
           AddUserSuccess,
           Error
       }

        //private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        //{
        //   string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
        //   var callbackUrl = Url.Action("ConfirmEmail", "Account",
        //      new { userId = userID, code = code }, protocol: Request.Url.Scheme);
        //   await UserManager.SendEmailAsync(userID, subject,
        //      "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //   return callbackUrl;
        //}
        #endregion


    }
}
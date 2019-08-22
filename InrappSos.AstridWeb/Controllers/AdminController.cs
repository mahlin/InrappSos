using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.AstridWeb.Filters;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using InrappSos.AstridWeb.Helpers;
using InrappSos.AstridWeb.Models;
using InrappSos.AstridWeb.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace InrappSos.AstridWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPortalSosService _portalSosService;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));

        }
        public AdminController()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        // GET: Astrid users
        [Authorize]
        public ActionResult GetAstridUsers()
        {
            var model = new AdminViewModels.AdminViewModel();
            
            try
            {
                var adminUsers = _portalSosService.HamtaAdminUsers();
                var roller = _portalSosService.HamtaAllaAstridRoller().ToList();
                model.AdminUsers = ConvertAdminUsersToViewModel(adminUsers, roller).ToList();
                //Skapa lista över astrid-roller 
                model.Roller = ConvertRolesToVM(roller);
                ViewBag.RolesList = CreateRolesDropDownList(roller);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "GetAstridUsers", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av admin-användare",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("Index", model);
        }

        

        [HttpPost]
        [Authorize]
        public ActionResult DeleteAdminUser(string userId)
        {
            try
            {
                _portalSosService.TaBortAdminAnvandare(userId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "DeleteAdminUser", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när Astrid-användare skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetAstridUsers");
        }
       

        public bool AddUserToRole(ApplicationUserManager _userManager, string userId, string roleName)
        {
            var idResult = _userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        //GET
        [Authorize]
        public ActionResult GetCaseTypes()
        {
            var model = new AdminViewModels.AdminViewModel();
            model.CaseTypes = _portalSosService.HamtaAllaArendetyper().ToList();
            return View("EditcaseType", model);
        }

        //GET
        [Authorize]
        public ActionResult GetCaseManagers()
        {
            var model = new AdminViewModels.AdminViewModel();
            model.CaseManagers = _portalSosService.HamtaAllaArendeansvariga().OrderBy(x => x.Epostadress).ToList();
            return View("EditCaseManagers", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCaseManager(ArendeAnsvarig caseManager)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraArendeAnsvarig(caseManager, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateCaseManager", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av ärendeansvarig.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetCaseManagers");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCaseType(Arendetyp caseType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraArendetyp(caseType, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateCaseType", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av ärendetyp.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetCasetypes");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateAdminUser(AdminViewModels.AppUserAdminViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    var userToUpdate = ConvertViewModelToAppUserAdmin(user);
                    _portalSosService.UppdateraAdminAnvandare(userToUpdate, userName);
                    //Lägg till användarens roller med multiselect/ListOfRoles
                    try
                    {
                        var astridRoller = _portalSosService.HamtaAllaAstridRoller();
                        foreach (var role in user.ListOfRoles)
                        {
                            var rollNamn = astridRoller.SingleOrDefault(x => x.BeskrivandeNamn == role.Name).Name;
                            if (role.Selected)
                            {
                                if (!UserManager.IsInRole(user.Id, rollNamn))
                                {
                                    _portalSosService.KopplaAstridAnvändareTillAstridRoll(userName, user.Id, role.Id);
                                }
                            }
                            else
                            {
                                if (UserManager.IsInRole(user.Id, rollNamn))
                                {
                                    UserManager.RemoveFromRole(user.Id, rollNamn);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException(e.Message);
                    }
                }
            }
            catch (ArgumentException e)
            {
                ApplicationService.Helpers.ErrorManager.WriteToErrorLog("AdminController", "UpdateAdminUser", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av Astrid-användare",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "UpdateAdminUser", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av Astrid-användare.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetAstridUsers");

        }

        [Authorize]
        public ActionResult CreateCaseManager()
        {
            var model = new AdminViewModels.ArendeAnsvarigViewModel();
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateCaseManager(ArendeAnsvarig arendeAnsvarig)
        {
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.SkapaArendeAnsvarig(arendeAnsvarig, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateCaseManager", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny ärendeansvarig skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetCaseManagers");
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateCaseType()
        {
            var model = new AdminViewModels.ArendetypViewModel();
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateCaseType(Arendetyp arendeTyp)
        {
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.SkapaArendetyp(arendeTyp, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateCaseType", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny ärendetyp skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetCaseTypes");
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateCaseStatus()
        {
            var model = new AdminViewModels.ArendeStatusViewModel();
            return View(model);
        }


        private IEnumerable<AdminViewModels.AppUserAdminViewModel> ConvertAdminUsersToViewModel(IEnumerable<AppUserAdmin> adminUsers, List<ApplicationRole> roller)
        {
            var adminUserViewList = new List<AdminViewModels.AppUserAdminViewModel>();
           

            foreach (var user in adminUsers)
            {
                var roleVMList = new List<IdentityRoleViewModel>();

                var adminUserView = new AdminViewModels.AppUserAdminViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    SkapadDatum = user.SkapadDatum,
                    SkapadAv = user.SkapadAv,
                    AndradDatum = user.AndradDatum,
                    AndradAv = user.AndradAv,
                    Roles = _portalSosService.HamtaAstridAnvandaresRoller(user.Id).Select(x => x.BeskrivandeNamn).ToList()
                };

                //Skapa lista över roller och markera valda roller för aktuell användare
                foreach (var roll in roller)
                {
                    var roleVm = new IdentityRoleViewModel
                    {
                        Id = roll.Id,
                        Name = roll.BeskrivandeNamn
                    };

                    if (adminUserView.Roles.Contains(roll.BeskrivandeNamn))
                    {
                        roleVm.Selected = true;
                    }
                    roleVMList.Add(roleVm);
                }

                //Skapa kommaseparerad textsträng över användarens roller 
                var rolesStr = String.Empty;
                foreach (var role in adminUserView.Roles)
                {
                    if (rolesStr.IsEmpty())
                    {
                        rolesStr = role;
                    }
                    else
                    {
                        rolesStr = rolesStr + ", "  + role;
                    }
                }

                adminUserView.StringOfRoles = rolesStr;
                adminUserView.ListOfRoles = roleVMList;
                adminUserViewList.Add(adminUserView);
            }
            return adminUserViewList;
        }

        private AppUserAdmin ConvertViewModelToAppUserAdmin(AdminViewModels.AppUserAdminViewModel adminUserVM)
        {
            var user = new AppUserAdmin()
            {
                Id = adminUserVM.Id,
                Email = adminUserVM.Email,
                PhoneNumber = adminUserVM.PhoneNumber,
            };

            return user;
        }

        private List<IdentityRoleViewModel> ConvertRolesToVM(List<ApplicationRole> roller)
        {
            var rollerList = new List<IdentityRoleViewModel>();

            foreach (var roll in roller)
            {
                var roleVM = new IdentityRoleViewModel()
                {
                    Id = roll.Id,
                    Name = roll.BeskrivandeNamn,
                    Selected = false
                };
                rollerList.Add(roleVM);
            }

            return rollerList;
        }

        /// <summary>  
        /// Create list for roles-dropdown  
        /// </summary>  
        /// <returns>Return roles for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateRolesDropDownList(IEnumerable<ApplicationRole> roles)
        {
            SelectList lstobj = null;

            var list = roles
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.BeskrivandeNamn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }
    }
}
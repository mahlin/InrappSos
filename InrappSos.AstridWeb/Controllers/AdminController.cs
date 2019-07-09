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

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
                        foreach (var role in user.ListOfRoles)
                        {
                            if (role.Selected)
                            {
                                UserManager.AddToRole(user.Id, role.Name);
                            }
                            else
                            {
                                if (UserManager.IsInRole(user.Id, role.Name))
                                {
                                    UserManager.RemoveFromRole(user.Id, role.Name);
                                }
                            }
                        }
                    }
                    ////Lägg till användarens roller från StringOfRoles
                    //try
                    //{
                    //    var roles = user.StringOfRoles.Split(',');
                    //    foreach (var role in roles)
                    //    {
                    //        if (!String.IsNullOrEmpty(role))
                    //        {
                    //            UserManager.AddToRole(user.Id, role.Trim());
                    //        }
                    //    }
                    //}
                    catch (Exception e)
                    {
                        throw new ArgumentException(e.Message);
                    }
                }
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

        [Authorize(Roles = "Admin")]
        // GET: /Roles/Create/Read/Update/Delete
        public ActionResult CRUDRoles()
        {
            //Hämta info för Create/Read/Update/Delete Roles
            var AstridRoles = _portalSosService.HamtaAllaAstridRoller();
            var astridRolelist = CreateRolesDropDownList(AstridRoles);
            ViewBag.AstridRoles = astridRolelist;

            var FilipRoles = _portalSosService.HamtaAllaFilipRoller();
            var filipRolelist = CreateRolesDropDownList(FilipRoles);
            ViewBag.FilipRoles = filipRolelist;

            ViewBag.Message = "";
            return View("CRUDRoles");
        }

        [Authorize(Roles = "Admin")]
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult CreateAstridRole(FormCollection collection)
        {
            try
            {
                _portalSosService.SkapaAstridRoll(collection["RoleName"]);
                ViewBag.Message = "Rollen skapad!";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "CreateAstridRole", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när Astrid-roll skulle skapas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("CRUDRoles");
        }

        [Authorize(Roles = "Admin")]
        //TODO - HasPermission?
        //[HasPermission("EditFAQ")]
        // GET: /Roles/Edit/5
        public ActionResult EditAstridRole(string roleName)
        {
            var model = new AdminViewModels.AdminRoleViewModel();
            model.SelectedApplication = "Astrid";
            model.Role = _portalSosService.HamtaAstridRoll(roleName);
            //var thisRole = _portalSosService.HamtaAstridRoll(roleName);
            ViewBag.Text = "Ändra Astrid-roll";

            return View("EditRole",model);
            //return RedirectToAction("CRUDRoles");
        }

        [Authorize(Roles = "Admin")]
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAstridRole(AdminViewModels.AdminRoleViewModel model)
        {
            try
            {
                _portalSosService.UppdateraAstridRoll(model.Role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "EditAstridRole", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när Astrid-roll skulle uppdateras.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("EditAstridRole", new {roleName = model.Role.Name});
    }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAstridRole(string roleName)
        {
            try
            {
                _portalSosService.TaBortAstridRoll(roleName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "DeleteAstridRole", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när Astrid-roll skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("CRUDRoles");
        }


        [Authorize(Roles = "Admin")]
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult CreateFilipRole(FormCollection collection)
        {
            try
            {
                _portalSosService.SkapaFilipRoll(collection["RoleName"]);
                ViewBag.Message = "Rollen skapad!";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "CreateFilipRole", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när Filip-roll skulle skapas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("CRUDRoles");
        }

        [Authorize(Roles = "Admin")]
        // GET: /Roles/Edit/5
        public ActionResult EditFilipRole(string roleName)
        {
            var model = new AdminViewModels.AdminRoleViewModel();
            model.SelectedApplication = "Filip";
            model.Role = _portalSosService.HamtaFilipRoll(roleName);
            ViewBag.Text = "Ändra Filip-roll";
            return View("EditRole", model);
            //return RedirectToAction("CRUDRoles");
        }

        [Authorize(Roles = "Admin")]
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFilipRole(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                _portalSosService.UppdateraFilipRoll(role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "EditFilipRole", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när Filip-roll skulle uppdateras.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("CRUDRoles");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteFilipRole(string roleName)
        {
            try
            {
                _portalSosService.TaBortFilipRoll(roleName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("AdminController", "DeleteFilipRole", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när Filip-roll skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("CRUDRoles");
        }


        private IEnumerable<AdminViewModels.AppUserAdminViewModel> ConvertAdminUsersToViewModel(IEnumerable<AppUserAdmin> adminUsers, List<IdentityRole> roller)
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
                    Roles = UserManager.GetRoles(user.Id)
                };

                //Skapa lista över roller och markera valda roller för aktuell användare
                foreach (var roll in roller)
                {
                    var roleVm = new IdentityRoleViewModel
                    {
                        Id = roll.Id,
                        Name = roll.Name
                    };

                    if (adminUserView.Roles.Contains(roll.Name))
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

        private List<IdentityRoleViewModel> ConvertRolesToVM(List<IdentityRole> roller)
        {
            var rollerList = new List<IdentityRoleViewModel>();

            foreach (var roll in roller)
            {
                var roleVM = new IdentityRoleViewModel()
                {
                    Id = roll.Id,
                    Name = roll.Name,
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
        private IEnumerable<SelectListItem> CreateRolesDropDownList(IEnumerable<IdentityRole> roles)
        {
            SelectList lstobj = null;

            var list = roles
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }
    }
}
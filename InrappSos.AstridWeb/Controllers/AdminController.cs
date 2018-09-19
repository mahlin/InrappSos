using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using InrappSos.AstridWeb.Helpers;
using InrappSos.AstridWeb.Models;
using InrappSos.AstridWeb.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace InrappSos.AstridWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IPortalAdminService _portalAdminService;
        private ApplicationUserManager _userManager;

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            _portalAdminService =
                new PortalAdminService(new PortalAdminRepository(new InrappSosDbContext(), new InrappSosIdentityDbContext()));
        }

        public AdminController()
        {
            _portalAdminService =
                new PortalAdminService(new PortalAdminRepository(new InrappSosDbContext(), new InrappSosIdentityDbContext()));
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
                var adminUsers = _portalAdminService.HamtaAdminUsers();
                model.AdminUsers = ConvertAdminUsersToViewModel(adminUsers);
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
                    _portalAdminService.UppdateraAdminAnvandare(userToUpdate, userName);
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
                _portalAdminService.TaBortAdminAnvandare(userId);
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

        private IEnumerable<AdminViewModels.AppUserAdminViewModel> ConvertAdminUsersToViewModel(IEnumerable<AppUserAdmin> adminUsers)
        {
            var adminUserViewList = new List<AdminViewModels.AppUserAdminViewModel>();

            foreach (var user in adminUsers)
            {
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
                var rolesStr = String.Empty;


                //TODO - Gör lista av roller till sträng tills vidare
                //for (int i = 0; i < adminUserView.Roles.Count; i++)
                //{
                //    //Om första eller sista i listan
                //    if (rolesStr.IsEmpty() || (i++ == adminUserView.Roles.Count))
                //    {
                //        rolesStr = rolesStr + adminUserView.Roles[i];
                //    }
                //    else
                //    {
                //        rolesStr = rolesStr + "," + adminUserView.Roles[i];
                //    }
                //}


                foreach (var role in adminUserView.Roles)
                {
                    if (rolesStr.IsEmpty())
                    {
                        rolesStr = role;
                    }
                    else
                    {
                        rolesStr = rolesStr + ","  + role;
                    }
                    
                }

                adminUserView.StringOfRoles = rolesStr;
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
                PhoneNumber = adminUserVM.PhoneNumber
            };

            return user;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.AstridWeb.Helpers;
using InrappSos.AstridWeb.Models;
using InrappSos.AstridWeb.Models.ViewModels;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace InrappSos.AstridWeb.Controllers
{
        public class RoleController : Controller
        {
            private readonly IPortalSosService _portalSosService;
            private ApplicationRoleManager _roleManager;
            private ApplicationUserManager _userManager;

            public RoleController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
            {
                _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
                RoleManager = roleManager;
                UserManager = userManager;
            }

            public RoleController()
            {
                _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
                var roleStore = new RoleStore<ApplicationRoleAstrid>(new InrappSosAstridDbContext());
                RoleManager = new ApplicationRoleManager(roleStore);
                
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


            public ActionResult Index()
            {
                //Hämta info för Create/Read/Update Roles
                var model = new RoleViewModels();
                var astridRolesList = _portalSosService.HamtaAllaAstridRoller();
                model.AstridRoller = astridRolesList.ToList();

                //var FilipRoles = _portalSosService.HamtaAllaFilipRoller();
                //var filipRolelist = CreateRolesDropDownList(FilipRoles);
                //ViewBag.FilipRoles = filipRolelist;

                ViewBag.Message = "";
                return View(model);
            }

            [Authorize(Roles = "Admin")]
            //GET
            public ActionResult CreateAstridRole()
            {
                return View("CreateAstrid");
            }

            [Authorize(Roles = "Admin")]
            // POST: /Roles/Create
            [HttpPost]
            public async Task<ActionResult> CreateAstridRole(RoleViewModels.RoleViewModelAstrid astridRole)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var user = User.Identity.GetUserName();
                        var dbRole = new ApplicationRoleAstrid
                        {
                            SkapadAv = user,
                            SkapadDatum = DateTime.Now,
                            AndradAv = user,
                            AndradDatum = DateTime.Now,
                            Name = astridRole.RoleName,
                            BeskrivandeNamn = astridRole.BeskrivandeNamn,
                            Beskrivning = astridRole.Beskrivning
                        };

                        await CreateRole(dbRole);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        ErrorManager.WriteToErrorLog("RoleController", "CreateAstridRole", e.ToString(), e.HResult,
                            User.Identity.Name);
                        var errorModel = new CustomErrorPageModel
                        {
                            Information = "Ett fel inträffade när Astrid-roll skulle skapas.",
                            ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                        };
                        return View("CustomError", errorModel);
                    }
                    return RedirectToAction("Index");
                }
                return View("CreateAstrid");
            }

            private async Task CreateRole(ApplicationRoleAstrid astridRole)
            {
                bool exists = await _roleManager.RoleExistsAsync(astridRole.Name);
                if (!exists)
                {
                    await _roleManager.CreateAsync(astridRole);
                }
            }


            [Authorize(Roles = "Admin")]
            // POST: /Roles/Edit/5
            [HttpPost]
            public ActionResult EditAstridRole(ApplicationRoleAstrid model)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var user = User.Identity.GetUserName();
                        _portalSosService.UppdateraAstridRoll(model, user);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RoleController", "EditAstridRole", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när Astrid-roll skulle uppdateras.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("Index");
            }

            public bool AddUserToRole(ApplicationUserManager _userManager, string userId, string roleName)
            {
                var idResult = _userManager.AddToRole(userId, roleName);
                return idResult.Succeeded;
            }

            public void ClearUserRoles(ApplicationUserManager userManager, string userId)
            {
                var user = userManager.FindById(userId);
                var currentRoles = new List<ApplicationUserRoleAstrid>();

                currentRoles.AddRange(user.UserRoles);
                foreach (ApplicationUserRoleAstrid role in currentRoles)
                {
                    userManager.RemoveFromRole(userId, role.Role.Name);
                }
            }


            public void RemoveFromRole(ApplicationUserManager userManager, string userId, string roleName)
            {
                userManager.RemoveFromRole(userId, roleName);
            }
        }
    }

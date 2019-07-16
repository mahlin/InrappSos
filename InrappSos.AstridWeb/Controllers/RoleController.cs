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
            private FilipApplicationRoleManager _filipRoleManager;
            private ApplicationUserManager _userManager;

            public RoleController(ApplicationUserManager userManager, ApplicationRoleManager roleManager, FilipApplicationRoleManager filipRoleManager)
            {
                _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
                RoleManager = roleManager;
                FilipRoleManager = filipRoleManager;
                UserManager = userManager;
            }

            public RoleController()
            {
                _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
                var roleStore = new RoleStore<ApplicationRoleAstrid>(new InrappSosAstridDbContext());
                var filipRoleStore = new RoleStore<ApplicationRole>(new InrappSosDbContext());
                RoleManager = new ApplicationRoleManager(roleStore);
                FilipRoleManager = new FilipApplicationRoleManager(filipRoleStore);
                
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

            public FilipApplicationRoleManager FilipRoleManager
            {
                get
                {
                    return _filipRoleManager ?? HttpContext.GetOwinContext().GetUserManager<FilipApplicationRoleManager>();
                }
                private set
                {
                    _filipRoleManager = value;
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
                var filipRolesList = _portalSosService.HamtaAllaFilipRoller();
                model.AstridRoller = astridRolesList.ToList();
                model.FilipRoller = filipRolesList.ToList();
                ViewBag.Message = "";
                return View(model);
            }

            [Authorize(Roles = "Admin")]
            //GET
            public ActionResult CreateAstridRole()
            {
                return View();
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

                        await CreateRoleForAstrid(dbRole);
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
                return View();
            }

            [Authorize(Roles = "Admin")]
            //GET
            public ActionResult CreateFilipRole()
            {
                return View();
            }

            [Authorize(Roles = "Admin")]
            // POST: /Roles/Create
            [HttpPost]
            public async Task<ActionResult> CreateFilipRole(RoleViewModels.RoleViewModelFilip filipRole)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var user = User.Identity.GetUserName();
                    var dbRole = new ApplicationRole
                        {
                            Name = filipRole.RoleName,
                            beskrivandeNamn = filipRole.BeskrivandeNamn,
                            beskrivning = filipRole.Beskrivning
                        };

                    _portalSosService.SkapaFilipRoll(dbRole, user);

                        //await CreateRoleForFilip(dbRole);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        ErrorManager.WriteToErrorLog("RoleController", "CreateFilipRole", e.ToString(), e.HResult,
                            User.Identity.Name);
                        var errorModel = new CustomErrorPageModel
                        {
                            Information = "Ett fel inträffade när Filip-roll skulle skapas.",
                            ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                        };
                        return View("CustomError", errorModel);
                    }
                    return RedirectToAction("Index");
                }
                return View();
            }

            private async Task CreateRoleForAstrid(ApplicationRoleAstrid astridRole)
            {
                bool exists = await _roleManager.RoleExistsAsync(astridRole.Name);
                if (!exists)
                {
                    await _roleManager.CreateAsync(astridRole);
                }
            }

            private async Task CreateRoleForFilip(ApplicationRole filipRole)
            {
                bool exists = await _filipRoleManager.RoleExistsAsync(filipRole.Name);
                if (!exists)
                {
                    await _filipRoleManager.CreateAsync(filipRole);
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

            [Authorize(Roles = "Admin")]
            // POST: /Roles/Edit/5
            [HttpPost]
            public ActionResult EditFilipRole(ApplicationRole model)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var user = User.Identity.GetUserName();
                        _portalSosService.UppdateraFilipRoll(model, user);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RoleController", "EditAstridRole", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när Filip-roll skulle uppdateras.",
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

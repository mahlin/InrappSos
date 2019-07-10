using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
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

            public RoleController(ApplicationRoleManager roleManager)
            {
                _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
                RoleManager = roleManager;

            }

            public RoleController()
            {
                _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));

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

            public ActionResult Index()
            {
                var _db = new InrappSosAstridDbContext();
                ApplicationRoleManager _roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(_db));
                var res = CreateRole(_roleManager, "Admin", "Test1");

            //var rolesList = new List<RoleViewModel>();
            //    foreach (var role in _db.Roles)
            //    {
            //        var roleModel = new RoleViewModel(role);
            //        rolesList.Add(roleModel);
            //    }
            //   return View(rolesList);
                return View();
            }

            public bool CreateRole(ApplicationRoleManager _roleManager, string name, string description = "")
            {
                var role = new ApplicationRole
                {
                    Name = "Admin",
                    Description = "Kan göra det mesta"
                };

                var idResult = _roleManager.Create(role);
                return idResult.Succeeded;
            }


            [Authorize(Roles = "Admin")]
            public ActionResult Create(string message = "")
            {
                ViewBag.Message = message;
                return View();
            }


            [HttpPost]
            [Authorize(Roles = "Admin")]
            public ActionResult Create([Bind(Include =
            "RoleName,Description")]RoleViewModels.RoleViewModel model)
            {
                string message = "That role name has already been used";
                if (ModelState.IsValid)
                {
                    var _db = new InrappSosAstridDbContext();

                    var role = new ApplicationRole(model.RoleName, model.Description);
                    ApplicationRoleManager _roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(_db));
                    //if (_db.RoleExists(_roleManager, model.RoleName))
                    //{
                    //    return View(message);
                    //}
                    //else
                    //{
                    //    _db.CreateRole(_roleManager, model.RoleName, model.Description);
                    //    return RedirectToAction("Index", "Roles");
                    //}
                }
                return View();
            }


            [Authorize(Roles = "Admin")]
            public ActionResult Edit(string id)
            {
            // It's actually the Role.Name tucked into the id param:
                var _db = new InrappSosAstridDbContext();
                var role = _db.Roles.First(r => r.Name == id);
                var roleModel = new RoleViewModels.EditRoleViewModel(role);
                return View(roleModel);
            }


            [HttpPost]
            [Authorize(Roles = "Admin")]
            public ActionResult Edit([Bind(Include =
            "RoleName,OriginalRoleName,Description")] RoleViewModels.EditRoleViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var _db = new InrappSosAstridDbContext();
                     var role = _db.Roles.First(r => r.Name == model.OriginalRoleName);
                    role.Name = model.RoleName;
                    role.Description = model.Description;
                    _db.Entry(role).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(model);
            }

        }
    }

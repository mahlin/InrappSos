using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using InrappSos.AstridWeb.Helpers;
using InrappSos.AstridWeb.Models;
using InrappSos.AstridWeb.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace InrappSos.AstridWeb.Controllers
{
    public class RegisterController : Controller
    {

        private readonly IPortalAdminService _portalAdminService;

        public RegisterController()
        {
            _portalAdminService =
                new PortalAdminService(new PortalAdminRepository(new InrappSosDbContext(), new InrappSosIdentityDbContext()));
        }


        // GET: Register
        [Authorize]
        public ActionResult Index()
        {
            var model = new RegisterViewModels.RegisterViewModel();
            var registerViewList = new List<RegisterViewModels.AdmRegisterViewModel>();
            try
            {
                 var registerList = _portalAdminService.HamtaRegister();

                foreach (var register in registerList)
                {
                    var registerView = new RegisterViewModels.AdmRegisterViewModel();
                    registerView.Id = register.Id;
                    registerView.Beskrivning = register.Beskrivning;
                    registerView.Kortnamn = register.Kortnamn;
                    registerView.Registernamn = register.Registernamn;
                    registerView.Inrapporteringsportal = register.Inrapporteringsportal;

                    registerViewList.Add(registerView);
                }

                model.Registers = registerViewList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("RegisterController", "GetDirectories", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av register",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("Index", model);
        }

        // GET
        [Authorize]
        public ActionResult GetSubDirectoriesForDirectory( RegisterViewModels.RegisterViewModel model, string regShortName = "")
        {
            try
            {
                var register = new AdmRegister();
                if (model.RegisterShortName.IsNullOrWhiteSpace() && regShortName.IsNullOrWhiteSpace())
                {
                    return RedirectToAction("GetAllSubDirectories");
                }
                if (!model.RegisterShortName.IsNullOrWhiteSpace())
                {
                    model.RegisterShortName = model.RegisterShortName;
                    register = _portalAdminService.HamtaRegisterMedKortnamn(model.RegisterShortName);
                }
                else if(regShortName != "")
                {
                    model.RegisterShortName = regShortName;
                    register = _portalAdminService.HamtaRegisterMedKortnamn(regShortName);
                }

                
                model.SelectedDirectoryId = register.Id;
                model.DelRegisters = _portalAdminService.HamtaDelRegisterForRegister(register.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("RegisterController", "GetSubDirectoriesForDirectory", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av delregister",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditSubDirectories", model);
        }

        // GET
        [Authorize]
        public ActionResult GetAllSubDirectories()
        {
            var model = new RegisterViewModels.RegisterViewModel();
            try
            {
                model.DelRegisters = _portalAdminService.HamtaDelRegister();
                model.RegisterShortName = "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("RegisterController", "GetAllSubDirectories", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av delregister",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditSubDirectories", model);
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateDirectory(AdmRegister register)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalAdminService.UppdateraRegister(register, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "UpdateDirectory", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppadtering av register.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateSubDirectory(AdmDelregister delRegister)
        {
            var regShortName = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    if (delRegister.RegisterId != 0)
                    {
                        var register = _portalAdminService.HamtaRegisterMedId(delRegister.RegisterId);
                        regShortName = register.Kortnamn;
                    }
                    _portalAdminService.UppdateraDelregister(delRegister, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "UpdateSubDirectory", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppadtering av delregister.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetSubDirectoriesForDirectory", new { regShortName = regShortName });

        }

        // GET
        [Authorize]
        public ActionResult CreateDirectory()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateDirectory(RegisterViewModels.AdmRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    AdmRegister register = new AdmRegister
                    {
                        Registernamn = model.Registernamn,
                        Beskrivning = model.Beskrivning,
                        Kortnamn = model.Kortnamn,
                        Inrapporteringsportal = model.Inrapporteringsportal
                    };
                    _portalAdminService.SkapaRegister(register, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "CreateDirectory", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när nytt register skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("Index");
            }

            return View();
        }


        // GET
        [Authorize]
        public ActionResult CreateSubDirectory(string regShortName)
        {
            var model = new RegisterViewModels.AdmDelregisterViewModel();
            if (regShortName == null)
            {
                ModelState.AddModelError("", "Du måste välja register som delregistret ska tillhöra.");
            }
            else
            {
                var register = _portalAdminService.HamtaRegisterMedKortnamn(regShortName);
                model.RegisterShortName = register.Kortnamn;
                model.RegisterId = register.Id;
            }
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateSubDirectory(AdmDelregister subDir)
        {
            var regShortName = String.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    _portalAdminService.SkapaDelregister(subDir, userName);
                    var register = _portalAdminService.HamtaRegisterMedId(subDir.RegisterId);
                    regShortName = register.Kortnamn;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "CreateSubDirectory", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när nytt delregister skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetSubDirectoriesForDirectory", new { regShortName = regShortName });
            }

            return View();
        }
    }
}
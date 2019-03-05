﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.DTOModel;
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

        private readonly IPortalSosService _portalSosService;

        public RegisterController()
        {
            _portalSosService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
        }


        // GET: Register
        [Authorize]
        public ActionResult Index()
        {
            var model = new RegisterViewModels.RegisterViewModel();
            var registerViewList = new List<RegisterViewModels.AdmRegisterViewModel>();
            try
            {
                var registerList = _portalSosService.HamtaRegister();

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
                ErrorManager.WriteToErrorLog("RegisterController", "GetDirectories", e.ToString(), e.HResult,
                    User.Identity.Name);
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
        public ActionResult GetSubDirectoriesForDirectory(RegisterViewModels.RegisterViewModel model,
            string regShortName = "")
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
                    register = _portalSosService.HamtaRegisterMedKortnamn(model.RegisterShortName);
                }
                else if (regShortName != "")
                {
                    model.RegisterShortName = regShortName;
                    register = _portalSosService.HamtaRegisterMedKortnamn(regShortName);
                }


                model.SelectedDirectoryId = register.Id;
                model.DelRegisters = _portalSosService.HamtaDelRegisterForRegister(register.Id).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("RegisterController", "GetSubDirectoriesForDirectory", e.ToString(),
                    e.HResult,
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
        public ActionResult GetRegulationsForDirectory(RegisterViewModels.AdmForeskriftViewModel model,
            string regShortName = "", int regId = 0)
        {
            try
            {
                var register = new AdmRegister();

                var dirId = model.SelectedDirectoryId;
                if (dirId == 0 && regId != 0)
                {
                    dirId = regId;
                }

                if (dirId == 0)
                {
                    return RedirectToAction("GetAllRegulations");
                }
                if (dirId != 0)
                {
                    model.SelectedDirectoryId = dirId;
                    model.ForeskriftList = _portalSosService.HamtaForeskrifterForRegister(dirId);

                    // Ladda drop down lists. 
                    var registerList = _portalSosService.HamtaAllaRegisterForPortalen();
                    this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("RegisterController", "GetRegulationsForDirectory", e.ToString(),
                    e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av föreskrift.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditRegulations", model);
        }

        // GET
        [Authorize]
        public ActionResult GetAllSubDirectories()
        {
            var model = new RegisterViewModels.RegisterViewModel();
            try
            {
                model.DelRegisters = _portalSosService.HamtaDelRegister().ToList();
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

        // GET
        [Authorize]
        public ActionResult GetAllSubDirectoriesOrgtypes()
        {
            var model = new RegisterViewModels.RegisterViewModel();
            try
            {
                var delregOrgtyper = _portalSosService.HamtaAllaDelRegistersOrganisationstyper();
                var allaOrgtyper = _portalSosService.HamtaAllaOrganisationstyper();
                model.DelRegistersOrganisationstyper =
                    ConvertAdmUppgiftsskyldighetOrganisationstypToVM(delregOrgtyper, allaOrgtyper);
                model.RegisterShortName = "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("RegisterController", "GetAllSubDirectoriesOrgtypes", e.ToString(),
                    e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av delregisters organisationstyper",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditSubDirectoriesOrgtypes", model);
        }




        // GET
        [Authorize]
        public ActionResult GetAllRegulations()
        {
            var model = new RegisterViewModels.AdmForeskriftViewModel();
            try
            {
                model.ForeskriftList = _portalSosService.HamtaAllaForeskrifter();
                model.RegisterShortName = "";
                // Ladda drop down lists. 
                var registerList = _portalSosService.HamtaAllaRegisterForPortalen();
                this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                model.SelectedDirectoryId = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("RegisterController", "GetAllRegulations", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av föreskrifter.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditRegulations", model);
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
                    _portalSosService.UppdateraRegister(register, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "UpdateDirectory", e.ToString(), e.HResult,
                        User.Identity.Name);
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
                        var register = _portalSosService.HamtaRegisterMedId(delRegister.RegisterId);
                        regShortName = register.Kortnamn;
                    }
                    _portalSosService.UppdateraDelregister(delRegister, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "UpdateSubDirectory", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av delregister.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetSubDirectoriesForDirectory", new {regShortName = regShortName});

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateSubdirOrgtypes(RegisterViewModels.AdmUppgiftsskyldighetOrganisationstypViewModel delRegisterOrgtypVM)
        {
            var regShortName = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var subdirOrgtypesToUpdate = ConvertViewModelToDb(delRegisterOrgtypVM);
                    var listOfOrgtypes = ConvertViewModelToOrgtypesDb(delRegisterOrgtypVM.ListOfOrgtypes);
                    _portalSosService.UppdateraUppgiftsskyldighetOrganisationstyp(subdirOrgtypesToUpdate, listOfOrgtypes, userName);
                    ////Lägg till användarens roller med multiselect/ListOfRoles
                    //try
                    //{
                    //    foreach (var role in user.ListOfRoles)
                    //    {
                    //        if (role.Selected)
                    //        {
                    //            UserManager.AddToRole(user.Id, role.Name);
                    //        }
                    //        else
                    //        {
                    //            if (UserManager.IsInRole(user.Id, role.Name))
                    //            {
                    //                UserManager.RemoveFromRole(user.Id, role.Name);
                    //            }
                    //        }
                    //    }
                    //}
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
                    //catch (Exception e)
                    //{
                    //    throw new ArgumentException(e.Message);
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "UpdateSubdirOrgtypes", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av uppgiftsskyldighet för delregister.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetAllSubDirectoriesOrgtypes");

        }


        

        [HttpPost]
        [Authorize]
        public ActionResult UpdateRegulation(RegisterViewModels.AdmForeskriftViewModel foreskriftVM)
        {
            //var regShortName = "";
            if (!String.IsNullOrEmpty(foreskriftVM.RegisterShortName))
            {
                foreskriftVM.SelectedDirectoryId =
                    _portalSosService.HamtaRegisterMedKortnamn(foreskriftVM.RegisterShortName).Id;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    //if (foreskriftVM.SelectedForeskrift.RegisterId != 0)
                    //{
                    //    var register = _portalSosService.HamtaRegisterMedId(foreskriftVM.SelectedForeskrift.RegisterId);
                    //    //regShortName = register.Kortnamn;
                    //}
                    //var foreskrift = ConvertVMToAdmForeskrift(foreskriftVM.SelectedForeskrift);
                    _portalSosService.UppdateraForeskrift(foreskriftVM.SelectedForeskrift, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "UpdateRegulation", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av föreskrift.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            if (foreskriftVM.SelectedDirectoryIdInUpdate != 0)
            {
                return RedirectToAction("GetRegulationsForDirectory",
                    new {regId = foreskriftVM.SelectedDirectoryIdInUpdate});
            }
            return RedirectToAction("GetAllRegulations");

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
                    _portalSosService.SkapaRegister(register, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "CreateDirectory", e.ToString(), e.HResult,
                        User.Identity.Name);
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
                var register = _portalSosService.HamtaRegisterMedKortnamn(regShortName);
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

                    _portalSosService.SkapaDelregister(subDir, userName);
                    var register = _portalSosService.HamtaRegisterMedId(subDir.RegisterId);
                    regShortName = register.Kortnamn;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "CreateSubDirectory", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när nytt delregister skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetSubDirectoriesForDirectory", new {regShortName = regShortName});
            }

            return View();
        }

        // GET
        [Authorize]
        public ActionResult EditSelectedRegulation(int foreskriftId = 0, int selectedDirectoryId = 0)
        {
            var model = new RegisterViewModels.AdmForeskriftViewModel();
            model.SelectedForeskrift = new AdmForeskrift();
            var selectedForeskriftDb = _portalSosService.HamtaForeskrift(foreskriftId);
            model.SelectedForeskrift.Id = selectedForeskriftDb.Id;
            model.SelectedForeskrift.Forfattningsnr = selectedForeskriftDb.Forfattningsnr;
            model.SelectedForeskrift.Forfattningsnamn = selectedForeskriftDb.Forfattningsnamn;
            model.SelectedForeskrift.GiltigFrom = selectedForeskriftDb.GiltigFrom;
            model.SelectedForeskrift.GiltigTom = selectedForeskriftDb.GiltigTom;
            model.SelectedForeskrift.Beslutsdatum = selectedForeskriftDb.Beslutsdatum;
            model.SelectedDirectoryId = selectedDirectoryId;
            model.SelectedDirectoryIdInUpdate = selectedDirectoryId;

            if (selectedForeskriftDb.RegisterId != null)
            {
                model.SelectedForeskrift.RegisterId = selectedForeskriftDb.RegisterId;
                //model.SelectedDirectoryId = selectedForeskriftDb.RegisterId;
            }
            model.RegisterShortName = _portalSosService.HamtaKortnamnForRegister(model.SelectedForeskrift.RegisterId);

            //// Ladda drop down lists. 
            //var registerList = _portalSosService.HamtaAllaRegisterForPortalen();
            //this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);

            return View("_EditSelectedRegulation", model);
        }

        //// POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult EditSelectedRegulation(RegisterViewModels.AdmForeskriftViewModel model)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var userName = User.Identity.GetUserName();
        //            _portalSosService.UppdateraForeskrift(model.SelectedForeskrift, userName);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //            ErrorManager.WriteToErrorLog("SystemController", "EditSelectedRegulation", e.ToString(), e.HResult, User.Identity.Name);
        //            var errorModel = new CustomErrorPageModel
        //            {
        //                Information = "Ett fel inträffade när föreskrift skulle sparas.",
        //                ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
        //            };
        //            return View("CustomError", errorModel);
        //        }
        //        return RedirectToAction("GetRegulationsForDirectory", new { regShortName = model.RegisterShortName });
        //    }

        //    return RedirectToAction("GetRegulationsForDirectory", new { regShortName = model.RegisterShortName });
        //}

        // GET
        [Authorize]
        public ActionResult CreateRegulation(int selectedRegId = 0)
        {
            var model = new RegisterViewModels.AdmForeskriftViewModel();
            var dir = _portalSosService.HamtaRegisterMedId(selectedRegId);
            model.RegisterShortName = dir.Kortnamn;
            model.SelectedDirectoryId = selectedRegId;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateRegulation(RegisterViewModels.AdmForeskriftViewModel foreskriftVM)
        {
            var regShortName = String.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var foreskrift = ConvertAdmForeskriftVMToNewAdmForeskrift(foreskriftVM);

                    _portalSosService.SkapaForeskrift(foreskrift, userName);
                    var register = _portalSosService.HamtaRegisterMedId(foreskriftVM.SelectedDirectoryId);
                    regShortName = register.Kortnamn;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("RegisterController", "CreateRegulation", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny föreskrift skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetRegulationsForDirectory", new {regShortName = regShortName});
            }

            return View();
        }

        /// <summary>  
        /// Create list for register-dropdown  
        /// </summary>  
        /// <returns>Return register for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateRegisterDropDownList(IEnumerable<AdmRegister> registerInfoList)
        {
            SelectList lstobj = null;

            var list = registerInfoList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Kortnamn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }


        private AdmForeskrift ConvertAdmForeskriftVMToNewAdmForeskrift(
            RegisterViewModels.AdmForeskriftViewModel foreskriftVM)
        {
            var foreskrift = new AdmForeskrift
            {
                RegisterId = foreskriftVM.SelectedDirectoryId,
                Forfattningsnr = foreskriftVM.NyForeskrift.Forfattningsnr,
                Forfattningsnamn = foreskriftVM.NyForeskrift.Forfattningsnamn,
                GiltigFrom = foreskriftVM.NyForeskrift.GiltigFrom,
                GiltigTom = foreskriftVM.NyForeskrift.GiltigTom,
                Beslutsdatum = foreskriftVM.NyForeskrift.Beslutsdatum
            };

            return foreskrift;
        }

        private List<RegisterViewModels.AdmUppgiftsskyldighetOrganisationstypViewModel>ConvertAdmUppgiftsskyldighetOrganisationstypToVM(IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> delregOrgtyper,IEnumerable<AdmOrganisationstyp> allaOrgtyper)
        {
            var delregOrgtyperVM = new List<RegisterViewModels.AdmUppgiftsskyldighetOrganisationstypViewModel>();

            foreach (var item in delregOrgtyper)
            {
                var orgtypVMList = new List<RegisterViewModels.OrganisationstypViewModel>();

                var delregOrgtyp = new RegisterViewModels.AdmUppgiftsskyldighetOrganisationstypViewModel
                {
                    Id = item.Id,
                    DelregisterId = item.DelregisterId,
                    DelregisterKortnamn = _portalSosService.HamtaDelregister(item.DelregisterId).Kortnamn,
                    OrganisationstypId = item.OrganisationstypId,
                    OrganisationstypNamn = _portalSosService.HamtaOrganisationstyp(item.OrganisationstypId).Typnamn,
                    Orgtyper = _portalSosService.HamtaOrganisationstyperForDelregister(item.DelregisterId), //Hämta namn på delregisters orgtyper
                    SkyldigFrom = item.SkyldigFrom,
                    SkyldigTom = item.SkyldigTom

                };

                //Skapa lista över orgtyper och markera valda orgtyper för aktuellt delregister
                foreach (var orgtyp in allaOrgtyper)
                {
                    var orgtypVm = new RegisterViewModels.OrganisationstypViewModel
                    {
                        Id = orgtyp.Id,
                        Name = orgtyp.Typnamn
                    };

                    if (delregOrgtyp.Orgtyper.Contains(orgtyp.Typnamn))
                    {
                        orgtypVm.Selected = true;
                    }
                    orgtypVMList.Add(orgtypVm);
                }

                //Skapa kommaseparerad textsträng över delregisters orgtyper
                var orgtyperStr = String.Empty;
                foreach (var orgtyp in delregOrgtyp.Orgtyper)
                {
                    if (orgtyperStr.IsEmpty())
                    {
                        orgtyperStr = orgtyp;
                    }
                    else
                    {
                        orgtyperStr = orgtyperStr + ", " + orgtyp;
                    }
                }
                delregOrgtyp.StringOfOrgtypes = orgtyperStr;
                delregOrgtyp.ListOfOrgtypes = orgtypVMList;
                delregOrgtyperVM.Add(delregOrgtyp);


            }
            return delregOrgtyperVM;
        }

        private AdmUppgiftsskyldighetOrganisationstyp ConvertViewModelToDb(RegisterViewModels.AdmUppgiftsskyldighetOrganisationstypViewModel subdirOrgtypesVM)
        {
            var subdirOrgtypes = new AdmUppgiftsskyldighetOrganisationstyp()
            {
                Id = subdirOrgtypesVM.Id,
                DelregisterId = subdirOrgtypesVM.DelregisterId,
                SkyldigFrom = subdirOrgtypesVM.SkyldigFrom,
                SkyldigTom = subdirOrgtypesVM.SkyldigTom
            };

            return subdirOrgtypes;
        }


        private List<OrganisationstypDTO> ConvertViewModelToOrgtypesDb(List<RegisterViewModels.OrganisationstypViewModel> listOfOrgTypesVM)
        {
            var listOfOrgtypes = new List<OrganisationstypDTO>();
            foreach (var itemVM in listOfOrgTypesVM)
            {
                var orgtype = new OrganisationstypDTO
                {
                    Organisationstypid = itemVM.Id,
                    Typnamn = itemVM.Name,
                    Selected = itemVM.Selected
                };
                listOfOrgtypes.Add(orgtype);
            }

            return listOfOrgtypes;
        }
    }

}
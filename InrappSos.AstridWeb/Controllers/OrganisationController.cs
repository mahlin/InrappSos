using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using InrappSos.AstridWeb.Helpers;
using InrappSos.AstridWeb.Models;
using InrappSos.AstridWeb.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace InrappSos.AstridWeb.Controllers
{
    public class OrganisationController : Controller
    {
        private readonly IPortalSosService _portalSosService;


        public OrganisationController()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));

        }

        [Authorize]
        public ActionResult Index()
        {
            // Ladda drop down lists. 
            var orgListDTO = GetOrganisationDTOList();
            ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            return View();
        }

        [Authorize]
        // GET: Organisation
        public ActionResult GetOrganisation(OrganisationViewModels.OrganisationViewModel model, int selectedOrganisationId)
        {
            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                model.Kommunkod = model.Organisation.Kommunkod;
                var contacts = _portalSosService.HamtaKontaktpersonerForOrg(model.Organisation.Id);
                model.ContactPersons = ConvertUsersViewModelUser(contacts);

                model.OrgUnits = _portalSosService.HamtaOrgEnheterForOrg(model.Organisation.Id);
                var reportObligationsDb = _portalSosService.HamtaUppgiftsskyldighetForOrg(model.Organisation.Id);
                model.ReportObligations = ConvertAdmUppgiftsskyldighetToViewModel(reportObligationsDb.ToList());
                // Ladda drop down lists. 
                var orgListDTO = GetOrganisationDTOList();
                ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
                //model.SelectedOrganisationId = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisation", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av organisation",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Felaktig kommunkod";
                }

                return View("CustomError", errorModel);

            }
            return View("Index",model);

        }

        // GET
        [Authorize]
        public ActionResult GetContacts()
        {
            // Ladda drop down lists. 
            var orgListDTO = GetOrganisationDTOList();
            ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");

            return View("EditContacts");
        }


        // GET
        [Authorize]
        public ActionResult GetOrganisationsContacts(OrganisationViewModels.OrganisationViewModel model, int selectedOrgId = 0)
        {
            try
            {
                if (selectedOrgId != 0)
                {
                    model.SelectedOrganisationId = selectedOrgId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                model.Kommunkod = model.Organisation.Kommunkod;
                var contacts = _portalSosService.HamtaKontaktpersonerForOrg(model.Organisation.Id);
                model.ContactPersons = ConvertUsersViewModelUser(contacts);
                foreach (var contact in model.ContactPersons)
                {
                    //Hämta användarens valda register
                    contact.ValdaDelregister = GetContactsChosenSubDirectories(contact);
                }
                // Ladda drop down lists. 
                var orgListDTO = GetOrganisationDTOList();
                ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsContacts", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av kontakter för organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Felaktig kommunkod";
                }
                return View("CustomError", errorModel);
            }
            return View("EditContacts", model);
        }

        //GET
        [Authorize]
        public ActionResult GetOrgUnits()
        {
            // Ladda drop down lists. 
            var orgListDTO = GetOrganisationDTOList();
            ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            return View("EditOrgUnits");
        }

        // GET
        [Authorize]
        public ActionResult GetOrganisationsOrgUnits(OrganisationViewModels.OrganisationViewModel model, int selectedOrgId = 0)
        {
            try
            {
                if (selectedOrgId != 0)
                {
                    model.SelectedOrganisationId = selectedOrgId;
                }

                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                model.Kommunkod = model.Organisation.Kommunkod;
                model.OrgUnits = _portalSosService.HamtaOrgEnheterForOrg(model.Organisation.Id);
                // Ladda drop down lists. 
                var orgListDTO = GetOrganisationDTOList();
                ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsOrgUnits", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av organisationsenheter för organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Felaktig kommunkod";
                }
                return View("CustomError", errorModel);
            }

            return View("EditOrgUnits", model);
        }

        //GET
        [Authorize]
        public ActionResult GetReportObligations()
        {
            // Ladda drop down lists. 
            var orgListDTO = GetOrganisationDTOList();
            ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            return View("EditReportObligations");
        }

        [Authorize]
        public ActionResult GetOrganisationsReportObligations(OrganisationViewModels.OrganisationViewModel model, int selectedOrgId = 0)
        {
            try
            {
                if (selectedOrgId != 0)
                {
                    model.SelectedOrganisationId = selectedOrgId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                model.Kommunkod = model.Organisation.Kommunkod;
                var admUppgSkyldighetList = _portalSosService.HamtaUppgiftsskyldighetForOrg(model.Organisation.Id);
                model.ReportObligations = ConvertAdmUppgiftsskyldighetToViewModel(admUppgSkyldighetList.ToList());
                // Ladda drop down lists. 
                var orgListDTO = GetOrganisationDTOList();
                ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsReportObligations", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av uppgiftsskyldighet för organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Felaktig kommunkod";
                }
                return View("CustomError", errorModel);
            }

            return View("EditReportObligations", model);
        }

        //GET
        [Authorize]
        public ActionResult GetUnitReportObligations()
        {
            var model = new OrganisationViewModels.UnitReportObligationsViewModel();
            //// Ladda drop down lists. 
            model = GetOrgDropDownLists(model);
            return View("EditUnitReportObligations", model);
        }


        [Authorize]
        public ActionResult GetOrganisationsUnitReportObligations(OrganisationViewModels.UnitReportObligationsViewModel model, int selectedOrgId = 0, int selectedOrgenhetsId = 0)
        {
            try
            {
                if (selectedOrgId != 0)
                {
                    model.SelectedOrganisationId = selectedOrgId;
                }
                if (selectedOrgenhetsId != 0)
                {
                    model.SelectedOrganisationsenhetsId = selectedOrgenhetsId;
                }
                var admEnhetUppgSkyldighetList = _portalSosService.HamtaEnhetsUppgiftsskyldighetForOrgEnhet(model.SelectedOrganisationsenhetsId).ToList();
                model.UnitReportObligations = ConvertEnhetsUppgSkyldighetToViewModel(admEnhetUppgSkyldighetList);
                // Ladda drop down lists. 
                model = GetOrgDropDownLists(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsUnitReportObligations", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av enhetsuppgiftsskyldighet.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Felaktig kommunkod";
                }
                return View("CustomError", errorModel);
            }

            return View("EditUnitReportObligations", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisation(OrganisationViewModels.OrganisationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraOrganisation(model.Organisation, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisation", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisation", new { selectedOrganisationId = model.SelectedOrganisationId });
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationsContact(ApplicationUser user)
        {
            var org = new Organisation();
            try
            {
                org = _portalSosService.HamtaOrgForAnvandare(user.Id);
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraKontaktperson(user, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationsContact", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av kontaktperson.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsContacts", new { selectedOrgId = org.Id });

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationsOrgUnit(Organisationsenhet orgUnit)
        {
            var org = new Organisation();
            try
            {
                org = _portalSosService.HamtaOrgForOrganisationsenhet(orgUnit.Id);
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraOrganisationsenhet(orgUnit, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationsOrgUnit", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av organisationsenhet.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsOrgUnits", new { selectedOrgId = org.Id });

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationsReportObligation(OrganisationViewModels.ReportObligationsViewModel admUppgSkyldighet)
        {
            var org = new Organisation();
            try
            {
                org = _portalSosService.HamtaOrgForUppgiftsskyldighet(admUppgSkyldighet.Id);
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    var admUppgiftsskyldighetToDb = ConvertViewModelToAdmUppgiftsskyldighet(admUppgSkyldighet);
                    _portalSosService.UppdateraUppgiftsskyldighet(admUppgiftsskyldighetToDb, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationsReportObligation", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av uppgiftsskyldighet.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsReportObligations", new { selectedOrgId = org.Id });

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationsUnitReportObligation(AdmEnhetsUppgiftsskyldighet admEnhetsUppgSkyldighet)
        {
            var org = new Organisation();
            try
            {
                org = _portalSosService.HamtaOrgForUppgiftsskyldighet(admEnhetsUppgSkyldighet.UppgiftsskyldighetId);
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraEnhetsUppgiftsskyldighet(admEnhetsUppgSkyldighet, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationsUnitReportObligation", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av enhetsuppgiftsskyldighet.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsUnitReportObligations", new { selectedOrgId = org.Id, selectedOrgenhetsId = admEnhetsUppgSkyldighet.OrganisationsenhetsId});

        }

        [Authorize]
        public ActionResult CreateOrganisation()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateOrganisation(OrganisationViewModels.OrganisationViewModel model)
        {
            var kommunkod = String.Empty;
            var orgId = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    orgId = _portalSosService.SkapaOrganisation(model.Organisation, userName);
                    //kommunkod = _portalSosService.HamtaKommunkodForOrg(orgId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateOrganisation", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny organisation skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisation", new { selectedOrganisationId = orgId });
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateOrganisationUnit(int selectedOrgId = 0)
        {
            var model = new OrganisationViewModels.OrganisationsenhetViewModel();
            model.Organisationsid = selectedOrgId;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateOrganisationUnit(Organisationsenhet orgenhet)
        {
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.SkapaOrganisationsenhet(orgenhet, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateOrganisationUnit", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny organisationsenhet skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisationsOrgUnits", new { selectedOrgId = orgenhet.OrganisationsId });
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateReportObligation(int selectedOrgId = 0)
        {
            var model = new OrganisationViewModels.ReportObligationsViewModel();
            model.OrganisationId = selectedOrgId;
            var delregisterList = _portalSosService.HamtaAllaDelregisterForPortalen().ToList();
            var uppgiftsskyldighetList = _portalSosService.HamtaUppgiftsskyldighetForOrg(selectedOrgId).ToList();
            var delregisterUtanUppgiftsskyldighetForOrgList = new List<AdmDelregister>();
            //Endast delregister som saknar uppgiftsskyldighet ska visas i dropdown
            foreach (var delregister in delregisterList)
            {
                var finnsRedan = uppgiftsskyldighetList.Find(r => r.DelregisterId == delregister.Id);
                if (finnsRedan == null)
                {
                    delregisterUtanUppgiftsskyldighetForOrgList.Add(delregister);
                }
            }

            this.ViewBag.DelregisterList = CreateDelRegisterDropDownList(delregisterUtanUppgiftsskyldighetForOrgList);
            model.DelregisterId = 0;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateReportObligation(OrganisationViewModels.ReportObligationsViewModel uppgSk, int selectedOrgId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var admUppgSkyldighet = ConvertToDbFromVM(uppgSk);
                    admUppgSkyldighet.OrganisationId = selectedOrgId;
                    _portalSosService.SkapaUppgiftsskyldighet(admUppgSkyldighet, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateReportObligation", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny uppgiftsskyldighet skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisationsReportObligations", new { selectedOrgId = selectedOrgId });
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateUnitReportObligation(int selectedOrgId = 0, int selectedOrgenhetsId = 0)
        {
            var model = new OrganisationViewModels.UnitReportObligationsViewModel();

            try
            {
                model.SelectedOrganisationId = selectedOrgId;
                model.SelectedOrganisationsenhetsId = selectedOrgenhetsId;
                //Skapa dropdown för valbara delregister
                var delregisterList = _portalSosService.HamtaAllaDelregisterForPortalen();
                var admUppgSkyldighetList = _portalSosService.HamtaUppgiftsskyldighetForOrg(selectedOrgId).ToList();
                var delregisterDropDownList = new List<AdmDelregister>();

                //Endast delregister som har uppgiftsskyldighet ska visas i dropdown
                foreach (var delregister in delregisterList)
                {
                    var finns = admUppgSkyldighetList.Find(r => r.DelregisterId == delregister.Id);
                    if (finns != null)
                    {
                        //Kolla att enhetsuppgiftsskyldighet inte redan finns för delreg
                        var uppgskh = admUppgSkyldighetList.SingleOrDefault(x => x.DelregisterId == delregister.Id);
                        var enhetsuppgiftsskyldighet = _portalSosService.HamtaEnhetsUppgiftsskyldighetForUppgiftsskyldighetOchOrgEnhet(uppgskh.Id, selectedOrgenhetsId);
                        if (enhetsuppgiftsskyldighet == null)
                        {
                            delregisterDropDownList.Add(delregister);
                        }
                    }
                }

                this.ViewBag.DelregisterList = CreateDelRegisterDropDownList(delregisterDropDownList);

                if (selectedOrgId != 0)
                {
                    model.Organisationsnamn = _portalSosService.HamtaOrganisation(selectedOrgId).Organisationsnamn;
                }

                var orgenhetsList = _portalSosService.HamtaOrgEnheterForOrg(selectedOrgId);
                this.ViewBag.OrgenhetList = CreateOrgenhetDropDownList(orgenhetsList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "CreateUnitReportObligation", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när formuläret för ny enhetsuppgiftsskyldighet skulle visas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateUnitReportObligation(OrganisationViewModels.UnitReportObligationsViewModel enhetsUppgSk)
        {
            var kommunkod = String.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var admEnhetsUppgSkyldighet = ConvertViewModelToAdmEnhetsUppgiftsskyldighet(enhetsUppgSk);
                    admEnhetsUppgSkyldighet.UppgiftsskyldighetId = _portalSosService.HamtaUppgiftsskyldighetForOrgOchDelreg(Convert.ToInt32(enhetsUppgSk.SelectedOrganisationId),
                            Convert.ToInt32(enhetsUppgSk.SelectedDelregisterId)).Id;
                    _portalSosService.SkapaEnhetsUppgiftsskyldighet(admEnhetsUppgSkyldighet, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateUnitReportObligation", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny enhetsuppgiftsskyldighet skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisationsUnitReportObligations", new { selectedOrgId = Convert.ToInt32(enhetsUppgSk.SelectedOrganisationId), selectedOrgenhetsId = Convert.ToInt32(enhetsUppgSk.SelectedOrganisationsenhetsId) });
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteContact(string contactId, int selectedOrgId = 0)
        {
            try
            {
                _portalSosService.TaBortKontaktperson(contactId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "DeleteContact", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när kontaktperson skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsContacts", new { selectedOrgId = selectedOrgId });
        }


        private OrganisationViewModels.UnitReportObligationsViewModel GetOrgDropDownLists(OrganisationViewModels.UnitReportObligationsViewModel model)
        {
            var orgList = _portalSosService.HamtaAllaOrganisationer();
            var orgListDTO = GetOrganisationDTOList();

            foreach (var org in orgListDTO)
            {
                    var orgenheter = _portalSosService.HamtaOrgEnheterForOrg(org.Id).ToList();
                    var orgenhetsListDTO = new List<OrganisationsenhetDTO>();

                    foreach (var orgenhet in orgenheter)
                    {
                        var orgenhetDTO = new OrganisationsenhetDTO
                        {
                            Id = orgenhet.Id,
                            Enhetsnamn = orgenhet.Enhetsnamn,
                            Enhetskod = orgenhet.Enhetskod
                        };
                        orgenhetsListDTO.Add(orgenhetDTO);
                    }
                    org.Organisationsenheter = orgenhetsListDTO;
            }

            model.OrganisationList = orgListDTO.ToList();
            ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");

            return model;

        }

        private IEnumerable<OrganisationDTO> GetOrganisationDTOList()
        {
            var orgList = _portalSosService.HamtaAllaOrganisationer();
            var orgListDTO = new List<OrganisationDTO>();

            foreach (var org in orgList)
            {
                if (org.Kommunkod != null) //Endast kommuner tills vidare
                {
                    var organisationDTO = new OrganisationDTO
                    {
                        Id = org.Id,
                        Kommunkod = org.Kommunkod,
                        Landstingskod = org.Landstingskod,
                        Organisationsnamn = org.Organisationsnamn,
                        KommunkodOchOrgnamn = org.Kommunkod + ", " + org.Organisationsnamn
                    };
                    orgListDTO.Add(organisationDTO);
                }
            }

            return orgListDTO;
        }



        private IEnumerable<OrganisationViewModels.ApplicationUserViewModel> ConvertUsersViewModelUser(IEnumerable<ApplicationUser> contacts)
        {
            var contactPersonsView = new List<OrganisationViewModels.ApplicationUserViewModel>();

            var okToDelete = false;

            foreach (var contact in contacts)
            {
                if (!contact.PhoneNumberConfirmed)
                {
                    okToDelete = true;
                }
                else
                {
                    okToDelete = false;
                }
                var contactView = new OrganisationViewModels.ApplicationUserViewModel
                {
                    ID = contact.Id,
                    OrganisationId = contact.OrganisationId,
                    Namn = contact.Namn,
                    AktivFrom = contact.AktivFrom,
                    AktivTom = contact.AktivTom,
                    Status = contact.Status,
                    Email = contact.Email,
                    PhoneNumber = contact.PhoneNumber,
                    PhoneNumberConfirmed = contact.PhoneNumberConfirmed,
                    SkapadDatum = contact.SkapadDatum,
                    SkapadAv = contact.SkapadAv,
                    AndradDatum = contact.AndradDatum,
                    AndradAv = contact.AndradAv,
                    OkToDelete = okToDelete
                };

                contactPersonsView.Add(contactView);
            }
            return contactPersonsView;


        }

        private AdmUppgiftsskyldighet ConvertViewModelToAdmUppgiftsskyldighet(OrganisationViewModels.ReportObligationsViewModel admUppgskylldighetView)
        {
            var uppgSkyldighet = new AdmUppgiftsskyldighet()
            {
                Id = admUppgskylldighetView.Id,
                OrganisationId = admUppgskylldighetView.OrganisationId,
                DelregisterId = admUppgskylldighetView.DelregisterId,
                SkyldigFrom = admUppgskylldighetView.SkyldigFrom,
                SkyldigTom = admUppgskylldighetView.SkyldigTom,
                RapporterarPerEnhet = admUppgskylldighetView.RapporterarPerEnhet
            };

            return uppgSkyldighet;
        }

        private List<OrganisationViewModels.ReportObligationsViewModel> ConvertAdmUppgiftsskyldighetToViewModel(List<AdmUppgiftsskyldighet> admUppgskyldighetList)
        {
            var uppgSkyldigheter = new List<OrganisationViewModels.ReportObligationsViewModel>();
            foreach (var admUppgskyldighet in admUppgskyldighetList)
            {
                var uppgSkyldighetView = new OrganisationViewModels.ReportObligationsViewModel()
                {
                    Id = admUppgskyldighet.Id,
                    OrganisationId = admUppgskyldighet.OrganisationId,
                    DelregisterId = admUppgskyldighet.DelregisterId,
                    DelregisterKortnamn = _portalSosService.HamtaKortnamnForDelregister(admUppgskyldighet.DelregisterId),
                    SkyldigFrom = admUppgskyldighet.SkyldigFrom,
                    SkyldigTom = admUppgskyldighet.SkyldigTom,
                    RapporterarPerEnhet = admUppgskyldighet.RapporterarPerEnhet
                };

                uppgSkyldigheter.Add(uppgSkyldighetView);
            }
            

            return uppgSkyldigheter;
        }

        private List<OrganisationViewModels.AdmEnhetsUppgiftsskyldighetViewModel> ConvertEnhetsUppgSkyldighetToViewModel(List<AdmEnhetsUppgiftsskyldighet> admEnhetsUppgskyldighetList)
        {
            var enhUppgSkyldigheter = new List<OrganisationViewModels.AdmEnhetsUppgiftsskyldighetViewModel>();
            foreach (var admEnhetsUppgskyldighet in admEnhetsUppgskyldighetList)
            {
                var enhetsUppgSkyldighetView = new OrganisationViewModels.AdmEnhetsUppgiftsskyldighetViewModel()
                {
                    Id = admEnhetsUppgskyldighet.Id,
                    OrganisationsenhetsId = admEnhetsUppgskyldighet.OrganisationsenhetsId,
                    UppgiftsskyldighetId = admEnhetsUppgskyldighet.UppgiftsskyldighetId,
                    SkyldigFrom = admEnhetsUppgskyldighet.SkyldigFrom,
                    SkyldigTom = admEnhetsUppgskyldighet.SkyldigTom,
                    DelregisterKortnamn = _portalSosService.HamtaDelRegisterForUppgiftsskyldighet(admEnhetsUppgskyldighet.UppgiftsskyldighetId).Kortnamn
                };

                enhUppgSkyldigheter.Add(enhetsUppgSkyldighetView);
            }


            return enhUppgSkyldigheter;
        }
        

        private AdmEnhetsUppgiftsskyldighet ConvertViewModelToAdmEnhetsUppgiftsskyldighet(OrganisationViewModels.UnitReportObligationsViewModel admEnhetsUppgskyldView)
        {
            var enhetsUppgskyldighet = new AdmEnhetsUppgiftsskyldighet()
            {
                Id = admEnhetsUppgskyldView.Id,
                OrganisationsenhetsId = admEnhetsUppgskyldView.SelectedOrganisationsenhetsId,
                UppgiftsskyldighetId = admEnhetsUppgskyldView.UppgiftsskyldighetId,
                SkyldigFrom = admEnhetsUppgskyldView.SkyldigFrom,
                SkyldigTom = admEnhetsUppgskyldView.SkyldigTom
            };

            return enhetsUppgskyldighet;
        }

        //private List<OrganisationViewModels.UnitReportObligationsViewModel> ConvertAdmEnhetsUppgiftsskyldighetToViewModel(List<AdmEnhetsUppgiftsskyldighet> admEnhetsUppgskyldighetList)
        //{
        //    var enhetsuppgkyldigheter = new List<OrganisationViewModels.UnitReportObligationsViewModel>();
        //    foreach (var admEnhetsUppgskyldighet in admEnhetsUppgskyldighetList)
        //    {
        //        var enhetsuppgSkyldighetView = new OrganisationViewModels.UnitReportObligationsViewModel()
        //        {
        //            Id = admEnhetsUppgskyldighet.Id,
        //            OrganisationenhetsId = admEnhetsUppgskyldighet.OrganisationsenhetsId,
        //            SkyldigFrom = admEnhetsUppgskyldighet.SkyldigFrom,
        //            SkyldigTom = admEnhetsUppgskyldighet.SkyldigTom,
        //        };

        //        enhetsuppgkyldigheter.Add(enhetsuppgSkyldighetView);
        //    }

        //    return enhetsuppgkyldigheter;
        //}


        private AdmUppgiftsskyldighet ConvertToDbFromVM(OrganisationViewModels.ReportObligationsViewModel uppgSkVM)
        {
            var admUppgSk = new AdmUppgiftsskyldighet
            {
                Id = uppgSkVM.Id,
                DelregisterId = uppgSkVM.DelregisterId,
                SkyldigFrom = uppgSkVM.SkyldigFrom,
                SkyldigTom = uppgSkVM.SkyldigTom,
                RapporterarPerEnhet =uppgSkVM.RapporterarPerEnhet
            };

            return admUppgSk;
        }

        private IEnumerable<SelectListItem> CreateDelRegisterDropDownList(IEnumerable<AdmDelregister> delregisterList)
        {
            SelectList lstobj = null;

            var list = delregisterList
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

        private IEnumerable<SelectListItem> CreateOrgenhetDropDownList(IEnumerable<Organisationsenhet> orgenhetList)
        {
            SelectList lstobj = null;

            var list = orgenhetList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Enhetsnamn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }

        private string GetContactsChosenSubDirectories(OrganisationViewModels.ApplicationUserViewModel contact)
        {
            var valdaDelregister = String.Empty;
            var regList = _portalSosService.HamtaValdaDelregisterForAnvandare(contact.ID, contact.OrganisationId).ToList();

            for (int i = 0; i < regList.Count(); i++)
            {
                if (i == 0)
                {
                    valdaDelregister = regList[i].Kortnamn;
                }
                else
                {
                    valdaDelregister = valdaDelregister + ", " + regList[i].Kortnamn;
                }
            }

            return valdaDelregister;

        }






    }
}

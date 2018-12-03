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
using Microsoft.Ajax.Utilities;
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
            var model = new OrganisationViewModels.OrganisationViewModel();
            model.OrgtypesForOrgList = new List<OrganisationstypDTO>();
            model.SearchResult = new List<List<Organisation>>();
            return View(model);
        }

        [Authorize]
        // GET: Organisation
        public ActionResult SearchOrganisation(string searchText, string origin)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();

            try
            {
                var orgList = _portalSosService.SokOrganisation(searchText);
                model.Origin = origin;

                //Then you just need to see if you want ANY matches or COMPLETE matches.
                //Throw your results together in a List<AXCustomer> and return it.

                //Om endats en träff, hämta datat direkt
                if (orgList.Count == 1 && orgList[0].Count == 1)
                {
                    switch (origin)
                    {
                        case "index":
                            return RedirectToAction("GetOrganisation", new { selectedOrganisationId = orgList[0][0].Id });
                        case "contacts":
                            return RedirectToAction("GetOrganisationsContacts", new { selectedOrganisationId = orgList[0][0].Id });
                        case "orgunits":
                            return RedirectToAction("GetOrganisationsOrgUnits", new { selectedOrganisationId = orgList[0][0].Id });
                        case "reportobligation":
                            return RedirectToAction("GetOrganisationsReportObligations", new { selectedOrganisationId = orgList[0][0].Id });
                        case "unitreportobligation":
                            return RedirectToAction("GetOrganisationsUnitReportObligations", new { selectedOrganisationId = orgList[0][0].Id });
                        case "privateEmailAdresses":
                            return RedirectToAction("GetOrganisationsPrivateEmailAdresses", new { selectedOrganisationId = orgList[0][0].Id });
                        case "cases":
                            return RedirectToAction("GetOrganisationsCases", new { selectedOrganisationId = orgList[0][0].Id });


                        default:
                            var errorModel = new CustomErrorPageModel
                            {
                                Information = "Felaktig avsändare till sökfunktionen.",
                                ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                            };
                            return View("CustomError", errorModel);
                    }
                }
                model.SearchResult = orgList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "SearchOrganisation", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid sökning av organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Felaktig kommunkod";
                }

                return View("CustomError", errorModel);

            }
            return View("Index", model);
        }
        

        [Authorize]
        // GET: Organisation
        public ActionResult GetOrganisation(int selectedOrganisationId)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();

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
                model.SearchResult = new List<List<Organisation>>();
                model.OrganisationTypes = _portalSosService.HamtaAllaOrganisationstyper().ToList();
                //Skapa lista över orgtyper och vilka som är valda för aktuell organisation
                model.OrgtypesForOrgList = _portalSosService.HamtaOrgtyperForOrganisation(model.SelectedOrganisationId,  model.OrganisationTypes);
                model.ChosenOrganisationTypesNames = SetOrgtypenames(model.OrgtypesForOrgList);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisation", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av organisation.",
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


        //GET
        [Authorize]
        public ActionResult GetOrganisationTypes()
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            model.OrganisationTypes = _portalSosService.HamtaAllaOrganisationstyper().ToList();
            return View("EditOrgTypes", model);
        }

        // GET
        [Authorize]
        public ActionResult GetContacts()
        {
            return View("EditContacts");
        }


        // GET
        [Authorize]
        public ActionResult GetOrganisationsContacts(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();

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
                foreach (var contact in model.ContactPersons)
                {
                    //Hämta användarens valda register
                    contact.ValdaDelregister = GetContactsChosenSubDirectories(contact);
                }
                model.SearchResult = new List<List<Organisation>>();
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


        // GET
        [Authorize]
        public ActionResult GetPrivateEmailAdresses()
        {
            return View("EditPrivateEmailAdresses");
        }


        // GET
        [Authorize]
        public ActionResult GetOrganisationsPrivateEmailAdresses(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();

            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                var privEmails = _portalSosService.HamtaPrivataEpostadresserForOrg(model.Organisation.Id);
                model.UndantagEpostDomaner = ConvertPrivateEmailAdressesToVM(privEmails.ToList());
                model.SearchResult = new List<List<Organisation>>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsPrivateEmailAdresses", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av privata epostadresser för vald organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("EditPrivateEmailAdresses", model);
        }

        // GET
        [Authorize]
        public ActionResult GetCases()
        {
            return View("EditCases");
        }


        // GET
        [Authorize]
        public ActionResult GetOrganisationsCases(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();

            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                var cases = _portalSosService.HamtaArendenForOrg(model.Organisation.Id);
                model.Arenden = ConvertArendeToVM(cases.ToList());
                model.SearchResult = new List<List<Organisation>>();
                // Ladda drop down lists. 
                var arendetypList = _portalSosService.HamtaAllaArendetyper();
                ViewBag.ArendetypDDL = CreateArendetypDropDownList(arendetypList);
                model.SelectedArendetypId = 0;
                var arendestatusList = _portalSosService.HamtaAllaArendestatusar();
                ViewBag.ArendestatusDDL = CreateArendestatusDropDownList(arendestatusList);
                model.SelectedArendestatusId = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsPrivateEmailAdresses", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av ärenden för vald organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("EditCases", model);
        }

        //GET
        [Authorize]
        public ActionResult GetOrgUnits()
        {
            return View("EditOrgUnits");
        }

        // GET
        [Authorize]
        public ActionResult GetOrganisationsOrgUnits(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }

                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                model.Kommunkod = model.Organisation.Kommunkod;
                model.OrgUnits = _portalSosService.HamtaOrgEnheterForOrg(model.Organisation.Id);
                model.SearchResult = new List<List<Organisation>>();
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
            return View("EditReportObligations");
        }

        [Authorize]
        public ActionResult GetOrganisationsReportObligations(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                model.Kommunkod = model.Organisation.Kommunkod;
                var admUppgSkyldighetList = _portalSosService.HamtaUppgiftsskyldighetForOrg(model.Organisation.Id);
                model.ReportObligations = ConvertAdmUppgiftsskyldighetToViewModel(admUppgSkyldighetList.ToList());
                model.SearchResult = new List<List<Organisation>>();
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
                return View("CustomError", errorModel);
            }

            return View("EditReportObligations", model);
        }

        //GET
        [Authorize]
        public ActionResult GetUnitReportObligations()
        {
            var model = new OrganisationViewModels.UnitReportObligationsViewModel();
            model.SearchResult = new List<List<Organisation>>();
            return View("EditUnitReportObligations", model);
        }


        [Authorize]
        public ActionResult GetOrganisationsUnitReportObligations(OrganisationViewModels.UnitReportObligationsViewModel model, int selectedOrganisationId = 0, int selectedOrgenhetsId = 0)
        {
            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }
                if (selectedOrgenhetsId != 0)
                {
                    model.SelectedOrganisationsenhetsId = selectedOrgenhetsId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                var admEnhetUppgSkyldighetList = _portalSosService.HamtaEnhetsUppgiftsskyldighetForOrgEnhet(model.SelectedOrganisationsenhetsId).ToList();
                model.UnitReportObligations = ConvertEnhetsUppgSkyldighetToViewModel(admEnhetUppgSkyldighetList);
                model.SearchResult = new List<List<Organisation>>();
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
                    model.Organisation.Organisationstyp = ConvertOrgTypesForOrgList(model.Organisation.Id, model.OrgtypesForOrgList, userName, false);
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
            return RedirectToAction("GetOrganisationsContacts", new { selectedOrganisationId = org.Id });

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationPrivateEmailAdress(OrganisationViewModels.UndantagEpostDomanViewModel privEmailAdressVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var privEmailAdress = ConvertPrivEmailAdressVMToDb(privEmailAdressVM);
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraPrivatEpostAdress(privEmailAdress, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationPrivateEmailAdress", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av information kopplad till privat epostdomän.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsPrivateEmailAdresses", new { selectedOrganisationId = privEmailAdressVM.OrganisationsId });
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationCase(OrganisationViewModels.ArendeViewModel arendeVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var arende = ConvertArendeVMToDb(arendeVM);
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraArende(arende, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationPrivateEmailAdress", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när ärendet skulle uppdateras.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsCases", new { selectedOrganisationId = arendeVM.OrganisationsId });
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationType(AdmOrganisationstyp orgtype)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraOrganisationstyp(orgtype, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationType", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av organisationstyp.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationtypes");

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
            return RedirectToAction("GetOrganisationsOrgUnits", new { selectedOrganisationId = org.Id });

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
            return RedirectToAction("GetOrganisationsReportObligations", new { selectedOrganisationId = org.Id });

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
            return RedirectToAction("GetOrganisationsUnitReportObligations", new { selectedOrganisationId = org.Id, selectedOrgenhetsId = admEnhetsUppgSkyldighet.OrganisationsenhetsId});

        }

        [Authorize]
        public ActionResult CreateOrganisation()
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            try
            {
                model.OrganisationTypes = _portalSosService.HamtaAllaOrganisationstyper().ToList();
                //Skapa lista över orgtyper 
                model.OrgtypesForOrgList = _portalSosService.HamtaOrgtyperForOrganisation(0, model.OrganisationTypes);
                model.ChosenOrganisationTypesNames = "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "CreateOrganisation", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när ny organisation skulle skapas.",
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
        public ActionResult CreateOrganisation(OrganisationViewModels.OrganisationViewModel model)
        {
            var kommunkod = String.Empty;
            var orgId = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var orgtyperForOrg = ConvertOrgTypesForOrgList(model.Organisation.Id, model.OrgtypesForOrgList, userName, true);
                    orgId = _portalSosService.SkapaOrganisation(model.Organisation, orgtyperForOrg, userName);
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
        public ActionResult CreateOrganisationType()
        {
            var model = new OrganisationViewModels.AdmOrganisationstypViewModel();
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateOrganisationType(AdmOrganisationstyp orgTyp)
        {
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.SkapaOrganisationstyp(orgTyp, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateOrganisationType", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny organisationstyp skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisationTypes");
            }

            return View();
        }

        [Authorize]
        public ActionResult CreatePrivateEmailAdress(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.UndantagEpostDomanViewModel();
            model.OrganisationsId = selectedOrganisationId;
            model.Organisationsnamn = _portalSosService.HamtaOrganisation(selectedOrganisationId).Organisationsnamn;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreatePrivateEmailAdress(OrganisationViewModels.UndantagEpostDomanViewModel privEmail)
        {
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var privEmailDb = ConvertPrivEmailAdressVMToDb(privEmail);
                    _portalSosService.SkapaPrivatEpostadress(privEmailDb, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateOrganisationUnit", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny privat epostadress skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisationsPrivateEmailAdresses", new { selectedOrganisationId = privEmail.OrganisationsId });
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateCase(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.ArendeViewModel();
            model.OrganisationsId = selectedOrganisationId;
            model.Organisationsnamn = _portalSosService.HamtaOrganisation(selectedOrganisationId).Organisationsnamn;
            var arendetypList = _portalSosService.HamtaAllaArendetyper();
            ViewBag.ArendetypList = CreateArendetypDropDownList(arendetypList);
            model.ArendetypId = 0;
            var arendestatusList = _portalSosService.HamtaAllaArendestatusar();
            ViewBag.ArendestatusList = CreateArendestatusDropDownList(arendestatusList);
            model.ArendetypId = 0;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateCase(OrganisationViewModels.ArendeViewModel arendeVM)
        {
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var arendeDb = ConvertArendeVMToDb(arendeVM);
                    _portalSosService.SkapaArende(arendeDb, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateCase", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ärende skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisationsCases", new { selectedOrganisationId = arendeVM.OrganisationsId });
            }

            return View();
        }


        [Authorize]
        public ActionResult CreateOrganisationUnit(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.OrganisationsenhetViewModel();
            model.Organisationsid = selectedOrganisationId;
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
                return RedirectToAction("GetOrganisationsOrgUnits", new { selectedOrganisationId = orgenhet.OrganisationsId });
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateReportObligation(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.ReportObligationsViewModel();
            model.OrganisationId = selectedOrganisationId;
            var delregisterList = _portalSosService.HamtaAllaDelregisterForPortalen().ToList();
            var uppgiftsskyldighetList = _portalSosService.HamtaUppgiftsskyldighetForOrg(selectedOrganisationId).ToList();
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
        public ActionResult CreateReportObligation(OrganisationViewModels.ReportObligationsViewModel uppgSk, int selectedOrganisationId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var admUppgSkyldighet = ConvertToDbFromVM(uppgSk);
                    admUppgSkyldighet.OrganisationId = selectedOrganisationId;
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
                return RedirectToAction("GetOrganisationsReportObligations", new { selectedOrganisationId = selectedOrganisationId });
            }

            return View();
        }

        [Authorize]
        public ActionResult CreateUnitReportObligation(int selectedOrganisationId = 0, int selectedOrgenhetsId = 0)
        {
            var model = new OrganisationViewModels.UnitReportObligationsViewModel();

            try
            {
                model.SelectedOrganisationId = selectedOrganisationId;
                model.SelectedOrganisationsenhetsId = selectedOrgenhetsId;
                //Skapa dropdown för valbara delregister
                var delregisterList = _portalSosService.HamtaAllaDelregisterForPortalen();
                var admUppgSkyldighetList = _portalSosService.HamtaUppgiftsskyldighetForOrg(selectedOrganisationId).ToList();
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

                if (selectedOrganisationId != 0)
                {
                    model.Organisationsnamn = _portalSosService.HamtaOrganisation(selectedOrganisationId).Organisationsnamn;
                }

                var orgenhetsList = _portalSosService.HamtaOrgEnheterForOrg(selectedOrganisationId);
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
                return RedirectToAction("GetOrganisationsUnitReportObligations", new { selectedOrganisationId = Convert.ToInt32(enhetsUppgSk.SelectedOrganisationId), selectedOrgenhetsId = Convert.ToInt32(enhetsUppgSk.SelectedOrganisationsenhetsId) });
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteContact(string contactId, int selectedOrganisationId = 0)
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
            return RedirectToAction("GetOrganisationsContacts", new { selectedOrganisationId = selectedOrganisationId });
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteOrgType(int orgTypeId)
        {
            try
            {
                _portalSosService.TaBortOrganisationstyp(orgTypeId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "DeleteOrgType", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när organisationstyp skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationTypes");
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
                    Kontaktnummer = contact.Kontaktnummer,
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


        private IEnumerable<SelectListItem> CreateOrgtypeDropDownList(IEnumerable<AdmOrganisationstyp> orgtypesList)
        {
            SelectList lstobj = null;

            var list = orgtypesList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Typnamn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }

        private string SetOrgtypenames(List<OrganisationstypDTO> orgtypListForOrg)
        {
            var str = "";
            foreach (var orgtypDTO in orgtypListForOrg)
            {
                if (orgtypDTO.Selected)
                {
                    if (str.IsNullOrWhiteSpace())
                    {
                        str = str + orgtypDTO.Typnamn;
                    }
                    else
                    {
                        str = str + ", " + orgtypDTO.Typnamn;
                    }
                }
            }

            return str;
        }

        private ICollection<Organisationstyp> ConvertOrgTypesForOrgList(int orgId, List<OrganisationstypDTO> orgtypesForOrgList, string userName, bool create)
        {
            var orgtypesList = new List<Organisationstyp>();
            if (orgtypesForOrgList != null)
            {
                foreach (var orgtypeForOrg in orgtypesForOrgList)
                {
                    if (orgtypeForOrg.Selected)
                    {
                        var orgtype = new Organisationstyp
                        {
                            OrganisationsId = orgId,
                            OrganisationstypId = orgtypeForOrg.Organisationstypid,
                            AndradAv = userName,
                            AndradDatum = DateTime.Now
                        };
                        if (create)
                        {
                            orgtype.SkapadAv = userName;
                            orgtype.SkapadDatum = DateTime.Now;
                        }
                        orgtypesList.Add(orgtype);
                    }
                }
            }
            
            return orgtypesList;
        }

        private UndantagEpostDoman ConvertPrivEmailAdressVMToDb(OrganisationViewModels.UndantagEpostDomanViewModel privEmailAdressVM)
        {
            var privEmail = new UndantagEpostDoman
            {
                Id = privEmailAdressVM.Id,
                OrganisationsId = privEmailAdressVM.OrganisationsId,
                PrivatEpostAdress = privEmailAdressVM.PrivatEpostAdress,
                Status = privEmailAdressVM.Status,
                AktivFrom = privEmailAdressVM.AktivFrom,
                AktivTom = privEmailAdressVM.AktivTom
            };

            //Hämta ärendeId om ärendenr satt
            if (privEmailAdressVM.ArendeNr != null)
            {
                var arende = _portalSosService.HamtaArende(privEmailAdressVM.ArendeNr);
                if (arende != null)
                {
                    privEmail.ArendeId = arende.Id;
                }
            }
            return privEmail;
        }

        private List<OrganisationViewModels.UndantagEpostDomanViewModel> ConvertPrivateEmailAdressesToVM(List<UndantagEpostDoman> privEmailAdressVM)
        {
            var privEmailList = new List<OrganisationViewModels.UndantagEpostDomanViewModel>();

            foreach (var privEmailDb in privEmailAdressVM)
            {
                var privEmailVM = new OrganisationViewModels.UndantagEpostDomanViewModel
                {
                    Id = privEmailDb.Id,
                    OrganisationsId = privEmailDb.OrganisationsId,
                    PrivatEpostAdress = privEmailDb.PrivatEpostAdress,
                    Status = privEmailDb.Status,
                    AktivFrom = privEmailDb.AktivFrom,
                    AktivTom = privEmailDb.AktivTom
                };

                //Hämta ärendenr om ärendeid satt
                if (privEmailDb.ArendeId != null)
                {
                    privEmailVM.ArendeId = privEmailDb.ArendeId.Value;
                    privEmailVM.ArendeNr = _portalSosService.HamtaArendeById(privEmailVM.ArendeId).Arendenr;
                }
               
                privEmailList.Add(privEmailVM);
            }
            return privEmailList;
        }

        private Arende ConvertArendeVMToDb(OrganisationViewModels.ArendeViewModel arendeVM)
        {
            var arende = new Arende
            {
                Id = arendeVM.Id,
                OrganisationsId = arendeVM.OrganisationsId,
                Arendenamn = arendeVM.Arendenamn,
                Arendenr = arendeVM.Arendenr,
                ArendetypId = arendeVM.ArendetypId,
                ArendestatusId = arendeVM.ArendestatusId,
                StartDatum = arendeVM.StartDatum,
                SlutDatum = arendeVM.SlutDatum
            };
            return arende;
        }

        private List<OrganisationViewModels.ArendeViewModel> ConvertArendeToVM(List<Arende> arendeDbList)
        {
            var arendeList = new List<OrganisationViewModels.ArendeViewModel>();

            foreach (var arendeDb in arendeDbList)
            {
                var arendeVM = new OrganisationViewModels.ArendeViewModel()
                {
                    Id = arendeDb.Id,
                    OrganisationsId = arendeDb.OrganisationsId,
                    Arendenamn = arendeDb.Arendenamn,
                    Arendenr = arendeDb.Arendenr,
                    ArendetypId = arendeDb.ArendetypId,
                    ArendestatusId = arendeDb.ArendestatusId,
                    StartDatum = arendeDb.StartDatum,
                    SlutDatum = arendeDb.SlutDatum
                };

                //Hämta ärentyp, klartext 
                arendeVM.Arendetyp = _portalSosService.HamtaArendetyp(arendeDb.ArendetypId).ArendetypNamn;
                arendeVM.SelectedArendetypId = arendeDb.ArendetypId;
                //Hämta ärendestatus, klartext
                arendeVM.Arendestatus = _portalSosService.HamtaArendestatus(arendeDb.ArendestatusId).ArendeStatusNamn;
                arendeVM.SelectedArendestatusId = arendeDb.ArendestatusId;

                arendeList.Add(arendeVM);
            }
            return arendeList;
        }

        private IEnumerable<SelectListItem> CreateArendetypDropDownList(IEnumerable<Arendetyp> arendetypList)
        {
            SelectList lstobj = null;
            var list = arendetypList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.ArendetypNamn
                    });
            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");
            return lstobj;
        }

        private IEnumerable<SelectListItem> CreateArendestatusDropDownList(IEnumerable<ArendeStatus> arendestatusList)
        {
            SelectList lstobj = null;
            var list = arendestatusList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.ArendeStatusNamn
                    });
            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");
            return lstobj;
        }


    }
}

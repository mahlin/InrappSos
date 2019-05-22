using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace InrappSos.AstridWeb.Controllers
{
    public class OrganisationController : Controller
    {
        private readonly IPortalSosService _portalSosService;
        private FilipApplicationRoleManager _filipRoleManager;
        private FilipApplicationUserManager _filipUserManager;


        public OrganisationController()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));

        }

        public OrganisationController(FilipApplicationRoleManager filipRoleManager, FilipApplicationUserManager filipUserManager)
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
            FilipRoleManager = filipRoleManager;
            FilipUserManager = filipUserManager;
        }

        public FilipApplicationRoleManager FilipRoleManager
        {
            get
            {
                return _filipRoleManager ?? Request.GetOwinContext().GetUserManager<FilipApplicationRoleManager>();
            }
            private set { _filipRoleManager = value; }
        }

        public FilipApplicationUserManager FilipUserManager
        {
            get
            {
                return _filipUserManager ?? HttpContext.GetOwinContext().GetUserManager<FilipApplicationUserManager>();
            }
            private set
            {
                _filipUserManager = value;
            }
        }


        [Authorize]
        public ActionResult Index()
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            model.OrgtypesForOrgList = new List<OrganisationstypDTO>();
            model.SearchResult = new List<List<Organisation>>();
            model.ContactSearchResult = new List<List<OrganisationViewModels.ApplicationUserViewModel>>();
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
                        case "exceptionsExpectedFiles":
                            return RedirectToAction("GetOrganisationsExceptionsExpectedFiles", new { selectedOrganisationId = orgList[0][0].Id });
                        case "cases":
                            return RedirectToAction("GetOrganisationsCases", new { selectedOrganisationId = orgList[0][0].Id });
                        case "sftpAccounts":
                            return RedirectToAction("GetOrganisationsSFTPAccounts", new { selectedOrganisationId = orgList[0][0].Id });


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
                model.ContactSearchResult = new List<List<OrganisationViewModels.ApplicationUserViewModel>>();
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
                    errorModel.Information = "Ingen organisation kunde hittas.";
                }

                return View("CustomError", errorModel);

            }
            return View("Index", model);
        }



        [Authorize]
        // GET: Contact
        public ActionResult SearchContact(string searchText, string origin)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            var searchResultContactVMList = new List<List<OrganisationViewModels.ApplicationUserViewModel>>();

            try
            {
                var searchResultContactList = _portalSosService.SokKontaktperson(searchText);
                model.Origin = origin;

                //Om endats en träff, hämta datat direkt
                if (searchResultContactList.Count == 1 && searchResultContactList[0].Count == 1)
                {
                    switch (origin)
                    {
                        case "contacts":
                            return RedirectToAction("GetOrganisationsContacts", new { selectedOrganisationId = searchResultContactList[0][0].OrganisationId });
                        default:
                            var errorModel = new CustomErrorPageModel
                            {
                                Information = "Felaktig avsändare till sökfunktionen.",
                                ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                            };
                            return View("CustomError", errorModel);
                    }
                }
                else //konvertera till VM och komplettera med organisationsnamn
                {
                    foreach (var item in searchResultContactList)
                    {
                        var contactListVM = new List<OrganisationViewModels.ApplicationUserViewModel>();
                        foreach (var contact in item)
                        {
                            contactListVM.Add(ConvertApplicationUserToVM(contact));
                        }
                        searchResultContactVMList.Add(contactListVM);
                    }
                    
                }
                model.ContactSearchResult = searchResultContactVMList;
                model.SearchResult = new List<List<Organisation>>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "SearchContact", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid sökning efter kontaktperson.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Ingen kontaktperson kunde hittas.";
                }

                return View("CustomError", errorModel);

            }
            return View("EditContacts", model);
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
                //TODO - roller även här?
                var roller = new List<IdentityRole>();
                model.ContactPersons = ConvertUsersViewModelUser(contacts, roller);

                model.OrgUnits = _portalSosService.HamtaOrgEnheterForOrg(model.Organisation.Id).ToList();
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
                    errorModel.Information = "Ingen organisation kunde hittas.";
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
            var model = new OrganisationViewModels.OrganisationViewModel();
            model.ContactPersons = new List<OrganisationViewModels.ApplicationUserViewModel>();
            model.SearchResult = new List<List<Organisation>>();
            model.ContactSearchResult = new List<List<OrganisationViewModels.ApplicationUserViewModel>>();
            return View("EditContacts", model);
        }


        // GET
        [Authorize]
        public ActionResult GetOrganisationsContacts(int selectedOrganisationId = 0, bool visaInaktiva = false)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();

            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }
                model.VisaInaktiva = visaInaktiva;
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                model.Kommunkod = model.Organisation.Kommunkod;
                var contacts = _portalSosService.HamtaKontaktpersonerForOrg(model.Organisation.Id);
                var roller = _portalSosService.HamtaAllaFilipRoller().ToList();
                model.ContactPersons = ConvertUsersViewModelUser(contacts, roller);
                foreach (var contact in model.ContactPersons)
                {
                    //Hämta användarens valda register
                    contact.ValdaDelregister = GetContactsChosenSubDirectories(contact);
                }
               
                //Skapa lista över filip-roller 
                model.Roller = ConvertRolesToVM(roller);
                ViewBag.RolesList = CreateRolesDropDownList(roller);
                model.SearchResult = new List<List<Organisation>>();
                model.ContactSearchResult = new List<List<OrganisationViewModels.ApplicationUserViewModel>>();
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
                    errorModel.Information = "Ingen organisation kunde hittas.";
                }
                return View("CustomError", errorModel);
            }
            return View("EditContacts", model);
        }

        // GET
        [Authorize]
        public ActionResult GetSFTPAccounts()
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            model.SFTPAccounts = new List<OrganisationViewModels.SFTPkontoViewModel>();
            model.SearchResult = new List<List<Organisation>>();
            return View("EditSFTPAccounts", model);
        }

        // GET
        [Authorize]
        public ActionResult GetOrganisationsSFTPAccounts(int selectedOrganisationId = 0)
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
                var accounts = _portalSosService.HamtaSFTPkontonForOrg(model.Organisation.Id);
                var contactsForOrg = _portalSosService.HamtaKontaktpersonerForOrg(model.Organisation.Id).ToList();
                model.SFTPAccounts = ConvertAccountToViewModel(accounts, contactsForOrg);
                //Dropdownlist for registers
                var registerList = _portalSosService.HamtaRegisterEjKoppladeTillSFTPKontoForOrg(selectedOrganisationId);
                ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                model.SelectedRegisterId = 0;

                model.SearchResult = new List<List<Organisation>>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsSFTPAccounts", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av SFTP-konton för organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Ingen organisation kunde hittas.";
                }
                return View("CustomError", errorModel);
            }
            return View("EditSFTPACcounts", model);
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
                model.UndantagEpostadresser = ConvertPrivateEmailAdressesToVM(privEmails.ToList());
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
        public ActionResult GetExceptionsExpectedFiles()
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            model.UndantagForvantadfiler = new List<OrganisationViewModels.UndantagForvantadfilViewModel>();
            model.SearchResult = new List<List<Organisation>>();
            return View("EditExceptionsExpectedFiles", model);
        }

        // GET
        [Authorize]
        public ActionResult GetOrganisationsExceptionsExpectedFiles(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            model.UndantagForvantadfiler = new List<OrganisationViewModels.UndantagForvantadfilViewModel>();

            try
            {
                if (selectedOrganisationId != 0)
                {
                    model.SelectedOrganisationId = selectedOrganisationId;
                }
                model.Organisation = _portalSosService.HamtaOrganisation(model.SelectedOrganisationId);
                var allExpectedFilesForOrg = _portalSosService.HamtaAllaForvantadeFilerForOrg(model.Organisation.Id);
                var exceptionsExpectedFiles = _portalSosService.HamtaUndantagnaForvantadeFilerForOrg(model.Organisation.Id);
                model.UndantagForvantadfiler = CreateExceptionList(exceptionsExpectedFiles.ToList(), allExpectedFilesForOrg.ToList(), model.Organisation.Id);
                model.SearchResult = new List<List<Organisation>>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "GetOrganisationsExceptionsExpectedFiles", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av undantag av förväntade filer för vald organisation.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("EditExceptionsExpectedFiles", model);
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
                model.OrgUnits = _portalSosService.HamtaOrgEnheterForOrg(model.Organisation.Id).ToList();
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
                    errorModel.Information = "Ingen organisation kunde hittas.";
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
                // Ladda drop down list. 
                var orgenhetList = _portalSosService.HamtaOrgEnheterForOrg(model.Organisation.Id).ToList();
                ViewBag.OrgenhetDDL = CreateOrgenhetDropDownList(orgenhetList);
                if (model.SelectedOrganisationsenhetsId != 0)
                {
                    var admEnhetUppgSkyldighetList = _portalSosService.HamtaEnhetsUppgiftsskyldighetForOrgEnhet(model.SelectedOrganisationsenhetsId).ToList();
                    model.UnitReportObligations = ConvertEnhetsUppgSkyldighetToViewModel(admEnhetUppgSkyldighetList);
                }
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
                    errorModel.Information = "Ingen organisation kunde hittas.";
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
                var orgtypeSet = false;
                //Check if orgtyp set
                foreach (var orgtype in model.OrgtypesForOrgList)
                {
                    if (orgtype.Selected)
                    {
                        orgtypeSet = true;
                    }
                }

                if (!orgtypeSet)
                {
                    ModelState.AddModelError("CustomError", "Minst en organisationstyp måste väljas.");
                    //model.SearchResult.Add();
                    //Fulfix
                    if (model.ContactPersons == null)
                    {
                        model.ContactPersons = new List<OrganisationViewModels.ApplicationUserViewModel>();
                    }
                    if (model.OrgUnits == null)
                    {
                        model.OrgUnits = new List<Organisationsenhet>();
                    }
                    if (model.ReportObligations == null)
                    {
                        model.ReportObligations = new List<OrganisationViewModels.ReportObligationsViewModel>();
                    }
                }
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    model.Organisation.Organisationstyp = ConvertOrgTypesForOrgList(model.Organisation.Id, model.OrgtypesForOrgList, userName, false);
                    _portalSosService.UppdateraOrganisation(model.Organisation, userName);
                    return RedirectToAction("GetOrganisation", new { selectedOrganisationId = model.SelectedOrganisationId });
                }
                //Fulfix
                if (model.ContactPersons == null)
                {
                    model.ContactPersons = new List<OrganisationViewModels.ApplicationUserViewModel>();
                }
                if (model.OrgUnits == null)
                {
                    model.OrgUnits = new List<Organisationsenhet>();
                }
                if (model.ReportObligations == null)
                {
                    model.ReportObligations = new List<OrganisationViewModels.ReportObligationsViewModel>();
                }
                model.SearchResult = new List<List<Organisation>>();
                return View("Index",model);
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
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationsContact(OrganisationViewModels.ApplicationUserViewModel user, bool visaInaktiva)
        {
            var org = new Organisation();
            try
            {
                org = _portalSosService.HamtaOrgForAnvandare(user.ID);
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    var userToUpdate = ConvertViewModelToApplicationUser(user);
                    _portalSosService.UppdateraKontaktperson(userToUpdate, userName);
                    //Lägg till användarens roller med multiselect/ListOfRoles
                    try
                    {
                        foreach (var role in user.ListOfRoles)
                        {
                            if (role.Selected)
                            {
                                FilipUserManager.AddToRole(user.ID, role.Name);
                            }
                            else
                            {
                                if (FilipUserManager.IsInRole(user.ID, role.Name))
                                {
                                    FilipUserManager.RemoveFromRole(user.ID, role.Name);
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
            return RedirectToAction("GetOrganisationsContacts", new { selectedOrganisationId = org.Id, visaInaktiva });

        }




        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationSFTPAccount(OrganisationViewModels.SFTPkontoViewModel account)
        {
            //var org = new Organisation();
            try
            {
                //org = _portalSosService.HamtaOrgForAnvandare(user.ID);
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    var accountToUpdate = ConvertViewModelToSFTPkonto(account);
                    accountToUpdate.KontaktpersonSFTPkonto = ConvertContactsForSFTPAccountList(account.Id, account.ListOfContacts, userName, false);
                    _portalSosService.UppdateraSFTPKonto(accountToUpdate, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationSFTPAccount", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av SFTP-konto.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsSFTPAccounts", new { selectedOrganisationId = account.OrganisationsId });
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateOrganisationPrivateEmailAdress(OrganisationViewModels.UndantagEpostadressViewModel privEmailAdressVM)
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
        public async Task<ActionResult> UpdateOrganisationCase(OrganisationViewModels.ArendeViewModel arendeVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var arende = ConvertArendeVMToDb(arendeVM);
                    var rapportorer = arendeVM.Rapportorer;
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraArende(arende, userName, rapportorer);
                    //Lägg till roll för de rapportörer som är reggade
                    //TODO - Flytta detta till svc-lagret eller repo-lagret?
                    var reporters = arendeVM.Rapportorer.Replace(' ', ',');
                    var newEmailStr = reporters.Split(',');
                    foreach (var email in newEmailStr)
                    {
                        if (!String.IsNullOrEmpty(email.Trim()))
                        {
                            var user = await FilipUserManager.FindByNameAsync(email.Trim());
                            if (user != null)
                            {
                                if (!FilipUserManager.IsInRole(user.Id, "RegSvcRapp"))
                                {
                                    FilipUserManager.AddToRole(user.Id, "RegSvcRapp");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateOrganisationCase", e.ToString(), e.HResult,
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
        public ActionResult UpdateExceptionsExpectedFiles(OrganisationViewModels.OrganisationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = User.Identity.GetUserName();
                    var undantagDbList = ConvertUndantagVMtoDbList(model.UndantagForvantadfiler);
                    _portalSosService.UppdateraUndantagForvantadFil(undantagDbList, userName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "UpdateExceptionsExpectedFiles", e.ToString(),
                    e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppdatering av undantag av förväntad fil.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetOrganisationsExceptionsExpectedFiles",new {selectedOrganisationId = model.SelectedOrganisationId});
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
            var orgtypeSet = false;
            //Check if orgtyp set
            foreach (var orgtype in model.OrgtypesForOrgList)
            {
                if (orgtype.Selected)
                {
                    orgtypeSet = true;
                }
            }

            if (!orgtypeSet)
            {
                ModelState.AddModelError("CustomError", "Minst en organisationstyp måste väljas.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var orgtyperForOrg = ConvertOrgTypesForOrgList(model.Organisation.Id, model.OrgtypesForOrgList, userName, true);
                    orgId = _portalSosService.SkapaOrganisation(model.Organisation, orgtyperForOrg, userName);
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

            return View(model);
        }

        [Authorize]
        public ActionResult CreateOrganisationSFTPAccount(int selectedOrganisationId = 0)
        {
            var model = new OrganisationViewModels.SFTPkontoViewModel();
            try
            {
                model.OrganisationsId = selectedOrganisationId;
                //Aktiva kontaktpersoner för org
                var activeContactsForOrg = _portalSosService.HamtaAktivaKontaktpersonerForOrg(selectedOrganisationId).ToList();
                model.ListOfContacts = ConvertContactsForOrgToDropdownList(activeContactsForOrg);
                model.StringOfChosenContacts = "";

                //Skapa lista över register. Visa bara de register som ännu ej kopplats till ett sftp-konto för aktuell organisation 
                var registerList = _portalSosService.HamtaRegisterEjKoppladeTillSFTPKontoForOrg(selectedOrganisationId);
                ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                model.RegisterId = 0;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("OrganisationController", "CreateOrganisationSFTPAccount", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när nytt SFTP-konto skulle skapas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateOrganisationSFTPAccount(OrganisationViewModels.SFTPkontoViewModel accountVM, int selectedOrganisationId = 0)
        {
            var accountId = 0;
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var contacts = ConvertContactsForSFTPAccountList(accountVM.Id, accountVM.ListOfContacts, userName, true);
                    var account = ConvertViewModelToSFTPkonto(accountVM);
                    account.OrganisationsId = selectedOrganisationId;
                    accountId = _portalSosService.SkapaSFTPkonto(account, contacts, userName);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreateOrganisationSFTPAccount", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när nytt SFTP-konto skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOrganisationsSFTPAccounts", new { selectedOrganisationId });
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
            var model = new OrganisationViewModels.UndantagEpostadressViewModel();
            model.OrganisationsId = selectedOrganisationId;
            model.Organisationsnamn = _portalSosService.HamtaOrganisation(selectedOrganisationId).Organisationsnamn;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreatePrivateEmailAdress(OrganisationViewModels.UndantagEpostadressViewModel privEmail)
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
                    ErrorManager.WriteToErrorLog("OrganisationController", "CreatePrivateEmailAdress", e.ToString(), e.HResult, User.Identity.Name);
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
            model.StartDatum = DateTime.Now;
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
        public async Task<ActionResult> CreateCase(OrganisationViewModels.ArendeViewModel arendeVM)
        {
            var org = new Organisation();
            if (ModelState.IsValid)
            {
                try
                {
                    //TODO - testa detta på klienten
                    //Check arendenr not already used
                    var alredayUsed = CaseNumberAlreadyUsed(arendeVM.Arendenr, arendeVM.OrganisationsId);
                    if (alredayUsed)
                    {
                        var errorModel = new CustomErrorPageModel
                        {
                            Information = "Ett ärende med valt ärendenummer (" + arendeVM.Arendenr + ")  finns redan upplagt. Ärendet kunde inte sparas.",
                            ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                        };
                        return View("CustomError", errorModel);
                    }
                    var userName = User.Identity.GetUserName();
                    var arendeDTO = ConvertArendeVMToDb(arendeVM);
                    _portalSosService.SkapaArende(arendeDTO, userName);
                    //Lägg till roll för de rapportörer som är reggade
                    //TODO - Flytta detta till svc-lagret eller repo-lagret?
                    var reporters = arendeVM.Rapportorer.Replace(' ', ',');
                    var newEmailStr = reporters.Split(',');
                    foreach (var email in newEmailStr)
                    {
                        if (!String.IsNullOrEmpty(email.Trim()))
                        {
                            var user = await FilipUserManager.FindByNameAsync(email.Trim());
                            if (user != null)
                            {
                                if (!FilipUserManager.IsInRole(user.Id, "RegSvcRapp"))
                                {
                                    FilipUserManager.AddToRole(user.Id, "RegSvcRapp");
                                }
                            }
                        }
                        
                    }
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


        //private OrganisationViewModels.UnitReportObligationsViewModel GetOrgDropDownLists(OrganisationViewModels.UnitReportObligationsViewModel model)
        //{
        //    var orgList = _portalSosService.HamtaAllaOrganisationer();
        //    var orgListDTO = GetOrganisationDTOList();

        //    foreach (var org in orgListDTO)
        //    {
        //            var orgenheter = _portalSosService.HamtaOrgEnheterForOrg(org.Id).ToList();
        //            var orgenhetsListDTO = new List<OrganisationsenhetDTO>();

        //            foreach (var orgenhet in orgenheter)
        //            {
        //                var orgenhetDTO = new OrganisationsenhetDTO
        //                {
        //                    Id = orgenhet.Id,
        //                    Enhetsnamn = orgenhet.Enhetsnamn,
        //                    Enhetskod = orgenhet.Enhetskod
        //                };
        //                orgenhetsListDTO.Add(orgenhetDTO);
        //            }
        //            org.Organisationsenheter = orgenhetsListDTO;
        //    }

        //    model.OrganisationList = orgListDTO.ToList();
        //    ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");

        //    return model;

        //}

        //private IEnumerable<OrganisationDTO> GetOrganisationDTOList()
        //{
        //    var orgList = _portalSosService.HamtaAllaOrganisationer();
        //    var orgListDTO = new List<OrganisationDTO>();

        //    foreach (var org in orgList)
        //    {
        //        if (org.Kommunkod != null) //Endast kommuner tills vidare
        //        {
        //            var organisationDTO = new OrganisationDTO
        //            {
        //                Id = org.Id,
        //                Kommunkod = org.Kommunkod,
        //                Landstingskod = org.Landstingskod,
        //                Organisationsnamn = org.Organisationsnamn,
        //                KommunkodOchOrgnamn = org.Kommunkod + ", " + org.Organisationsnamn
        //            };
        //            orgListDTO.Add(organisationDTO);
        //        }
        //    }

        //    return orgListDTO;
        //}



        private List<OrganisationViewModels.ApplicationUserViewModel> ConvertUsersViewModelUser(IEnumerable<ApplicationUser> contacts, List<IdentityRole> roller)
        {
            var contactPersonsView = new List<OrganisationViewModels.ApplicationUserViewModel>();

            var okToDelete = false;

            foreach (var contact in contacts)
            {
                var roleVMList = new List<IdentityRoleViewModel>();

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
                    OkToDelete = okToDelete,
                    Roles = FilipUserManager.GetRoles(contact.Id)
                };

                //Skapa lista över roller och markera valda roller för aktuell användare
                foreach (var roll in roller)
                {
                    var roleVm = new IdentityRoleViewModel
                    {
                        Id = roll.Id,
                        Name = roll.Name
                    };

                    if (contactView.Roles.Contains(roll.Name))
                    {
                        roleVm.Selected = true;
                    }
                    roleVMList.Add(roleVm);
                }

                //Skapa kommaseparerad textsträng över användarens roller 
                var rolesStr = String.Empty;
                foreach (var role in contactView.Roles)
                {
                    if (rolesStr.IsEmpty())
                    {
                        rolesStr = role;
                    }
                    else
                    {
                        rolesStr = rolesStr + ", " + role;
                    }
                }

                contactView.StringOfRoles = rolesStr;
                contactView.ListOfRoles = roleVMList;
                contactPersonsView.Add(contactView);
            }
            return contactPersonsView;
        }

        private List<OrganisationViewModels.SFTPkontoViewModel> ConvertAccountToViewModel(IEnumerable<SFTPkonto> accounts, List<ApplicationUser> contacts)
        {
            var sftpAccountsView = new List<OrganisationViewModels.SFTPkontoViewModel>();

            var okToDelete = false;

            foreach (var account in accounts)
            {
                var contactsVMList = new List<OrganisationViewModels.ContactViewModel>();

                var accountView = new OrganisationViewModels.SFTPkontoViewModel
                {
                    Id = account.Id,
                    OrganisationsId = account.OrganisationsId,
                    Kontonamn = account.Kontonamn,
                    RegisterId = account.RegisterId,
                    RegisterNamn = _portalSosService.HamtaRegisterMedId(account.RegisterId).Kortnamn,
                    SkapadDatum = account.SkapadDatum,
                    SkapadAv = account.SkapadAv,
                    AndradDatum = account.AndradDatum,
                    AndradAv = account.AndradAv,
                    ChosenContacts = _portalSosService.HamtaEpostadresserForSFTPKonto(account.Id)
                };

                //Skapa lista över kontakter och markera valda kontakter för aktuellt konto
                foreach (var contact in contacts)
                {
                    var contactVm = new OrganisationViewModels.ContactViewModel
                    {
                        Id = contact.Id,
                        Email = contact.Email
                    };

                    if (accountView.ChosenContacts.Contains(contact.Email))
                    {
                        contactVm.Selected = true;
                    }
                    contactsVMList.Add(contactVm);
                }

                //Skapa kommaseparerad textsträng över kontots kontaktpersoner 
                var contactsStr = String.Empty;
                foreach (var contact in accountView.ChosenContacts)
                {
                    if (contactsStr.IsEmpty())
                    {
                        contactsStr = contact;
                    }
                    else
                    {
                        contactsStr = contactsStr + ", " + contact;
                    }
                }

                accountView.StringOfChosenContacts = contactsStr;
                accountView.ListOfContacts = contactsVMList;
                sftpAccountsView.Add(accountView);
            }
            return sftpAccountsView;
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

        private List<UndantagForvantadfilDTO> ConvertUndantagVMtoDbList(List<OrganisationViewModels.UndantagForvantadfilViewModel> undantagVMList)
        {
            var undantagDbList = new List<UndantagForvantadfilDTO>();
            foreach (var undantagForvFil in undantagVMList)
            {

                var undantagDb = new UndantagForvantadfilDTO
                {
                    OrganisationsId = undantagForvFil.OrganisationsId,
                    DelregisterId = undantagForvFil.DelregisterId,
                    ForvantadfilId = undantagForvFil.ForvantadfilId,
                    Selected = undantagForvFil.Selected
                };
                undantagDbList.Add(undantagDb);
            }

            return undantagDbList;
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

        private ICollection<KontaktpersonSFTPkonto> ConvertContactsForSFTPAccountList(int accountId, List<OrganisationViewModels.ContactViewModel> contactsForAccountList, string userName, bool create)
        {
            var contactsList = new List<KontaktpersonSFTPkonto>();
            if (contactsForAccountList != null)
            {
                foreach (var contact in contactsForAccountList)
                {
                    if (contact.Selected)
                    {
                        var contactForAccount = new KontaktpersonSFTPkonto()
                        {
                            ApplicationUserId = contact.Id,
                            SFTPkontoId = accountId,
                            AndradAv = userName,
                            AndradDatum = DateTime.Now
                        };
                        if (create)
                        {
                            contactForAccount.SkapadAv = userName;
                            contactForAccount.SkapadDatum = DateTime.Now;
                        }
                        contactsList.Add(contactForAccount);
                    }
                }
            }

            return contactsList;
        }

        private UndantagEpostadress ConvertPrivEmailAdressVMToDb(OrganisationViewModels.UndantagEpostadressViewModel privEmailAdressVM)
        {
            var privEmail = new UndantagEpostadress
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

        private List<OrganisationViewModels.UndantagEpostadressViewModel> ConvertPrivateEmailAdressesToVM(List<UndantagEpostadress> privEmailAdressVM)
        {
            var privEmailList = new List<OrganisationViewModels.UndantagEpostadressViewModel>();

            foreach (var privEmailDb in privEmailAdressVM)
            {
                var privEmailVM = new OrganisationViewModels.UndantagEpostadressViewModel
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

        private ArendeDTO ConvertArendeVMToDb(OrganisationViewModels.ArendeViewModel arendeVM)
        {
            var arende = new ArendeDTO
            {
                Id = arendeVM.Id,
                OrganisationsId = arendeVM.OrganisationsId,
                Arendenamn = arendeVM.Arendenamn,
                Arendenr = arendeVM.Arendenr,
                ArendetypId = arendeVM.ArendetypId,
                ArendestatusId = arendeVM.ArendestatusId,
                AnsvarigEpost = arendeVM.AnsvarigEpost,
                Rapportorer = arendeVM.Rapportorer,
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

                arendeVM.AnsvarigEpost = arendeDb.AnsvarigEpost;
                //Hämta rapportörers epostadress
                arendeVM.Rapportorer = _portalSosService.HamtaArendesRapportorer(arendeVM.OrganisationsId, arendeDb.Id);

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
            var y = lstobj.ToList();
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
            var y = lstobj.ToList();
            return lstobj;
        }

        private IEnumerable<SelectListItem> CreateOrgUnitDropDownList(IEnumerable<Organisationsenhet> orgenhetList)
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

        private bool CaseNumberAlreadyUsed(string arendenr, int orgId)
        {
            var alreadyUsed = false;
            var organisationCasenumbers = _portalSosService.HamtaArendenForOrg(orgId).Select(x => x.Arendenr);
            if (organisationCasenumbers.Contains(arendenr))
            {
                alreadyUsed = true;
            }
            return alreadyUsed;
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

        private OrganisationViewModels.ApplicationUserViewModel ConvertApplicationUserToVM(ApplicationUser user)
        {
            var userVM= new OrganisationViewModels.ApplicationUserViewModel()
            {
                ID = user.Id,
                OrganisationId = user.OrganisationId,
                OrganisationsNamn = _portalSosService.HamtaOrganisation(user.OrganisationId).Organisationsnamn,
                Namn = user.Namn,
                Email = user.Email,
            };

            return userVM;
        }

        private ApplicationUser ConvertViewModelToApplicationUser(OrganisationViewModels.ApplicationUserViewModel userVM)
        {
            var user = new ApplicationUser()
            {
                Id = userVM.ID,
                OrganisationId = userVM.OrganisationId,
                Namn = userVM.Namn,
                Kontaktnummer = userVM.Kontaktnummer,
                AktivFrom = userVM.AktivFrom,
                AktivTom = userVM.AktivTom,
                Status = userVM.Status,
                Email = userVM.Email,
                PhoneNumber = userVM.PhoneNumber,
                PhoneNumberConfirmed = userVM.PhoneNumberConfirmed
            };

            return user;
        }

        private SFTPkonto ConvertViewModelToSFTPkonto(OrganisationViewModels.SFTPkontoViewModel account)
        {
            var konto = new SFTPkonto()
            {
                Id = account.Id,
                OrganisationsId = account.OrganisationsId,
                Kontonamn  = account.Kontonamn,
                RegisterId = account.RegisterId
            };

            return konto;
        }

        private List<OrganisationViewModels.UndantagForvantadfilViewModel> CreateExceptionList(List<UndantagForvantadfil> uList, List<AdmForvantadfilDTO> forvFilList, int orgId)
        {
            var undantagsList = new List<OrganisationViewModels.UndantagForvantadfilViewModel>();
            var undantagFinnsList = new List<int>();
            foreach (var forvFil in forvFilList)
            {
                var undantagForvFil = new OrganisationViewModels.UndantagForvantadfilViewModel
                {
                    OrganisationsId = orgId,
                    DelregisterId = forvFil.DelregisterId,
                    ForvantadfilId = forvFil.Id,
                    Filmask = forvFil.Filmask
                };
                undantagFinnsList = uList.Where(x => x.ForvantadfilId == forvFil.Id).Select(x => x.ForvantadfilId).ToList();
                if (undantagFinnsList.Count != 0)
                {
                    undantagForvFil.Selected = true;
                }
                undantagsList.Add(undantagForvFil);
            }

            var sortedList = undantagsList.OrderBy(x => x.Filmask).ToList();

            return sortedList;
        }


        /// <summary>  
        /// Create list for delregister-dropdown  
        /// </summary>  
        /// <returns>Return delregister for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateRegisterDropDownList(IEnumerable<AdmRegister> registerList)
        {
            SelectList lstobj = null;

            var list = registerList
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

        private List<OrganisationViewModels.ContactViewModel> ConvertContactsForOrgToDropdownList(List<ApplicationUser> orgContactsList)
        {
            var listOfContacts = new List<OrganisationViewModels.ContactViewModel>();

            foreach (var orgContact in orgContactsList)
            {
                var contact = new OrganisationViewModels.ContactViewModel
                {
                    Id = orgContact.Id,
                    Email = orgContact.Email,
                    Selected = false
                };
                listOfContacts.Add(contact);
            }
            return listOfContacts;
        }


        /// <summary>  
        /// Create list for delregister-dropdown  
        /// </summary>  
        /// <returns>Return delregister for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateRegisterDropDownList(IEnumerable<RegisterInfo> registerList)
        {
            SelectList lstobj = null;

            var list = registerList
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
    }
}


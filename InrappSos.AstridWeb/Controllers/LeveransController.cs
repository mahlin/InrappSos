using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Threading.Tasks;
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
using Microsoft.Owin.Security.Provider;

namespace InrappSos.AstridWeb.Controllers
{
    public class LeveransController : Controller
    {

        private readonly IPortalAdminService _portalAdminService;

        public LeveransController()
        {
            _portalAdminService =
                new PortalAdminService(new PortalAdminRepository(new InrappSosDbContext(), new InrappSosIdentityDbContext()));
        }

        [Authorize]
        // GET: Leverans
        public ActionResult Index(bool filterPgnde = false, int regId = 0)
        {

            var model = new LeveransViewModels.LeveransViewModel();
            try
            {
                var tmpPagaende = Request.QueryString["filterPagaende"];
                var filterPagaende = false;

                if (tmpPagaende != null)
                {
                    Char delimiter = ',';
                    String[] substrings = tmpPagaende.Split(delimiter);
                    filterPagaende = Convert.ToBoolean(substrings.Last());
                }
                else
                {
                    filterPagaende = filterPgnde;
                }
                var forvLevList = _portalAdminService.HamtaForvantadeLeveranser();
                model.ForvantadeLeveranser = ConvertForvLevToViewModel(forvLevList.ToList());
                // Ladda drop down lists. 
                var registerList = _portalAdminService.HamtaAllaRegisterForPortalen();
                this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                model.SelectedRegisterId = regId;
                model.FilterPagaende = filterPagaende;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "Index", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av förväntade leveranser",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("Index", model);
        }

        // GET
        [Authorize]
        public ActionResult GetDirectorysExpectedDeliveries(LeveransViewModels.LeveransViewModel model, bool filterPgnde = false, int regId = 0)
        {
            try
            {
                var tmpPagaende = Request.QueryString["filterPagaende"];
                var filterPagaende = false;

                if (tmpPagaende != null)
                {
                    Char delimiter = ',';
                    String[] substrings = tmpPagaende.Split(delimiter);
                    filterPagaende = Convert.ToBoolean(substrings.Last());
                }
                else if (filterPgnde)
                {
                    filterPagaende = filterPgnde;
                }
                else
                {
                    filterPagaende = model.FilterPagaende;
                }
                var dirId = model.SelectedRegisterId;
                if (dirId == 0 && regId != 0)
                {
                    dirId = regId;
                }
                if (dirId != 0)
                {
                    var forvLevList = _portalAdminService.HamtaForvantadeLeveranserForRegister(dirId);
                    //Lägg över i modellen
                    model.ForvantadeLeveranser = ConvertForvLevToViewModel(forvLevList.ToList());
                    // Ladda drop down lists. 
                    var registerList = _portalAdminService.HamtaAllaRegisterForPortalen();
                    this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                    model.SelectedRegisterId = dirId;
                    model.FilterPagaende = filterPagaende;
                }
                else
                {
                    return RedirectToAction("Index", new { filterPgnde = filterPagaende });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetDirectorysExpectedDeliveries", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av förväntade leveranser för register",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("Index", model);
        }

        // GET
        [Authorize]
        public ActionResult GetDirectorysExpectedFiles(LeveransViewModels.LeveransViewModel model, int regId = 0)
        {
            try
            {
                var dirId = model.SelectedRegisterId;
                if (dirId == 0 && regId != 0)
                {
                    dirId = regId;
                }
                if (dirId != 0)
                {
                    var register = _portalAdminService.HamtaRegisterMedId(dirId);
                    var forvFilList = _portalAdminService.HamtaForvantadeFilerForRegister(register.Id);
                    //Lägg över i modellen
                    model.ForvantadeFiler = ConvertForvFilToViewModel(forvFilList.ToList());
                    // Ladda drop down lists. 
                    var registerList = _portalAdminService.HamtaAllaRegisterForPortalen();
                    this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                    model.SelectedRegisterId = dirId;
                }
                else
                {
                    return RedirectToAction("GetForvantadeFiler");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetDirectorysExpectedFiles", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av förväntade filer för register",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("EditForvantadFil", model);
        }

        // GET
        [Authorize]
        public ActionResult GetDirectorysFilerequirements(LeveransViewModels.LeveransViewModel model, int regId = 0)
        {
            try
            {
                var dirId = model.SelectedRegisterId;
                if (dirId == 0 && regId != 0)
                {
                    dirId = regId;
                }
                if (dirId != 0)
                {
                    var register = _portalAdminService.HamtaRegisterMedId(dirId);
                    var filkravList = _portalAdminService.HamtaFilkravForRegister(register.Id);
                    //Lägg över i modellen
                    model.Filkrav = ConvertFilkravToViewModel(filkravList.ToList());
                    // Ladda drop down lists. 
                    var registerList = _portalAdminService.HamtaAllaRegisterForPortalen();
                    this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                    model.SelectedRegisterId = dirId;
                    var insamlingsfrekvensList = _portalAdminService.HamtaAllaInsamlingsfrekvenser();
                    ViewBag.InsamlingsfrekvensList = CreateInsamlingsfrekvensDropDownList(insamlingsfrekvensList);
                    model.SelectedInsamlingsfrekvensId = 0;
                }
                else
                {
                    return RedirectToAction("GetFilkrav");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetDirectorysFilerequirements", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av filkrav för register",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("EditFilkrav", model);
        }

        

        // GET: AdmForvantadfil
        [Authorize]
        public ActionResult GetForvantadeFiler()
        {
            var model = new LeveransViewModels.LeveransViewModel();
            try
            {
                var forvFilViewList = new List<LeveransViewModels.AdmForvantadfilViewModel>();
                var forvFiler = _portalAdminService.HamtaAllaForvantadeFiler();

                foreach (var forvFil in forvFiler)
                {
                    var forvFilView = new LeveransViewModels.AdmForvantadfilViewModel
                        {
                          Id  = forvFil.Id,
                          FilkravId = forvFil.FilkravId,
                          FilkravNamn = _portalAdminService.HamtaNamnForFilkrav(forvFil.FilkravId),
                          ForeskriftsId = forvFil.ForeskriftsId,
                          DelregisterKortnamn = _portalAdminService.HamtaKortnamnForDelregisterMedFilkravsId(forvFil.FilkravId),
                          Filmask = forvFil.Filmask,
                          Regexp = forvFil.Regexp,
                          Obligatorisk = forvFil.Obligatorisk,
                          Tom = forvFil.Tom
                        };

                    forvFilViewList.Add(forvFilView);
                }

                model.ForvantadeFiler = forvFilViewList;

                // Ladda drop down lists. 
                var registerList = _portalAdminService.HamtaAllaRegisterForPortalen();
                this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                model.SelectedRegisterId = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetForvantadeFiler", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av forvantad fil",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditForvantadFil", model);
        }

        // GET: AdmFilkrav
        [Authorize]
        public ActionResult GetFilkrav(int regId = 0)
        {
            var model = new LeveransViewModels.LeveransViewModel();
            try
            {
                var filkravViewList = new List<LeveransViewModels.AdmFilkravViewModel>();
                var filkravList = _portalAdminService.HamtaAllaFilkrav();
               
                //Lägg över i modellen
                model.Filkrav = ConvertFilkravToViewModel(filkravList.ToList());

                // Ladda drop down lists. 
                var registerList = _portalAdminService.HamtaAllaRegisterForPortalen();
                this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                model.SelectedRegisterId = regId;
                var insamlingsfrekvensList = _portalAdminService.HamtaAllaInsamlingsfrekvenser();
                ViewBag.InsamlingsfrekvensList = CreateInsamlingsfrekvensDropDownList(insamlingsfrekvensList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetForvantadeFiler", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av forvantad fil",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditFilkrav", model);
        }

        // GET: AdmInsamlingsfrekvens
        [Authorize]
        public ActionResult GetInsamlingsfrekvens()
        {
            var model = new LeveransViewModels.LeveransViewModel();
            try
            {
                model.Insamlingsfrekvenser = _portalAdminService.HamtaAllaInsamlingsfrekvenser();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetInsamlingsfrekvens", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning insamlingsfrekvenser",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditInsamlingsfrekvens", model);
        }


        // GET
        [Authorize]
        public ActionResult GetDeliveries()
        {
            // Ladda drop down lists. 
            var orgListDTO = GetOrganisationDTOList();
            ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            return View("EditLeverans");
        }


        // GET
        public ActionResult GetOrganisationsDeliveries(int selectedOrganisationId)
        {
            var model = new LeveransViewModels.LeveransViewModel();

            try
            {
                var org = _portalAdminService.HamtaOrganisation(selectedOrganisationId);
                IEnumerable<FilloggDetaljDTO>
                    historyFileList = _portalAdminService.HamtaHistorikForOrganisation(org.Id);
                
                model.Leveranser = historyFileList;
                model.Kommunkod = org.Kommunkod;
                // Ladda drop down lists. 
                var orgListDTO = GetOrganisationDTOList();
                ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetOrganisationsDeliveries", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av leveranser för kommun.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("EditLeverans", model);
        }

        // GET
        [Authorize]
        public ActionResult GetReminderinfo()
        {
            var model = new LeveransViewModels.ReminderViewModel();
            model.AntRader = 0;
            //// Ladda drop down lists. 
            model = GetDropDownLists(model);
            
            return View("ReminderInfo", model);
        }


        // GET
        [Authorize]
        public ActionResult GetReminderInfoForRegAndPeriod(LeveransViewModels.ReminderViewModel model, int regId = 0, int delregId = 0, string period = "")
        {
            try
            {
                if (regId != 0)
                {
                    model.SelectedRegisterId = regId;
                }
                if (delregId != 0)
                {
                    model.SelectedDelregisterId = delregId;
                }
                if (!period.IsNullOrWhiteSpace())
                {
                    model.SelectedPeriod = period;
                }

                var rappList = new List<RapporteringsresultatDTO>();

                if (model.SelectedDelregisterId != 0)
                {
                    rappList = _portalAdminService.HamtaRapporteringsresultatForDelregOchPeriod(model.SelectedDelregisterId, model.SelectedPeriod).ToList();
                }
                else
                {
                    rappList = _portalAdminService.HamtaRapporteringsresultatForRegOchPeriod(model.SelectedRegisterId, model.SelectedPeriod).ToList();
                }
                
                model.RapportResList = rappList.ToList();
                model.AntRader = model.RapportResList.Count();
                model = GetDropDownLists(model);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "GetReminderInfoForRegAndPeriod", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av påminnelseinformation",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View("ReminderInfo", model);
        }

        //POST
        [Authorize]
        public async Task< ActionResult> SendReminder(LeveransViewModels.ReminderViewModel model)
        {
            try
            {
                var mail = false;
                model = GetDropDownLists(model);
                foreach (var rappRes in model.RapportResList)
                {
                    if (rappRes.Mail)
                        mail = true;
                }
                if (!mail)
                {
                    ModelState.AddModelError("", "Inga rader valda.");
                    return View("ReminderInfo",model);
                }
                var userId = User.Identity.GetUserId();
                _portalAdminService.SkickaPaminnelse(model.RapportResList, userId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("LeveransController", "SendReminder", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när påminnelsemail skulle skickas.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            ViewBag.StatusMessage = "Mail med fil innehållande epostadresser för valda leveranser har skickats.";
            return View("ReminderInfo", model);
        }

        // GET
        public ActionResult GetDeliveryStatus()
        {
            var model = new LeveransViewModels.HistoryViewModel();
            model.SelectableYears = new List<int>();
            // Ladda drop down lists. 
            var orgListDTO = GetOrganisationDTOList();
            ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            return View("LatestDeliveries", model);
        }

        // POST: Leveransstatus
        public ActionResult GetDeliveryStatusForOrg( int selectedOrganisationId,int chosenYear = 0)
        {
            var org = _portalAdminService.HamtaOrganisation(selectedOrganisationId);
            var model = new LeveransViewModels.HistoryViewModel();
            model.LeveransListaRegister = new List<RegisterLeveransDTO>();
            var selectableYearsForUser = new List<int>();

            try
            {
                if (chosenYear == 0)
                {
                    chosenYear = DateTime.Now.Year;
                }

                model.SelectedYear = chosenYear;
                model.Kommunkod = org.Kommunkod;
                model.SelectedOrganisationId = selectedOrganisationId;
                model.OrganisationsNamn = org.Organisationsnamn;
                IEnumerable<AdmRegister> admRegList = _portalAdminService.HamtaRegisterForOrg(org.Id);

                //hämta historik före resp register och period inom valt år
                foreach (var register in admRegList)
                {
                    var periodsForRegister = new List<string>();
                    var regLev = new RegisterLeveransDTO
                    {
                        RegisterKortnamn = register.Kortnamn,
                        Leveranser = new List<LeveransStatusDTO>()
                    };

                    //För att hitta giltiga perioder för valt år måste vi ner på registrets delregister
                    foreach (var delregister in register.AdmDelregister)
                    {
                        var delregPerioder = _portalAdminService.HamtaDelregistersPerioderForAr(delregister.Id, chosenYear);
                        //för varje period för delregistret, spara i lista med perioder för registret
                        foreach (var period in delregPerioder)
                        {
                            if (!periodsForRegister.Contains(period))
                                periodsForRegister.Add(period);
                        }
                        //Hämta valbara år för användarens valda register
                        var yearsForSubDir = _portalAdminService.HamtaValbaraAr(delregister.Id);
                        foreach (var year in yearsForSubDir)
                        {
                            if (!selectableYearsForUser.Contains(year))
                                selectableYearsForUser.Add(year);
                        }
                    }
                    //För varje (distinct) period i listan ovan spara i ett LeveransStatusDTO-objekt.
                    //I detta objekt spara registerId och registernamn oxå
                    //För varje period för registret hämta historik för alla delregister - spara som historiklista i LeveransStatusDTO-objekt
                    var i = 0;
                    foreach (var period in periodsForRegister)
                    {
                        i++;
                        var leveransStatus = new LeveransStatusDTO();
                        leveransStatus.Id = register.Id * 100 + i; //Behöver unikt id för togglingen i vyn
                        leveransStatus.RegisterId = register.Id;
                        leveransStatus.RegisterKortnamn = register.Kortnamn;
                        leveransStatus.Period = period;
                        //TODO - fulfix. Refactor this. Special för EKB-År
                        if (register.Kortnamn == "EKB" && period.Length == 4)
                        {
                            leveransStatus.Rapporteringsstart = _portalAdminService.HamtaRapporteringsstartForRegisterOchPeriodSpecial(register.Id, period);
                            leveransStatus.Rapporteringssenast = _portalAdminService.HamtaSenasteRapporteringForRegisterOchPeriodSpecial(register.Id, period);
                        }
                        else
                        {
                            leveransStatus.Rapporteringsstart = _portalAdminService.HamtaRapporteringsstartForRegisterOchPeriod(register.Id, period);
                            leveransStatus.Rapporteringssenast = _portalAdminService.HamtaSenasteRapporteringForRegisterOchPeriod(register.Id, period);
                        }
                        leveransStatus.HistorikLista = _portalAdminService.HamtaHistorikForOrganisationRegisterPeriod(org.Id, register.Id, period).ToList();
                        if (leveransStatus.HistorikLista.Any())
                        {
                            leveransStatus.Status = _portalAdminService.HamtaSammanlagdStatusForPeriod(leveransStatus.HistorikLista);
                        }
                        else
                        {
                            if (leveransStatus.Rapporteringsstart <= DateTime.Now)
                            {
                                leveransStatus.Status = "error";
                            }
                            else
                            {
                                leveransStatus.Status = "notStarted";
                            }

                        }

                        //Lägg hela DTO-objektet i regLev.Leveranser
                        regLev.Leveranser.Add(leveransStatus);
                    }

                    regLev.Leveranser = regLev.Leveranser.OrderBy(x => x.RegisterKortnamn).ThenBy(x => x.Period).ToList();
                    model.LeveransListaRegister.Add(regLev);
                    model.SelectableYears = selectableYearsForUser;
                }

                // Ladda drop down lists. 
                var orgListDTO = GetOrganisationDTOList();
                ViewBag.OrganisationList = new SelectList(orgListDTO, "Id", "KommunkodOchOrgnamn");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("HistoryController", "GetDeliveryStatus", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av senaste leveranser",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("LatestDeliveries", model);
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateForvantadLeverans(LeveransViewModels.AdmForvantadleveransViewModel forvLevModel, string regId = "0")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var forvLev = ConvertViewModelToForvLev(forvLevModel);
                    _portalAdminService.UppdateraForvantadLeverans(forvLev, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "UpdateForvantadLeverans", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av förväntad leverans.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetDirectorysExpectedDeliveries", new { regId = regId});

        }

        private AdmForvantadleverans ConvertViewModelToForvLev(LeveransViewModels.AdmForvantadleveransViewModel forvLevModel)
        {
            var forvLev = new AdmForvantadleverans
            {
                Id = forvLevModel.Id,
                FilkravId = forvLevModel.FilkravId,
                Period = forvLevModel.Period,
                Uppgiftsstart = forvLevModel.Uppgiftsstart,
                Uppgiftsslut = forvLevModel.Uppgiftsslut,
                Rapporteringsstart = forvLevModel.Rapporteringsstart,
                Rapporteringsslut = forvLevModel.Rapporteringsslut,
                Rapporteringsenast = forvLevModel.Rapporteringsenast,
                Paminnelse1 = forvLevModel.Paminnelse1,
                Paminnelse2 = forvLevModel.Paminnelse2,
                Paminnelse3 = forvLevModel.Paminnelse3
            };

            if (forvLevModel.SelectedDelregisterId > 0)
            {
                forvLev.DelregisterId = forvLevModel.SelectedDelregisterId;
            }
            else if (forvLevModel.DelregisterKortnamn != null)
            {
                forvLev.DelregisterId = _portalAdminService.HamtaDelRegisterForKortnamn(forvLevModel.DelregisterKortnamn).Id;
            }

            return forvLev;
        }

        private AdmForvantadfil ConvertViewModelToForvFil(LeveransViewModels.AdmForvantadfilViewModel forvFilModel)
        {
            var forvFil = new AdmForvantadfil
            {
                Id = forvFilModel.Id,
                Filmask = forvFilModel.Filmask,
                Regexp = forvFilModel.Regexp,
                Obligatorisk = forvFilModel.Obligatorisk,
                Tom = forvFilModel.Tom
            };

            return forvFil;
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateForvantadFil(LeveransViewModels.AdmForvantadfilViewModel forvFilModel, string regId = "0")
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var forvFil = ConvertViewModelToForvFil(forvFilModel);
                    _portalAdminService.UppdateraForvantadFil(forvFil, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "UpdateForvantadFil", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av förväntad fil.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetDirectorysExpectedFiles", new {regId = regId});

        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateFilkrav(LeveransViewModels.AdmFilkravViewModel filkrav, string regId = "0")
        {
            var x = filkrav.InsamlingsfrekvensId;
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var filkravDb = ConvertAdmFilkravVMToDb(filkrav);

                     _portalAdminService.UppdateraFilkrav(filkravDb, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "UpdateFilkrav", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av filkrav.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetDirectorysFileRequirements", new { regId = regId });

        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateInsamlingsfrekvens(AdmInsamlingsfrekvens insamlingsfrekvens)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalAdminService.UppdateraInsamlingsfrekvens(insamlingsfrekvens, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "UpdateInsamlingsfrekvens", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av insamlingsfrekvens.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetInsamlingsfrekvens");

        }

        // GET
        [Authorize]
        public ActionResult CreateForvantadeLeveranser(bool filterPgnde = false, int selectedRegId = 0)
        {
            // Ladda drop down lists
            var model = new LeveransViewModels.LeveransViewModel();
            model.RegisterList = _portalAdminService.HamtaDelregisterOchFilkrav();
            var delregisterList = _portalAdminService.HamtaAllaDelregisterForPortalen();
            ViewBag.DelregisterList = CreateDelRegisterDropDownList(delregisterList);
            ViewBag.FilkravList = CreateDummyFilkravDropDownList();
            ViewBag.YearList = CreateYearDropDownList();
            model.SelectedDelregisterId = 0;
            model.SelectedFilkravId = 0;
            model.SelectedRegisterId = selectedRegId;
            model.FilterPagaende = filterPgnde;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateForvantadeLeveranserDraft(LeveransViewModels.LeveransViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var forvLevList = _portalAdminService.SkapaForvantadeLeveranserUtkast(model.SelectedYear, model.SelectedDelregisterId, model.SelectedFilkravId);
                    //Lägg över i modellen
                    model.BlivandeForvantadeLeveranser = ConvertForvLevDTOToViewModel(forvLevList.ToList());
                    model.RegisterList = _portalAdminService.HamtaDelregisterOchFilkrav();
                    var delregisterList = _portalAdminService.HamtaAllaDelregisterForPortalen();
                    ViewBag.DelregisterList = CreateDelRegisterDropDownList(delregisterList);
                    ViewBag.FilkravList = CreateDummyFilkravDropDownList();
                    ViewBag.YearList = CreateYearDropDownList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "CreateForvantadeLeveranserDraft", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när utkast för förväntade leveranser skulle skapas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return View("CreateForvantadeLeveranser", model);
            }

            return View("CreateForvantadeLeveranser");
        }

        // POST
        //public ActionResult SaveForvantadeLeveranser(IEnumerable<LeveransViewModels.AdmForvantadleveransViewModel> forvLevLista)
        public ActionResult SaveForvantadeLeveranser(LeveransViewModels.LeveransViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var forvLevDbList = new List<AdmForvantadleverans>();

                    foreach (var forvLev in model.BlivandeForvantadeLeveranser)
                    {
                        if (!forvLev.AlreadyExists) //spara endast nya
                        {
                            forvLev.SelectedDelregisterId = model.SelectedDelregisterId;
                            var forvLevDB = ConvertViewModelToForvLev(forvLev);
                            forvLevDbList.Add(forvLevDB);
                        }
                    }
                    _portalAdminService.SkapaForvantadLeveranser(forvLevDbList, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "SaveForvantadeLeveranser", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när förväntade leveranser skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        // GET
        [Authorize]
        public ActionResult CreateForvantadLeverans(bool filterPgnde = false, int selectedRegId = 0)
        {
            // Ladda drop down lists
            var model = new LeveransViewModels.AdmForvantadleveransViewModel();
            model.RegisterList = _portalAdminService.HamtaDelregisterOchFilkrav();
            var delregisterList = _portalAdminService.HamtaAllaDelregisterForPortalen();
            ViewBag.DelregisterList = CreateDelRegisterDropDownList(delregisterList);
            ViewBag.FilkravList = CreateDummyFilkravDropDownList();
            model.SelectedDelregisterId = 0;
            model.SelectedFilkravId = 0;
            model.SelectedRegisterId = selectedRegId;
            model.Pagaende = filterPgnde;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateForvantadLeverans(LeveransViewModels.AdmForvantadleveransViewModel forvantadLeverans)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    var admForvlev = new AdmForvantadleverans();
                    admForvlev.DelregisterId = forvantadLeverans.SelectedDelregisterId;
                    admForvlev.FilkravId = forvantadLeverans.SelectedFilkravId;
                    admForvlev.Period = forvantadLeverans.Period;
                    admForvlev.Uppgiftsstart = forvantadLeverans.Uppgiftsstart;
                    admForvlev.Uppgiftsslut = forvantadLeverans.Uppgiftsslut;
                    admForvlev.Rapporteringsstart = forvantadLeverans.Rapporteringsstart;
                    admForvlev.Rapporteringsslut = forvantadLeverans.Rapporteringsslut;
                    admForvlev.Rapporteringsenast = forvantadLeverans.Rapporteringsenast;
                    _portalAdminService.SkapaForvantadLeverans(admForvlev, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "CreateForvantadLeverans", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny förväntad leverans skulle sparas.",
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
        public ActionResult CreateForvantadFil(int selectedRegId = 0)
        {
            // Ladda drop down lists
            var model = new LeveransViewModels.AdmForvantadfilViewModel();
            model.RegisterList = _portalAdminService.HamtaDelregisterOchFilkrav();
            var delregisterList = _portalAdminService.HamtaAllaDelregisterForPortalen();
            this.ViewBag.DelregisterList = CreateDelRegisterDropDownList(delregisterList);
            ViewBag.FilkravList = CreateDummyFilkravDropDownList();
            model.SelectedDelregisterId = 0;
            model.SelectedFilkravId = 0;
            model.SelectedRegisterId = selectedRegId;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateForvantadFil(LeveransViewModels.AdmForvantadfilViewModel forvantadFil)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    var admForvFil = new AdmForvantadfil();
                    admForvFil.FilkravId = forvantadFil.SelectedFilkravId;
                    if (forvantadFil.SelectedFilkravId > 0)
                    {
                        admForvFil.ForeskriftsId = _portalAdminService.HamtaForeskriftByFilkrav(forvantadFil.SelectedFilkravId).Id;
                    }
                    admForvFil.Filmask= forvantadFil.Filmask;
                    admForvFil.Regexp = forvantadFil.Regexp;
                    admForvFil.Obligatorisk = forvantadFil.Obligatorisk;
                    admForvFil.Tom = forvantadFil.Tom;
                    _portalAdminService.SkapaForvantadFil(admForvFil, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "CreateForvantadFil", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny förväntad fil skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetForvantadeFiler");
            }

            return View();
        }

        // GET
        [Authorize]
        public ActionResult CreateFilkrav(int selectedRegId = 0)
        {
            // Ladda drop down lists
            var model = new LeveransViewModels.AdmFilkravViewModel();
            var delregisterList = _portalAdminService.HamtaAllaDelregisterForPortalen();
            this.ViewBag.DelregisterList = CreateDelRegisterDropDownList(delregisterList);
            model.SelectedDelregisterId = 0;
            model.SelectedRegisterId = selectedRegId;
            var insamlingsfrekvensList = _portalAdminService.HamtaAllaInsamlingsfrekvenser();
            ViewBag.InsamlingsfrekvensList = CreateInsamlingsfrekvensDropDownList(insamlingsfrekvensList);
            model.InsamlingsfrekvensId = 0;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateFilkrav(LeveransViewModels.AdmFilkravViewModel filkrav)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    var admFilkrav = ConvertAdmFilkravVMToDb(filkrav);
                    _portalAdminService.SkapaFilkrav(admFilkrav, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "CreateFilkrav", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när nytt filkrav skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetFilkrav");
            }

            return View();
        }

        // GET
        [Authorize]
        public ActionResult CreateInsamlingsfrekvens()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateInsamlingsfrekvens(LeveransViewModels.LeveransViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalAdminService.SkapaInsamlingsfrekvens(model.Insamlingsfrekvens, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("LeveransController", "CreateInsamlingsfrekvens", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny insamlingsfrekvens skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetInsamlingsfrekvens");
            }

            return View();
        }

        private List<LeveransViewModels.AdmForvantadfilViewModel> ConvertForvFilToViewModel(List<AdmForvantadfil> forvFilList)
        {
            var forvFilViewList = new List<LeveransViewModels.AdmForvantadfilViewModel>();
            foreach (var forvFil in forvFilList)
            {
                var forvFilView = new LeveransViewModels.AdmForvantadfilViewModel
                {
                    Id = forvFil.Id,
                    FilkravId = forvFil.FilkravId,
                    DelregisterKortnamn = _portalAdminService.HamtaKortnamnForDelregisterMedFilkravsId(forvFil.FilkravId),
                    Filmask = forvFil.Filmask,
                    Regexp = forvFil.Regexp,
                    Obligatorisk = forvFil.Obligatorisk,
                    Tom = forvFil.Tom
                };

                forvFilViewList.Add(forvFilView);
            }
            return forvFilViewList;
        }

        private List<LeveransViewModels.AdmForvantadleveransViewModel> ConvertForvLevToViewModel(List<AdmForvantadleverans> forvLevList)
        {
            var forvLevViewList = new List<LeveransViewModels.AdmForvantadleveransViewModel>();
            foreach (var forvLev in forvLevList)
            {
                var forvLevView = new LeveransViewModels.AdmForvantadleveransViewModel
                {
                    Id = forvLev.Id,
                    FilkravId = forvLev.FilkravId,
                    FilkravNamn = _portalAdminService.HamtaNamnForFilkrav(forvLev.FilkravId),
                    DelregisterId = forvLev.DelregisterId,
                    DelregisterKortnamn = _portalAdminService.HamtaKortnamnForDelregisterMedFilkravsId(forvLev.FilkravId),
                    Period = forvLev.Period,
                    Uppgiftsstart = forvLev.Uppgiftsstart,
                    Uppgiftsslut = forvLev.Uppgiftsslut,
                    Rapporteringsstart = forvLev.Rapporteringsstart,
                    Rapporteringsslut = forvLev.Rapporteringsslut,
                    Rapporteringsenast = forvLev.Rapporteringsenast,
                    Paminnelse1 = forvLev.Paminnelse1,
                    Paminnelse2 = forvLev.Paminnelse2,
                    Paminnelse3 = forvLev.Paminnelse3,
                    Pagaende = IsOngoing(forvLev),
                    Sen = IsLate(forvLev)

                };

                if (!String.IsNullOrEmpty(forvLevView.FilkravNamn))
                {
                    forvLevView.FilkravNamn = forvLevView.FilkravNamn.Trim();
                }

                forvLevViewList.Add(forvLevView);
            }
            return forvLevViewList;
        }

        private List<LeveransViewModels.AdmForvantadleveransViewModel> ConvertForvLevDTOToViewModel(List<ForvantadLeveransDTO> forvLevList)
        {
            var forvLevViewList = new List<LeveransViewModels.AdmForvantadleveransViewModel>();
            foreach (var forvLev in forvLevList)
            {
                var forvLevView = new LeveransViewModels.AdmForvantadleveransViewModel
                {
                    Id = forvLev.Id,
                    FilkravId = forvLev.FilkravId,
                    FilkravNamn = _portalAdminService.HamtaNamnForFilkrav(forvLev.FilkravId),
                    DelregisterId = forvLev.DelregisterId,
                    DelregisterKortnamn = _portalAdminService.HamtaKortnamnForDelregisterMedFilkravsId(forvLev.FilkravId),
                    Period = forvLev.Period,
                    Uppgiftsstart = forvLev.Uppgiftsstart,
                    Uppgiftsslut = forvLev.Uppgiftsslut,
                    Rapporteringsstart = forvLev.Rapporteringsstart,
                    Rapporteringsslut = forvLev.Rapporteringsslut,
                    Rapporteringsenast = forvLev.Rapporteringsenast,
                    Paminnelse1 = forvLev.Paminnelse1,
                    Paminnelse2 = forvLev.Paminnelse2,
                    Paminnelse3 = forvLev.Paminnelse3,
                    AlreadyExists = forvLev.AlreadyExists
                };

                forvLevViewList.Add(forvLevView);
            }
            return forvLevViewList;
        }

        private List<LeveransViewModels.AdmFilkravViewModel> ConvertFilkravToViewModel(List<AdmFilkrav> filkravList)
        {
            var filkravViewList = new List<LeveransViewModels.AdmFilkravViewModel>();
            foreach (var filkrav in filkravList)
            {
                var filkravView = new LeveransViewModels.AdmFilkravViewModel
                {
                    Id = filkrav.Id,
                    DelregisterId = filkrav.DelregisterId,
                    DelregisterKortnamn = _portalAdminService.HamtaKortnamnForDelregister(filkrav.DelregisterId),
                    InsamlingsfrekvensId = filkrav.InsamlingsfrekvensId,
                    ForeskriftsId = filkrav.ForeskriftsId,
                    Namn = filkrav.Namn,
                    Uppgiftsstartdag = filkrav.Uppgiftsstartdag,
                    Uppgiftslutdag = filkrav.Uppgiftslutdag,
                    Rapporteringsstartdag = filkrav.Rapporteringsstartdag,
                    Rapporteringsslutdag = filkrav.Rapporteringsslutdag,
                    RapporteringSenastdag = filkrav.RapporteringSenastdag,
                    Paminnelse1dag = filkrav.Paminnelse1dag,
                    Paminnelse2dag = filkrav.Paminnelse2dag,
                    Paminnelse3dag = filkrav.Paminnelse3dag,
                    RapporteringEfterAntalManader = filkrav.RapporteringEfterAntalManader,
                    UppgifterAntalmanader = filkrav.UppgifterAntalmanader
                };

                if (filkravView.InsamlingsfrekvensId != null)
                {
                    var id = filkravView.InsamlingsfrekvensId.Value;
                    filkravView.Insamlingsfrekvens =
                        _portalAdminService.HamtaInsamlingsfrekvens(id).Insamlingsfrekvens;
                    filkravView.InsamlingsfrekvensId = filkravView.InsamlingsfrekvensId.Value;
                }
                else
                {
                    filkravView.Insamlingsfrekvens = String.Empty;
                }

                filkravViewList.Add(filkravView);
            }
            return filkravViewList;
        }

        private AdmFilkrav ConvertAdmFilkravVMToDb(LeveransViewModels.AdmFilkravViewModel filkrav)
        {
            var admFilkravDb = new AdmFilkrav
            {
                Id = filkrav.Id,
                Namn = filkrav.Namn,
                DelregisterId = filkrav.SelectedDelregisterId,
                InsamlingsfrekvensId = filkrav.InsamlingsfrekvensId,
                ForeskriftsId = filkrav.ForeskriftsId,
                Uppgiftsstartdag = filkrav.Uppgiftsstartdag,
                Uppgiftslutdag = filkrav.Uppgiftslutdag,
                Rapporteringsstartdag = filkrav.Rapporteringsstartdag,
                Rapporteringsslutdag = filkrav.Rapporteringsslutdag,
                RapporteringSenastdag = filkrav.RapporteringSenastdag,
                Paminnelse1dag = filkrav.Paminnelse1dag,
                Paminnelse2dag = filkrav.Paminnelse2dag,
                Paminnelse3dag = filkrav.Paminnelse3dag,
                RapporteringEfterAntalManader = filkrav.RapporteringEfterAntalManader,
                UppgifterAntalmanader = filkrav.UppgifterAntalmanader
            };

            return admFilkravDb;
        }


        private bool IsOngoing(AdmForvantadleverans forvLev)
        {
            var result = false;
            DateTime dagensDatum = DateTime.Now.Date;
            DateTime startDate;
            DateTime endDate;

            startDate = forvLev.Rapporteringsstart;
            endDate = forvLev.Rapporteringsslut;
            if (dagensDatum >= startDate && dagensDatum <= endDate)
            {
                result = true;
            }
            return result;

        }

        private bool IsLate(AdmForvantadleverans forvLev)
        {
            var result = false;
            DateTime omTvaVeckor = DateTime.Now.Date.AddDays(14);
            DateTime endDate;

            endDate = forvLev.Rapporteringsslut;
            if (omTvaVeckor >= endDate)
            {
                result = true;
            }
            return result;
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

        /// <summary>  
        /// Create list for delregister-dropdown  
        /// </summary>  
        /// <returns>Return delregister for drop down list.</returns>  
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


        /// <summary>  
        /// Create list for delregister-dropdown  
        /// </summary>  
        /// <returns>Return delregister for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateInsamlingsfrekvensDropDownList(IEnumerable<AdmInsamlingsfrekvens> insamlingsfrekvensList)
        {
            SelectList lstobj = null;

            var list = insamlingsfrekvensList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Insamlingsfrekvens
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }

        /// <summary>  
        /// Create dummt-list for filkrav-dropdown  
        /// </summary>  
        /// <returns>Return dummy for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateDummyFilkravDropDownList()
        {
            SelectList lstobj = null;

            var list = new List<SelectListItem>();

            lstobj = new SelectList(list,"Value", "Text");

            return lstobj;
        }

        /// <summary>  
        /// Create list for year-dropdown  
        /// </summary>  
        /// <returns>Return years for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateYearDropDownList()
        {
            SelectList lstobj = null;

            var yearList = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                yearList.Add(DateTime.Now.Year + i);
            }

            var list = yearList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.ToString(),
                        Text = p.ToString()
                    });

            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }

        private LeveransViewModels.ReminderViewModel GetDropDownLists(LeveransViewModels.ReminderViewModel model)
        {
            var registerList = _portalAdminService.HamtaAllaRegisterForPortalen();
            var regListDTO = new List<RegisterBasicDTO>();
           
            foreach (var registerDB in registerList)
            {
                var registerDTO = new RegisterBasicDTO
                {
                    Id = registerDB.Id,
                    Registernamn = registerDB.Registernamn,
                    Kortnamn = registerDB.Kortnamn
                };
                var delregListDTO = new List<DelregisterBasicDTO>();

                var delregisterList = _portalAdminService.HamtaDelRegisterForRegister(registerDB.Id);
                foreach (var delregisterDB in delregisterList)
                {
                    var delregDTO = new DelregisterBasicDTO
                    {
                        Id = delregisterDB.Id,
                        Delregisternamn = delregisterDB.Delregisternamn,
                        Kortnamn = delregisterDB.Kortnamn
                    };

                    var forvLevList = _portalAdminService.HamtaForvantadeLeveranserForDelregister(delregisterDB.Id);
                    var forvLevListDTO = new List<ForvantadLeveransBasicDTO>();
                    foreach (var forvLevDB in forvLevList)
                    {
                        var forvlevDTO = new ForvantadLeveransBasicDTO()
                        {
                            Id = forvLevDB.Id,
                            FilkravId = forvLevDB.FilkravId,
                            Period = forvLevDB.Period
                        };
                        forvLevListDTO.Add(forvlevDTO);
                    }
                    delregDTO.ForvantadeLeveranserList = forvLevListDTO;
                    delregListDTO.Add(delregDTO);
                }
                registerDTO.DelregisterList = delregListDTO;
                regListDTO.Add(registerDTO);
            }
            model.RegisterList = regListDTO;

            // Ladda första dropdownlist (Register). 
            ViewBag.RegisterList = new SelectList(registerList, "Id", "Kortnamn");

            //model.SelectedRegisterId = 0;

            return model;
        }

        private IEnumerable<OrganisationDTO> GetOrganisationDTOList()
        {
            var orgList = _portalAdminService.HamtaAllaOrganisationer();
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
    }
}
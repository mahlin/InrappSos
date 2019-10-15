using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using InrappSos.DataAccess;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Helpers;
using InrappSos.ApplicationService.Interface;
using InrappSos.DomainModel;
using InrappSos.FilipWeb.Models;
using InrappSos.FilipWeb.Models;
using Microsoft.AspNet.Identity;

namespace InrappSos.FilipWeb.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IPortalSosService _portalService;


        public HistoryController()
        {
            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));

        }

        [Authorize]
        // GET: History
        public ActionResult Index(int selectedRegId = 0)
        {
            //Kolla om öppet, annars visa stängt-sida
            if (!_portalService.IsOpen())
            {
                ViewBag.Text = _portalService.HamtaInfoText("Stangtsida").Text;
                return View("Closed");
            }
            var model = new HistoryViewModels.HistoryViewModel();
            try
            {
                var userId = User.Identity.GetUserId();
                var userOrg = _portalService.HamtaOrgForAnvandare(User.Identity.GetUserId());
                IEnumerable<FilloggDetaljDTO> historyFileList = _portalService.HamtaHistorikForOrganisation(userOrg.Id);
                
                var valdaDelregisterInfoList = _portalService.HamtaValdaDelregisterForAnvandare(User.Identity.GetUserId(), userOrg.Id);

                //Filtrera historiken utfrån användarens valda register och enheter
                IEnumerable<FilloggDetaljDTO> filteredHistoryFileList = _portalService.FiltreraHistorikForAnvandare(userId, valdaDelregisterInfoList, historyFileList.ToList());

                model.HistorikLista = filteredHistoryFileList.ToList();

                model.OrganisationsNamn = userOrg.Organisationsnamn;
                IEnumerable<AdmRegister> admRegList = _portalService.HamtaRegisterForAnvandare(userId, userOrg.Id);
                model.RegisterList = ConvertAdmRegisterToViewModel(admRegList.ToList());
                model.SelectedYear = DateTime.Now.Year;
                model.SelectedRegisterId = selectedRegId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("HistoryController", "Index", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade i historiksidan.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }


            return View(model);
        }

        // GET: Leveransstatus
        [Authorize]
        public ActionResult GetDeliveryStatus(int chosenYear = 0)
        {
            var model = new HistoryViewModels.HistoryViewModel();
            model.LeveransListaRegister = new List<RegisterLeveransDTO>();
            var selectableYearsForUser = new List<int>();
            
            try
            {
                if (chosenYear == 0)
                {
                    chosenYear = DateTime.Now.Year;
                }
                model.SelectedYear = chosenYear;

                var userId = User.Identity.GetUserId();
                var userOrg = _portalService.HamtaOrgForAnvandare(User.Identity.GetUserId());
                model.OrganisationsNamn = userOrg.Organisationsnamn;
                //Hämta register och delregister utifrån användarens val
                IEnumerable<AdmRegister> admRegList = _portalService.HamtaRegisterForAnvandare(userId, userOrg.Id);
                var delregForAnvandareList = _portalService.HamtaAnvandarensValdaDelregister(userId, userOrg.Id).ToList();

                //hämta historik för resp register och period inom valt år
                foreach (var register in admRegList)
                {
                    var periodsForRegister = new List<string>();
                    var regLev = new RegisterLeveransDTO
                    {
                        RegisterKortnamn = register.Kortnamn,
                        Leveranser = new List<LeveransStatusDTO>()
                    };
                    //var delregList = _portalService.HamtaDelRegisterMedUndertabellerForRegister(register.Id).ToList();

                    //var delregList = _portalService.HamtaDelRegisterMedUndertabeller(delregForAnvandareList).ToList();
                    //var delregList = delregForAnvandareList.Where(x => x.RegisterId == register.Id).ToList();
                    var delregList = _portalService.HamtaAnvandarensDelregisterForRegister(register.Id, delregForAnvandareList).ToList();


                    //För att hitta giltiga perioder för valt år måste vi ner på registrets delregister
                    foreach (var delregister in delregList)
                    {
                        var delregPerioder = _portalService.HamtaDelregistersPerioderForAr(delregister.Id, chosenYear);
                        //för varje period för delregistret, spara i lista med perioder för registret
                        foreach (var period in delregPerioder)
                        {
                            if (!periodsForRegister.Contains(period))
                                periodsForRegister.Add(period);
                        }
                        //Hämta valbara år för användarens valda register
                        var yearsForSubDir = _portalService.HamtaValbaraAr(delregister.Id);
                        foreach (var year in yearsForSubDir)
                        {
                            if (!selectableYearsForUser.Contains(year))
                                selectableYearsForUser.Add(year);
                        }
                    }
                    //Förväntade leveranser för aktuella perioder
                    var forvlevList = _portalService.HamtaForvLevForRegisterOchPerioder(delregList, periodsForRegister);
                    var leveransStatusRapportList = _portalService.HamtaLeveransStatusRapporterForOrgDelregPerioder(userOrg.Id, delregList, periodsForRegister).ToList();

                    if (leveransStatusRapportList != null)
                    {
                        //För varje (distinct) period i listan ovan spara i ett LeveransStatusDTO-objekt.
                        //I detta objekt spara registerId och registernamn oxå
                        //För varje period för registret hämta historik för alla delregister - spara som historiklista i LevereansStatusDTO-objekt
                        var i = 0;
                        foreach (var period in periodsForRegister)
                        {
                            i++;
                            var leveransStatus = new LeveransStatusDTO();
                            leveransStatus.Id = register.Id * 100 + i; //Behöver unikt id för togglingen i vyn
                            leveransStatus.RegisterId = register.Id;
                            leveransStatus.RegisterKortnamn = register.Kortnamn;
                            leveransStatus.Period = period;
                            leveransStatus.Rapporteringsstart = forvlevList.FirstOrDefault(x => x.Period == period).Rapporteringsstart;
                            leveransStatus.Rapporteringssenast = forvlevList.FirstOrDefault(x => x.Period == period).Rapporteringsenast;

                            leveransStatus.HistorikLista = _portalService.HamtaHistorikForOrganisationRegisterPeriodUser(userOrg.Id, delregList, period, leveransStatusRapportList, userId).ToList();
                            if (leveransStatus.HistorikLista.Any())
                            {
                                leveransStatus.Status = _portalService.HamtaSammanlagdStatusForPeriod(leveransStatus.HistorikLista);
                                //kan org rapportera per enhet för registrets delregister? => kontrollera att alla enheter rapporterat (#180)
                                //Änding 2019-10-15 - nu ska bara de av användaren valda enheterna för aktuellt delregister kontrolleras
                                //leveransStatus.Status = _portalService.KontrolleraOmKomplettaEnhetsleveranser(userOrg.Id, leveransStatus, delregList);
                                leveransStatus.Status = _portalService.KontrolleraOmKomplettaEnhetsleveranserForAnv(userOrg.Id,leveransStatus, delregList, userId);

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
                    }

                    regLev.Leveranser = regLev.Leveranser.OrderBy(x => x.RegisterKortnamn).ThenBy(x => x.Period).ToList();
                    model.LeveransListaRegister.Add(regLev);
                    model.SelectableYears = selectableYearsForUser;
                }
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

        private List<HistoryViewModels.AdmRegisterViewModel> ConvertAdmRegisterToViewModel(List<AdmRegister> admRegisterList)
        {
            var registerList = new List<HistoryViewModels.AdmRegisterViewModel>();
            foreach (var register in admRegisterList)
            {
                var delregisterList = new List<HistoryViewModels.AdmDelregisterViewModel>();
                var registerView = new HistoryViewModels.AdmRegisterViewModel()
                {
                    Id = register.Id,
                    Registernamn = register.Registernamn,
                    Beskrivning = register.Beskrivning,
                    Kortnamn = register.Kortnamn
                };

                foreach (var delregister in register.AdmDelregister)
                {
                    var delregisterView = new HistoryViewModels.AdmDelregisterViewModel()
                    {
                        Id = delregister.Id,
                        RegisterId = delregister.RegisterId,
                        Delregisternamn = delregister.Delregisternamn,
                        Kortnamn = delregister.Kortnamn,
                        Beskrivning = delregister.Beskrivning
                    };
                    delregisterList.Add(delregisterView);

                }
                registerView.DelRegister = delregisterList.ToList();
                registerList.Add(registerView);
            }
            return registerList;
        }
    }
}
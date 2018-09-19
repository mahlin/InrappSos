using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Interface;
using InrappSos.ApplicationService.Helpers;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.ApplicationService
{
    public class PortalAdminService : IPortalAdminService
    {

        private readonly IPortalAdminRepository _portalAdminRepository;

        public PortalAdminService(IPortalAdminRepository portalRepository)
        {
            _portalAdminRepository = portalRepository;
        }

        public Organisation HamtaOrganisation(int orgId)
        {
            var org = _portalAdminRepository.GetOrganisation(orgId);
            return org;
        }

        public string HamtaKommunkodForOrg(int orgId)
        {
            var kommunkod = _portalAdminRepository.GetKommunkodForOrg(orgId);
            return kommunkod;
        }

        public IEnumerable<ApplicationUser> HamtaKontaktpersonerForOrg(int orgId)
        {
            var contacts = _portalAdminRepository.GetContactPersonsForOrg(orgId);
            return contacts;
        }

        public IEnumerable<AppUserAdmin> HamtaAdminUsers()
        {
            var adminUsers = _portalAdminRepository.GetAdminUsers();
            return adminUsers;
        }

        public Organisation HamtaOrganisationForKommunkod(string kommunkod)
        {
            var org = _portalAdminRepository.GetOrganisationFromKommunkod(kommunkod);
            return org;
        }

        public IEnumerable<Organisationsenhet> HamtaOrgEnheterForOrg(int orgId)
        {
            var orgEnheter = _portalAdminRepository.GetOrgUnitsForOrg(orgId);
            return orgEnheter;
        }

        public IEnumerable<AdmUppgiftsskyldighet> HamtaUppgiftsskyldighetForOrg(int orgId)
        {
            var uppgiftsskyldigheter = _portalAdminRepository.GetReportObligationInformationForOrg(orgId);
            return uppgiftsskyldigheter;
        }

        public AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrgOchDelreg(int orgId, int delregId)
        {
            var uppgiftsskyldighet = _portalAdminRepository.GetReportObligationInformationForOrgAndSubDir(orgId, delregId);
            return uppgiftsskyldighet;
        }

        public IEnumerable<AdmEnhetsUppgiftsskyldighet> HamtaEnhetsUppgiftsskyldighetForOrgEnhet(int orgenhetId)
        {
            var uppgiftsskyldigheter = _portalAdminRepository.GetUnitReportObligationInformationForOrgUnit(orgenhetId);
            return uppgiftsskyldigheter;
        }
        public Organisation HamtaOrgForAnvandare(string userId)
        {
            var org = _portalAdminRepository.GetOrgForUser(userId);
            return org;
        }

        public Organisation HamtaOrgForOrganisationsenhet(int orgUnitId)
        {
            var org = _portalAdminRepository.GetOrgForOrgUnit(orgUnitId);
            return org;
        }

        public Organisation HamtaOrgForUppgiftsskyldighet(int uppgSkId)
        {
            var org = _portalAdminRepository.GetOrgForReportObligation(uppgSkId);
            return org;
        }

        public IEnumerable<AdmFAQKategori> HamtaFAQkategorier()
        {
            var faqCats = _portalAdminRepository.GetFAQCategories();
            return faqCats;
        }

        public IEnumerable<AdmFAQ> HamtaFAQs(int faqCatId)
        {
            var faqs = _portalAdminRepository.GetFAQs(faqCatId);
            return faqs;
        }

        public IEnumerable<AdmHelgdag> HamtaAllaHelgdagar()
        {
            var helgdagar = _portalAdminRepository.GetAllHolidays();
            return helgdagar;
        }

        public IEnumerable<AdmSpecialdag> HamtaAllaSpecialdagar()
        {
            var specialdagar = _portalAdminRepository.GetAllSpecialDays();
            return specialdagar;
        }

        public IEnumerable<AdmInformation> HamtaInformationstexter()
        {
            var infoTexts = _portalAdminRepository.GetInformationTexts();
            return infoTexts;
        }

        public AdmInsamlingsfrekvens HamtaInsamlingsfrekvens(int insamlingsid)
        {
            var insamlingsfrekvens = _portalAdminRepository.GetInsamlingsfrekvens(insamlingsid);
            return insamlingsfrekvens;
        }

        public OpeningHoursInfoDTO HamtaOppettider()
        {
            var configInfo = _portalAdminRepository.GetAdmConfiguration();
            var oppettiderObj = new OpeningHoursInfoDTO();

            foreach (var item in configInfo)
            {
                switch (item.Typ)
                {
                    case "ClosedFromHour":
                        oppettiderObj.ClosedFromHour = item.Varde;
                        break;
                    case "ClosedFromMin":
                        oppettiderObj.ClosedFromMin = item.Varde;
                        break;
                    case "ClosedToHour":
                        oppettiderObj.ClosedToHour = item.Varde;
                        break;
                    case "ClosedToMin":
                        oppettiderObj.ClosedToMin = item.Varde;
                        break;
                    case "ClosedAnyway":
                        oppettiderObj.ClosedAnyway = Convert.ToBoolean(item.Varde);
                        break;
                    case "ClosedDays":
                        oppettiderObj.ClosedDays= item.Varde.Split(new char[] { ',' }).ToList();
                        break;
                }
            }

            return oppettiderObj;
        }

        public AdmInformation HamtaInfoText(string infoTyp)
        {
            var info = _portalAdminRepository.GetInfoText(infoTyp);
            return info;
        }

        public AdmInformation HamtaInfo(int infoId)
        {
            var info = _portalAdminRepository.GetInfoText(infoId);
            return info;
        }

        public IEnumerable<AdmRegister> HamtaRegister()
        {
            var registerList = _portalAdminRepository.GetDirectories();
            return registerList;
        }

        public AdmRegister HamtaRegisterMedKortnamn(string regKortNamn)
        {
            var register = _portalAdminRepository.GetDirectoryByShortName(regKortNamn);
            return register;
        }

        public AdmRegister HamtaRegisterMedId(int regId)
        {
            var register = _portalAdminRepository.GetDirectoryById(regId);
            return register;
        }

        public IEnumerable<AdmDelregister> HamtaDelRegister()
        {
            var subDirectories = _portalAdminRepository.GetSubDirectories();
            return subDirectories;
        }

        public IEnumerable<AdmDelregister> HamtaDelRegisterForRegister(int regId)
        {
            var subDirectories = _portalAdminRepository.GetSubDirectoriesForDirectory(regId);
            return subDirectories;
        }

        public AdmDelregister HamtaDelRegisterForUppgiftsskyldighet(int uppgSkId)
        {
            var reportObligation = _portalAdminRepository.GetReportObligationById(uppgSkId);
            var subdir = _portalAdminRepository.GetSubDirectoryById(reportObligation.DelregisterId);
            return subdir;
        }


        public AdmDelregister HamtaDelRegisterForKortnamn(string shortName)
        {
            var subDirectory = _portalAdminRepository.GetSubDirectoryByShortName(shortName);
            return subDirectory;
        }

        public IEnumerable<RegisterInfo> HamtaDelregisterOchFilkrav()
        {
            var delregMedFilkravList = new List<RegisterInfo>();

            var delregList = _portalAdminRepository.GetAllSubDirectoriesForPortal();
            foreach (var delreg in delregList)
            {
                var regFilkravList = new List<RegisterFilkrav>();
                var regInfo = new RegisterInfo
                {
                    Id = delreg.Id,
                    Kortnamn = delreg.Kortnamn
                };

                //Hämta varje delregisters filkrav
                var filkravList = _portalAdminRepository.GetFileRequirementsForSubDirectory(delreg.Id).ToList();
                foreach (var filkrav in filkravList)
                {
                    var regFilkrav = new RegisterFilkrav
                    {
                        Id = filkrav.Id
                    };
                    var namn = delreg.Kortnamn;
                    if (filkrav.Namn != null)
                    {
                        namn = namn + " - " + filkrav.Namn;
                    }
                    regFilkrav.Namn = namn;
                    regFilkravList.Add(regFilkrav);
                }

                regInfo.Filkrav = regFilkravList;
                delregMedFilkravList.Add(regInfo);
            }
            return delregMedFilkravList;
        }


        public IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranser()
        {
            var forvlevList = _portalAdminRepository.GetExpectedDeliveries();
            return forvlevList;
        }

        public IEnumerable<AdmForvantadfil> HamtaAllaForvantadeFiler()
        {
            var forvFilList = _portalAdminRepository.GetAllExpectedFiles();
            return forvFilList;
        }

        public IEnumerable<AdmFilkrav> HamtaAllaFilkrav()
        {
            var filkravList = _portalAdminRepository.GetAllFileRequirements();
            return filkravList;
        }

        public IEnumerable<AdmInsamlingsfrekvens> HamtaAllaInsamlingsfrekvenser()
        {
            var insamlingsfrekvensList = _portalAdminRepository.GetAllCollectionFrequencies();
            return insamlingsfrekvensList;
        }

        public string HamtaKortnamnForDelregisterMedFilkravsId(int filkravId)
        {
            var delRegKortnamn = _portalAdminRepository.GetSubDirectoryShortNameForExpectedFile(filkravId);
            return delRegKortnamn;
        }

        public string HamtaKortnamnForRegister(int regId)
        {
            var regKortnamn = _portalAdminRepository.GetDirectoryShortName(regId);
            return regKortnamn;
        }

        public string HamtaNamnForFilkrav(int filkravId)
        {
            var filkravNamn = _portalAdminRepository.GetFileRequirementName(filkravId);
            return filkravNamn;
        }

        public string HamtaKortnamnForDelregister(int delregId)
        {
            var delRegKortnamn = _portalAdminRepository.GetSubDirectoryShortName(delregId);
            return delRegKortnamn;
        }

        public IEnumerable<AdmForvantadfil> HamtaForvantadeFilerForRegister(int regId)
        {
            var forvantadeFiler = _portalAdminRepository.GetExpectedFilesForDirectory(regId);
            return forvantadeFiler;
        }

        public IEnumerable<AdmRegister> HamtaAllaRegister()
        {
            var registersList = _portalAdminRepository.GetAllRegisters();
            return registersList;
        }

        public IEnumerable<AdmRegister> HamtaAllaRegisterForPortalen()
        {
            var registersList = _portalAdminRepository.GetAllRegistersForPortal();
            return registersList;
        }

        public IEnumerable<AdmDelregister> HamtaAllaDelregisterForPortalen()
        {
            var delregistersList = _portalAdminRepository.GetAllSubDirectoriesForPortal();
            return delregistersList;
        }

        public IEnumerable<Organisation> HamtaAllaOrganisationer()
        {
            var orgList = _portalAdminRepository.GetAllOrganisations();
            return orgList;
        }

        public IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForRegister(int regId)
        {
            var forvLeveranser = _portalAdminRepository.GetExpectedDeliveriesForDirectory(regId);
            return forvLeveranser;
        }

        public IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForDelregister(int delregId)
        {
            var forvLeveranser = _portalAdminRepository.GetExpectedDeliveriesForSubDirectory(delregId);
            return forvLeveranser;
        }

        public IEnumerable<AdmFilkrav> HamtaFilkravForRegister(int regId)
        {
            var filkrav= _portalAdminRepository.GetFileRequirementsForDirectory(regId); 
            return filkrav;
        }

        public AdmFAQKategori HamtaFAQKategori(int faqCatId)
        {
            var faqcat = _portalAdminRepository.GetFAQCategory(faqCatId);
            return faqcat;
        }

        public AdmFAQ HamtaFAQ(int faqId)
        {
            var faq = _portalAdminRepository.GetFAQ(faqId);
            return faq;
        }

        public IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisation(int orgId)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            //TODO - tidsintervall?
            //var leveransIdList = _portalRepository.GetLeveransIdnForOrganisation(orgId).OrderByDescending(x => x);
            var leveransList = _portalAdminRepository.GetLeveranserForOrganisation(orgId);
            foreach (var leverans in leveransList)
            {
                var filloggDetalj = new FilloggDetaljDTO();
                //Kolla om återkopplingsfil finns för aktuell leverans
                var aterkoppling = _portalAdminRepository.GetAterkopplingForLeverans(leverans.Id);

                //Kolla om enhetskod finns för aktuell leverans (stadsdelsleverans)
                var enhetskod = String.Empty;
                if (leverans.OrganisationsenhetsId != null)
                {
                    var orgenhetid = Convert.ToInt32(leverans.OrganisationsenhetsId);
                    enhetskod = _portalAdminRepository.GetEnhetskodForLeverans(orgenhetid);
                }

                //Hämta period för aktuell leverans
                var period = _portalAdminRepository.GetPeriodForAktuellLeverans(leverans.ForvantadleveransId);


                var filer = _portalAdminRepository.GetFilerForLeveransId(leverans.Id).ToList();
                var registerKortnamn = _portalAdminRepository.GetSubDirectoryShortName(leverans.DelregisterId);

                //Vid "Inget att rapportera" finns det leveranser som saknar filer. Se till att även dessa visas i historiken (#101)
                if (!filer.Any())
                {
                    filloggDetalj.LeveransId = leverans.Id;
                    filloggDetalj.Kontaktperson = leverans.ApplicationUserId;
                    filloggDetalj.Leveransstatus = leverans.Leveransstatus;
                    filloggDetalj.Leveranstidpunkt = leverans.Leveranstidpunkt;
                    filloggDetalj.RegisterKortnamn = registerKortnamn;
                    filloggDetalj.Resultatfil = "";
                    filloggDetalj.Filstatus = "";
                    filloggDetalj.Enhetskod = enhetskod;
                    filloggDetalj.Period = period;
                    historikLista.Add(filloggDetalj);
                }
                else
                {
                    foreach (var fil in filer)
                    {
                        filloggDetalj = (FilloggDetaljDTO.FromFillogg(fil));
                        filloggDetalj.Kontaktperson = leverans.ApplicationUserId;
                        filloggDetalj.Leveransstatus = leverans.Leveransstatus;
                        filloggDetalj.Leveranstidpunkt = leverans.Leveranstidpunkt;
                        filloggDetalj.RegisterKortnamn = registerKortnamn;
                        filloggDetalj.Resultatfil = "Ej kontrollerad";
                        filloggDetalj.Enhetskod = enhetskod;
                        filloggDetalj.Period = period;
                        if (aterkoppling != null)
                        {
                            filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus;
                            filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                        }
                        historikLista.Add(filloggDetalj);
                    }
                }
                
            }
            var sorteradHistorikLista = historikLista.OrderByDescending(x => x.Leveranstidpunkt).ToList();


            return sorteradHistorikLista;
        }

        public AdmForeskrift HamtaForeskriftByFilkrav(int filkravId)
        {
            var foreskrift = _portalAdminRepository.GetForeskriftByFileReq(filkravId);
            return foreskrift;
        }

        public IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForDelregOchPeriod(int delRegId, string period)
        {
            var rappResList = _portalAdminRepository.GetReportResultForSubdirAndPeriod(delRegId, period);
            var ejLevList = new List<Rapporteringsresultat>();

            //Ta bara med dem som inte rapporterat alls eller som har leveransstatus "Leveransen är inte godkänd"
            foreach (var rappRes in rappResList)
            {
                if (rappRes.AntalLeveranser == null)
                {
                    ejLevList.Add(rappRes);
                }
                else if (rappRes.Leveransstatus == "Leveransen är inte godkänd")
                {
                    ejLevList.Add(rappRes);
                }
                
            }

            var rappResListDTO = ConvertRappListDBToVM(ejLevList, 0, delRegId);
            return rappResListDTO;
        }

        public IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForRegOchPeriod(int regId, string period)
        {
            var rappResList = _portalAdminRepository.GetReportResultForDirAndPeriod(regId, period);
            var ejLevList = new List<Rapporteringsresultat>();

            //Ta bara med dem som inte rapporterat alls eller som har leveransstatus "Leveransen är inte godkänd"
            foreach (var rappRes in rappResList)
            {
                if (rappRes.AntalLeveranser == null)
                {
                    ejLevList.Add(rappRes);
                }
                else if (rappRes.Leveransstatus == "Leveransen är inte godkänd")
                {
                    ejLevList.Add(rappRes);
                }

            }

            var rappResListDTO = ConvertRappListDBToVM(ejLevList, regId, 0);
            return rappResListDTO;
        }

        public IEnumerable<AdmRegister> HamtaRegisterForOrg(int orgId)
        {
            var uppgskyldighetList = _portalAdminRepository.GetReportObligationInformationForOrg(orgId);
            var delregList = new List<AdmDelregister>();
            var regList = new List<AdmRegister>();
            foreach (var uppgskyldighet in uppgskyldighetList)
            {
                var delreg = _portalAdminRepository.GetSubDirectoryById(uppgskyldighet.DelregisterId);
                delregList.Add(delreg);
            }

            foreach (var delreg in delregList)
            {
                var reg = _portalAdminRepository.GetDirectoryById(delreg.RegisterId);
                if (!regList.Contains(reg))
                        regList.Add(reg);
            }

            return regList;
        }

        public IEnumerable<string> HamtaDelregistersPerioderForAr(int delregId, int ar)
        {
            var perioder = _portalAdminRepository.GetSubDirectoysPeriodsForAYear(delregId, ar);
            return perioder;
        }

        public List<int> HamtaValbaraAr(int delregId)
        {
            var arsLista = new List<int>();
            var uppgiftsstartLista = _portalAdminRepository.GetTaskStartForSubdir(delregId);

            foreach (var uppgiftsstart in uppgiftsstartLista)
            {
                var year = uppgiftsstart.Year;
                if (!arsLista.Contains(year))
                    arsLista.Add(year);
            }

            return arsLista;
        }

        public DateTime HamtaRapporteringsstartForRegisterOchPeriod(int regId, string period)
        {
            var rappStart = _portalAdminRepository.GetReportstartForRegisterAndPeriod(regId, period);
            return rappStart;
        }

        public DateTime HamtaSenasteRapporteringForRegisterOchPeriod(int regId, string period)
        {
            var rappSenast = _portalAdminRepository.GetLatestReportDateForRegisterAndPeriod(regId, period);
            return rappSenast;
        }

        //TODO - special för EKB-År. Lös på annat sätt.
        public DateTime HamtaRapporteringsstartForRegisterOchPeriodSpecial(int regId, string period)
        {
            var rappStart = _portalAdminRepository.GetReportstartForRegisterAndPeriodSpecial(regId, period);
            return rappStart;
        }

        public DateTime HamtaSenasteRapporteringForRegisterOchPeriodSpecial(int regId, string period)
        {
            var rappSenast = _portalAdminRepository.GetLatestReportDateForRegisterAndPeriodSpecial(regId, period);
            return rappSenast;
        }

        public IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisationRegisterPeriod(int orgId, int regId, string periodForReg)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            var sorteradHistorikLista = new List<FilloggDetaljDTO>();
            var delregisterLista = _portalAdminRepository.GetSubdirsForDirectory(regId);
            //var forvLevId = _portalRepository.get

            foreach (var delregister in delregisterLista)
            {
                //Hämta forvantadleveransid för delregister och period
                var forvLevId = _portalAdminRepository.GetExpextedDeliveryIdForSubDirAndPeriod(delregister.Id, periodForReg);

                var senasteLeverans = new Leverans();
                //kan org rapportera per enhet för aktuellt delregister? => hämta senaste leverans per enhet
                var uppgiftsskyldighet = _portalAdminRepository.GetUppgiftsskyldighetForOrganisationAndRegister(orgId, delregister.Id);
                if (uppgiftsskyldighet != null)
                {
                    if (uppgiftsskyldighet.RapporterarPerEnhet)
                    {
                        var orgEnhetsList = _portalAdminRepository.GetOrgUnitsForOrg(orgId);
                        foreach (var orgenhet in orgEnhetsList)
                        {
                            senasteLeverans =
                                _portalAdminRepository.GetLatestDeliveryForOrganisationSubDirectoryPeriodAndOrgUnit(orgId, delregister.Id, forvLevId, orgenhet.Id);
                            if (senasteLeverans != null)
                            {
                                AddHistorikListItem(senasteLeverans, historikLista);
                            }
                        }
                    }
                    else
                    {
                        senasteLeverans =
                            _portalAdminRepository.GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(orgId, delregister.Id,
                                forvLevId);
                        if (senasteLeverans != null)
                        {
                            AddHistorikListItem(senasteLeverans, historikLista);
                        }

                    }
                }
                
            }
            if (historikLista.Count > 0)
            {
                sorteradHistorikLista = historikLista.OrderBy(x => x.Enhetskod).ThenBy(x => x.RegisterKortnamn).ThenByDescending(x => x.Id).ToList();
            }

            return sorteradHistorikLista;
        }

        //TODO - hårdkodat. Lös på annat sätt
        public string HamtaSammanlagdStatusForPeriod(IEnumerable<FilloggDetaljDTO> historikLista)
        {
            string status = String.Empty;
            bool ok = false;
            bool warning = false;
            bool error = false;
            bool ekbMan = false;
            bool ekbAo = false;
            bool sol1 = false;
            bool sol2 = false;



            foreach (var rad in historikLista)
            {
                if (rad.RegisterKortnamn == "EKB-Månad")
                {
                    ekbMan = true;
                }
                else if (rad.RegisterKortnamn == "EKB-AO")
                {
                    ekbAo = true;
                }
                else if (rad.RegisterKortnamn == "SOL1")
                {
                    sol1 = true;
                }
                else if (rad.RegisterKortnamn == "SOL2")
                {
                    sol2 = true;
                }

                if (rad.Leveransstatus.Trim() == "Inget att rapportera" || rad.Leveransstatus == "Leveransen är godkänd")
                {
                    ok = true;
                }
                else if (rad.Leveransstatus == "Leveransen är godkänd med varningar")
                {
                    warning = true;
                }
                else if (rad.Leveransstatus == "Leveransen är inte godkänd" || rad.Leveransstatus == "Levererad")
                {
                    error = true;
                }
            }

            if ((ekbMan && !ekbAo) || (!ekbMan && ekbAo))
                status = "error";
            else if ((sol1 && !sol2) || (!sol1 && sol2))
                status = "error";
            else if (warning && !error)
                status = "warning";
            else if (error)
                status = "error";
            else if (ok && !error && !warning)
                status = "ok";

            return status;
        }

        public IEnumerable<RegisterInfo> HamtaValdaRegistersForAnvandare(string userId, int orgId)
        {
            var registerList = _portalAdminRepository.GetChosenDelRegistersForUser(userId);
            //var allaRegisterList = _portalRepository.GetAllRegisterInformation();
            var allaRegisterList = _portalAdminRepository.GetAllRegisterInformationForOrganisation(orgId);
            var userRegisterList = new List<RegisterInfo>();

            foreach (var register in allaRegisterList)
            {
                foreach (var userRegister in registerList)
                {
                    if (register.Id == userRegister.DelregisterId)
                    {
                        register.SelectedFilkrav = "0";
                        userRegisterList.Add(register);
                    }
                }
            }

            //Check if users organisation reports per unit. If thats the case, get list of units
            foreach (var item in userRegisterList)
            {
                var uppgiftsskyldighet = HamtaUppgiftsskyldighetForOrganisationOchRegister(orgId, item.Id);
                if (uppgiftsskyldighet.RapporterarPerEnhet)
                {
                    item.RapporterarPerEnhet = true;
                    var orgUnits = _portalAdminRepository.GetOrganisationUnits(orgId);
                    item.Organisationsenheter = new List<KeyValuePair<string, string>>();
                    foreach (var orgUnit in orgUnits)
                    {
                        KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Enhetsnamn);
                        item.Organisationsenheter.Add(keyValuePair);
                    }

                }
            }

            return userRegisterList;
        }

        public AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrganisationOchRegister(int orgId, int delregid)
        {
            var uppgiftsskyldighet = _portalAdminRepository.GetUppgiftsskyldighetForOrganisationAndRegister(orgId, delregid);

            return uppgiftsskyldighet;
        }

        public int SkapaOrganisation(Organisation org, string userName)
        {
            //Sätt datum och användare
            org.SkapadDatum = DateTime.Now;
            org.SkapadAv = userName;
            org.AndradDatum = DateTime.Now;
            org.AndradAv = userName;

            var orgId = _portalAdminRepository.CreateOrganisation(org);
            return orgId;
        }

        public void SkapaOrganisationsenhet(Organisationsenhet orgUnit, string userName)
        {
            //Sätt datum och användare
            orgUnit.SkapadDatum = DateTime.Now;
            orgUnit.SkapadAv = userName;
            orgUnit.AndradDatum = DateTime.Now;
            orgUnit.AndradAv = userName;

            _portalAdminRepository.CreateOrgUnit(orgUnit);
        }

        public void SkapaFAQKategori(AdmFAQKategori faqKategori, string userName)
        {
            //Sätt datum och användare
            faqKategori.SkapadDatum = DateTime.Now;
            faqKategori.SkapadAv = userName;
            faqKategori.AndradDatum = DateTime.Now;
            faqKategori.AndradAv = userName;

            _portalAdminRepository.CreateFAQCategory(faqKategori);
        }

        public void SkapaFAQ(AdmFAQ faq, string userName)
        {
            //Sätt datum och användare
            faq.SkapadDatum = DateTime.Now;
            faq.SkapadAv = userName;
            faq.AndradDatum = DateTime.Now;
            faq.AndradAv = userName;

            _portalAdminRepository.CreateFAQ(faq);
        }

        public void SkapaHelgdag(AdmHelgdag helgdag, string userName)
        {
            //Sätt datum och användare
            helgdag.SkapadDatum = DateTime.Now;
            helgdag.SkapadAv = userName;
            helgdag.AndradDatum = DateTime.Now;
            helgdag.AndradAv = userName;

            _portalAdminRepository.CreateHoliday(helgdag);
        }

        public void SkapaSpecialdag(AdmSpecialdag specialdag, string userName)
        {
            //Sätt datum och användare
            specialdag.SkapadDatum = DateTime.Now;
            specialdag.SkapadAv = userName;
            specialdag.AndradDatum = DateTime.Now;
            specialdag.AndradAv = userName;

            _portalAdminRepository.CreateSpecialDay(specialdag);
        }

        public void SkapaInformationsText(AdmInformation infoText, string userName)
        {
            //Sätt datum och användare
            infoText.SkapadDatum = DateTime.Now;
            infoText.SkapadAv = userName;
            infoText.AndradDatum = DateTime.Now;
            infoText.AndradAv = userName;
            _portalAdminRepository.CreateInformationText(infoText);
        }

        public void SkapaUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSk, string userName)
        {
            //Sätt datum och användare
            uppgSk.SkapadDatum = DateTime.Now;
            uppgSk.SkapadAv = userName;
            uppgSk.AndradDatum = DateTime.Now;
            uppgSk.AndradAv = userName;
            _portalAdminRepository.CreateReportObligation(uppgSk);
        }

        public void SkapaEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSk, string userName)
        {
            //Sätt datum och användare
            enhetsUppgSk.SkapadDatum = DateTime.Now;
            enhetsUppgSk.SkapadAv = userName;
            enhetsUppgSk.AndradDatum = DateTime.Now;
            enhetsUppgSk.AndradAv = userName;
            _portalAdminRepository.CreateUnitReportObligation(enhetsUppgSk);
        }

        public void SkapaRegister(AdmRegister reg, string userName)
        {
            //Sätt datum och användare
            reg.SkapadDatum = DateTime.Now;
            reg.SkapadAv = userName;
            reg.AndradDatum = DateTime.Now;
            reg.AndradAv = userName; 
            _portalAdminRepository.CreateDirectory(reg);
        }

        public void SkapaDelregister(AdmDelregister delReg, string userName)
        {
            //Sätt datum och användare
            delReg.SkapadDatum = DateTime.Now;
            delReg.SkapadAv = userName;
            delReg.AndradDatum = DateTime.Now;
            delReg.AndradAv = userName;
            _portalAdminRepository.CreateSubDirectory(delReg);
        }

        public void SkapaForvantadLeverans(AdmForvantadleverans forvLev, string userName)
        {
            //Sätt datum och användare
            forvLev.SkapadDatum = DateTime.Now;
            forvLev.SkapadAv = userName;
            forvLev.AndradDatum = DateTime.Now;
            forvLev.AndradAv = userName;
            _portalAdminRepository.CreateExpectedDelivery(forvLev);
        }

        public void SkapaForvantadLeveranser(IEnumerable<AdmForvantadleverans> forvLevList, string userName)
        {
            foreach (var forvLev in forvLevList)
            {
                //Sätt datum och användare
                forvLev.SkapadDatum = DateTime.Now;
                forvLev.SkapadAv = userName;
                forvLev.AndradDatum = DateTime.Now;
                forvLev.AndradAv = userName;
                _portalAdminRepository.CreateExpectedDelivery(forvLev);
            }
        }

        public IEnumerable<ForvantadLeveransDTO> SkapaForvantadeLeveranserUtkast(int selectedYear, int selectedDelRegId, int selectedFilkravId)
        {
            var forvantadeLeveranser = new List<ForvantadLeveransDTO>();
            var forvantadeLeveranserUtkast = new List<ForvantadLeveransDTO>();
            //Hämta regler från aktuellt AdmFilkrav 
            var filkrav = _portalAdminRepository.GetFileRequirementsForSubDirectoryAndFileReqId(selectedDelRegId, selectedFilkravId);
            //Skapa forväntade leveranser
            switch (filkrav.InsamlingsfrekvensId)
            {
                case 1: //Månadsvis
                    forvantadeLeveranser = CreateMonthlyDeliveries(filkrav, selectedYear).ToList();
                    //Markera rader som redan finns i db
                    forvantadeLeveranserUtkast = MarkAlreadyExisting(forvantadeLeveranser).ToList();
                    break;
                case 2: //Årlig
                    forvantadeLeveranser = CreateAnnualDeliveries(filkrav, selectedYear).ToList();
                    forvantadeLeveranserUtkast = MarkAlreadyExisting(forvantadeLeveranser).ToList();
                    break;
            }

            return forvantadeLeveranserUtkast;
        }


        public void SkapaForvantadFil(AdmForvantadfil forvFil, string userName)
        {
            //Sätt datum och användare
            forvFil.SkapadDatum = DateTime.Now;
            forvFil.SkapadAv = userName;
            forvFil.AndradDatum = DateTime.Now;
            forvFil.AndradAv = userName;
            _portalAdminRepository.CreateExpectedFile(forvFil);
        }

        public void SkapaFilkrav(AdmFilkrav filkrav, string userName)
        {
            //Sätt datum och användare
            filkrav.SkapadDatum = DateTime.Now;
            filkrav.SkapadAv = userName;
            filkrav.AndradDatum = DateTime.Now;
            filkrav.AndradAv = userName;
            _portalAdminRepository.CreateFileRequirement(filkrav);
        }

        public void SkapaInsamlingsfrekvens(AdmInsamlingsfrekvens insamlingsfrekvens, string userName)
        {
            //Sätt datum och användare
            insamlingsfrekvens.SkapadDatum = DateTime.Now;
            insamlingsfrekvens.SkapadAv = userName;
            insamlingsfrekvens.AndradDatum = DateTime.Now;
            insamlingsfrekvens.AndradAv = userName;
            _portalAdminRepository.CreateCollectFrequence(insamlingsfrekvens);
        }

        public void UppdateraOrganisation(Organisation org, string userName)
        {
            //Sätt datum och användare
            org.AndradDatum = DateTime.Now;
            org.AndradAv = userName;
            _portalAdminRepository.UpdateOrganisation(org);
        }

        public void UppdateraKontaktperson(ApplicationUser user, string userName)
        {
            user.AndradDatum = DateTime.Now;
            user.AndradAv = userName;
            //Om man nollställt användarens telefonnummer, säkerställ att phonenumberconfirmed = false 
            if (user.PhoneNumber == null)
            {
                user.PhoneNumberConfirmed = false;
            }
            _portalAdminRepository.UpdateContactPerson(user);
        }

        public void UppdateraAdminAnvandare(AppUserAdmin user, string userName)
        {
            user.AndradDatum = DateTime.Now;
            user.AndradAv = userName;
            _portalAdminRepository.UpdateAdminUser(user);
        }

        public void UppdateraOrganisationsenhet(Organisationsenhet orgUnit, string userName)
        {
            //Sätt datum och användare
            orgUnit.AndradDatum = DateTime.Now;
            orgUnit.AndradAv = userName;
            _portalAdminRepository.UpdateOrgUnit(orgUnit);
        }

        public void UppdateraUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSkyldighet, string userName)
        {
            //Sätt datum och användare
            uppgSkyldighet.AndradDatum = DateTime.Now;
            uppgSkyldighet.AndradAv = userName;
            _portalAdminRepository.UpdateReportObligation(uppgSkyldighet);
        }

        public void UppdateraEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSkyldighet, string userName)
        {
            //Sätt datum och användare
            enhetsUppgSkyldighet.AndradDatum = DateTime.Now;
            enhetsUppgSkyldighet.AndradAv = userName;
            _portalAdminRepository.UpdateUnitReportObligation(enhetsUppgSkyldighet);
        }

        public void UppdateraFAQKategori(AdmFAQKategori faqKategori, string userName)
        {
            faqKategori.AndradDatum = DateTime.Now;
            faqKategori.AndradAv = userName;
            faqKategori.Sortering = Convert.ToInt32(faqKategori.Sortering);
            _portalAdminRepository.UpdateFAQCategory(faqKategori);
        }

        public void UppdateraFAQ(AdmFAQ faq, string userName)
        {
            faq.AndradDatum = DateTime.Now;
            faq.AndradAv = userName;
            _portalAdminRepository.UpdateFAQ(faq);
        }


        public void UppdateraHelgdag(AdmHelgdag holiday, string userName)
        {
            holiday.AndradDatum = DateTime.Now;
            holiday.AndradAv = userName;
            _portalAdminRepository.UpdateHoliday(holiday);
        }

        public void UppdateraSpecialdag(AdmSpecialdag specialday, string userName)
        {
            specialday.AndradDatum = DateTime.Now;
            specialday.AndradAv = userName;
            _portalAdminRepository.UpdateSpecialDay(specialday);
        }


        public void UppdateraInformationstext(AdmInformation infoText, string userName)
        {
            infoText.AndradDatum = DateTime.Now;
            infoText.AndradAv = userName;
            _portalAdminRepository.UpdateInfoText(infoText);
        }

        public void UppdateraRegister(AdmRegister register, string userName)
        {
            register.AndradAv = userName;
            register.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateDirectory(register);
        }

        public void UppdateraDelregister(AdmDelregister delregister, string userName)
        {
            delregister.AndradAv = userName;
            delregister.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateSubDirectory(delregister);
        }

        public void UppdateraForvantadLeverans(AdmForvantadleverans forvLev, string userName)
        {
            forvLev.AndradAv = userName;
            forvLev.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateExpectedDelivery(forvLev);
        }

        public void UppdateraForvantadFil(AdmForvantadfil forvFil, string userName)
        {
            forvFil.AndradAv = userName;
            forvFil.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateExpectedFile(forvFil);
        }

        public void UppdateraFilkrav(AdmFilkrav filkrav, string userName)
        {
            filkrav.AndradAv = userName;
            filkrav.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateFileRequirement(filkrav);
        }

        public void UppdateraInsamlingsfrekvens(AdmInsamlingsfrekvens insamlingsfrekvens, string userName)
        {
            insamlingsfrekvens.AndradAv = userName;
            insamlingsfrekvens.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateCollectFrequency(insamlingsfrekvens);
        }

        public void UppdateraAnvandarInfo(AppUserAdmin user, string userName)
        {
            user.AndradAv = userName;
            user.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateUserInfo(user);
        }

        public void SparaOppettider(OpeningHoursInfoDTO oppetTider, string userName)
        {
            //Dela upp informationen i konf-objekt och spara till databasen
            AdmKonfiguration admKonfClosedAnayway = new AdmKonfiguration();
            admKonfClosedAnayway.AndradAv = userName;
            admKonfClosedAnayway.AndradDatum = DateTime.Now;
            admKonfClosedAnayway.Typ = "ClosedAnyway";

            //Closed anyway
            if (oppetTider.ClosedAnyway)
            { 
                admKonfClosedAnayway.Varde = "True";
            }
            else
            {
                admKonfClosedAnayway.Varde = "False";
            }
            _portalAdminRepository.SaveOpeningHours(admKonfClosedAnayway);

            //Closed days
            string daysJoined = string.Join(",", oppetTider.ClosedDays);
            AdmKonfiguration admKonfClosedDays = new AdmKonfiguration
            {
                Typ = "ClosedDays",
                Varde = daysJoined
            };
            admKonfClosedDays.AndradAv = userName;
            admKonfClosedDays.AndradDatum = DateTime.Now;
            _portalAdminRepository.SaveOpeningHours(admKonfClosedDays);

            //Closed from hour
            AdmKonfiguration admKonfClosedFromHour = new AdmKonfiguration
            {
                Typ = "ClosedFromHour",
                Varde = oppetTider.ClosedFromHour.ToString()
            };
            admKonfClosedFromHour.AndradAv = userName;
            admKonfClosedFromHour.AndradDatum = DateTime.Now;
            _portalAdminRepository.SaveOpeningHours(admKonfClosedFromHour);

            //Closed from minute
            AdmKonfiguration admKonfClosedFromMin = new AdmKonfiguration
            {
                Typ = "ClosedFromMin",
                Varde = oppetTider.ClosedFromMin.ToString()
            };
            admKonfClosedFromMin.AndradAv = userName;
            admKonfClosedFromMin.AndradDatum = DateTime.Now;
            _portalAdminRepository.SaveOpeningHours(admKonfClosedFromMin);

            //Closed to hour
            AdmKonfiguration admKonfClosedToHour = new AdmKonfiguration
            {
                Typ = "ClosedToHour",
                Varde = oppetTider.ClosedToHour.ToString()
            };
            admKonfClosedToHour.AndradAv = userName;
            admKonfClosedToHour.AndradDatum = DateTime.Now;
            _portalAdminRepository.SaveOpeningHours(admKonfClosedToHour);

            //Closed to minute
            AdmKonfiguration admKonfClosedToMin = new AdmKonfiguration
            {
                Typ = "ClosedFromMin",
                Varde = oppetTider.ClosedToMin.ToString()
            };
            admKonfClosedToMin.AndradAv = userName;
            admKonfClosedToMin.AndradDatum = DateTime.Now;
            _portalAdminRepository.SaveOpeningHours(admKonfClosedToMin);

            //Closed informationtext
            var infoPageId = _portalAdminRepository.GetPageInfoTextId("Stangtsida");
            AdmInformation infoTextClosedpage = new AdmInformation
            {
                Id = infoPageId,
                Informationstyp = "Stangtsida",
                Text = oppetTider.InfoTextForClosedPage
            };
            infoTextClosedpage.AndradAv = userName;
            infoTextClosedpage.AndradDatum = DateTime.Now;
            _portalAdminRepository.UpdateInfoText(infoTextClosedpage);
        }
        
        public List<OpeningDay> MarkeraStangdaDagar(List<string> closedDays)
        {
            var daysOfWeek = ExtensionMethods.GetDaysOfWeek();

            foreach (var day in daysOfWeek)
            {
                if (closedDays.Contains(day.NameEnglish))
                {
                    day.Selected = true;
                }
            }
            return daysOfWeek;
        }

        public void TaBortFAQKategori(int faqKategoriId)
        {
            _portalAdminRepository.DeleteFAQCategory(faqKategoriId);
        }



        public void TaBortFAQ(int faqId)
        {
            _portalAdminRepository.DeleteFAQ(faqId);
        }

        public void TaBortHelgdag(int holidayId)
        {
            _portalAdminRepository.DeleteHoliday(holidayId);
        }

        public void TaBortSpecialdag(int specialDayId)
        {
            _portalAdminRepository.DeleteSpecialDay(specialDayId);
        }

        public void TaBortKontaktperson(string contactId)
        {
            _portalAdminRepository.DeleteContact(contactId);
        }

        public void TaBortAdminAnvandare(string userId)
        {
            _portalAdminRepository.DeleteAdminUser(userId);
        }

        private IEnumerable<RapporteringsresultatDTO> ConvertRappListDBToVM(IEnumerable<Rapporteringsresultat> rappResList, int regId, int delRegId)
        {
            var rappListDTO = new List<RapporteringsresultatDTO>();
            var i = 0;
            foreach (var resRad in rappResList)
            {
                i++;

                var rappResRadVM = new RapporteringsresultatDTO
                {
                    Id = i,
                    Lankod = resRad.Lankod,
                    Kommunkod = resRad.Kommunkod,
                    Organisationsnamn = resRad.Organisationsnamn,
                    Register = resRad.Register,
                    Period = resRad.Period,
                    RegisterKortnamn = resRad.RegisterKortnamn,
                    Enhetskod = resRad.Enhetskod,
                    AntalLeveranser = resRad.AntalLeveranser,
                    Leveranstidpunkt = resRad.Leveranstidpunkt,
                    Leveransstatus = resRad.Leveransstatus,
                    Email = resRad.Email,
                    Filnamn = resRad.Filnamn,
                    NyttFilnamn = resRad.NyttFilnamn,
                    Filstatus = resRad.Filstatus,
                    SkyldigFrom = resRad.SkyldigFrom,
                    SkyldigTom = resRad.SkyldigTom,
                    Uppgiftsstart = resRad.Uppgiftsstart,
                    Uppgiftsslut = resRad.Uppgiftsslut,
                    Rapporteringsstart = resRad.Rapporteringsstart,
                    Rapporteringsslut = resRad.Rapporteringsslut,
                    Rapporteringsenast = resRad.Rapporteringsenast,
                    UppgiftsskyldighetId = resRad.UppgiftsskyldighetId,
                    OrganisationsId = resRad.OrganisationsId,
                    DelregisterId = resRad.DelregisterId,
                    ForvantadLeveransId = resRad.ForvantadLeveransId,
                    OrganisationsenhetsId = resRad.OrganisationsenhetsId,
                    LeveransId = resRad.LeveransId,
                    Mail= false
                };

                //If user is inactive, remove emailadress from list
                if (!String.IsNullOrEmpty(rappResRadVM.Email))
                {
                    var user = _portalAdminRepository.GetUserByEmail(rappResRadVM.Email);
                    if (user.AktivTom < DateTime.Now.Date)
                        rappResRadVM.Email = null;
                }
                //If no emailadress/contactperson for delivery, get organisations contactperson for chosen subdir
                if (String.IsNullOrEmpty(rappResRadVM.Email))
                {
                    rappResRadVM.Email = GetEmail(rappResRadVM, regId, delRegId);
                }
                rappListDTO.Add(rappResRadVM);
            }
                
            return rappListDTO;
        }

        private string GetEmail(RapporteringsresultatDTO rappRes, int regId, int delRegId)
        {
            var email = String.Empty;
            var contactList = new List<ApplicationUser>();

            //Get email from Organisations Kontaktperson
            if (delRegId == 0)
            {
                var delregContactList = new List<ApplicationUser>();
                //Hämta delregister för valt register
                var delregisterList = _portalAdminRepository.GetSubDirectoriesForDirectory(regId);
                foreach (var delregister in delregisterList)
                {
                    delregContactList = _portalAdminRepository.GetContactPersonsForOrgAndSubdir(rappRes.OrganisationsId, delregister.Id).ToList();
                    contactList.Union(delregContactList).ToList();
                }
                
            }
            else
            {
                contactList = _portalAdminRepository.GetContactPersonsForOrgAndSubdir(rappRes.OrganisationsId, delRegId).ToList();
            }

            if (contactList.Count > 0)
            {
                for (int i = 0; i < contactList.Count; i++)
                {
                    //Check if user is active
                    if (contactList[i].AktivTom == null)
                    {
                        if (email == String.Empty)
                        {
                            email = contactList[i].Email;
                        }
                        else
                        {
                            email = email + ", " + contactList[i].Email;
                        }
                    }
                    else
                    {
                        if (contactList[i].AktivTom >= DateTime.Now.Date)
                        {
                            if (email == String.Empty)
                            {
                                email = contactList[i].Email;
                            }
                            else
                            {
                                email = email + ", " + contactList[i].Email;
                            }
                        }
                    }
                }
                    
            }
            else
            {
                //Get email from organisation
                var org = _portalAdminRepository.GetOrganisation(rappRes.OrganisationsId);
                if (org != null)
                {
                    email = org.EpostAdress;
                }
            }
            return email;
        }

        public void SkickaPaminnelse(IEnumerable<RapporteringsresultatDTO> rappResList, string userId)
        {
            var emailList = String.Empty;
            //ta fram lista med epostadresser för valda rader
            foreach (var rappRes in rappResList)
            {
                if (rappRes.Mail)
                {
                    var x = rappRes.Email.IndexOf(",");
                    if (rappRes.Email.IndexOf(",", StringComparison.Ordinal) > 0)
                    {
                        //Fler epostadresser finns för raden, splitta
                        var newEmailStr = rappRes.Email.Split(',');
                        foreach (var email in newEmailStr)
                        {
                            emailList = emailList + email + ";";
                        }
                    }
                    else
                    {
                        emailList = emailList + rappRes.Email + ";";
                    }
                }
            }
            //Skapa fil/attachment 'in memory' and send mail
            using (var stream = GenerateStreamFromString(emailList))
            {
                // ... Do stuff to stream
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    var emailAttachment = new Attachment(ms, new ContentType("text/bzk"));
                    emailAttachment.Name = "epostadresser" + ".txt";

                    //Send mail
                    MailMessage msg = new MailMessage();
                    msg.IsBodyHtml = true;
                    msg.From = new MailAddress(ConfigurationManager.AppSettings["MailSender"]);
                    //TODO
                    //msg.To.Add(new MailAddress("marie.ahlin@socialstyrelsen.se"));
                    msg.To.Add(new MailAddress(_portalAdminRepository.GetUserEmail(userId)));
                    msg.Subject = "Påminnelse";
                    msg.Body = "'Påminnelsetext här'.<br /> Detta mail innehåller en bifogad fil med epostadresser till användare/organisationer som inte levererat godkända filer för valt register och period.<br />Med vänliga hälsningar Astrid";

                    if (emailAttachment != null)
                    {
                        ContentDisposition disposition = emailAttachment.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(emailAttachment.Name);
                        disposition.ModificationDate = File.GetLastWriteTime(emailAttachment.Name);
                        disposition.ReadDate = File.GetLastAccessTime(emailAttachment.Name);
                        disposition.FileName = Path.GetFileName(emailAttachment.Name);
                        //disposition.Size = new FileInfo(emailAttachment.Name).Length;
                        disposition.DispositionType = DispositionTypeNames.Attachment;
                        msg.Attachments.Add(emailAttachment);
                    }
                    using (SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]))
                    {
                        if (ConfigurationManager.AppSettings["EnableSsl"] == "True")
                        {
                            smtpClient.EnableSsl = true;
                        }
                        smtpClient.Send(msg);
                    }
                }
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            //var x = stream.Length;
            //string jsonString = Encoding.ASCII.GetString(stream.ToArray());
            return stream;
        }

        private IEnumerable<ForvantadLeveransDTO> CreateMonthlyDeliveries(AdmFilkrav filkrav, int selectedYear)
        {
            var forvantadeLeveranserUtkast = new List<ForvantadLeveransDTO>();

            for (int i = 1; i < 13; i++)
            {
                var newDate = new DateTime(selectedYear, i, 1);
                var forvLev = new ForvantadLeveransDTO
                {
                    DelregisterId = filkrav.DelregisterId,
                    FilkravId = filkrav.Id,
                    Period = newDate.ToString("yyyyMM")
                };

                forvLev = SetDeliveryDates(forvLev,filkrav, newDate);

                //Uppgiftsstart/uppgiftsslut
                var uppgDate = newDate.AddMonths(- (Convert.ToInt32(filkrav.UppgifterAntalmanader) -1));
                forvLev.Uppgiftsstart = new DateTime(uppgDate.Year, uppgDate.Month, Convert.ToInt32(filkrav.Uppgiftsstartdag));
                var uppgslutDate = forvLev.Uppgiftsstart.AddMonths(Convert.ToInt32(filkrav.UppgifterAntalmanader));
                if (filkrav.Uppgiftslutdag == 31)
                {
                    forvLev.Uppgiftsslut = uppgslutDate.AddDays(-1);
                }
                else
                {
                    forvLev.Uppgiftsslut = new DateTime(uppgslutDate.Year, uppgslutDate.Month, Convert.ToInt32(filkrav.Uppgiftslutdag));
                }

                //Rapporteringsstart/rapporteringsslut/rapporteringsenast
                var rappDate = newDate.AddMonths(Convert.ToInt32(filkrav.RapporteringEfterAntalManader));
                forvLev.Rapporteringsstart = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Rapporteringsstartdag));
                if (filkrav.Rapporteringsslutdag == 31)
                {
                    forvLev.Rapporteringsslut = forvLev.Rapporteringsstart.AddMonths(1).AddDays(-1);
                }
                else
                {
                    forvLev.Rapporteringsslut = new DateTime(rappDate.Year, rappDate.Month + Convert.ToInt32(filkrav.RapporteringEfterAntalManader), Convert.ToInt32(filkrav.Rapporteringsslutdag));
                }
                if (filkrav.RapporteringSenastdag == 31)
                {
                    forvLev.Rapporteringsenast = forvLev.Rapporteringsstart.AddMonths(1).AddDays(-1);
                }
                else
                {
                    //forvLev.Rapporteringsenast = new DateTime(rappDate.Year, rappDate.Month + Convert.ToInt32(filkrav.RapporteringEfterAntalManader), Convert.ToInt32(filkrav.RapporteringSenastdag));
                    forvLev.Rapporteringsenast = new DateTime(forvLev.Rapporteringsslut.Year, forvLev.Rapporteringsslut.Month, Convert.ToInt32(filkrav.RapporteringSenastdag));

                }

                //Påminnelse1,-2, -3
                if (filkrav.Paminnelse1dag != null)
                {
                    var p1Date = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Paminnelse1dag));
                    //Kontrollera att datum ej infaller på helgen
                    if (p1Date.DayOfWeek == DayOfWeek.Saturday)
                        forvLev.Paminnelse1 = p1Date.AddDays(2);
                    else if (p1Date.DayOfWeek == DayOfWeek.Sunday)
                        forvLev.Paminnelse1 = p1Date.AddDays(1);
                    else
                        forvLev.Paminnelse1 = p1Date;
                }

                if (filkrav.Paminnelse2dag != null)
                {
                    var p2Date = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Paminnelse2dag));
                    //Kontrollera att datum ej infaller på helgen
                    if (p2Date.DayOfWeek == DayOfWeek.Saturday)
                        forvLev.Paminnelse2 = p2Date.AddDays(2);
                    else if (p2Date.DayOfWeek == DayOfWeek.Sunday)
                        forvLev.Paminnelse2 = p2Date.AddDays(1);
                    else
                        forvLev.Paminnelse2 = p2Date;
                }
                if (filkrav.Paminnelse3dag != null)
                {
                    var p3Date = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Paminnelse3dag));
                    //Kontrollera att datum ej infaller på helgen
                    if (p3Date.DayOfWeek == DayOfWeek.Saturday)
                        forvLev.Paminnelse3 = p3Date.AddDays(2);
                    else if (p3Date.DayOfWeek == DayOfWeek.Sunday)
                        forvLev.Paminnelse3 = p3Date.AddDays(1);
                    else
                        forvLev.Paminnelse3 = p3Date;
                }
                forvantadeLeveranserUtkast.Add(forvLev);
            }

            return forvantadeLeveranserUtkast;
        }

        private IEnumerable<ForvantadLeveransDTO> CreateAnnualDeliveries(AdmFilkrav filkrav, int selectedYear)
        {
            var forvantadeLeveranserUtkast = new List<ForvantadLeveransDTO>();
            var newDate = new DateTime(selectedYear,1,1);
            var forvLev = new ForvantadLeveransDTO
            {
                DelregisterId = filkrav.DelregisterId,
                FilkravId = filkrav.Id,
                Period = newDate.ToString("yyyy")
            };
            forvLev = SetDeliveryDates(forvLev, filkrav, newDate);
            forvantadeLeveranserUtkast.Add(forvLev);
            return forvantadeLeveranserUtkast;
        }

        private ForvantadLeveransDTO SetDeliveryDates(ForvantadLeveransDTO forvLev, AdmFilkrav filkrav, DateTime newDate)
        {
            //Uppgiftsstart/uppgiftsslut
            var uppgDate = newDate.AddMonths(-(Convert.ToInt32(filkrav.UppgifterAntalmanader) - 1));
            forvLev.Uppgiftsstart = new DateTime(uppgDate.Year, uppgDate.Month, Convert.ToInt32(filkrav.Uppgiftsstartdag));
            var uppgslutDate = forvLev.Uppgiftsstart.AddMonths(Convert.ToInt32(filkrav.UppgifterAntalmanader));
            if (filkrav.Uppgiftslutdag == 31)
            {
                forvLev.Uppgiftsslut = uppgslutDate.AddDays(-1);
            }
            else
            {
                forvLev.Uppgiftsslut = new DateTime(uppgslutDate.Year, uppgslutDate.Month, Convert.ToInt32(filkrav.Uppgiftslutdag));
            }

            //Rapporteringsstart/rapporteringsslut/rapporteringsenast
            var rappDate = newDate.AddMonths(Convert.ToInt32(filkrav.RapporteringEfterAntalManader));
            forvLev.Rapporteringsstart = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Rapporteringsstartdag));
            if (filkrav.Rapporteringsslutdag == 31)
            {
                forvLev.Rapporteringsslut = forvLev.Rapporteringsstart.AddMonths(1).AddDays(-1);
            }
            else
            {
                forvLev.Rapporteringsslut = new DateTime(rappDate.Year, rappDate.Month + Convert.ToInt32(filkrav.RapporteringEfterAntalManader), Convert.ToInt32(filkrav.Rapporteringsslutdag));
            }
            if (filkrav.RapporteringSenastdag == 31)
            {
                forvLev.Rapporteringsenast = forvLev.Rapporteringsstart.AddMonths(1).AddDays(-1);
            }
            else
            {
                //forvLev.Rapporteringsenast = new DateTime(rappDate.Year, rappDate.Month + Convert.ToInt32(filkrav.RapporteringEfterAntalManader), Convert.ToInt32(filkrav.RapporteringSenastdag));
                forvLev.Rapporteringsenast = new DateTime(forvLev.Rapporteringsslut.Year, forvLev.Rapporteringsslut.Month, Convert.ToInt32(filkrav.RapporteringSenastdag));

            }

            //Påminnelse1,-2, -3
            if (filkrav.Paminnelse1dag != null)
            {
                var p1Date = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Paminnelse1dag));
                //Kontrollera att datum ej infaller på helgen
                if (p1Date.DayOfWeek == DayOfWeek.Saturday)
                    forvLev.Paminnelse1 = p1Date.AddDays(2);
                else if (p1Date.DayOfWeek == DayOfWeek.Sunday)
                    forvLev.Paminnelse1 = p1Date.AddDays(1);
                else
                    forvLev.Paminnelse1 = p1Date;
            }

            if (filkrav.Paminnelse2dag != null)
            {
                var p2Date = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Paminnelse2dag));
                //Kontrollera att datum ej infaller på helgen
                if (p2Date.DayOfWeek == DayOfWeek.Saturday)
                    forvLev.Paminnelse2 = p2Date.AddDays(2);
                else if (p2Date.DayOfWeek == DayOfWeek.Sunday)
                    forvLev.Paminnelse2 = p2Date.AddDays(1);
                else
                    forvLev.Paminnelse2 = p2Date;
            }
            if (filkrav.Paminnelse3dag != null)
            {
                var p3Date = new DateTime(rappDate.Year, rappDate.Month, Convert.ToInt32(filkrav.Paminnelse3dag));
                //Kontrollera att datum ej infaller på helgen
                if (p3Date.DayOfWeek == DayOfWeek.Saturday)
                    forvLev.Paminnelse3 = p3Date.AddDays(2);
                else if (p3Date.DayOfWeek == DayOfWeek.Sunday)
                    forvLev.Paminnelse3 = p3Date.AddDays(1);
                else
                    forvLev.Paminnelse3 = p3Date;
            }

            return forvLev;

        }

        private IEnumerable<ForvantadLeveransDTO> MarkAlreadyExisting(IEnumerable<ForvantadLeveransDTO> forvLevList)
        {
            var checkedForvLevList = new List<ForvantadLeveransDTO>();

            foreach (var forvLev in forvLevList)
            {
                var dbForvLev =
                    _portalAdminRepository.GetExpectedDeliveryBySubDirAndFileReqIdAndPeriod(forvLev.DelregisterId, forvLev.FilkravId, forvLev.Period);
                if (dbForvLev != null)
                {
                    var forvLevDTO = new ForvantadLeveransDTO()
                    {
                        Id = dbForvLev.Id,
                        FilkravId = dbForvLev.FilkravId,
                        DelregisterId = dbForvLev.DelregisterId,
                        Period = dbForvLev.Period,
                        Uppgiftsstart = dbForvLev.Uppgiftsstart,
                        Uppgiftsslut = dbForvLev.Uppgiftsslut,
                        Rapporteringsstart = dbForvLev.Rapporteringsstart,
                        Rapporteringsslut = dbForvLev.Rapporteringsslut,
                        Rapporteringsenast = dbForvLev.Rapporteringsenast,
                        Paminnelse1 = dbForvLev.Paminnelse1,
                        Paminnelse2 = dbForvLev.Paminnelse2,
                        Paminnelse3 = dbForvLev.Paminnelse3,
                        SkapadDatum = dbForvLev.SkapadDatum,
                        SkapadAv = dbForvLev.SkapadAv,
                        AndradDatum = dbForvLev.AndradDatum,
                        AndradAv = dbForvLev.AndradAv,
                        AlreadyExists = true
                    };
                    checkedForvLevList.Add(forvLevDTO);
                }
                else
                {
                    forvLev.AlreadyExists = false;
                    checkedForvLevList.Add(forvLev);
                }
            }
            return checkedForvLevList;
        }

        private List<FilloggDetaljDTO> AddHistorikListItem(Leverans senasteLeverans, List<FilloggDetaljDTO> historikLista)
        {
            var filloggDetalj = new FilloggDetaljDTO();
            //Kolla om återkopplingsfil finns för aktuell leverans
            var aterkoppling = _portalAdminRepository.GetAterkopplingForLeverans(senasteLeverans.Id);
            //Kolla om enhetskod finns för aktuell leverans (stadsdelsleverans)
            var enhetskod = String.Empty;

            if (senasteLeverans.OrganisationsenhetsId != null)
            {
                var orgenhetid = Convert.ToInt32(senasteLeverans.OrganisationsenhetsId);
                enhetskod = _portalAdminRepository.GetEnhetskodForLeverans(orgenhetid);
            }

            //Hämta period för aktuell leverans
            var period = _portalAdminRepository.GetPeriodForAktuellLeverans(senasteLeverans.ForvantadleveransId);

            var filer = _portalAdminRepository.GetFilerForLeveransId(senasteLeverans.Id).ToList();
            var registerKortnamn = _portalAdminRepository.GetSubDirectoryShortName(senasteLeverans.DelregisterId);

            if (!filer.Any())
            {
                filloggDetalj = new FilloggDetaljDTO();
                filloggDetalj.Id = 0;
                filloggDetalj.LeveransId = senasteLeverans.Id;
                filloggDetalj.Filnamn = " - ";
                filloggDetalj.Filstatus = " - ";
                filloggDetalj.Kontaktperson = senasteLeverans.ApplicationUserId;
                filloggDetalj.Leveransstatus = senasteLeverans.Leveransstatus;
                filloggDetalj.Leveranstidpunkt = senasteLeverans.Leveranstidpunkt;
                filloggDetalj.RegisterKortnamn = registerKortnamn;
                filloggDetalj.Resultatfil = " - ";
                filloggDetalj.Enhetskod = enhetskod;
                filloggDetalj.Period = period;
                if (aterkoppling != null)
                {
                    filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus;
                    filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                }
                historikLista.Add(filloggDetalj);
            }
            else
            {
                foreach (var fil in filer)
                {
                    filloggDetalj = (FilloggDetaljDTO.FromFillogg(fil));
                    filloggDetalj.Kontaktperson = senasteLeverans.ApplicationUserId;
                    filloggDetalj.Leveransstatus = senasteLeverans.Leveransstatus;
                    filloggDetalj.Leveranstidpunkt = senasteLeverans.Leveranstidpunkt;
                    filloggDetalj.RegisterKortnamn = registerKortnamn;
                    filloggDetalj.Resultatfil = "Ej kontrollerad";
                    filloggDetalj.Enhetskod = enhetskod;
                    filloggDetalj.Period = period;
                    if (aterkoppling != null)
                    {
                        //filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus; //Skriv ej över leveransstatusen från återkopplingen. Beslut 20180912, ärende #128
                        filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                    }
                    historikLista.Add(filloggDetalj);
                }
            }
            return historikLista;
        }

    }
}

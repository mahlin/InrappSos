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
using System.Diagnostics;
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
    public class PortalSosService : IPortalSosService
    {

        private readonly IPortalSosRepository _portalSosRepository;


        System.Globalization.CultureInfo _culture = new System.Globalization.CultureInfo("sv-SE");


        public PortalSosService(IPortalSosRepository portalSosRepository)
        {
            _portalSosRepository = portalSosRepository;

        }



        public void AktiveraKontaktperson(string userId)
        {
            _portalSosRepository.EnableContact(userId);
        }
        public string ClosedComingWeek()
        {
            string avvikandeOppettider = String.Empty;
            avvikandeOppettider = SpecialdagComingWeek();
            avvikandeOppettider = avvikandeOppettider + HelgdagComingWeek();
            return avvikandeOppettider;
        }

        public string HelgdagComingWeek()
        {
            var helgdagarInomEnVecka = String.Empty;
            var date = DateTime.Now.Date;
            var dateNowPlusOneWeek = date.AddDays(7);
            var helgdagar = _portalSosRepository.GetHolidays();

            foreach (var helgdag in helgdagar)
            {
                if (helgdag.Helgdatum >= date && helgdag.Helgdatum <= dateNowPlusOneWeek)
                {
                    var veckodag = helgdag.Helgdatum.ToString("dddd", _culture);
                    veckodag = char.ToUpper(veckodag[0]) + veckodag.Substring(1);
                    var dagNr = helgdag.Helgdatum.Day;
                    var manad = helgdag.Helgdatum.ToString("MMMM", _culture);
                    var dagStr = veckodag + " " + dagNr + " " + manad + " stängt " + helgdag.Helgdag + "<br>";
                    helgdagarInomEnVecka = helgdagarInomEnVecka + dagStr;
                }
            }

            return helgdagarInomEnVecka;
        }

        public string SpecialdagComingWeek()
        {
            var specialdagarInomEnVecka = String.Empty;
            var now = DateTime.Now;
            var dateNow = DateTime.Now.Date;
            var dateNowPlusOneWeek = dateNow.AddDays(7);

            var specialdagar = _portalSosRepository.GetSpecialDays();

            foreach (var dag in specialdagar)
            {
                if (dag.Specialdagdatum >= dateNow && dag.Specialdagdatum <= dateNowPlusOneWeek)
                {
                    //string FormatDate = dag.Specialdagdatum.ToString("dddd dd MMMM yyyy", culture);
                    var veckodag = dag.Specialdagdatum.ToString("dddd", _culture);
                    veckodag = char.ToUpper(veckodag[0]) + veckodag.Substring(1);
                    var dagNr = dag.Specialdagdatum.Day;
                    var manad = dag.Specialdagdatum.ToString("MMMM", _culture);
                    var klockslagFrom = dag.Oppna.ToString(@"hh\:mm");
                    var klockslagTom = dag.Stang.ToString(@"hh\:mm");
                    var dagStr = veckodag + " " + dagNr + " " + manad + " " + klockslagFrom + "-" + klockslagTom + " " + dag.Anledning + "<br>";

                    specialdagarInomEnVecka = specialdagarInomEnVecka + dagStr;
                }
            }
            return specialdagarInomEnVecka;
        }

        
        public Organisation HamtaOrganisation(int orgId)
        {
            var org = _portalSosRepository.GetOrganisation(orgId);
            return org;
        }

        public string HamtaAnvandaresNamn(string userId)
        {
            var userName = _portalSosRepository.GetUserName(userId);
            return userName;
        }

        public ApplicationUser HamtaAnvandareMedEpost(string epost)
        {
            var user = _portalSosRepository.GetUserByEmail(epost);
            return user;
        }

        public string HamtaAnvandaresKontaktnummer(string userId)
        {
            var contactNumber = _portalSosRepository.GetUserContactNumber(userId);
            return contactNumber;
        }

        public string HamtaAnvandaresMobilnummer(string userId)
        {
            var phoneNumber = _portalSosRepository.GetUserPhoneNumber(userId);
            return phoneNumber;
        }

        public Arende HamtaArende(string arendeNr)
        {
            var arende = _portalSosRepository.GetArende(arendeNr);
            return arende;
        }

        public Arende HamtaArendeById(int arendeId)
        {
            var arende = _portalSosRepository.GetArendeById(arendeId);
            return arende;
        }

        public int HamtaUserOrganisationId(string userId)
        {
            var orgId = _portalSosRepository.GetUserOrganisationId(userId);
            return orgId;
        }

        public string HamtaKommunkodForOrg(int orgId)
        {
            var kommunkod = _portalSosRepository.GetKommunkodForOrganisation(orgId);
            return kommunkod;
        }

        public string HamtaKommunKodForAnvandare(string userId)
        {
            var orgId = _portalSosRepository.GetUserOrganisationId(userId);
            var kommunKod = _portalSosRepository.GetKommunkodForOrganisation(orgId);
            return kommunKod;
        }

        public List<OrganisationstypDTO> HamtaOrgtyperForOrganisation(int orgId, List<AdmOrganisationstyp> orgtyperList)
        {
            var chosenOrgTypeIdsForOrgList = _portalSosRepository.GetOrgTypesIdsForOrg(orgId);
            var orgTypeList = new List<OrganisationstypDTO>();
            foreach (var orgtyp in orgtyperList)
            {
                var orgtypDTO = new OrganisationstypDTO()
                {
                    Organisationstypid = orgtyp.Id,
                    Typnamn = orgtyp.Typnamn,
                    Beskrivning = orgtyp.Beskrivning,
                };
                if (chosenOrgTypeIdsForOrgList.Contains(orgtyp.Id))
                {
                    orgtypDTO.Selected = true;
                }
                orgTypeList.Add(orgtypDTO);
            }
            return orgTypeList;
        }

        //public List<UserRolesDTO> HamtaAstridRoller()
        //{
        //    var roles = _portalSosRepository.GetAllAstridRoles();
        //    return roles;
        //}

        public IEnumerable<ApplicationUser> HamtaKontaktpersonerForOrg(int orgId)
        {
            var contacts = _portalSosRepository.GetContactPersonsForOrg(orgId);
            return contacts;
        }

        public IEnumerable<UndantagEpostDoman> HamtaPrivataEpostadresserForOrg(int orgId)
        {
            var privEmails = _portalSosRepository.GetPrivateEmailAdressesForOrg(orgId);
            return privEmails;
        }

        public IEnumerable<Arende> HamtaArendenForOrg(int orgId)
        {
            var cases = _portalSosRepository.GetCasesForOrg(orgId);
            return cases;
        }

        public IEnumerable<AppUserAdmin> HamtaAdminUsers()
        {
            var adminUsers = _portalSosRepository.GetAdminUsers();
            return adminUsers;
        }

        public Organisation HamtaOrganisationForKommunkod(string kommunkod)
        {
            var org = _portalSosRepository.GetOrganisationFromKommunkod(kommunkod);
            return org;
        }

        public Organisation HamtaOrgForEmailDomain(string modelEmail)
        {
            MailAddress address = new MailAddress(modelEmail);
            string host = address.Host;
            //Check if Host contains multiple parts, get the last one
            string domain = GetLastPartOfHostAdress(host);

            var organisation = _portalSosRepository.GetOrgForEmailDomain(domain);
            return organisation;
        }

        public IEnumerable<Organisationsenhet> HamtaOrgEnheterForOrg(int orgId)
        {
            var orgEnheter = _portalSosRepository.GetOrgUnitsForOrg(orgId);
            return orgEnheter;
        }
        public Organisationsenhet HamtaOrganisationsenhetMedEnhetskod(string kod, int orgId)
        {
            var orgenhet = _portalSosRepository.GetOrganisationUnitByCode(kod, orgId);
            return orgenhet;
        }

        public IEnumerable<AdmUppgiftsskyldighet> HamtaUppgiftsskyldighetForOrg(int orgId)
        {
            var uppgiftsskyldigheter = _portalSosRepository.GetReportObligationInformationForOrg(orgId);
            return uppgiftsskyldigheter;
        }


        public AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrgOchDelreg(int orgId, int delregId)
        {
            var uppgiftsskyldighet = _portalSosRepository.GetReportObligationInformationForOrgAndSubDir(orgId, delregId);
            return uppgiftsskyldighet;
        }

        public IEnumerable<AdmEnhetsUppgiftsskyldighet> HamtaEnhetsUppgiftsskyldighetForOrgEnhet(int orgenhetId)
        {
            var uppgiftsskyldigheter = _portalSosRepository.GetUnitReportObligationInformationForOrgUnit(orgenhetId);
            return uppgiftsskyldigheter;
        }

        public AdmEnhetsUppgiftsskyldighet HamtaEnhetsUppgiftsskyldighetForUppgiftsskyldighetOchOrgEnhet(int uppgskhId, int orgenhetId)
        {
            var enhetsuppgiftsskyldighet = _portalSosRepository.GetUnitReportObligationForReportObligationAndOrg(uppgskhId, orgenhetId);
            return enhetsuppgiftsskyldighet;
        }
        public Organisation HamtaOrgForAnvandare(string userId)
        {
            var org = _portalSosRepository.GetOrgForUser(userId);
            return org;
        }

        public Organisation HamtaOrgForOrganisationsenhet(int orgUnitId)
        {
            var org = _portalSosRepository.GetOrgForOrgUnit(orgUnitId);
            return org;
        }

        public Organisation HamtaOrgForUppgiftsskyldighet(int uppgSkId)
        {
            var org = _portalSosRepository.GetOrgForReportObligation(uppgSkId);
            return org;
        }

        public IdentityRole HamtaAstridRoll(string roleName)
        {
            var astridRoll = _portalSosRepository.GetAstridRole(roleName);
            return astridRoll;
        }

        public IdentityRole HamtaFilipRoll(string roleName)
        {
            var filipRoll = _portalSosRepository.GetFilipRole(roleName);
            return filipRoll;
        }

        public IEnumerable<AdmFAQKategori> HamtaAllaFAQs()
        {
            var faqs = _portalSosRepository.GetAllFAQs();
            return faqs;
        }

        public IEnumerable<AdmOrganisationstyp> HamtaAllaOrganisationstyper()
        {
            var orgtyper = _portalSosRepository.GetAllOrgTypes();
            return orgtyper;
        }

        public IEnumerable<AdmFAQKategori> HamtaFAQkategorier()
        {
            var faqCats = _portalSosRepository.GetFAQCategories();
            return faqCats;
        }

        public IEnumerable<AdmFAQ> HamtaFAQs(int faqCatId)
        {
            var faqs = _portalSosRepository.GetFAQs(faqCatId);
            return faqs;
        }

        public IEnumerable<AdmHelgdag> HamtaAllaHelgdagar()
        {
            var helgdagar = _portalSosRepository.GetAllHolidays();
            return helgdagar;
        }

        public IEnumerable<AdmSpecialdag> HamtaAllaSpecialdagar()
        {
            var specialdagar = _portalSosRepository.GetAllSpecialDays();
            return specialdagar;
        }

        public IEnumerable<AdmInformation> HamtaInformationstexter()
        {
            var infoTexts = _portalSosRepository.GetInformationTexts();
            return infoTexts;
        }

        public AdmInsamlingsfrekvens HamtaInsamlingsfrekvens(int insamlingsid)
        {
            var insamlingsfrekvens = _portalSosRepository.GetInsamlingsfrekvens(insamlingsid);
            return insamlingsfrekvens;
        }

        public string HamtaHelgEllerSpecialdagsInfo()
        {
            var text = String.Empty;
            if (IsHelgdag())
            {
                text = HamtaHelgdagsInfo();
            }
            else if (IsSpecialdag())
            {
                text = HamtaSpecialdagsInfo();
            }

            return text;
        }
        
        public OpeningHoursInfoDTO HamtaOppettider()
        {
            var configInfo = _portalSosRepository.GetAdmConfiguration();
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
            var info = _portalSosRepository.GetInfoText(infoTyp);
            return info;
        }

        public string HamtaInformationsText(string infoTyp)
        {
            var infoText = _portalSosRepository.GetInfoText(infoTyp).Text;
            return infoText;
        }

        public AdmInformation HamtaInfo(int infoId)
        {
            var info = _portalSosRepository.GetInfoText(infoId);
            return info;
        }

        public IEnumerable<AdmRegister> HamtaRegister()
        {
            var registerList = _portalSosRepository.GetDirectories();
            return registerList;
        }

        public AdmRegister HamtaRegisterMedKortnamn(string regKortNamn)
        {
            var register = _portalSosRepository.GetDirectoryByShortName(regKortNamn);
            return register;
        }

        public AdmRegister HamtaRegisterMedId(int regId)
        {
            var register = _portalSosRepository.GetDirectoryById(regId);
            return register;
        }

        public IEnumerable<AdmDelregister> HamtaDelRegister()
        {
            var subDirectories = _portalSosRepository.GetSubDirectories();
            return subDirectories;
        }

        public IEnumerable<AdmDelregister> HamtaDelRegisterForRegister(int regId)
        {
            var subDirectories = _portalSosRepository.GetSubDirectoriesForDirectory(regId);
            return subDirectories;
        }

        public AdmDelregister HamtaDelRegisterForUppgiftsskyldighet(int uppgSkId)
        {
            var reportObligation = _portalSosRepository.GetReportObligationById(uppgSkId);
            var subdir = _portalSosRepository.GetSubDirectoryById(reportObligation.DelregisterId);
            return subdir;
        }


        public AdmDelregister HamtaDelRegisterForKortnamn(string shortName)
        {
            var subDirectory = _portalSosRepository.GetSubDirectoryByShortName(shortName);
            return subDirectory;
        }

        public IEnumerable<RegisterInfo> HamtaDelregisterOchFilkrav()
        {
            var delregMedFilkravList = new List<RegisterInfo>();

            var delregList = _portalSosRepository.GetAllSubDirectoriesForPortal();
            foreach (var delreg in delregList)
            {
                var regFilkravList = new List<RegisterFilkrav>();
                var regInfo = new RegisterInfo
                {
                    Id = delreg.Id,
                    Kortnamn = delreg.Kortnamn
                };

                //Hämta varje delregisters filkrav
                var filkravList = _portalSosRepository.GetFileRequirementsForSubDirectory(delreg.Id).ToList();
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

        public IEnumerable<RegisterInfo> HamtaDelregisterOchAktuellaFilkrav()
        {
            var delregMedFilkravList = new List<RegisterInfo>();

            var delregList = _portalSosRepository.GetAllSubDirectoriesForPortal();
            foreach (var delreg in delregList)
            {
                var regFilkravList = new List<RegisterFilkrav>();
                var regInfo = new RegisterInfo
                {
                    Id = delreg.Id,
                    Kortnamn = delreg.Kortnamn
                };

                //Hämta varje delregisters filkrav
                var filkravList = _portalSosRepository.GetFileRequirementsForSubDirectory(delreg.Id).ToList();
                foreach (var filkrav in filkravList)
                {
                    //Kolla om filkravet har en aktuell foreskrift
                    var currDate = DateTime.Now;
                    var foreskrift = _portalSosRepository.GetForeskriftByFileReq(filkrav.Id);
                    if (foreskrift != null)
                    {
                        if (foreskrift.GiltigTom == null || foreskrift.GiltigTom > currDate)
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
                    }
                    
                }

                regInfo.Filkrav = regFilkravList;
                delregMedFilkravList.Add(regInfo);
            }
            return delregMedFilkravList;
        }


        public IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranser()
        {
            var forvlevList = _portalSosRepository.GetExpectedDeliveries();
            return forvlevList;
        }

        public IEnumerable<AdmForvantadfil> HamtaAllaForvantadeFiler()
        {
            var forvFilList = _portalSosRepository.GetAllExpectedFiles();
            return forvFilList;
        }

        public IEnumerable<AdmFilkrav> HamtaAllaFilkrav()
        {
            var filkravList = _portalSosRepository.GetAllFileRequirements();
            return filkravList;
        }

        public IEnumerable<AdmInsamlingsfrekvens> HamtaAllaInsamlingsfrekvenser()
        {
            var insamlingsfrekvensList = _portalSosRepository.GetAllCollectionFrequencies();
            return insamlingsfrekvensList;
        }

        public IEnumerable<Arendetyp> HamtaAllaArendetyper()
        {
            var arendetypList = _portalSosRepository.GetAllCaseTypes();
            return arendetypList;
        }

        public IEnumerable<ArendeStatus> HamtaAllaArendestatusar()
        {
            var arendestatusList = _portalSosRepository.GetAllCaseStatuses();
            return arendestatusList;
        }

        public string HamtaKortnamnForDelregisterMedFilkravsId(int filkravId)
        {
            var delRegKortnamn = _portalSosRepository.GetSubDirectoryShortNameForExpectedFile(filkravId);
            return delRegKortnamn;
        }

        public string HamtaKortnamnForRegister(int regId)
        {
            var regKortnamn = _portalSosRepository.GetDirectoryShortName(regId);
            return regKortnamn;
        }

        public string HamtaNamnForFilkrav(int filkravId)
        {
            var filkravNamn = _portalSosRepository.GetFileRequirementName(filkravId);
            return filkravNamn;
        }
        public int HamtaNyttLeveransId(string userId, string userName, int orgId, int registerId, int orgenhetsId, int forvLevId, string status)
        {
            var levId = _portalSosRepository.GetNewLeveransId(userId, userName, orgId, registerId, orgenhetsId, forvLevId, status);
            return levId;
        }

        public string HamtaKortnamnForDelregister(int delregId)
        {
            var delRegKortnamn = _portalSosRepository.GetSubDirectoryShortName(delregId);
            return delRegKortnamn;
        }

        public IEnumerable<AdmForvantadfil> HamtaForvantadeFilerForRegister(int regId)
        {
            var forvantadeFiler = _portalSosRepository.GetExpectedFilesForDirectory(regId);
            return forvantadeFiler;
        }

        public IEnumerable<AdmForvantadfil> HamtaForvantadFil(int filkravId)
        {
            var forvFiler = _portalSosRepository.GetExpectedFile(filkravId);
            return forvFiler;
        }

        public IEnumerable<AdmRegister> HamtaAllaRegister()
        {
            var registersList = _portalSosRepository.GetAllRegisters();
            return registersList;
        }

        public IEnumerable<AdmRegister> HamtaAllaRegisterForPortalen()
        {
            var registersList = _portalSosRepository.GetAllRegistersForPortal();
            return registersList;
        }

        public IEnumerable<IdentityRole> HamtaAllaAstridRoller()
        {
            var roller = _portalSosRepository.GetAllAstridRoles();
            return roller;
        }

        public IEnumerable<IdentityRole> HamtaAllaFilipRoller()
        {
            var roller = _portalSosRepository.GetAllFilipRoles();
            return roller;
        }

        public IEnumerable<AdmDelregister> HamtaAllaDelregisterForPortalen()
        {
            var delregistersList = _portalSosRepository.GetAllSubDirectoriesForPortal();
            return delregistersList;
        }

        public IEnumerable<AdmDelregister> HamtaDelregisterMedInsamlingsfrekvens(int insamlingsfrekvensId)
        {
            var delregisterManadList = new List<AdmDelregister>();
           
            var delregisterList = _portalSosRepository.GetAllSubDirectoriesForPortal();
            foreach (var delreg in delregisterList)
            {
                var manadsinsamling = false;
                var filkravList = delreg.AdmFilkrav;
                foreach (var filkrav in filkravList)
                {
                    if (filkrav.InsamlingsfrekvensId == 1) //Månadsinsamling
                    {
                        manadsinsamling = true;
                    }
                }
                if (manadsinsamling)
                {
                    delregisterManadList.Add(delreg);
                }
            }
            return delregisterManadList;
        }

        public IEnumerable<RegisterInfo> HamtaAllRegisterInformation()
        {
            var registerList = _portalSosRepository.GetAllRegisterInformation();
            return registerList;
        }

        public IEnumerable<Organisation> HamtaAllaOrganisationer()
        {
            var orgList = _portalSosRepository.GetAllOrganisations();
            return orgList;
        }

        public IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForRegister(int regId)
        {
            var forvLeveranser = _portalSosRepository.GetExpectedDeliveriesForDirectory(regId);
            return forvLeveranser;
        }

        public IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForDelregister(int delregId)
        {
            var forvLeveranser = _portalSosRepository.GetExpectedDeliveriesForSubDirectory(delregId);
            return forvLeveranser;
        }

        public int HamtaForvantadleveransIdForRegisterOchPeriod(int delregId, string period)
        {
            var forvLevId = _portalSosRepository.GetExpextedDeliveryIdForSubDirAndPeriod(delregId, period);
            return forvLevId;
        }

        public List<string> HamtaGiltigaPerioderForDelregister(int delregId)
        {
            var allaForvLevForDelreg = _portalSosRepository.GetExpectedDeliveriesForSubDirectory(delregId);
            string period = String.Empty;
            DateTime startDate;
            DateTime endDate;

            DateTime dagensDatum = DateTime.Now.Date;
            var perioder = new List<string>();

            foreach (var forvLev in allaForvLevForDelreg)
            {
                startDate = forvLev.Rapporteringsstart;
                endDate = forvLev.Rapporteringsslut;
                if (dagensDatum >= startDate && dagensDatum <= endDate)
                {
                    perioder.Add(forvLev.Period);
                }
            }
            return perioder;
        }

        public IEnumerable<AdmFilkrav> HamtaFilkravForRegister(int regId)
        {
            var filkrav= _portalSosRepository.GetFileRequirementsForDirectory(regId); 
            return filkrav;
        }

        public AdmFAQKategori HamtaFAQKategori(int faqCatId)
        {
            var faqcat = _portalSosRepository.GetFAQCategory(faqCatId);
            return faqcat;
        }

        public AdmFAQ HamtaFAQ(int faqId)
        {
            var faq = _portalSosRepository.GetFAQ(faqId);
            return faq;
        }

        public IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisation(int orgId)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            //TODO - tidsintervall?
            //var leveransIdList = _portalRepository.GetLeveransIdnForOrganisation(orgId).OrderByDescending(x => x);
            var leveransList = _portalSosRepository.GetLeveranserForOrganisation(orgId);

            //Skapa historikrader/filloggrader
            historikLista = SkapaHistorikrader(leveransList);
            
            var sorteradHistorikLista = historikLista.OrderByDescending(x => x.Leveranstidpunkt).ToList();

            return sorteradHistorikLista;
        }

        public IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisation(int orgId)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            //TODO - tidsintervall?
            //var leveransIdList = _portalRepository.GetLeveransIdnForOrganisation(orgId).OrderByDescending(x => x);
            var leveransList = _portalSosRepository.GetTop10LeveranserForOrganisation(orgId);

            //Skapa historikrader/filloggrader
            historikLista = SkapaHistorikrader(leveransList);

            var sorteradHistorikLista = historikLista.OrderByDescending(x => x.Leveranstidpunkt).ToList();

            return sorteradHistorikLista;
        }


        public IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndUser(int orgId, string userId)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            //TODO - tidsintervall?
            //var leveransIdList = _portalRepository.GetLeveransIdnForOrganisation(orgId).OrderByDescending(x => x);
            var leveransList = _portalSosRepository.GetTop10LeveranserForOrganisationAndUser(orgId, userId);

            //Skapa historikrader/filloggrader
            historikLista = SkapaHistorikrader(leveransList);

            var sorteradHistorikLista = historikLista.OrderByDescending(x => x.Leveranstidpunkt).ToList();

            return sorteradHistorikLista;
        }

        public IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndDelreg(int orgId, List<RegisterInfo> valdaDelregister)
        {
            var leveransList = new List<Leverans>();
            foreach (var delreg in valdaDelregister)
            {
                var delregLeveransList = _portalSosRepository.GetTop10LeveranserForOrganisationAndDelreg(orgId, delreg.Id).ToList();
                leveransList.AddRange(delregLeveransList);
            }
            //Skapa historikrader/filloggrader
            var historikLista = SkapaHistorikrader(leveransList.OrderByDescending(x => x.Leveranstidpunkt).Take(10));

            //var sorteradHistorikLista = historikLista.OrderByDescending(x => x.Leveranstidpunkt).ToList();

            return historikLista;

        }

        public IEnumerable<FilloggDetaljDTO> FiltreraHistorikForAnvandare(string userId, List<RegisterInfo> valdaDelregisterList, List<FilloggDetaljDTO> historikForOrganisation)
        {
            var historikForAnvandareList = new List<FilloggDetaljDTO>();

            foreach (var rad in valdaDelregisterList)
            {
                var aktuellaLeveranser = historikForOrganisation.Where(x => x.RegisterKortnamn == rad.Kortnamn).ToList();
                historikForAnvandareList.AddRange(aktuellaLeveranser);
            }
            return historikForAnvandareList.OrderByDescending(x => x.Leveranstidpunkt).ToList();
        }

        private List<FilloggDetaljDTO> SkapaHistorikrader(IEnumerable<Leverans> leveransList)
        {
            var historikLista = new List<FilloggDetaljDTO>();

            foreach (var leverans in leveransList)
            {
                var filloggDetalj = new FilloggDetaljDTO();
                //Kolla om återkopplingsfil finns för aktuell leverans
                var aterkoppling = _portalSosRepository.GetAterkopplingForLeverans(leverans.Id);

                //Kolla om enhetskod finns för aktuell leverans (stadsdelsleverans)
                var enhetskod = String.Empty;
                if (leverans.OrganisationsenhetsId != null)
                {
                    var orgenhetid = Convert.ToInt32(leverans.OrganisationsenhetsId);
                    enhetskod = _portalSosRepository.GetEnhetskodForLeverans(orgenhetid);
                }

                //Hämta period för aktuell leverans
                var period = _portalSosRepository.GetPeriodForAktuellLeverans(leverans.ForvantadleveransId);

                var filer = _portalSosRepository.GetFilerForLeveransId(leverans.Id).ToList();
                var registerKortnamn = _portalSosRepository.GetSubDirectoryShortName(leverans.DelregisterId);

                if (!filer.Any())
                {
                    filloggDetalj = new FilloggDetaljDTO();
                    filloggDetalj.Id = 0;
                    filloggDetalj.LeveransId = leverans.Id;
                    filloggDetalj.Filnamn = " - ";
                    filloggDetalj.Filstatus = " - ";
                    filloggDetalj.Kontaktperson = leverans.ApplicationUserId;
                    filloggDetalj.Leveransstatus = leverans.Leveransstatus;
                    filloggDetalj.Leveranstidpunkt = leverans.Leveranstidpunkt;
                    filloggDetalj.RegisterKortnamn = registerKortnamn;
                    filloggDetalj.Resultatfil = " - ";
                    filloggDetalj.Enhetskod = enhetskod;
                    filloggDetalj.Period = period;
                    if (aterkoppling != null)
                    {
                        //filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus; //Skriv ej över leveransstatusen från återkopplingen. Beslut 20180912, ärende #128
                        filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                    }
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

            return historikLista;
        }

        public AdmForeskrift HamtaForeskriftByFilkrav(int filkravId)
        {
            var foreskrift = _portalSosRepository.GetForeskriftByFileReq(filkravId);
            return foreskrift;
        }

        public IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForDelregOchPeriod(int delRegId, string period)
        {
            var rappResList = _portalSosRepository.GetReportResultForSubdirAndPeriod(delRegId, period);
            //Filtrera bort dubletter
            IEnumerable<Rapporteringsresultat> filteredList = rappResList
                .GroupBy(x => x.Email)
                .Select(group => group.First());

            var ejLevList = new List<Rapporteringsresultat>();

            //Ta bara med dem som inte rapporterat alls eller som har leveransstatus "Leveransen är inte godkänd"
            foreach (var rappRes in filteredList)
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
            //Filtrera bort dubletter
            var distinctRappListDTO = rappResListDTO.GroupBy(x => x.Email).Select(x => x.First()).ToList();
            return distinctRappListDTO;
        }

        public IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForRegOchPeriod(int regId, string period)
        {
            var rappResList = _portalSosRepository.GetReportResultForDirAndPeriod(regId, period);
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

            var rappResListDTO = ConvertRappListDBToVM(ejLevList, regId, 0).ToList();

            //Filtrera bort dubletter
            var distinctRappListDTO = rappResListDTO.GroupBy(x => x.Email).Select(x => x.First()).ToList();

            return distinctRappListDTO;
        }

        public IEnumerable<AdmRegister> HamtaRegisterForOrg(int orgId)
        {
            var uppgskyldighetList = _portalSosRepository.GetReportObligationInformationForOrg(orgId);
            var delregList = new List<AdmDelregister>();
            var regList = new List<AdmRegister>();
            foreach (var uppgskyldighet in uppgskyldighetList)
            {
                var delreg = _portalSosRepository.GetSubDirectoryById(uppgskyldighet.DelregisterId);
                delregList.Add(delreg);
            }

            foreach (var delreg in delregList)
            {
                var reg = _portalSosRepository.GetDirectoryById(delreg.RegisterId);
                if (!regList.Contains(reg))
                        regList.Add(reg);
            }

            return regList;
        }

        public IEnumerable<string> HamtaDelregistersPerioderForAr(int delregId, int ar)
        {
            var perioder = _portalSosRepository.GetSubDirectoysPeriodsForAYear(delregId, ar);
            return perioder;
        }

        public List<int> HamtaValbaraAr(int delregId)
        {
            var arsLista = new List<int>();
            var uppgiftsstartLista = _portalSosRepository.GetTaskStartForSubdir(delregId);

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
            var rappStart = _portalSosRepository.GetReportstartForRegisterAndPeriod(regId, period);
            return rappStart;
        }

        public DateTime HamtaSenasteRapporteringForRegisterOchPeriod(int regId, string period)
        {
            var rappSenast = _portalSosRepository.GetLatestReportDateForRegisterAndPeriod(regId, period);
            return rappSenast;
        }

        //TODO - special för EKB-År. Lös på annat sätt.
        public DateTime HamtaRapporteringsstartForRegisterOchPeriodSpecial(int regId, string period)
        {
            var rappStart = _portalSosRepository.GetReportstartForRegisterAndPeriodSpecial(regId, period);
            return rappStart;
        }

        public DateTime HamtaSenasteRapporteringForRegisterOchPeriodSpecial(int regId, string period)
        {
            var rappSenast = _portalSosRepository.GetLatestReportDateForRegisterAndPeriodSpecial(regId, period);
            return rappSenast;
        }

        public IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisationRegisterPeriod(int orgId, int regId, string periodForReg)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            var sorteradHistorikLista = new List<FilloggDetaljDTO>();
            var delregisterLista = _portalSosRepository.GetSubdirsForDirectory(regId);
            //var forvLevId = _portalSosRepository.get

            foreach (var delregister in delregisterLista)
            {
                //Hämta forvantadleveransid för delregister och period
                var forvLevId = _portalSosRepository.GetExpextedDeliveryIdForSubDirAndPeriod(delregister.Id, periodForReg);

                var senasteLeverans = new Leverans();
                //kan org rapportera per enhet för aktuellt delregister? => hämta senaste leverans per enhet
                var uppgiftsskyldighet = _portalSosRepository.GetUppgiftsskyldighetForOrganisationAndRegister(orgId, delregister.Id);
                if (uppgiftsskyldighet != null)
                {
                    if (uppgiftsskyldighet.RapporterarPerEnhet)
                    {
                        var orgEnhetsList = _portalSosRepository.GetOrgUnitsForOrg(orgId);
                        foreach (var orgenhet in orgEnhetsList)
                        {
                            senasteLeverans =
                                _portalSosRepository.GetLatestDeliveryForOrganisationSubDirectoryPeriodAndOrgUnit(orgId, delregister.Id, forvLevId, orgenhet.Id);
                            if (senasteLeverans != null)
                            {
                                AddHistorikListItem(senasteLeverans, historikLista);
                            }
                        }
                    }
                    else
                    {
                        senasteLeverans =
                            _portalSosRepository.GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(orgId, delregister.Id,
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

        //public IEnumerable<RegisterInfo> HamtaValdaRegistersForAnvandare(string userId, int orgId)
        //{
        //    var registerList = _portalSosRepository.GetChosenDelRegistersForUser(userId);
        //    //var allaRegisterList = _portalSosRepository.GetAllRegisterInformation();
        //    var allaRegisterList = _portalSosRepository.GetAllRegisterInformationForOrganisation(orgId);
        //    var userRegisterList = new List<RegisterInfo>();

        //    foreach (var register in allaRegisterList)
        //    {
        //        foreach (var userRegister in registerList)
        //        {
        //            if (register.Id == userRegister.DelregisterId)
        //            {
        //                register.SelectedFilkrav = "0";
        //                userRegisterList.Add(register);
        //            }
        //        }
        //    }

        //    //Check if users organisation reports per unit. If thats the case, get list of units
        //    foreach (var item in userRegisterList)
        //    {
        //        var uppgiftsskyldighet = HamtaUppgiftsskyldighetForOrganisationOchRegister(orgId, item.Id);
        //        if (uppgiftsskyldighet.RapporterarPerEnhet)
        //        {
        //            item.RapporterarPerEnhet = true;
        //            var orgUnits = _portalSosRepository.GetOrganisationUnits(orgId);
        //            item.Organisationsenheter = new List<KeyValuePair<string, string>>();
        //            foreach (var orgUnit in orgUnits)
        //            {
        //                KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Enhetsnamn);
        //                item.Organisationsenheter.Add(keyValuePair);
        //            }

        //        }
        //    }

        //    return userRegisterList;
        //}

        public List<RegisterInfo> HamtaValdaDelregisterForAnvandare(string userId, int orgId)
        {
            var registerList = _portalSosRepository.GetChosenDelRegistersForUser(userId).ToList();
            //var allaRegisterList = _portalRepository.GetAllRegisterInformation();
            var allaRegisterList = _portalSosRepository.GetAllRegisterInformationForOrganisation(orgId).ToList();
            var userRegisterList = new List<RegisterInfo>();

            foreach (var register in allaRegisterList)
            {
                var finns = registerList.Find(r => r.DelregisterId == register.Id);
                if (finns != null)
                {
                    register.SelectedFilkrav = "0";
                    userRegisterList.Add(register);
                }
            }

            //Check if users organisation reports per unit. If thats the case, get list of units
            foreach (var item in userRegisterList)
            {
                var uppgiftsskyldighet = HamtaUppgiftsskyldighetForOrganisationOchRegister(orgId, item.Id);
                if (uppgiftsskyldighet.RapporterarPerEnhet)
                {
                    item.RapporterarPerEnhet = true;
                    var orgUnits = _portalSosRepository.GetOrganisationUnits(orgId);
                    //Ger cirkulär reference, därva keyValuePair nedan
                    //item.Orgenheter = orgUnits.ToList();
                    item.Organisationsenheter = new List<KeyValuePair<string, string>>();
                    item.Orgenheter = new List<KeyValuePair<string, string>>();
                    foreach (var orgUnit in orgUnits)
                    {
                        KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Enhetsnamn);
                        item.Organisationsenheter.Add(keyValuePair);
                        KeyValuePair<string, string> keyValuePairFilkod = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Filkod);
                        item.Orgenheter.Add(keyValuePairFilkod);
                    }

                }
            }

            return userRegisterList;
        }

        public IEnumerable<RegisterInfo> HamtaRegistersMedAnvandaresVal(string userId, int orgId)
        {
            var registerList = _portalSosRepository.GetChosenDelRegistersForUser(userId);
            //var allaRegisterList = _portalRepository.GetAllRegisterInformation();
            var allaRegisterList = _portalSosRepository.GetAllRegisterInformationForOrganisation(orgId);

            foreach (var register in allaRegisterList)
            {
                foreach (var userRegister in registerList)
                {
                    if (register.Id == userRegister.DelregisterId)
                    {
                        register.Selected = true;
                    }
                }
            }

            return allaRegisterList;
        }

        public IEnumerable<AdmRegister> HamtaRegisterForAnvandare(string userId, int orgId)
        {
            var registerList = _portalSosRepository.GetChosenRegistersForUser(userId);
            //Rensa bort dubbletter avseende kortnamn
            //IEnumerable<string> allaKortnamn = registerList.Select(x => x.Kortnamn).Distinct();

            return registerList;
        }

        public Arendetyp HamtaArendetyp(int arendetypId)
        {
            var arendetyp = _portalSosRepository.GetCaseType(arendetypId);
            return arendetyp;
        }

        public ArendeStatus HamtaArendestatus(int arendestatusId)
        {
            var arendestatus = _portalSosRepository.GetCaseStatus(arendestatusId);
            return arendestatus;
        }

        public string HamtaArendesRapportorer(int orgId, int arendeId)
        {
            string rapportorsLista = "";
            var arendeRapportorerIdList = _portalSosRepository.GetCaseReporterIds(arendeId);
            var arendeRappotorerIdUndantagList = _portalSosRepository.GetPrivateEmailAdressesForOrgAndCase(orgId, arendeId);
            foreach (var rapportorId in arendeRapportorerIdList)
            {
                var epostadress = _portalSosRepository.GetUserEmail(rapportorId);
                if (String.IsNullOrEmpty(rapportorsLista))
                {
                    rapportorsLista = epostadress;
                }
                else
                {
                    rapportorsLista = rapportorsLista + ", " + epostadress;
                }
            }
            foreach (var item in arendeRappotorerIdUndantagList)
            {
                if (String.IsNullOrEmpty(rapportorsLista))
                {
                    rapportorsLista = item.PrivatEpostAdress;
                }
                else
                {
                    rapportorsLista = rapportorsLista + ", " + item.PrivatEpostAdress;
                }
            }
            return rapportorsLista;
        }

        public AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrganisationOchRegister(int orgId, int delregid)
        {
            var uppgiftsskyldighet = _portalSosRepository.GetUppgiftsskyldighetForOrganisationAndRegister(orgId, delregid);

            return uppgiftsskyldighet;
        }

        public void InaktiveraKontaktperson(string userId)
        {
            _portalSosRepository.DisableContact(userId);
            //Remove users chosen registers
            _portalSosRepository.DeleteChosenSubDirectoriesForUser(userId);
        }

        public void SkapaAstridRoll(string rollNamn)
        {
            _portalSosRepository.CreateAstridRole(rollNamn);
        }

        public void SkapaFilipRoll(string rollNamn)
        {
            _portalSosRepository.CreateFilipRole(rollNamn);
        }

        public int SkapaOrganisation(Organisation org, ICollection<Organisationstyp> orgtyperForOrg, string userName)
        {
            //Sätt datum och användare
            org.SkapadDatum = DateTime.Now;
            org.SkapadAv = userName;
            org.AndradDatum = DateTime.Now;
            org.AndradAv = userName;

            var orgId = _portalSosRepository.CreateOrganisation(org, orgtyperForOrg);
            return orgId;
        }

        public void SkapaOrganisationsenhet(Organisationsenhet orgUnit, string userName)
        {
            //Sätt datum och användare
            orgUnit.SkapadDatum = DateTime.Now;
            orgUnit.SkapadAv = userName;
            orgUnit.AndradDatum = DateTime.Now;
            orgUnit.AndradAv = userName;

            _portalSosRepository.CreateOrgUnit(orgUnit);
        }

        public void SkapaPrivatEpostadress(UndantagEpostDoman privEmail, string userName)
        {
            //Sätt datum och användare
            privEmail.SkapadDatum = DateTime.Now;
            privEmail.SkapadAv = userName;
            privEmail.AndradDatum = DateTime.Now;
            privEmail.AndradAv = userName;

            _portalSosRepository.CreatePrivateEmail(privEmail);
        }

        public void SkapaArende(ArendeDTO arende, string userName)
        {
            var registeredReportersList = new List<string>();
            var unregisteredReportersList = new List<UndantagEpostDoman>();

            var arendeDb = ConvertArendeDTOToDb(arende);
            //Sätt datum och användare
            arendeDb.SkapadDatum = DateTime.Now;
            arendeDb.SkapadAv = userName;
            arendeDb.AndradDatum = DateTime.Now;
            arendeDb.AndradAv = userName;

            _portalSosRepository.CreateCase(arendeDb);
            //Hantera rapportörer för ärendet
            //Kontrollera om rapportör redan är registrerad i Filip, annars spara i undantagstabell
            var reporters = arende.Rapportorer.Replace(' ', ',');
            var newEmailStr = reporters.Split(',');
            foreach (var email in newEmailStr)
            {
                if (!String.IsNullOrEmpty(email.Trim()))
                {
                    var redanReggadAnv = _portalSosRepository.GetUserByEmail(email.Trim());
                    if (redanReggadAnv != null)
                    {
                        registeredReportersList.Add(redanReggadAnv.Id);
                    }
                    else
                    {
                        //Spara i undantagstabell
                        var undantag = new UndantagEpostDoman
                        {
                            OrganisationsId = arende.OrganisationsId,
                            ArendeId = arendeDb.Id,
                            PrivatEpostAdress = email.Trim(),
                            AktivFrom = DateTime.Now,
                            SkapadAv = userName,
                            SkapadDatum = DateTime.Now,
                            AndradAv = userName,
                            AndradDatum = DateTime.Now
                        };
                        unregisteredReportersList.Add(undantag);
                    }
                }
            }

            _portalSosRepository.UpdateCaseReporters(arendeDb.Id, registeredReportersList, userName);
            //Lägg till rollen RegSvcRapp för redan reggad användare (om den inte redan är satt)
            foreach (var reporterId in registeredReportersList)
            {
                _portalSosRepository.AddRoleToFilipUser(reporterId, "RegSvcRapp");
            }

            //Oreggade användare läggs i undantagstabellen tills användaren registrerat sig
            foreach (var unregistered in unregisteredReportersList)
            {
                _portalSosRepository.CreatePrivateEmail(unregistered);
            }

        }

        public void SkapaOrganisationstyp(AdmOrganisationstyp orgtyp, string userName)
        {
            //Sätt datum och användare
            orgtyp.SkapadDatum = DateTime.Now;
            orgtyp.SkapadAv = userName;
            orgtyp.AndradDatum = DateTime.Now;
            orgtyp.AndradAv = userName;

            _portalSosRepository.CreateOrgType(orgtyp);
        }

        public void SkapaFAQKategori(AdmFAQKategori faqKategori, string userName)
        {
            //Sätt datum och användare
            faqKategori.SkapadDatum = DateTime.Now;
            faqKategori.SkapadAv = userName;
            faqKategori.AndradDatum = DateTime.Now;
            faqKategori.AndradAv = userName;

            _portalSosRepository.CreateFAQCategory(faqKategori);
        }

        public void SkapaFAQ(AdmFAQ faq, string userName)
        {
            //Sätt datum och användare
            faq.SkapadDatum = DateTime.Now;
            faq.SkapadAv = userName;
            faq.AndradDatum = DateTime.Now;
            faq.AndradAv = userName;

            _portalSosRepository.CreateFAQ(faq);
        }

        public void SkapaHelgdag(AdmHelgdag helgdag, string userName)
        {
            //Sätt datum och användare
            helgdag.SkapadDatum = DateTime.Now;
            helgdag.SkapadAv = userName;
            helgdag.AndradDatum = DateTime.Now;
            helgdag.AndradAv = userName;

            _portalSosRepository.CreateHoliday(helgdag);
        }

        public void SkapaSpecialdag(AdmSpecialdag specialdag, string userName)
        {
            //Sätt datum och användare
            specialdag.SkapadDatum = DateTime.Now;
            specialdag.SkapadAv = userName;
            specialdag.AndradDatum = DateTime.Now;
            specialdag.AndradAv = userName;

            _portalSosRepository.CreateSpecialDay(specialdag);
        }

        public void SkapaInformationsText(AdmInformation infoText, string userName)
        {
            //Sätt datum och användare
            infoText.SkapadDatum = DateTime.Now;
            infoText.SkapadAv = userName;
            infoText.AndradDatum = DateTime.Now;
            infoText.AndradAv = userName;
            _portalSosRepository.CreateInformationText(infoText);
        }

        public void SkapaUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSk, string userName)
        {
            //Sätt datum och användare
            uppgSk.SkapadDatum = DateTime.Now;
            uppgSk.SkapadAv = userName;
            uppgSk.AndradDatum = DateTime.Now;
            uppgSk.AndradAv = userName;
            _portalSosRepository.CreateReportObligation(uppgSk);
        }

        public void SkapaEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSk, string userName)
        {
            //Sätt datum och användare
            enhetsUppgSk.SkapadDatum = DateTime.Now;
            enhetsUppgSk.SkapadAv = userName;
            enhetsUppgSk.AndradDatum = DateTime.Now;
            enhetsUppgSk.AndradAv = userName;
            _portalSosRepository.CreateUnitReportObligation(enhetsUppgSk);
        }

        public void SkapaRegister(AdmRegister reg, string userName)
        {
            //Sätt datum och användare
            reg.SkapadDatum = DateTime.Now;
            reg.SkapadAv = userName;
            reg.AndradDatum = DateTime.Now;
            reg.AndradAv = userName; 
            _portalSosRepository.CreateDirectory(reg);
        }

        public void SkapaDelregister(AdmDelregister delReg, string userName)
        {
            //Sätt datum och användare
            delReg.SkapadDatum = DateTime.Now;
            delReg.SkapadAv = userName;
            delReg.AndradDatum = DateTime.Now;
            delReg.AndradAv = userName;
            _portalSosRepository.CreateSubDirectory(delReg);
        }

        public void SkapaForvantadLeverans(AdmForvantadleverans forvLev, string userName)
        {
            //Sätt datum och användare
            forvLev.SkapadDatum = DateTime.Now;
            forvLev.SkapadAv = userName;
            forvLev.AndradDatum = DateTime.Now;
            forvLev.AndradAv = userName;
            _portalSosRepository.CreateExpectedDelivery(forvLev);
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
                _portalSosRepository.CreateExpectedDelivery(forvLev);
            }
        }

        public IEnumerable<ForvantadLeveransDTO> SkapaForvantadeLeveranserUtkast(int selectedYear, int selectedDelRegId, int selectedFilkravId)
        {
            var forvantadeLeveranser = new List<ForvantadLeveransDTO>();
            var forvantadeLeveranserUtkast = new List<ForvantadLeveransDTO>();
            //Hämta regler från aktuellt AdmFilkrav 
            var filkrav = _portalSosRepository.GetFileRequirementsForSubDirectoryAndFileReqId(selectedDelRegId, selectedFilkravId);
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
            _portalSosRepository.CreateExpectedFile(forvFil);
        }

        public void SkapaFilkrav(AdmFilkrav filkrav, string userName)
        {
            //Sätt datum och användare
            filkrav.SkapadDatum = DateTime.Now;
            filkrav.SkapadAv = userName;
            filkrav.AndradDatum = DateTime.Now;
            filkrav.AndradAv = userName;
            _portalSosRepository.CreateFileRequirement(filkrav);
        }

        public void SkapaInsamlingsfrekvens(AdmInsamlingsfrekvens insamlingsfrekvens, string userName)
        {
            //Sätt datum och användare
            insamlingsfrekvens.SkapadDatum = DateTime.Now;
            insamlingsfrekvens.SkapadAv = userName;
            insamlingsfrekvens.AndradDatum = DateTime.Now;
            insamlingsfrekvens.AndradAv = userName;
            _portalSosRepository.CreateCollectFrequence(insamlingsfrekvens);
        }

        public void UppdateraAstridRoll(IdentityRole role)
        {
            _portalSosRepository.UpdateAstridRole(role);
        }

        public void UppdateraFilipRoll(IdentityRole role)
        {
            _portalSosRepository.UpdateFilipRole(role);
        }

        public void UppdateraOrganisation(Organisation org, string userName)
        {
            //Sätt datum och användare
            org.AndradDatum = DateTime.Now;
            org.AndradAv = userName;
            _portalSosRepository.UpdateOrganisation(org);
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
            _portalSosRepository.UpdateContactPerson(user);
        }

        public void UppdateraNamnForAnvandare(string userId, string userName)
        {
            _portalSosRepository.UpdateNameForUser(userId, userName);
        }

        public void UppdateraAdminAnvandare(AppUserAdmin user, string userName)
        {
            user.AndradDatum = DateTime.Now;
            user.AndradAv = userName;
            _portalSosRepository.UpdateAdminUser(user);
        }

        public void UppdateraKontaktnummerForAnvandare(string userId, string tfnnr)
        {
            _portalSosRepository.UpdateContactNumberForUser(userId, tfnnr);
        }

        public void UppdateraAktivFromForAnvandare(string userId)
        {
            _portalSosRepository.UpdateActiveFromForUser(userId);
        }

        public void UppdateraAnvandarInfo(ApplicationUser user)
        {
            _portalSosRepository.UpdateUserInfo(user);
        }

        public void UppdateraOrganisationsenhet(Organisationsenhet orgUnit, string userName)
        {
            //Sätt datum och användare
            orgUnit.AndradDatum = DateTime.Now;
            orgUnit.AndradAv = userName;
            _portalSosRepository.UpdateOrgUnit(orgUnit);
        }

        public void UppdateraOrganisationstyp(AdmOrganisationstyp orgtyp, string userName)
        {
            //Sätt datum och användare
            orgtyp.AndradDatum = DateTime.Now;
            orgtyp.AndradAv = userName;
            _portalSosRepository.UpdateOrgType(orgtyp);
        }

        public void UppdateraUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSkyldighet, string userName)
        {
            //Sätt datum och användare
            uppgSkyldighet.AndradDatum = DateTime.Now;
            uppgSkyldighet.AndradAv = userName;
            _portalSosRepository.UpdateReportObligation(uppgSkyldighet);
        }

        public void UppdateraValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> registerList)
        {
            _portalSosRepository.UpdateChosenRegistersForUser(userId, userName, registerList);
        }

        public void UppdateraEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSkyldighet, string userName)
        {
            //Sätt datum och användare
            enhetsUppgSkyldighet.AndradDatum = DateTime.Now;
            enhetsUppgSkyldighet.AndradAv = userName;
            _portalSosRepository.UpdateUnitReportObligation(enhetsUppgSkyldighet);
        }

        public void UppdateraFAQKategori(AdmFAQKategori faqKategori, string userName)
        {
            faqKategori.AndradDatum = DateTime.Now;
            faqKategori.AndradAv = userName;
            faqKategori.Sortering = Convert.ToInt32(faqKategori.Sortering);
            _portalSosRepository.UpdateFAQCategory(faqKategori);
        }

        public void UppdateraFAQ(AdmFAQ faq, string userName)
        {
            faq.AndradDatum = DateTime.Now;
            faq.AndradAv = userName;
            _portalSosRepository.UpdateFAQ(faq);
        }


        public void UppdateraHelgdag(AdmHelgdag holiday, string userName)
        {
            holiday.AndradDatum = DateTime.Now;
            holiday.AndradAv = userName;
            _portalSosRepository.UpdateHoliday(holiday);
        }

        public void UppdateraSpecialdag(AdmSpecialdag specialday, string userName)
        {
            specialday.AndradDatum = DateTime.Now;
            specialday.AndradAv = userName;
            _portalSosRepository.UpdateSpecialDay(specialday);
        }


        public void UppdateraInformationstext(AdmInformation infoText, string userName)
        {
            infoText.AndradDatum = DateTime.Now;
            infoText.AndradAv = userName;
            _portalSosRepository.UpdateInfoText(infoText);
        }

        public void UppdateraRegister(AdmRegister register, string userName)
        {
            register.AndradAv = userName;
            register.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateDirectory(register);
        }

        public void UppdateraDelregister(AdmDelregister delregister, string userName)
        {
            delregister.AndradAv = userName;
            delregister.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateSubDirectory(delregister);
        }

        public void UppdateraForvantadLeverans(AdmForvantadleverans forvLev, string userName)
        {
            forvLev.AndradAv = userName;
            forvLev.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateExpectedDelivery(forvLev);
        }

        public void UppdateraForvantadFil(AdmForvantadfil forvFil, string userName)
        {
            forvFil.AndradAv = userName;
            forvFil.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateExpectedFile(forvFil);
        }

        public void UppdateraFilkrav(AdmFilkrav filkrav, string userName)
        {
            filkrav.AndradAv = userName;
            filkrav.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateFileRequirement(filkrav);
        }

        public void UppdateraInsamlingsfrekvens(AdmInsamlingsfrekvens insamlingsfrekvens, string userName)
        {
            insamlingsfrekvens.AndradAv = userName;
            insamlingsfrekvens.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateCollectFrequency(insamlingsfrekvens);
        }

        public void UppdateraAnvandarInfo(AppUserAdmin user, string userName)
        {
            user.AndradAv = userName;
            user.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateAdminUserInfo(user);
        }

        public void UppdateraPrivatEpostAdress(UndantagEpostDoman privEpostDoman, string userName)
        {
            privEpostDoman.AndradAv = userName;
            privEpostDoman.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdatePrivateEmail(privEpostDoman);
        }

        public void UppdateraArende(ArendeDTO arende,  string userName, string rapportorer)
        {
            var registeredReportersList = new List<string>();
            var unregisteredReportersList = new List<UndantagEpostDoman>();
            var arendeDb = ConvertArendeDTOToDb(arende);
            arendeDb.AndradAv = userName;
            arendeDb.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateCase(arendeDb);

            //Hantera rapportörer för ärendet
            //Kontrollera om rapportör redan är registrerad i Filip, annars spara i undantagstabell
            var reporters = rapportorer.Replace(' ', ',');
            var newEmailStr = reporters.Split(',');
            foreach (var email in newEmailStr)
            {
                if (!String.IsNullOrEmpty(email.Trim()))
                {
                    var redanReggadAnv = _portalSosRepository.GetUserByEmail(email.Trim());
                    if (redanReggadAnv != null)
                    {
                        registeredReportersList.Add(redanReggadAnv.Id);
                    }
                    else
                    {
                        //Spara i undantagstabell
                        var undantag = new UndantagEpostDoman
                        {
                            OrganisationsId = arende.OrganisationsId,
                            ArendeId = arende.Id,
                            PrivatEpostAdress = email.Trim(),
                            AktivFrom = DateTime.Now,
                            SkapadAv = userName,
                            SkapadDatum = DateTime.Now,
                            AndradAv = userName,
                            AndradDatum = DateTime.Now
                        };
                        unregisteredReportersList.Add(undantag);
                    }
                }
            }

            //Användare som redan är registrerade
            _portalSosRepository.UpdateCaseReporters(arendeDb.Id,registeredReportersList, userName);

            //Användare som ej är registrerade
            _portalSosRepository.UpdateCaseUnregisteredReporters(arendeDb.Id, unregisteredReportersList, userName);

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
            _portalSosRepository.SaveOpeningHours(admKonfClosedAnayway);

            //Closed days
            string daysJoined = string.Join(",", oppetTider.ClosedDays);
            AdmKonfiguration admKonfClosedDays = new AdmKonfiguration
            {
                Typ = "ClosedDays",
                Varde = daysJoined
            };
            admKonfClosedDays.AndradAv = userName;
            admKonfClosedDays.AndradDatum = DateTime.Now;
            _portalSosRepository.SaveOpeningHours(admKonfClosedDays);

            //Closed from hour
            AdmKonfiguration admKonfClosedFromHour = new AdmKonfiguration
            {
                Typ = "ClosedFromHour",
                Varde = oppetTider.ClosedFromHour.ToString()
            };
            admKonfClosedFromHour.AndradAv = userName;
            admKonfClosedFromHour.AndradDatum = DateTime.Now;
            _portalSosRepository.SaveOpeningHours(admKonfClosedFromHour);

            //Closed from minute
            AdmKonfiguration admKonfClosedFromMin = new AdmKonfiguration
            {
                Typ = "ClosedFromMin",
                Varde = oppetTider.ClosedFromMin.ToString()
            };
            admKonfClosedFromMin.AndradAv = userName;
            admKonfClosedFromMin.AndradDatum = DateTime.Now;
            _portalSosRepository.SaveOpeningHours(admKonfClosedFromMin);

            //Closed to hour
            AdmKonfiguration admKonfClosedToHour = new AdmKonfiguration
            {
                Typ = "ClosedToHour",
                Varde = oppetTider.ClosedToHour.ToString()
            };
            admKonfClosedToHour.AndradAv = userName;
            admKonfClosedToHour.AndradDatum = DateTime.Now;
            _portalSosRepository.SaveOpeningHours(admKonfClosedToHour);

            //Closed to minute
            AdmKonfiguration admKonfClosedToMin = new AdmKonfiguration
            {
                Typ = "ClosedToMin",
                Varde = oppetTider.ClosedToMin.ToString()
            };
            admKonfClosedToMin.AndradAv = userName;
            admKonfClosedToMin.AndradDatum = DateTime.Now;
            _portalSosRepository.SaveOpeningHours(admKonfClosedToMin);

            //Closed informationtext
            var infoPageId = _portalSosRepository.GetPageInfoTextId("Stangtsida");
            AdmInformation infoTextClosedpage = new AdmInformation
            {
                Id = infoPageId,
                Informationstyp = "Stangtsida",
                Text = oppetTider.InfoTextForClosedPage
            };
            infoTextClosedpage.AndradAv = userName;
            infoTextClosedpage.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateInfoText(infoTextClosedpage);
        }

        public void SparaTillDatabasFillogg(string userName, string ursprungligFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber)
        {
            _portalSosRepository.SaveToFilelogg(userName, ursprungligFilNamn, nyttFilNamn, leveransId, sequenceNumber);
        }

        public void SparaValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> regIdList)
        {
            _portalSosRepository.SaveChosenRegistersForUser(userId, userName, regIdList);
        }

        public void SaveToLoginLog(string userid, string userName)
        {
            _portalSosRepository.SaveToLoginLog(userid, userName);
        }

        public void TaBortAstridRoll(string roleName)
        {
            _portalSosRepository.DeleteAstridRole(roleName);
        }

        public void TaBortFilipRoll(string roleName)
        {
            _portalSosRepository.DeleteFilipRole(roleName);
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

        public string MaskPhoneNumber(string phoneNumber)
        {
            var maskedPhoneNumber = String.Empty;

            //Replace initial numbers with *
            var lengthToMask = phoneNumber.Length - 4;
            for (int i = 0; i < lengthToMask; i++)
            {
                maskedPhoneNumber += "*";
            }
            //Add four last number to masked string
            maskedPhoneNumber += phoneNumber.Substring(lengthToMask);

            return maskedPhoneNumber;
        }

        public void TaBortFAQKategori(int faqKategoriId)
        {
            _portalSosRepository.DeleteFAQCategory(faqKategoriId);
        }



        public void TaBortFAQ(int faqId)
        {
            _portalSosRepository.DeleteFAQ(faqId);
        }

        public void TaBortOrganisationstyp(int orgTypeId)
        {
            _portalSosRepository.DeleteOrgType(orgTypeId);
        }

        public void TaBortHelgdag(int holidayId)
        {
            _portalSosRepository.DeleteHoliday(holidayId);
        }

        public void TaBortSpecialdag(int specialDayId)
        {
            _portalSosRepository.DeleteSpecialDay(specialDayId);
        }

        public void TaBortKontaktperson(string contactId)
        {
            _portalSosRepository.DeleteContact(contactId);
        }

        public void TaBortAdminAnvandare(string userId)
        {
            _portalSosRepository.DeleteAdminUser(userId);
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
                    Mail = false
                };

                //If user is inactive, remove emailadress from list
                if (!String.IsNullOrEmpty(rappResRadVM.Email))
                {
                    var user = _portalSosRepository.GetUserByEmail(rappResRadVM.Email);
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

        private Arende ConvertArendeDTOToDb(ArendeDTO arendeDto)
        {
            var arende = new Arende
            {
                Id = arendeDto.Id,
                OrganisationsId = arendeDto.OrganisationsId,
                Arendenamn = arendeDto.Arendenamn,
                Arendenr = arendeDto.Arendenr,
                ArendetypId = arendeDto.ArendetypId,
                ArendestatusId = arendeDto.ArendestatusId,
                StartDatum = arendeDto.StartDatum,
                SlutDatum = arendeDto.SlutDatum
            };
            return arende;
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
                var delregisterList = _portalSosRepository.GetSubDirectoriesForDirectory(regId);
                foreach (var delregister in delregisterList)
                {
                    delregContactList = _portalSosRepository.GetContactPersonsForOrgAndSubdir(rappRes.OrganisationsId, delregister.Id).ToList();
                    contactList.Union(delregContactList).ToList();
                }
                
            }
            else
            {
                contactList = _portalSosRepository.GetContactPersonsForOrgAndSubdir(rappRes.OrganisationsId, delRegId).ToList();
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
            //Om ändå ingen epostadress hittats, hämta från organisationen
            if (email == String.Empty)
            {
                //Get email from organisation
                var org = _portalSosRepository.GetOrganisation(rappRes.OrganisationsId);
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
            var i = 0;
            //ta fram lista med epostadresser för valda rader
            foreach (var rappRes in rappResList)
            {
                i++;
                if (rappRes.Mail)
                {
                    if (rappRes.Email == null)
                    {
                        throw new ApplicationException("rappRes.Email är null i svc. Tot: " + rappResList.Count() + ", Index: " + i + ", Id:" + rappRes.Id.ToString() + ", Kommunkod: " + rappRes.Kommunkod + ", Orgnamn: " + rappRes.Organisationsnamn);
                    }
                    //var x = rappRes.Email.IndexOf(",");
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
                    msg.To.Add(new MailAddress(_portalSosRepository.GetAdminUserEmail(userId)));
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

        public string HamtaHelgdagsInfo()
        {
            var date = DateTime.Now.Date;
            var text = String.Empty;
            var helgdagar = _portalSosRepository.GetHolidays();

            foreach (var helgdag in helgdagar)
            {
                if (helgdag.Helgdatum == date)
                {
                    text = _portalSosRepository.GetInfoText(helgdag.InformationsId).Text;
                }
            }

            return text;
        }

        public string HamtaSpecialdagsInfo()
        {
            var date = DateTime.Now.Date;
            var text = String.Empty;
            var specialdagar = _portalSosRepository.GetSpecialDays();

            foreach (var dag in specialdagar)
            {
                if (dag.Specialdagdatum == date)
                {
                    text = _portalSosRepository.GetInfoText(dag.InformationsId).Text;
                }
            }

            return text;
        }


        public bool IsHelgdag()
        {
            var date = DateTime.Now.Date;
            var helgdagar = _portalSosRepository.GetHolidays();

            foreach (var helgdag in helgdagar)
            {
                if (helgdag.Helgdatum == date)
                    return true;
            }

            return false;
        }

        public bool IsSpecialdag()
        {
            var now = DateTime.Now;
            var date = DateTime.Now.Date;

            var timeNow = now.TimeOfDay;
            var specialdagar = _portalSosRepository.GetSpecialDays();

            foreach (var dag in specialdagar)
            {
                if (dag.Specialdagdatum == date)
                    //Kolla klockslag
                    if (timeNow < dag.Oppna || timeNow >= dag.Stang)
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool IsOpen()
        {
            if (IsHelgdag())
            {
                return false;
            }
            else if (IsSpecialdag())
            {
                return false;
            }

            var now = DateTime.Now;
            var today = now.DayOfWeek.ToString();
            var hourNow = now.Hour;
            var minuteNow = now.Minute;

            //Get Openinghours from database
            var closedDays = _portalSosRepository.GetClosedDays().Split(new char[] { ',' });
            var closedFromHour = Int32.Parse(_portalSosRepository.GetClosedFromHour());
            var closedFromMin = Int32.Parse(_portalSosRepository.GetClosedFromMin());
            var closedToHour = Int32.Parse(_portalSosRepository.GetClosedToHour());
            var closedToMin = Int32.Parse(_portalSosRepository.GetClosedToMin());
            var closedAnyway = Convert.ToBoolean(_portalSosRepository.GetClosedAnnyway());

            //Test
            //hourNow = 8;
            //minuteNow = 2;

            if (closedAnyway)
            {
                return false;
            }

            //Check day
            foreach (var day in closedDays)
            {
                if (day == today)
                {
                    return false;
                }
            }

            //After closing
            if ((closedFromHour <= hourNow))
            {
                //Check minute
                if (minuteNow < closedFromMin)
                {
                    return true;
                }
                return false;
            }

            //Before opening?
            if ((closedToHour > hourNow))
            {
                return false;
            }

            if (closedToHour == hourNow)
            {
                //Check minute
                if (minuteNow > closedToMin)
                {
                    return true;
                }
                return false;
            }

            return true;
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
                    _portalSosRepository.GetExpectedDeliveryBySubDirAndFileReqIdAndPeriod(forvLev.DelregisterId, forvLev.FilkravId, forvLev.Period);
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
            var aterkoppling = _portalSosRepository.GetAterkopplingForLeverans(senasteLeverans.Id);
            //Kolla om enhetskod finns för aktuell leverans (stadsdelsleverans)
            var enhetskod = String.Empty;

            if (senasteLeverans.OrganisationsenhetsId != null)
            {
                var orgenhetid = Convert.ToInt32(senasteLeverans.OrganisationsenhetsId);
                enhetskod = _portalSosRepository.GetEnhetskodForLeverans(orgenhetid);
            }

            //Hämta period för aktuell leverans
            var period = _portalSosRepository.GetPeriodForAktuellLeverans(senasteLeverans.ForvantadleveransId);

            var filer = _portalSosRepository.GetFilerForLeveransId(senasteLeverans.Id).ToList();
            var registerKortnamn = _portalSosRepository.GetSubDirectoryShortName(senasteLeverans.DelregisterId);

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

        private string GetLastPartOfHostAdress(string hostAdress)
        {
            List<int> indexes = hostAdress.AllIndexesOf(".");

            if (indexes.Count == 1)
                return hostAdress;

            var indexToCutFrom = indexes[indexes.Count - 2];
            return hostAdress.Substring(indexToCutFrom + 1);

        }

        public List<List<Organisation>> SokOrganisation(string sokStr)
        {
            string[] searchstring = sokStr.Split(' ');
            var orgList = _portalSosRepository.SearchOrganisation(searchstring);
            return orgList;
        }


    }
}

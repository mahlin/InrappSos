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
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
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


        public bool IsConnectedViaPrivateEmailadress(string email)
        {
            var userException = _portalSosRepository.GetUserFromUndantagEpostadress(email);
            var currDate = DateTime.Now;
            var isConnected = false;
            if (userException != null)
            {
                if (userException.AktivTom == null || userException.AktivTom >= currDate)
                {
                    isConnected = true;
                }
            }
            return isConnected;
        }

        public bool IsConnectedViaArende(string email)
        {
            var userException = _portalSosRepository.GetUserFromPreKontakt(email);
            if (userException != null)
            {
                return true;
            }
            return false;
        }

        public UndantagEpostadress HamtaUndantagEpostadress(string email)
        {
            var userException = _portalSosRepository.GetUserFromUndantagEpostadress(email);
            return userException;
        }

        public PreKontakt HamtaPrekontakt(string email)
        {
            var userException = _portalSosRepository.GetUserFromPreKontakt(email);
            return userException;
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

        public string DeviatingOpeningHoursNextThreeWeeks()
        {
            var deviatingOpeningHoursNextThreeWeeks = String.Empty;
            var deviatingHoursNextThreeWeeks = new List<DeviatingOpeningHoursDTO>();
            var specialDaysNextThreeWeeks = SpecialdaysNextThreeWeeks();
            var holidaysNextThreeWeeks = HolidaysNextThreeWeeks();
            deviatingHoursNextThreeWeeks.AddRange(specialDaysNextThreeWeeks);
            deviatingHoursNextThreeWeeks.AddRange(holidaysNextThreeWeeks);
            var sortedDeviatingHoursNextThreeWeeks = deviatingHoursNextThreeWeeks.OrderBy(x => x.Datum);
            foreach (var day in sortedDeviatingHoursNextThreeWeeks)
            {
                deviatingOpeningHoursNextThreeWeeks = deviatingOpeningHoursNextThreeWeeks + day.DeviatingInfo;
            }
            return deviatingOpeningHoursNextThreeWeeks;
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

        public List<DeviatingOpeningHoursDTO> HolidaysNextThreeWeeks()
        {
            var helgdagarInomTreVeckor = new List<DeviatingOpeningHoursDTO>();
            var date = DateTime.Now.Date;
            var dateNowPlusThreeWeeks = date.AddDays(21);
            var helgdagar = _portalSosRepository.GetHolidays();

            foreach (var helgdag in helgdagar)
            {
                if (helgdag.Helgdatum >= date && helgdag.Helgdatum <= dateNowPlusThreeWeeks)
                {
                    var veckodag = helgdag.Helgdatum.ToString("dddd", _culture);
                    veckodag = char.ToUpper(veckodag[0]) + veckodag.Substring(1);
                    var dagNr = helgdag.Helgdatum.Day;
                    var manad = helgdag.Helgdatum.ToString("MMMM", _culture);
                    var dagStr = veckodag + " " + dagNr + " " + manad + " stängt " + helgdag.Helgdag + "<br>";
                    var devOpeningHoursObj = new DeviatingOpeningHoursDTO
                    {
                        Datum = helgdag.Helgdatum,
                        DeviatingInfo = dagStr
                    };
                    helgdagarInomTreVeckor.Add(devOpeningHoursObj); 
                }
            }

            return helgdagarInomTreVeckor;
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

        public List<DeviatingOpeningHoursDTO> SpecialdaysNextThreeWeeks()
        {
            var specialdagarInomTreVeckor = new List<DeviatingOpeningHoursDTO>();
            var now = DateTime.Now;
            var dateNow = DateTime.Now.Date;
            var dateNowPlusThreeWeeks = dateNow.AddDays(21);

            var specialdagar = _portalSosRepository.GetSpecialDays();

            foreach (var dag in specialdagar)
            {
                if (dag.Specialdagdatum >= dateNow && dag.Specialdagdatum <= dateNowPlusThreeWeeks)
                {
                    //string FormatDate = dag.Specialdagdatum.ToString("dddd dd MMMM yyyy", culture);
                    var veckodag = dag.Specialdagdatum.ToString("dddd", _culture);
                    veckodag = char.ToUpper(veckodag[0]) + veckodag.Substring(1);
                    var dagNr = dag.Specialdagdatum.Day;
                    var manad = dag.Specialdagdatum.ToString("MMMM", _culture);
                    var klockslagFrom = dag.Oppna.ToString(@"hh\:mm");
                    var klockslagTom = dag.Stang.ToString(@"hh\:mm");
                    var dagStr = veckodag + " " + dagNr + " " + manad + " öppet " + klockslagFrom + "-" + klockslagTom + " med anledning av " + dag.Anledning + "<br>";

                    var devOpeningHoursObj = new DeviatingOpeningHoursDTO
                    {
                        Datum = dag.Specialdagdatum,
                        DeviatingInfo = dagStr
                    };
                    specialdagarInomTreVeckor.Add(devOpeningHoursObj);
                }
            }
            return specialdagarInomTreVeckor;
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

        public string HamtaAnvandarId(ApplicationUser user)
        {
            var userId = user.Id;
            return userId;
        }

        public string HamtaAnvandaresKontaktnummer(string userId)
        {
            var contactNumber = _portalSosRepository.GetUserContactNumber(userId);
            return contactNumber;
        }

        public IEnumerable<Arende> HamtaAnvandaresArenden(string userId)
        {
            var userCases = _portalSosRepository.GetUserCases(userId);
            return userCases;
        }

        public IEnumerable<Leverans> HamtaAnvandaresLeveranser(string userId)
        {
            var leveranser = _portalSosRepository.GetUserDeliveries(userId);
            return leveranser;
        }

        public IEnumerable<Arende> HamtaAnvandaresOppnaArenden(string userId)
        {
            var userOpenCases = new List<Arende>();
            var userCases = _portalSosRepository.GetUserCases(userId);
            foreach (var userCase in userCases)
            {
                if (userCase.Aktiv)
                {
                    userOpenCases.Add(userCase);
                }
            }

            return userOpenCases;
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

        public string HamtaLandstingskodForOrg(int orgId)
        {
            var landstingskodkod = _portalSosRepository.GetLandstingskodForOrganisation(orgId);
            return landstingskodkod;
        }

        public string HamtaKommunKodForAnvandare(string userId)
        {
            var orgId = _portalSosRepository.GetUserOrganisationId(userId);
            var kommunKod = _portalSosRepository.GetKommunkodForOrganisation(orgId);
            return kommunKod;
        }

        public string HamtaLandstingsKodForAnvandare(string userId)
        {
            var orgId = _portalSosRepository.GetUserOrganisationId(userId);
            var landstingsKod = _portalSosRepository.GetLandstingskodForOrganisation(orgId);
            return landstingsKod;
        }

        public string HamtaInrapporteringskodKodForAnvandare(string userId)
        {
            var orgId = _portalSosRepository.GetUserOrganisationId(userId);
            var inrapporteringsKod = _portalSosRepository.GetInrapporteringskodForOrganisation(orgId);
            return inrapporteringsKod;
        }

        public string HamtaInrapporteringskodKodForOrganisation(int orgId)
        {
            var inrapporteringsKod = _portalSosRepository.GetInrapporteringskodForOrganisation(orgId);
            return inrapporteringsKod;
        }

        public AdmOrganisationstyp HamtaOrgtyp(string orgtypnamn)
        {
            var orgtyp = _portalSosRepository.GetOrgtypeByName(orgtypnamn);
            return orgtyp;
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

        public List<OrganisationstypDTO> HamtaOrgtyperForOrganisation(int orgId)
        {
            var orgTypeList = new List<OrganisationstypDTO>();
            var chosenOrgTypeIdsForOrgList = _portalSosRepository.GetOrgTypesIdsForOrg(orgId);
            var orgtyper = HamtaAllaOrganisationstyper().ToList();

            foreach (var orgtyp in orgtyper)
            {
                if (chosenOrgTypeIdsForOrgList.Contains(orgtyp.Id))
                {
                    var orgtypDTO = new OrganisationstypDTO()
                    {
                        Organisationstypid = orgtyp.Id,
                        Typnamn = orgtyp.Typnamn,
                        Beskrivning = orgtyp.Beskrivning,
                    };
                    orgTypeList.Add(orgtypDTO);
                }
                
            }

            var sortedOrgTypeList = orgTypeList.OrderBy(x => x.Organisationstypid).ToList();
            return sortedOrgTypeList;
        }

        public List<OrganisationstypDTO> HamtaOrgtyperForDelregister(int delregId)
        {
            var orgTypeList = new List<OrganisationstypDTO>();
            var orgTypeIdsForSubDir = _portalSosRepository.GetOrgTypesIdsForSubDir(delregId);
            var orgtyper = HamtaAllaOrganisationstyper().ToList();

            foreach (var orgtyp in orgtyper)
            {
                if (orgTypeIdsForSubDir.Contains(orgtyp.Id))
                {
                    var orgtypDTO = new OrganisationstypDTO()
                    {
                        Organisationstypid = orgtyp.Id,
                        Typnamn = orgtyp.Typnamn,
                        Beskrivning = orgtyp.Beskrivning,
                    };
                    orgTypeList.Add(orgtypDTO);
                }

            }

            var sortedOrgTypeList = orgTypeList.OrderBy(x => x.Organisationstypid).ToList();
            return sortedOrgTypeList;

        }

        public AdmOrganisationstyp HamtaOrganisationstyp(int orgtypId)
        {
            var orgtyp = _portalSosRepository.GetOrgtype(orgtypId);
            return orgtyp;
        }
        
        public IEnumerable<ApplicationUser> HamtaKontaktpersonerForOrg(int orgId)
        {
            var contacts = _portalSosRepository.GetContactPersonsForOrg(orgId);
            return contacts;
        }

        public IEnumerable<ApplicationUser> HamtaAktivaKontaktpersonerForOrg(int orgId)
        {
            var activeContacts = _portalSosRepository.GetActiveContactPersonsForOrg(orgId);
            return activeContacts;
        }

        public IEnumerable<SFTPkonto> HamtaSFTPkontonForOrg(int orgId)
        {
            var accounts = _portalSosRepository.GetSFTPAccountsForOrg(orgId);
            return accounts;
        }

        public IEnumerable<ApplicationUser> HamtaKontaktpersonerForSFTPKonto(int sftpKontoId)
        {
            var contacts = _portalSosRepository.GetContactPersonsForSFTPAccount(sftpKontoId);
            return contacts;
        }

        public List<string> HamtaEpostadresserForSFTPKonto(int sftpKontoId)
        {
            var emailadresses = new List<string>();
            var contacts = _portalSosRepository.GetContactPersonsForSFTPAccount(sftpKontoId);
            foreach (var contact in contacts)
            {
                //Check that contact is still active
                if (contact.AktivTom == null)
                {
                    emailadresses.Add(contact.Email);
                }
                else if (contact.AktivTom >= DateTime.Now.Date)
                {
                    emailadresses.Add(contact.Email);
                }
                
            }
            return emailadresses;
        }

        public IEnumerable<UndantagEpostadress> HamtaPrivataEpostadresserForOrg(int orgId)
        {
            var privEmails = _portalSosRepository.GetPrivateEmailAdressesForOrg(orgId);
            return privEmails;
        }

        public IEnumerable<UndantagForvantadfil> HamtaUndantagnaForvantadeFilerForOrg(int orgId)
        {
            var exceptionsExpectedFiles = _portalSosRepository.GetExceptionsExpectedFilesforOrg(orgId);
            return exceptionsExpectedFiles;
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
        

        public IEnumerable<Organisationsenhet> HamtaOrganisationsenheterMedUppgSkyldighetsId(int uppgSkyldighetsid)
        {
            var orgEnheter = _portalSosRepository.GetOrgUnitsByRepOblId(uppgSkyldighetsid);
            return orgEnheter;
        }

        public IEnumerable<Organisationsenhet> HamtaOrganisationsenheterMedUppgSkyldighetInomPerioden(int uppgSkyldighetsid, string period)
        {
            var orgEnheter = _portalSosRepository.GetOrgUnitsByRepOblWithInPeriod(uppgSkyldighetsid, period);
            return orgEnheter;
        }

        public Organisationsenhet HamtaOrganisationsenhetMedEnhetskod(string kod, int orgId)
        {
            var orgenhet = _portalSosRepository.GetOrganisationUnitByCode(kod, orgId);
            return orgenhet;
        }

        public Organisationsenhet HamtaOrganisationsenhetMedFilkod(string kod, int orgId)
        {
            var orgenhet = _portalSosRepository.GetOrganisationUnitByFileCode(kod, orgId);
            return orgenhet;
        }

        public IEnumerable<AdmUppgiftsskyldighet> HamtaUppgiftsskyldighetForOrg(int orgId)
        {
            var uppgiftsskyldigheter = _portalSosRepository.GetReportObligationInformationForOrg(orgId);
            return uppgiftsskyldigheter;
        }

        public IEnumerable<AdmUppgiftsskyldighet> HamtaAktivUppgiftsskyldighetForOrg(int orgId)
        {
            var uppgiftsskyldigheter = _portalSosRepository.GetActiveReportObligationInformationForOrg(orgId);
            return uppgiftsskyldigheter;
        }


        public AdmUppgiftsskyldighet HamtaAktivUppgiftsskyldighetForOrganisationOchRegister(int orgId, int delregid)
        {
            var uppgiftsskyldighet = _portalSosRepository.GetActiveReportObligationInformationForOrgAndSubDir(orgId, delregid);
            return uppgiftsskyldighet;
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

        public AdmEnhetsUppgiftsskyldighet HamtaAktivEnhetsUppgiftsskyldighetForUppgiftsskyldighetOchOrgEnhet(int uppgskhId,
            int orgenhetId)
        {
            var enhetsuppgiftsskyldighet = _portalSosRepository.GetActiveUnitReportObligationForReportObligationAndOrg(uppgskhId, orgenhetId);
            return enhetsuppgiftsskyldighet;
        }

        public IEnumerable<AdmEnhetsUppgiftsskyldighet> HamtaAktivEnhetsUppgiftsskyldighetForUppgiftsskyldighet(int uppgskhId)
        {
            var enhetsuppgskhList = _portalSosRepository.GetActiveUnitReportObligationForReportObligation(uppgskhId);
            return enhetsuppgskhList;
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

        public ApplicationRole HamtaAstridRoll(string roleName)
        {
            var astridRoll = _portalSosRepository.GetAstridRole(roleName);
            return astridRoll;
        }

        public List<string> HamtaFilipRollerForAnvandare(string userId)
        {
            var rollLista = new List<string>();
            var userRoles = _portalSosRepository.GetFilipUserRolesForUser(userId);

            foreach (var userRole in userRoles)
            {
                var role = _portalSosRepository.GetFilipRoleById(userRole.RoleId);
                rollLista.Add(role.BeskrivandeNamn);
            }
            return rollLista;
        }

        public ApplicationRole HamtaFilipRoll(string roleName)
        {
            var role = _portalSosRepository.GetFilipRoleByName(roleName);
            return role;
        }

        public void HandlePrekontaktUserRegistration(ApplicationUser user, PreKontakt preKontakt)
        {
            //Om användaren är kopplad till ett ärende som hör till användarens organisation
            var arende = _portalSosRepository.GetCase(preKontakt.ArendeId);
            if (arende.OrganisationsId == user.OrganisationId)
            {
                //Lägg till som kontaktperson för ärendet och sätt roll
                KopplaKontaktpersonTillArende(user.Email, preKontakt.ArendeId, user.Id);
                //Ta bort från PreKontakt
                _portalSosRepository.DeletePreKontakt(preKontakt.Id);
            }
            //else
            //{
            //    //TODO
            //    //var org = _portalSosRepository.GetOrganisation(arende.OrganisationsId);
            //    //if ()
            //    KopplaFilipAnvändareTillFilipRollNamn(user.UserName, user.Id, "RegUpp");
            //}
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

        public List<string> HamtaGodkandaFilnamnsStarter()
        {
            var allaFilmasker = _portalSosRepository.GetAllFileMasks();
            var allaFilmaskStarter = HamtaFilnamnsstarterFranFilmasker(allaFilmasker);
            return allaFilmaskStarter;
        }

        public IEnumerable<AdmFilkrav> HamtaAktivaFilkravForRegister(int regId)
        {
            var aktivaFilkravList = new List<AdmFilkrav>();
            var filkravList = _portalSosRepository.GetFileRequirementsForDirectory(regId);
            //Check if active föreskrift
            foreach (var filkrav in filkravList)
            {
                if (filkrav.ForeskriftsId != null)
                {
                    var foreskrift = _portalSosRepository.GetForeskriftById(filkrav.ForeskriftsId.GetValueOrDefault());
                    if (foreskrift.GiltigTom != null)
                    {
                        if (foreskrift.GiltigTom >= DateTime.Now)
                        {
                            aktivaFilkravList.Add(filkrav);
                        }
                    }
                    else
                    {
                        aktivaFilkravList.Add(filkrav);
                    }
                }
                else
                {
                    aktivaFilkravList.Add(filkrav);
                }
            }
            return aktivaFilkravList;
        }

        public AdmFilkrav HamtaFilkravById(int filkravsId)
        {
            var filkrav = _portalSosRepository.GetFileRequirementById(filkravsId);
            return filkrav;
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

        public IEnumerable<Inloggning> HamtaInloggning(string userId)
        {
            var logins = _portalSosRepository.GetLogins(userId);
            return logins;
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

        public AdmDelregister HamtaDelregister(int delregId)
        {
            var delreg = _portalSosRepository.GetSubDirectoryById(delregId);
            return delreg;
        }

        public IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> HamtaAllaDelRegistersOrganisationstyper()
        {
            var delregOrgtyper = _portalSosRepository.GetAllSubDirectoriesOrgtypes();
            return delregOrgtyper;
        }

        public IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> HamtaDelRegistersOrganisationstyper(int delregId)
        {
            var delregOrgtyper = _portalSosRepository.GetOrgTypesForSubDir(delregId); 
            return delregOrgtyper;
        }

        public List<string> HamtaOrganisationstyperForDelregister(int delregId)
        {
            var orgtypNamnList = new List<string>();
            var delregOrgtypes = _portalSosRepository.GetOrgTypesForSubDir(delregId);
            foreach (var delregOrgtype in delregOrgtypes)
            {
                orgtypNamnList.Add( _portalSosRepository.GetOrgtype(delregOrgtype.OrganisationstypId).Typnamn);
            }

            return orgtypNamnList;
        }

        public IEnumerable<AdmDelregister> HamtaDelRegisterForRegister(int regId)
        {
            var subDirectories = _portalSosRepository.GetSubDirectoriesForDirectory(regId);
            return subDirectories;
        }

        public IEnumerable<AdmDelregister> HamtaDelRegisterMedUndertabellerForRegister(int regId)
        {
            var subDirectories = _portalSosRepository.GetSubDirectoriesWithIncludesForDirectory(regId);
            return subDirectories;
        }

        public IEnumerable<AdmDelregister> HamtaDelRegisterMedUndertabeller(List<RegisterInfo> delregList)
        {
            var subdirList = new List<AdmDelregister>();
            foreach (var delreg in delregList)
            {
                var subDirectory = _portalSosRepository.GetSubDirectoryWithIncludes(delreg.Id);
                subdirList.Add(subDirectory);
            }
            return subdirList;
        }

        public IEnumerable<AdmDelregister> HamtaAnvandarensDelregisterForRegister(int regId, List<AdmDelregister> delregList)
        {
            var subdirList = new List<AdmDelregister>();
            foreach (var delreg in delregList)
            {
                if (delreg.RegisterId == regId)
                {
                    subdirList.Add(delreg);
                }
            }
            return subdirList;
        }


        public IEnumerable<AdmForeskrift> HamtaForeskrifterForRegister(int regId)
        {
            var foreskrifter = _portalSosRepository.GetRegulationsForDirectory(regId);
            return foreskrifter;
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

        public IEnumerable<AdmForvantadfil> HamtaAllaAktivaForvantadeFiler()
        {
            var aktivaForvFilList = new List<AdmForvantadfil>();
            var forvFilList = _portalSosRepository.GetAllExpectedFiles();
            //Check if active föreskrift
            foreach (var forvFil in forvFilList)
            {
                if (forvFil.ForeskriftsId != null)
                {
                    var foreskrift = _portalSosRepository.GetForeskriftById(forvFil.ForeskriftsId.GetValueOrDefault());
                    if (foreskrift.GiltigTom != null)
                    {
                        if (foreskrift.GiltigTom >= DateTime.Now)
                        {
                            aktivaForvFilList.Add(forvFil);
                        }
                    }
                    else
                    {
                        aktivaForvFilList.Add(forvFil);
                    }
                }
                else
                {
                    aktivaForvFilList.Add(forvFil);
                }
            }
            return aktivaForvFilList;
        }

        public IEnumerable<AdmFilkrav> HamtaAllaFilkrav()
        {
            var filkravList = _portalSosRepository.GetAllFileRequirements();
            return filkravList;
        }

        public IEnumerable<AdmFilkrav> HamtaAllaAktivaFilkrav()
        {
            var aktivaFilkravList = new List<AdmFilkrav> ();
            var filkravList = _portalSosRepository.GetAllFileRequirements();
            //Check if active föreskrift
            foreach (var filkrav in filkravList)
            {
                if (filkrav.ForeskriftsId != null)
                {
                    var foreskrift = _portalSosRepository.GetForeskriftById(filkrav.ForeskriftsId.GetValueOrDefault());
                    if (foreskrift.GiltigTom != null)
                    {
                        if (foreskrift.GiltigTom >= DateTime.Now)
                        {
                            aktivaFilkravList.Add(filkrav);
                        }
                    }
                    else
                    {
                        aktivaFilkravList.Add(filkrav);
                    }
                }
                else
                {
                    aktivaFilkravList.Add(filkrav);
                }
            }
            return aktivaFilkravList;
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
            var levId = _portalSosRepository.GetNewLeveransId(userId, userName, orgId, registerId, orgenhetsId, forvLevId, status,0);
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

        public IEnumerable<AdmForvantadfil> HamtaAktivaForvantadeFilerForRegister(int regId)
        {
            var activeForvantadeFiler = new List<AdmForvantadfil>();
            var forvantadeFiler = _portalSosRepository.GetExpectedFilesForDirectory(regId);
            //Check if active föreskrift
            foreach (var forvFil in forvantadeFiler)
            {
                if (forvFil.ForeskriftsId != null)
                {
                    var foreskrift = _portalSosRepository.GetForeskriftById(forvFil.ForeskriftsId.GetValueOrDefault());
                    if (foreskrift.GiltigTom != null)
                    {
                        if (foreskrift.GiltigTom >= DateTime.Now)
                        {
                            activeForvantadeFiler.Add(forvFil);
                        }
                    }
                    else
                    {
                        activeForvantadeFiler.Add(forvFil);
                    }
                }
                else
                {
                    activeForvantadeFiler.Add(forvFil);
                }
            }
            return activeForvantadeFiler;
        }

        public IEnumerable<AdmForvantadfil> HamtaForvantadFil(int filkravId)
        {
            var forvFiler = _portalSosRepository.GetExpectedFile(filkravId);
            return forvFiler;
        }

        public IEnumerable<AdmForvantadfilDTO> HamtaAllaForvantadeFilerForOrg(int orgId)
        {
            var forvantadeFilerList = new List<AdmForvantadfil>();
            var forvantadeFilerDTOList = new List<AdmForvantadfilDTO>();
            var delregisterList = _portalSosRepository.GetSubDirsObligatedForOrg(orgId);

            foreach (var delReg in delregisterList)
            {
                foreach (var filkrav in delReg.AdmFilkrav)
                {
                    //TODO - ta bara med filkrav med giltig föreskrift?
                    //Sök forväntad fil för varje filkrav 
                    forvantadeFilerList.AddRange(filkrav.AdmForvantadfil.ToList());
                }
            }
            forvantadeFilerDTOList = ConvertForvFiltoDTO(forvantadeFilerList);
            return forvantadeFilerDTOList;
        }

        public SFTPkonto HamtaFtpKontoByName(string name)
        {
            var sftpKonto = _portalSosRepository.GetSFTPAccountByName(name);
            return sftpKonto;
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

        public IEnumerable<ApplicationRole> HamtaAllaAstridRoller()
        {
            var roller = _portalSosRepository.GetAllAstridRoles();
            return roller;
        }

        public IEnumerable<ApplicationRole> HamtaAllaFilipRoller()
        {
            var roller = _portalSosRepository.GetAllFilipRoles();
            return roller;
        }

        public IEnumerable<AdmDelregister> HamtaAllaDelregisterForPortalen()
        {
            var delregistersList = _portalSosRepository.GetAllSubDirectoriesForPortal();
            return delregistersList;
        }

        public IEnumerable<AdmDelregister> HamtaAllaDelregisterForOrganisationen(int orgId)
        {
            //Hämta endast de delregister som organisationens orgtyper ska rapportera för
            var delregistersList = new List<AdmDelregister>();
            var delregisterIdn = new List<int>();
            var orgsOrgtypIdn = _portalSosRepository.GetOrgTypesIdsForOrg(orgId);
            foreach (var orgtypeId in orgsOrgtypIdn)
            {
                var delregidn = _portalSosRepository.GetSubdirIdsForOrgtype(orgtypeId);
                foreach (var delregid in delregidn)
                {
                    if (!delregisterIdn.Contains(delregid))
                    delregisterIdn.Add(delregid);
                }
            }
            foreach (var delregid in delregisterIdn)
            {
                delregistersList.Add(_portalSosRepository.GetSubDirectoryById(delregid));
            }
            return delregistersList;
        }

        public IEnumerable<RegisterInfo> HamtaAllRegisterInformationForOrganisation(int orgId)
        {
            var allRegisterinfoForOrg = _portalSosRepository.GetAllRegisterInformationForOrganisation(orgId).ToList();
            allRegisterinfoForOrg = HamtaOrgenheter(allRegisterinfoForOrg, orgId);
            return allRegisterinfoForOrg;
        }

        public IEnumerable<AppUserAdmin> HamtaAllaArendeadministratorerForOrg(int orgId)
        {
            var roll = _portalSosRepository.GetAstridRole("ArendeAdmin");
            var userRoles = _portalSosRepository.GetAstridUsersInRole(roll.Id);
            var arendeadmins = new List<AppUserAdmin>();
            foreach (var userRole in userRoles)
            {
                var user = _portalSosRepository.GetAstridUserById(userRole.UserId);
                arendeadmins.Add(user);
            }
            return arendeadmins;
        }

        public IEnumerable<ArendeAnsvarig> HamtaAllaArendeansvarigaForOrg(int orgId)
        {
            var arendeansvariga = new List<ArendeAnsvarig>();
            var casesForOrg = _portalSosRepository.GetCasesForOrg(orgId);
            foreach (var arende in casesForOrg)
            {
                var arendeansv = _portalSosRepository.GetCaseResponsible(arende.ArendeansvarId);
                arendeansvariga.Add(arendeansv);
            }
            return arendeansvariga;
        }

        public IEnumerable<ArendeAnsvarig> HamtaAllaArendeansvariga()
        {
            var arendeansvariga = _portalSosRepository.GetAllCaseResponsibles();
            return arendeansvariga;
        }

        public IEnumerable<AdmForeskrift> HamtaAllaForeskrifter()
        {
            var foreskrifter = _portalSosRepository.GetAllRegulations();
            return foreskrifter;
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

        //public IEnumerable<RegisterInfo> HamtaAllRegisterInformation()
        //{
        //    var registerList = _portalSosRepository.GetAllRegisterInformation();
        //    return registerList;
        //}

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

        public List<string> HamtaGiltigaFilkoderForOrganisation(int orgId)
        {
            var orgenheterForOrg = _portalSosRepository.GetOrgUnitsForOrg(orgId);
            var orgenhetskoder = new List<string>();
            foreach (var orgenhet in orgenheterForOrg)
            {
                orgenhetskoder.Add(orgenhet.Enhetskod);
            }
            return orgenhetskoder;
        }

        public List<string> HamtaGiltigaFilkoderForSFTPKonto(int sftpAccountId)
        {
            var filkoder = new List<string>();
            var filkoderDistinct = new List<string>();
            var sftpAccount = _portalSosRepository.GetSFTPAccount(sftpAccountId);
            var orgenheterForOrg = _portalSosRepository.GetOrgUnitsForOrg(sftpAccount.OrganisationsId);
            var delRegList = _portalSosRepository.GetSubDirectoriesForDirectory(sftpAccount.RegisterId);
            var uppgiftskyldighetIdnForOrgOchRegisterList = new List<int>();
            foreach (var delregister in delRegList)
            {
                var uppgiftsskyldighet =_portalSosRepository.GetReportObligationInformationForOrgAndSubDir(sftpAccount.OrganisationsId, delregister.Id);
                if (uppgiftsskyldighet!= null)
                    if (uppgiftsskyldighet.SkyldigTom == null)
                    {
                        uppgiftskyldighetIdnForOrgOchRegisterList.Add(uppgiftsskyldighet.Id);
                    }
                    else
                    {
                        if (uppgiftsskyldighet.SkyldigTom <= DateTime.Now)
                            uppgiftskyldighetIdnForOrgOchRegisterList.Add(uppgiftsskyldighet.Id);
                    }
            }

            //Kolla vilka orgenheter som är uppgiftsskyldiga för aktuellt register
            foreach (var orgenhet in orgenheterForOrg)
            {
                var enhetsUppgiftsskyldighetList = _portalSosRepository.GetUnitReportObligationInformationForOrgUnit(orgenhet.Id);
                foreach (var enhetsuppgiftsskyldighet in enhetsUppgiftsskyldighetList)
                {
                    if (uppgiftskyldighetIdnForOrgOchRegisterList.Contains(enhetsuppgiftsskyldighet.UppgiftsskyldighetId))
                        filkoder.Add(orgenhet.Filkod);
                }
                
            }
            filkoderDistinct.AddRange(filkoder.Distinct());
            return filkoderDistinct;
        }

        public IEnumerable<AdmFilkrav> HamtaFilkravForRegister(int regId)
        {
            var filkrav= _portalSosRepository.GetFileRequirementsForDirectory(regId); 
            return filkrav;
        }

        public AdmForeskrift HamtaForeskrift(int foreskriftId)
        {
            var foreskrift = _portalSosRepository.GetRegulation(foreskriftId);
            return foreskrift;
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

        public IEnumerable<FildroppDetaljDTO> HamtaFildroppsHistorikForAnvandaresArenden(string userId)
        {
            var usersCases = _portalSosRepository.GetUserCases(userId);
            var leveransList = new List<DroppadFil>();
            foreach (var arende in usersCases)
            {
                leveransList.AddRange(_portalSosRepository.GetDroppedFilesForCase(arende.Id).ToList());
            }
            var historikLista = SkapaHistorikraderForDroppadeFiler(leveransList);

            var sorteradHistorikLista = historikLista.OrderByDescending(x => x.AndradDatum).ToList();

            return sorteradHistorikLista;
        }

        public IEnumerable<FildroppDetaljDTO> HamtaFildroppsHistorikForValtArende(int arendeId)
        {
            var leveransList = _portalSosRepository.GetDroppedFilesForCase(arendeId).ToList();
            var historikLista = SkapaHistorikraderForDroppadeFiler(leveransList);

            var sorteradHistorikLista = historikLista.OrderByDescending(x => x.AndradDatum).ToList();
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

        public IEnumerable<FilloggDetaljDTO> HamtaTop100HistorikForOrganisationAndDelreg(int orgId, List<RegisterInfo> valdaDelregister)
        {
            var leveransList = new List<Leverans>();
            foreach (var delreg in valdaDelregister)
            {
                var delregLeveransList = _portalSosRepository.GetTop100LeveranserForOrganisationAndDelreg(orgId, delreg.Id).ToList();
                leveransList.AddRange(delregLeveransList);
            }
            //Skapa historikrader/filloggrader
            var historikLista = SkapaHistorikrader(leveransList.OrderByDescending(x => x.Leveranstidpunkt));

            //var sorteradHistorikLista = historikLista.OrderByDescending(x => x.Leveranstidpunkt).ToList();

            return historikLista;
        }

        public IEnumerable<FilloggDetaljDTO> HamtaTop100HistorikForOrganisationAndDelregAndOrgenheter(int orgId, List<RegisterInfo> valdaDelregister,
            List<Organisationsenhet> orgenhetList)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndDelregOchValdaOrgenheter(int orgId,List<RegisterInfo> valdaDelregister, string userId)
        {
            var historikForAnvandaren = new List<FilloggDetaljDTO>();
            var leveranser = HamtaTop100HistorikForOrganisationAndDelreg(orgId, valdaDelregister);

            foreach (var valtDelregister in valdaDelregister)
            {
                if (valtDelregister.RapporterarPerEnhet)
                {
                    //var valdaOrgenheter = _portalSosRepository.GetUsersChosenOrgUnits(userId);
                    //if (valdaOrgenheter.Any())
                    //{
                    //    foreach (var leverans in leveranser)
                    //    {
                    //        //TODO bara chosenOrgUnits för aktuell delregister när databasen ändrats
                    //        //var valdaOrgenheter = _portalSosRepository.GetUsersChosenOrgUnitsForSubDir(userId, leverans.Id);

                    //        if (leverans.OrganisationsenhetsId != 0)
                    //        {
                    //            //check if current user report for this orgUnit 
                    //            var valdOrgEnhet = valdaOrgenheter.Where(x =>
                    //                x.OrganisationsenhetsId == leverans.OrganisationsenhetsId);
                    //            if (valdOrgEnhet.Any())
                    //            {
                    //                historikForAnvandaren.Add(leverans);
                    //            }
                    //        }
                    //    }
                    //}
                }
                else
                {
                    foreach (var leverans in leveranser)
                    {
                        historikForAnvandaren.Add(leverans);
                    }
                }
            }
            return historikForAnvandaren.OrderByDescending(a => a.Leveranstidpunkt).Take(10);
        }

        public List<List<Organisation>> SokCaseOrganisation(string sokStr)
        {
            var orgtypeId = _portalSosRepository.GetOrgtypeByName("Ärendeorganisation").Id;
            var caseOrgList = new List<List<Organisation>>();
            var orgList = new List<Organisation>();

            if (orgtypeId != null)
            {
                string[] searchstring = sokStr.Split(' ');
                var orgSearchList = _portalSosRepository.SearchOrganisation(searchstring);

                foreach (var orgs in orgSearchList)
                {
                    foreach (var org in orgs)
                    {
                        var orgtypes = _portalSosRepository.GetOrgTypesIdsForOrg(org.Id);
                        if (orgtypes.Contains(orgtypeId))
                        {
                            orgList.Add(org);
                        }
                    }
                    caseOrgList.Add(orgList);
                }
            }
            
            return caseOrgList;
        }

        public List<List<Arende>> SokArende(string sokStr)
        {
            string[] searchstring = sokStr.Split(' ');
            var caseList = _portalSosRepository.SearchCase(searchstring);
            //Sök på kontaktpersoner
            var contactList = _portalSosRepository.SearchContact(searchstring);
            var allContactsCases = new List<Arende>();
            foreach (var itemsList in contactList)
            {
                var contactCases = new List<Arende>();
                foreach (var contact in itemsList)
                {
                    //Check if contact has any cases
                    contactCases = _portalSosRepository.GetCasesForContact(contact.Id).ToList();
                    allContactsCases.AddRange(contactCases);
                }
            }
            caseList.Add(allContactsCases);

            //Sök på ärendeansvariga
            var caseManagerList = _portalSosRepository.SearchCaseManager(searchstring);
            var allCaseManagerCases = new List<Arende>();
            foreach (var itemsList in caseManagerList)
            {
                var caseManagerCases = new List<Arende>();
                foreach (var caseManager in itemsList)
                {
                    //Check if caseManager has any cases
                    caseManagerCases = _portalSosRepository.GetCasesForCaseManager(caseManager.Id);
                    allCaseManagerCases.AddRange(caseManagerCases);
                }
            }
            caseList.Add(allCaseManagerCases);

            //Sök på ej registrerade kontaktpersoner (Prekontakt)
            var preContactList = _portalSosRepository.SearchPreContact(searchstring);
            var allPreContactsCases = new List<Arende>();
            foreach (var itemsList in preContactList)
            {
                var preContactCases = new List<Arende>();
                foreach (var preContact in itemsList)
                {
                    //Check if preContact has any cases
                    preContactCases = _portalSosRepository.GetCasesForPreContact(preContact.Id);
                    allPreContactsCases.AddRange(preContactCases);
                }
            }
            caseList.Add(allPreContactsCases);

            //Sök på ärendetyp
            var caseTypeList = _portalSosRepository.SearchCaseType(searchstring);
            var allCasesTypeCases = new List<Arende>();
            foreach (var itemlist in caseTypeList)
            {
                var caseTypeCases = new List<Arende>();
                foreach (var caseType in itemlist)
                {
                    //get all cases with current caseType
                    caseTypeCases = _portalSosRepository.GetCasesByCaseType(caseType.Id);
                    allCasesTypeCases.AddRange(caseTypeCases);
                }
            }
            caseList.Add(allCasesTypeCases);

            return caseList;
        }

        public List<List<ApplicationUser>> SokKontaktperson(string sokStr)
        {
            string[] searchstring = sokStr.Split(' ');
            var contactList = _portalSosRepository.SearchContact(searchstring);
            return contactList;

        }

        public IEnumerable<FilloggDetaljDTO> FiltreraHistorikForAnvandare(string userId, List<RegisterInfo> valdaDelregisterList, List<FilloggDetaljDTO> historikForOrganisation)
        {
            var historikForAnvandareList = new List<FilloggDetaljDTO>();

            foreach (var rad in valdaDelregisterList)
            {
                var aktuellaLeveranser = historikForOrganisation.Where(x => x.RegisterKortnamn == rad.Kortnamn).ToList();

                if (rad.RapporterarPerEnhet)
                {
                    var anvValdaOrgenheterForDelreg = HamtaAnvandarensValdaEnheterForDelreg(userId, rad.Id);
                    foreach (var valdOrgenhet in anvValdaOrgenheterForDelreg)
                    {
                        historikForAnvandareList.AddRange(aktuellaLeveranser.Where(x => x.OrganisationsenhetsId == valdOrgenhet.Id).ToList());
                    }
                }
                else
                {
                    historikForAnvandareList.AddRange(aktuellaLeveranser);
                }
            }
            return historikForAnvandareList.OrderByDescending(x => x.Leveranstidpunkt).ToList();
        }

        private List<FilloggDetaljDTO> SkapaHistorikrader(IEnumerable<Leverans> leveransList)
        {
            var historikLista = new List<FilloggDetaljDTO>();

            foreach (var leverans in leveransList)
            {
                //TODO
                if (leverans.Id == 37847)
                {
                    var x = 1;
                }
                var filloggDetalj = new FilloggDetaljDTO();
                var aterkopplingskontakt = "";
                //Kolla om återkopplingsfil finns för aktuell leverans
                var aterkoppling = _portalSosRepository.GetAterkopplingForLeverans(leverans.Id);

                //Kolla om enhetskod finns för aktuell leverans (stadsdelsleverans)
                var enhetskod = String.Empty;
                var orgenhetid = 0;
                if (leverans.OrganisationsenhetsId != null)
                {
                    orgenhetid = Convert.ToInt32(leverans.OrganisationsenhetsId);
                    enhetskod = _portalSosRepository.GetEnhetskodForLeverans(orgenhetid);
                }

                //Hämta period för aktuell leverans
                var period = _portalSosRepository.GetPeriodForAktuellLeverans(leverans.ForvantadleveransId);
                var filer = _portalSosRepository.GetFilerForLeveransId(leverans.Id).ToList();
                var sftpKonto = new SFTPkonto();
                var sftpkontoId = leverans.SFTPkontoId.GetValueOrDefault();
                if (sftpkontoId != 0)
                {
                    sftpKonto = _portalSosRepository.GetSFTPAccount(sftpkontoId);
                }
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
                    filloggDetalj.OrganisationsenhetsId = orgenhetid;
                    filloggDetalj.Period = period;
                    filloggDetalj.SFTPkontoNamn = sftpKonto.Kontonamn;
                    if (aterkoppling != null)
                    {
                        //filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus; //Skriv ej över leveransstatusen från återkopplingen. Beslut 20180912, ärende #128
                        filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                        //Kontaktperson
                        filloggDetalj.Aterkopplingskontakt = _portalSosRepository.GetUserEmail(leverans.ApplicationUserId);
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
                        filloggDetalj.OrganisationsenhetsId = orgenhetid;
                        filloggDetalj.Period = period;
                        filloggDetalj.SFTPkontoNamn = sftpKonto.Kontonamn;
                        if (aterkoppling != null)
                        {
                            filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus;
                            filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                            //Kontaktperson
                            filloggDetalj.Aterkopplingskontakt = _portalSosRepository.GetUserEmail(leverans.ApplicationUserId);
                        }
                        historikLista.Add(filloggDetalj);
                    }
                }
            }

            return historikLista;
        }


        private List<FildroppDetaljDTO> SkapaHistorikraderForDroppadeFiler(IEnumerable<DroppadFil> leveransList)
        {
            var historikLista = new List<FildroppDetaljDTO>();

            foreach (var leverans in leveransList)
            {
                var fildroppDetalj = new FildroppDetaljDTO();
                var arende = _portalSosRepository.GetArendeById(leverans.ArendeId);

                fildroppDetalj.Id = leverans.Id;
                fildroppDetalj.ArendeId= leverans.ArendeId;
                fildroppDetalj.Arendenummer = arende.Arendenr;
                fildroppDetalj.Arendenamn = arende.Arendenamn;
                fildroppDetalj.Filnamn = leverans.Filnamn;
                fildroppDetalj.NyttFilnamn = leverans.NyttFilnamn;
                fildroppDetalj.Kontaktperson = _portalSosRepository.GetUserById(leverans.ApplicationUserId).Email;
                fildroppDetalj.AndradDatum = leverans.AndradDatum;
                historikLista.Add(fildroppDetalj);
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

        public IEnumerable<AdmRegister> HamtaRegisterEjKoppladeTillSFTPKontoForOrg(int orgId)
        {
            var ledigaRegister = new List<AdmRegister>();
            var allaregForOrg = HamtaRegisterForOrg(orgId);
            var sftpKontonForOrg = HamtaSFTPkontonForOrg(orgId);

            foreach (var reg in allaregForOrg)
            {
                if (sftpKontonForOrg.Any())
                {
                    if (!sftpKontonForOrg.Select(x => x.RegisterId).Contains(reg.Id))
                    {
                        ledigaRegister.Add(reg);
                    }
                }
                else
                {
                    ledigaRegister.Add(reg);
                }

            }
            return ledigaRegister;
        }

        public IEnumerable<string> HamtaDelregistersPerioderForAr(int delregId, int ar)
        {
            var perioder = _portalSosRepository.GetSubDirectoysPeriodsForAYear(delregId, ar);
            return perioder;
        }

        public IEnumerable<string> HamtaDelregistersPerioderForAr(AdmDelregister delreg, int ar)
        {
            var dateFrom = new DateTime(ar, 01, 01);
            var dateTom = new DateTime(ar, 12, 31).Date;
            var periods = delreg.AdmForvantadleverans
                .Where(x => x.DelregisterId == delreg.Id && x.Uppgiftsstart >= dateFrom && x.Uppgiftsslut <= dateTom)
                .Select(x => x.Period).ToList();
            return periods;
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

        public List<int> HamtaValbaraAr(AdmDelregister delreg)
        {
            var arsLista = new List<int>();
            var uppgiftsstartLista = delreg.AdmForvantadleverans.Where(x => x.DelregisterId == delreg.Id)
                .Select(x => x.Uppgiftsstart).ToList();

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
        
        public IEnumerable<AdmForvantadleverans> HamtaForvLevForRegisterOchPerioder(List<AdmDelregister> delregList, List<string> perioder)
        {
            var allForvlevsForReg = new List<AdmForvantadleverans>();
            foreach (var delreg in delregList)
            {
                var forvlevs = delreg.AdmForvantadleverans.Where(x => perioder.Contains(x.Period)).ToList();
                allForvlevsForReg.AddRange(forvlevs);
            }
            return allForvlevsForReg;
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

        //public IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisationRegisterPeriodOLD(int orgId, List<AdmDelregister> delregisterList, string periodForReg)
        //{
        //    var historikLista = new List<FilloggDetaljDTO>();
        //    var sorteradHistorikLista = new List<FilloggDetaljDTO>();

        //    ////Använd procedur för att hämta datat på en gång istället för inne i loopen nedan. För att förbättra prestanda
        //    ////TODO - test. Ta bort
        //    //var delregIdList = new List<int>();
        //    //foreach (var delreg in delregisterList)
        //    //{
        //    //    delregIdList.Add(delreg.Id);
        //    //}
        //    //var tmp = _portalSosRepository.GetDeliveryStatusReport(orgId, delregIdList, periodForReg);

        //    foreach (var delregister in delregisterList)
        //    {
        //        //Forvantadleveransid för delregister och period
        //        var forvLevId = delregister.AdmForvantadleverans.Where(x => x.Period == periodForReg).Select(x => x.Id).SingleOrDefault();

        //        var senasteLeverans = new Leverans();
        //        //kan org rapportera per enhet för aktuellt delregister och period? => hämta senaste leverans per enhet
        //        var uppgiftsskyldighet = delregister.AdmUppgiftsskyldighet.SingleOrDefault(x => x.OrganisationId == orgId);
        //        if (uppgiftsskyldighet != null)
        //        {
        //            if (uppgiftsskyldighet.RapporterarPerEnhet)
        //            {
        //                var orgEnhetsList = _portalSosRepository.GetOrgUnitsForOrg(orgId);
        //                foreach (var orgenhet in orgEnhetsList)
        //                {
        //                    //
        //                    senasteLeverans =
        //                        _portalSosRepository.GetLatestDeliveryForOrganisationSubDirectoryPeriodAndOrgUnit(orgId, delregister.Id, forvLevId, orgenhet.Id);
        //                    if (senasteLeverans != null)
        //                    {
        //                        AddHistorikListItem(senasteLeverans, historikLista);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                senasteLeverans =
        //                    _portalSosRepository.GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(orgId, delregister.Id,
        //                        forvLevId);
        //                if (senasteLeverans != null)
        //                {
        //                    AddHistorikListItem(senasteLeverans, historikLista);
        //                }

        //            }
        //        }
                
        //    }
        //    if (historikLista.Count > 0)
        //    {
        //        sorteradHistorikLista = historikLista.OrderBy(x => x.Enhetskod).ThenBy(x => x.RegisterKortnamn).ThenByDescending(x => x.Id).ToList();
        //    }

        //    return sorteradHistorikLista;
        //}


        public IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisationRegisterPeriod(int orgId, List<AdmDelregister> delregisterList, string periodForReg, List<Leverans> levstatusRapportList)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            var sorteradHistorikLista = new List<FilloggDetaljDTO>();

            foreach (var delregister in delregisterList)
            {
                //Forvantadleveransid för delregister och period
                var forvLevId = delregister.AdmForvantadleverans.Where(x => x.Period == periodForReg).Select(x => x.Id).SingleOrDefault();

                var senasteLeverans = new Leverans();
                //kan org rapportera per enhet för aktuellt delregister och period? => hämta senaste leverans per enhet
                var uppgiftsskyldighet = delregister.AdmUppgiftsskyldighet.SingleOrDefault(x => x.OrganisationId == orgId && x.SkyldigTom == null || x.SkyldigTom > DateTime.Now);
                if (uppgiftsskyldighet != null)
                {
                    if (uppgiftsskyldighet.RapporterarPerEnhet)
                    {
                        var orgEnhetsList = _portalSosRepository.GetOrgUnitsForOrg(orgId);
                        foreach (var orgenhet in orgEnhetsList)
                        {
                            //
                            senasteLeverans = levstatusRapportList.Where(x => x.DelregisterId == delregister.Id && x.AdmForvantadleverans.Period == periodForReg && x.OrganisationsenhetsId == orgenhet.Id).OrderByDescending(x=> x.Id).FirstOrDefault();
                            if (senasteLeverans != null)
                            {
                                AddHistorikListItem(senasteLeverans, historikLista);
                            }
                        }
                    }
                    else
                    {
                        //senasteLeverans =
                        //    _portalSosRepository.GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(orgId, delregister.Id,
                        //        forvLevId);
                        senasteLeverans = levstatusRapportList.SingleOrDefault(x => x.DelregisterId == delregister.Id && x.AdmForvantadleverans.Period == periodForReg);
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

        public IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisationRegisterPeriodUser(int orgId, List<AdmDelregister> delregisterList, string periodForReg, List<Leverans> levstatusRapportList, string userId)
        {
            var historikLista = new List<FilloggDetaljDTO>();
            var sorteradHistorikLista = new List<FilloggDetaljDTO>();

            foreach (var delregister in delregisterList)
            {
                //Forvantadleveransid för delregister och period
                var forvLevId = delregister.AdmForvantadleverans.Where(x => x.Period == periodForReg).Select(x => x.Id).SingleOrDefault();

                var senasteLeverans = new Leverans();
                //kan org rapportera per enhet för aktuellt delregister och period? => hämta senaste leverans per enhet
                var uppgiftsskyldighet = delregister.AdmUppgiftsskyldighet.SingleOrDefault(x => x.OrganisationId == orgId && x.SkyldigTom == null || x.SkyldigTom > DateTime.Now);
                if (uppgiftsskyldighet != null)
                {
                    if (uppgiftsskyldighet.RapporterarPerEnhet)
                    {
                        var anvValdaOrgenheterForDelreg = HamtaAnvandarensValdaEnheterForDelreg(userId, delregister.Id);
                        //var orgEnhetsList = _portalSosRepository.GetOrgUnitsForOrg(orgId);
                        foreach (var valdOrgenhet in anvValdaOrgenheterForDelreg)
                        {
                            senasteLeverans = levstatusRapportList.Where(x => x.DelregisterId == delregister.Id && x.AdmForvantadleverans.Period == periodForReg && x.OrganisationsenhetsId == valdOrgenhet.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (senasteLeverans != null)
                            {
                                AddHistorikListItem(senasteLeverans, historikLista);
                            }
                        }
                    }
                    else
                    {
                        //senasteLeverans =
                        //    _portalSosRepository.GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(orgId, delregister.Id,
                        //        forvLevId);
                        senasteLeverans = levstatusRapportList.SingleOrDefault(x => x.DelregisterId == delregister.Id && x.AdmForvantadleverans.Period == periodForReg);
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
                if (rad.RegisterKortnamn == "EKB-Månad"){
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


                if (rad.Leveransstatus == "Leveransen är godkänd")
                {
                    ok = true;
                }
                else if (rad.Leveransstatus.Trim() == "Inget att rapportera" || rad.Leveransstatus == "Leveransen är godkänd med varningar")
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

        public string KontrolleraOmKomplettaEnhetsleveranser(int orgId, LeveransStatusDTO leveransStatusObj, List<AdmDelregister> delregisterList)
        {
            var status = leveransStatusObj.Status;
            var year = leveransStatusObj.Period.Substring(0, 4);
            var month = "01";
            if (leveransStatusObj.Period.Length == 6)
            {
                var monthTmp = leveransStatusObj.Period.Substring(4, 2);
                //TODO - fulfix för HSL halvårsrapp. Används ej??
                if (monthTmp == "H1") 
                {
                    month = "01";
                }
                else if (monthTmp == "H2")
                {
                    month = "07";
                }
                else
                {
                    month = leveransStatusObj.Period.Substring(4, 2);
                }
            }
            var periodDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);

            foreach (var delregister in delregisterList)
            {
                var uppgiftsskyldighet = delregister.AdmUppgiftsskyldighet.SingleOrDefault(x => x.OrganisationId == orgId);
                if (uppgiftsskyldighet != null)
                {
                    if (uppgiftsskyldighet.RapporterarPerEnhet && uppgiftsskyldighet.SkyldigFrom <= periodDate) //Rapporterar organisationen detta register per enhet?
                    {
                        if (uppgiftsskyldighet.SkyldigTom == null)
                        {
                            var orgEnhetsList = _portalSosRepository
                                .GetOrgUnitsByRepOblWithInPeriod(uppgiftsskyldighet.Id, leveransStatusObj.Period)
                                .ToList();
                            if (orgEnhetsList.Any()) //Kontrollera att alla oganisationsenheter har levererat fil
                            {
                                foreach (var orgenhet in orgEnhetsList)
                                {
                                    var xList = leveransStatusObj.HistorikLista.Where(hist =>hist.RegisterKortnamn == delregister.Kortnamn).ToList();
                                    if (xList.All(x => x.Enhetskod != orgenhet.Enhetskod))
                                    {
                                        status = "error";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return status;
        }

        public string KontrolleraOmKomplettaEnhetsleveranserForAnv(int orgId, LeveransStatusDTO leveransStatusObj, List<AdmDelregister> delregisterList, string userId)
        {
            var status = leveransStatusObj.Status;
            var year = leveransStatusObj.Period.Substring(0, 4);
            var month = "01";
            if (leveransStatusObj.Period.Length == 6)
            {
                var monthTmp = leveransStatusObj.Period.Substring(4, 2);
                //TODO - fulfix för HSL halvårsrapp. Används ej??
                if (monthTmp == "H1")
                {
                    month = "01";
                }
                else if (monthTmp == "H2")
                {
                    month = "07";
                }
                else
                {
                    month = leveransStatusObj.Period.Substring(4, 2);
                }
            }
            var periodDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);

            foreach (var delregister in delregisterList)
            {
                var uppgiftsskyldighet = delregister.AdmUppgiftsskyldighet.SingleOrDefault(x => x.OrganisationId == orgId);
                if (uppgiftsskyldighet != null)
                {
                    if (uppgiftsskyldighet.RapporterarPerEnhet && uppgiftsskyldighet.SkyldigFrom <= periodDate) //Rapporterar organisationen detta register per enhet?
                    {
                        if (uppgiftsskyldighet.SkyldigTom == null || uppgiftsskyldighet.SkyldigTom > DateTime.Now)
                        {
                            var orgEnhetsList = _portalSosRepository
                                .GetOrgUnitsByRepOblWithInPeriod(uppgiftsskyldighet.Id, leveransStatusObj.Period)
                                .ToList();
                            if (orgEnhetsList.Any()) //Kontrollera att alla användarens valda oganisationsenheter har levererat fil
                            {
                                var anvValdaOrgenheterForDelreg = HamtaAnvandarensValdaEnheterForDelreg(userId, delregister.Id);
                                if (anvValdaOrgenheterForDelreg != null)
                                {
                                    var orgenheterAttKolla = new List<Organisationsenhet>();
                                    foreach (var orgenhet in orgEnhetsList)
                                    {
                                        var match = anvValdaOrgenheterForDelreg.Where(x => x.Id == orgenhet.Id).SingleOrDefault();
                                        if (match != null)
                                        {
                                            orgenheterAttKolla.Add(match);
                                        }
                                    }
                                    foreach (var orgenhet in orgenheterAttKolla)
                                    {
                                        var xList = leveransStatusObj.HistorikLista.Where(hist => hist.RegisterKortnamn == delregister.Kortnamn).ToList();
                                        if (xList.All(x => x.Enhetskod != orgenhet.Enhetskod))
                                        {
                                            status = "error";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return status;
        }

        public void KopplaAstridAnvändareTillAstridRoll(string userName, string astridUserId, string rollId)
        {
            var userRole = new ApplicationUserRole
            {
                UserId = astridUserId,
                RoleId = rollId,
                SkapadDatum = DateTime.Now,
                SkapadAv = userName,
                AndradDatum = DateTime.Now,
                AndradAv = userName
            };
            _portalSosRepository.SetAstridRoleForAstridUser(userRole);
            //Om Ärendeansvarig ska info sparas i Filipdatbasen/Ärendeansvarigtabellen
            var role = _portalSosRepository.GetAstridRoleById(rollId);
            if (role.Name == "ArendeAdmin")
            {
                
            }
        }

        public void KopplaFilipAnvändareTillFilipRoll(string userName, string filipUserId, string rollId)
        {
            //Check if user already has role, then skip
            var userRoles = _portalSosRepository.GetFilipUserRolesForUser(filipUserId).Select(x => x.RoleId);

            if (!userRoles.Contains(rollId))
            {
                var userRole = new ApplicationUserRole
                {
                    UserId = filipUserId,
                    RoleId = rollId,
                    SkapadDatum = DateTime.Now,
                    SkapadAv = userName,
                    AndradDatum = DateTime.Now,
                    AndradAv = userName
                };
                _portalSosRepository.SetFilipRoleForFilipUser(userRole);
            }
        }

        public void KopplaFilipAnvändareTillFilipRollNamn(string userName, string filipUserId, string rollnamn)
        {
            var rollId = _portalSosRepository.GetFilipRoleByName(rollnamn).Id;
            KopplaFilipAnvändareTillFilipRoll(userName,filipUserId,rollId);
        }

        public void KopplaKontaktpersonTillArende(string userName, int arendeId, string kontaktId)
        {
            //check if already exists
            var caseContacts = _portalSosRepository.GetCaseContacts(arendeId).ToList();
            var caseContactExists = caseContacts.SingleOrDefault(x => x.ApplicationUserId == kontaktId);

            if (caseContactExists == null)
            {
                var caseContact = new ArendeKontaktperson()
                {
                    ArendeId = arendeId,
                    ApplicationUserId = kontaktId,
                    SkapadDatum = DateTime.Now,
                    SkapadAv = userName,
                    AndradDatum = DateTime.Now,
                    AndradAv = userName
                };
                _portalSosRepository.SetCaseContact(caseContact);
                //Sätt rollen "ArendeUpp" om inte redan satt
                var arendeRoleId = _portalSosRepository.GetFilipRoleByName("ArendeUpp").Id;
                var userRole = new ApplicationUserRole
                {
                    UserId = kontaktId,
                    RoleId = arendeRoleId,
                    SkapadDatum = DateTime.Now,
                    SkapadAv = userName,
                    AndradDatum = DateTime.Now,
                    AndradAv = userName
                };
                _portalSosRepository.SetFilipRoleForFilipUser(userRole);
            }
            
        }

        public void HanteraArendesEjReggadeKontaktpersoner(ArendeDTO arende, string userName)
        {
            var reporters = arende.Rapportorer.Replace(' ', ',');
            var newEmailStr = reporters.Split(',');
            //Rensa först alla ej reggade kontaktpersoner för ärendet
            _portalSosRepository.DeleteAllNotRegistredContactsForCase(arende.Id);

            //Spara nuvarande ej reggade kontaktpersoner för ärendet
            foreach (var email in newEmailStr)
            {
                if (!String.IsNullOrEmpty(email.Trim()))
                {
                    var preContact = new PreKontakt()
                    {
                        ArendeId = arende.Id,
                        Epostadress = email.Trim(),
                        SkapadDatum = DateTime.Now,
                        SkapadAv = userName,
                        AndradDatum = DateTime.Now,
                        AndradAv = userName
                    };
                    _portalSosRepository.CreatePreKontakt(preContact);
                }
            }
        }

        public void TaBortFilipRollForanvandare(string userName, string filipUserId, string rollId)
        {
            var userRoles =  _portalSosRepository.GetFilipUserRolesForUser(filipUserId).Select(x => x.RoleId).ToList();
            //Om användare har rollen ska den tas bort
            if (userRoles.Contains(rollId))
            {
                _portalSosRepository.DeleteRoleFromFilipUser(filipUserId, rollId);
            }
        }

        public void TaBortKontaktpersonFranArende(string userName, int arendeId, string kontaktId)
        {
            var caseContacts = _portalSosRepository.GetCaseContacts(arendeId).Select(x => x.ApplicationUserId).ToList();
            //Om ärendet har kontaktpersonen ska den tas bort
            if (caseContacts.Contains(kontaktId))
            {
                _portalSosRepository.DeleteContactFromCase(arendeId, kontaktId);
            }
        }

        public IEnumerable<ApplicationRole> HamtaAstridAnvandaresRoller(string userId)
        {
            var astridAnvRoller = _portalSosRepository.GetAstridUsersRoles(userId);
            return astridAnvRoller;
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
            var allaRegisterList = _portalSosRepository.GetAllActiveRegisterInformationForOrganisation(orgId).ToList();
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
            userRegisterList = HamtaOrgenheter(userRegisterList, orgId);
            //foreach (var item in userRegisterList)
            //{
            //    var uppgiftsskyldighet = HamtaUppgiftsskyldighetForOrganisationOchRegister(orgId, item.Id);
            //    if (uppgiftsskyldighet.RapporterarPerEnhet)
            //    {
            //        //Ger cirkulär reference, därav keyValuePair nedan
            //        //item.Orgenheter = orgUnits.ToList();
            //        item.Organisationsenheter = new List<KeyValuePair<string, string>>();
            //        item.Orgenheter = new List<KeyValuePair<string, string>>();

            //        item.RapporterarPerEnhet = true;
            //        var enhetsuppgiftsskyldighetList = _portalSosRepository.GetUnitReportObligationForReportObligation(uppgiftsskyldighet.Id);
            //        foreach (var enhetsuppgiftsskyldighet in enhetsuppgiftsskyldighetList)
            //        {
            //            var orgUnit = _portalSosRepository.GetOrganisationUnit(enhetsuppgiftsskyldighet.OrganisationsenhetsId);
            //            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Enhetsnamn);
            //            item.Organisationsenheter.Add(keyValuePair);
            //            KeyValuePair<string, string> keyValuePairFilkod = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Filkod);
            //            item.Orgenheter.Add(keyValuePairFilkod);
            //        }
            //    }
            //}

            return userRegisterList;
        }

        public List<Roll> HamtaValdaDelregisterRollForAnvandare(string userId)
        {
            var rollList = _portalSosRepository.GetChosenDelRegistersForUser(userId).ToList();
            return rollList;
        }

        public List<RegisterInfo> HamtaValdaDelregisterOchOrgenheterForAnvandare(string userId, int orgId)
        {
            var registerList = _portalSosRepository.GetChosenDelRegistersForUser(userId).ToList();
            //var allaRegisterList = _portalRepository.GetAllRegisterInformation();
            var allaRegisterList = _portalSosRepository.GetAllActiveRegisterInformationForOrganisation(orgId).ToList();
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
            userRegisterList = HamtaOrgenheterForAnvandare(userRegisterList, orgId, userId);

            return userRegisterList;
        }

        public AdmDelregister HamtaValtDelRegisterMedFilnamnsstart(string filnamnsStart)
        {
            throw new NotImplementedException();
        }

        public string HamtaForstaPeriod(List<string> periodsForRegister)
        {
            throw new NotImplementedException();
        }

        public List<RegisterInfo> HamtaRelevantaDelregisterForSFTPKonto(SFTPkonto sftpKonto)
        {
            var registerList = _portalSosRepository.GetSubDirectoriesForDirectory(sftpKonto.RegisterId).ToList();
            var allaRegisterList = _portalSosRepository.GetAllRegisterInformationForOrganisation(sftpKonto.OrganisationsId).ToList();
            var sftpKontoRegisterList = new List<RegisterInfo>();
            foreach (var register in allaRegisterList)
            {
                var finns = registerList.Find(r => r.Id == register.Id);
                if (finns != null)
                {
                    register.SelectedFilkrav = "0";
                    sftpKontoRegisterList.Add(register);
                }
            }
            sftpKontoRegisterList = HamtaOrgenheter(sftpKontoRegisterList, sftpKonto.OrganisationsId);

            return sftpKontoRegisterList;
        }

        public IEnumerable<RegisterInfo> HamtaRegistersMedAnvandaresVal(string userId, int orgId)
        {
            var registerList = _portalSosRepository.GetChosenDelRegistersForUser(userId);
            var allaRegisterList = _portalSosRepository.GetAllActiveRegisterInformationForOrganisation(orgId);

            foreach (var register in allaRegisterList)
            {
                //För enhetsrapportering
                if (RapporterarPerEnhet(register.Id, orgId))
                {
                    var availableOrgUnits = HamtaDelregistersAktuellaEnheter(register.Id, orgId).ToList();
                    register.OrgInfo = new List<OrgUnitInfo>();
                    foreach (var availableOrgUnit in availableOrgUnits)
                    {
                        var orgUnit = new OrgUnitInfo()
                        {
                            Id = availableOrgUnit.Id,
                            OrganisationsId = availableOrgUnit.OrganisationsId,
                            Enhetsnamn = availableOrgUnit.Enhetsnamn,
                            Enhetskod = availableOrgUnit.Enhetskod
                        };
                        register.OrgInfo.Add(orgUnit);
                    }
                }
                //Användarens valda register/orgenheter
                foreach (var userRegister in registerList)
                {
                    if (register.Id == userRegister.DelregisterId)
                    {
                        register.Selected = true;
                        register.RapporterarPerEnhet = RapporterarPerEnhet(register.Id, orgId);
                        if (RapporterarPerEnhet(register.Id, orgId))
                        {
                            var usersOrgUnits = HamtaAnvandarensValdaEnheterForDelreg(userId, userRegister.DelregisterId).ToList();
                            foreach (var orgiInfo in register.OrgInfo)
                            {
                                var chosenOrgUnit = usersOrgUnits.Where(x => x.Id == orgiInfo.Id);
                                if (chosenOrgUnit.Any())
                                {
                                    orgiInfo.Selected = true;
                                }
                            }
                        }
                    }
                }

            }

            return allaRegisterList;
        }

        public IEnumerable<AdmDelregister> HamtaAnvandarensValdaDelregister(string userId, int orgId)
        {
            var delregList = new List<AdmDelregister>();
            var rollList = _portalSosRepository.GetChosenDelRegistersForUser(userId);
            foreach (var roll in rollList)
            {
                var delreg = _portalSosRepository.GetSubDirectoryWithIncludes(roll.DelregisterId);
                delregList.Add(delreg);
            }
            return delregList;
        }

        public bool RapporterarPerEnhet(int delregId, int orgId)
        {
            var uppgskh = _portalSosRepository.GetActiveUppgiftsskyldighetForOrganisationAndRegister(orgId, delregId);
            if (uppgskh.RapporterarPerEnhet)
                return true;
            return false;
        }

        public IEnumerable<Organisationsenhet> HamtaDelregistersAktuellaEnheter(int delregId, int orgId)
        {
            var orgUnitsList = new List<Organisationsenhet>();
            var uppgskhList = _portalSosRepository.GetAllActiveReportObligationsForSubDirAndOrg(delregId, orgId);

            foreach (var uppgskh in uppgskhList)
            {
                if (uppgskh.RapporterarPerEnhet)
                {
                    orgUnitsList = _portalSosRepository.GetOrgUnitsByRepOblId(uppgskh.Id).ToList();
                }
            }
            return orgUnitsList;

        }

        public IEnumerable<Organisationsenhet> HamtaAnvandarensValdaEnheterForDelreg(string userId, int subdirId)
        {
            var chosenOrgUnitsForDir = new List<Organisationsenhet>();
            var roll = _portalSosRepository.GetRollForUserAndSubdir(userId, subdirId);
            foreach (var rollOrgenhet in roll.RollOrganisationsenhet)
            {
                var orgenhet = _portalSosRepository.GetOrganisationUnit(rollOrgenhet.OrganisationsenhetsId);
                chosenOrgUnitsForDir.Add(orgenhet);
            }
            return chosenOrgUnitsForDir;
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
        
        public ArendeAnsvarig HamtaArendeAnsvarig(int arendeAnsvId)
        {
            var arendeansvarig = _portalSosRepository.GetCaseResponsible(arendeAnsvId);
            return arendeansvarig;
        }

        public string HamtaArendesKontaktpersoner(int orgId, int arendeId)
        {
            string kontaktLista = "";
            var arendeKontaktIdList = _portalSosRepository.GetCaseRegisteredContactIds(arendeId);
            foreach (var kontaktId in arendeKontaktIdList)
            {
                var epostadress = _portalSosRepository.GetUserEmail(kontaktId);
                if (String.IsNullOrEmpty(kontaktLista))
                {
                    kontaktLista = epostadress;
                }
                else
                {
                    kontaktLista = kontaktLista + ", " + epostadress;
                }
            }
            return kontaktLista;
        }

        public IEnumerable<ArendeKontaktperson> HamtaArendesKontaktpersoner(int arendeid)
        {
            var kontakter = _portalSosRepository.GetCaseContacts(arendeid);
            return kontakter;
        }

        public string HamtaArendesEjRegistreradeKontaktpersoner(int orgId, int arendeId)
        {
            string kontaktLista = "";
            var arendeKontaktList = _portalSosRepository.GetCaseNotRegisteredContact(arendeId);
            foreach (var kontakt in arendeKontaktList)
            {
                if (String.IsNullOrEmpty(kontaktLista))
                {
                    kontaktLista = kontakt.Epostadress;
                }
                else
                {
                    kontaktLista = kontaktLista + ", " + kontakt.Epostadress;
                }
            }
            return kontaktLista;
        }

        public IEnumerable<Leverans> HamtaLeveransStatusRapporterForOrgDelregPerioder(int orgId, List<AdmDelregister> delregisterList, List<string> periodsForRegister)
        {
            
            var delregIdList = new List<int>();
            var levStatusRappList = new List<Leverans>();

            foreach (var delreg in delregisterList)
            {
                delregIdList.Add(delreg.Id);
            }

            var forvlevIdList = _portalSosRepository.GetExpectedDeliveriesForSubdirsAndPeriods(delregIdList, periodsForRegister).Select(x => x.Id).ToList();

            if (forvlevIdList.Count != 0)
            {
                levStatusRappList = _portalSosRepository.GetDeliveryStatusReport(orgId, delregIdList, forvlevIdList).ToList();
            }

            return levStatusRappList;
        }

        public IEnumerable<Leverans> HamtaLeveransStatusRapporterForOrgDelregPerioderUser(int orgId, List<AdmDelregister> delregisterList, List<string> periodsForRegister, string userId)
        {

            var delregIdList = new List<int>();
            var levStatusRappList = new List<Leverans>();

            foreach (var delreg in delregisterList)
            {
                delregIdList.Add(delreg.Id);
            }

            var forvlevIdList = _portalSosRepository.GetExpectedDeliveriesForSubdirsAndPeriods(delregIdList, periodsForRegister).Select(x => x.Id).ToList();

            if (forvlevIdList.Count != 0)
            {
                levStatusRappList = _portalSosRepository.GetDeliveryStatusReport(orgId, delregIdList, forvlevIdList).ToList();
            }

            return levStatusRappList;
        }

        public AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrganisationOchRegister(int orgId, int delregid)
        {
            var uppgiftsskyldighet = _portalSosRepository.GetUppgiftsskyldighetForOrganisationAndRegister(orgId, delregid);

            return uppgiftsskyldighet;
        }

        public void InaktiveraKontaktperson(string userId)
        {
            _portalSosRepository.DisableContact(userId);
            //Remove users chosen registers and chosen orgUnits per register
            _portalSosRepository.DeleteChosenSubDirectoriesForUser(userId);
        }

        public IEnumerable<AdmOrganisationstyp> HamtaAllaValbaraOrganisationstyperForDelregister(int subdirId)
        {
            var resList = new List<AdmOrganisationstyp>();
            var allOrgtypesList = _portalSosRepository.GetAllOrgTypes();
            var activesList = _portalSosRepository.GetAllActiveReportObligationsForSubDir(subdirId);

            foreach (var orgtype in allOrgtypesList)
            {
                var exists = activesList.Where(x => x.OrganisationstypId == orgtype.Id).ToList();
                if (!exists.Any())
                {
                   resList.Add(orgtype); 
                }
            }
            return resList;
        }

        public void SkapaFilipRoll(ApplicationRole filipRoll, string userName)
        {
            //Sätt datum och användare
            filipRoll.SkapadDatum = DateTime.Now;
            filipRoll.SkapadAv = userName;
            filipRoll.AndradDatum = DateTime.Now;
            filipRoll.AndradAv = userName;
            _portalSosRepository.CreateFilipRole(filipRoll);
        }

        public int SkapaOrganisation(Organisation org, ICollection<Organisationstyp> orgtyperForOrg, string userName)
        {
            //Sätt datum och användare
            org.SkapadDatum = DateTime.Now;
            org.SkapadAv = userName;
            org.AndradDatum = DateTime.Now;
            org.AndradAv = userName;

            var orgId = _portalSosRepository.CreateOrganisation(org, orgtyperForOrg);
            //Check and set reportobligation depending on orgtypes
            foreach (var orgtyp in orgtyperForOrg)
            {
                var admUppgskhetOrgtyper = _portalSosRepository.GetReportObligationForOrgtype(orgtyp.OrganisationstypId);
                foreach (var admUppgskhetOrgtyp in admUppgskhetOrgtyper)
                {
                    var orguppgskheter = _portalSosRepository.GetAllReportObligationsForSubDirAndOrg(admUppgskhetOrgtyp.DelregisterId, org.Id);
                    var exists = false;
                    //If alreday exists, do nothing. Else - create
                    foreach (var orguppgskhet in orguppgskheter)
                    {
                        if (orguppgskhet.SkyldigTom == null)
                        {
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        var newUppgskhet = new AdmUppgiftsskyldighet
                        {
                         OrganisationId =  org.Id,
                         DelregisterId = admUppgskhetOrgtyp.DelregisterId,
                         SkyldigFrom = DateTime.Now
                        };
                        SkapaUppgiftsskyldighet(newUppgskhet,userName);
                    }
                }
            }

            return orgId;
        }

        public int SkapaSFTPkonto(SFTPkonto account, ICollection<KontaktpersonSFTPkonto> contacts, string userName)
        {
            //Sätt datum och användare
            account.SkapadDatum = DateTime.Now;
            account.SkapadAv = userName;
            account.AndradDatum = DateTime.Now;
            account.AndradAv = userName;

            var accountId = _portalSosRepository.CreateSFTPAccount(account, contacts);
            return accountId;
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

        public void SkapaPrivatEpostadress(UndantagEpostadress privEmail, string userName)
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
            var arendeDb = ConvertArendeDTOToDb(arende);
            //Sätt datum och användare
            arendeDb.SkapadDatum = DateTime.Now;
            arendeDb.SkapadAv = userName;
            arendeDb.AndradDatum = DateTime.Now;
            arendeDb.AndradAv = userName;

            arende.Id = _portalSosRepository.CreateCase(arendeDb);

            //Hantera reggade rapportörer för ärendet
            HandleRegisteredCaseReporters(arende, userName);

            //Hantera oreggade rapportörer för ärendet
            if (arende.Rapportorer != null)
            {
                HandleUnRegisteredCaseReporters(arende, userName);
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

        public void SkapaArendetyp(Arendetyp arendetyp, string userName)
        {
            //Sätt datum och användare
            arendetyp.SkapadDatum = DateTime.Now;
            arendetyp.SkapadAv = userName;
            arendetyp.AndradDatum = DateTime.Now;
            arendetyp.AndradAv = userName;

            _portalSosRepository.CreateCaseType(arendetyp);
        }

        public void SkapaArendeAnsvarig(ArendeAnsvarig arendeAnsvarig, string userName)
        {
            //Sätt datum och användare
            arendeAnsvarig.SkapadDatum = DateTime.Now;
            arendeAnsvarig.SkapadAv = userName;
            arendeAnsvarig.AndradDatum = DateTime.Now;
            arendeAnsvarig.AndradAv = userName;

            _portalSosRepository.CreateCaseManager(arendeAnsvarig);
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

        public void SkapaUppgiftsskyldighetOrgtyp(AdmUppgiftsskyldighetOrganisationstyp uppgSk, string userName)
        {
            //Sätt datum och användare
            uppgSk.SkapadDatum = DateTime.Now;
            uppgSk.SkapadAv = userName;
            uppgSk.AndradDatum = DateTime.Now;
            uppgSk.AndradAv = userName;
            _portalSosRepository.CreateReportObligationForOrgtype(uppgSk);
            //Alla org som har aktuell orgtyp ska få uppgiftsskyldighet för valt delregister
            var uppgSkDTO = ConvertUppgskOrgtypToDTO(uppgSk);
            UppdateraUppgiftsskyldighetForOrganisationer(uppgSkDTO, userName);
        }

        public int SkapaEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSk, string userName)
        {
            //Sätt datum och användare
            enhetsUppgSk.SkapadDatum = DateTime.Now;
            enhetsUppgSk.SkapadAv = userName;
            enhetsUppgSk.AndradDatum = DateTime.Now;
            enhetsUppgSk.AndradAv = userName;
            _portalSosRepository.CreateUnitReportObligation(enhetsUppgSk);
            return enhetsUppgSk.Id;
        }

        public void KopplaOrganisationsensKPTillEnheterna(int orgId, int orgUnitId, int subdirId, string userName)
        {
            var contacts = _portalSosRepository.GetContactPersonsForOrg(orgId);
            foreach (var contact in contacts)
            {
                var rollList = _portalSosRepository.GetChosenDelRegistersForUser(contact.Id).ToList();
                if (rollList.Any())
                {
                    foreach (var roll in rollList)
                    {
                       if (roll.DelregisterId == subdirId)
                        {
                            var exists = _portalSosRepository.GetRollOrganisationsenhet(roll.Id);
                            if (exists == null)
                            {
                                var rollOrgEnhet = new RollOrganisationsenhet
                                {
                                    RollId = roll.Id,
                                    OrganisationsenhetsId = orgUnitId,
                                    SkapadDatum = DateTime.Now,
                                    SkapadAv = userName
                                };
                                _portalSosRepository.CreateRollOrganisationsenhet(rollOrgEnhet);
                            }
                        }
                    }
                }
            }

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

        public void SkapaForeskrift(AdmForeskrift foreskrift, string userName)
        {
            //Sätt datum och användare
            foreskrift.SkapadDatum = DateTime.Now;
            foreskrift.SkapadAv = userName;
            foreskrift.AndradDatum = DateTime.Now;
            foreskrift.AndradAv = userName;
            _portalSosRepository.CreateRegulation(foreskrift);
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

        public void UppdateraAstridRoll(ApplicationRole role, string userName)
        {
            role.AndradDatum = DateTime.Now;
            role.AndradAv = userName;
            _portalSosRepository.UpdateAstridRole(role);
        }

        public void UppdateraFilipRoll(ApplicationRole role, string userName)
        {
            role.AndradDatum = DateTime.Now;
            role.AndradAv = userName;
            _portalSosRepository.UpdateFilipRole(role);
        }

        public void UppdateraOrganisation(Organisation org, string userName)
        {
            //Sätt datum och användare
            org.AndradDatum = DateTime.Now;
            org.AndradAv = userName;
            _portalSosRepository.UpdateOrganisation(org);

            var notIncludedOrgtypes = new List<AdmOrganisationstyp>();
            var allOrgtypes = _portalSosRepository.GetAllOrgTypes();
            foreach (var orgtype in allOrgtypes)
            {
                if (org.Organisationstyp.All(i => i.OrganisationstypId != orgtype.Id))
                {
                    notIncludedOrgtypes.Add(orgtype);
                }
            }
            //set closingdate on reportobligations on other orgtypes subdirs and disconnect users from that subdir
            foreach (var orgtype in notIncludedOrgtypes)
            {
                //Check if open reportobligations exists on other orgtypes
                var aktivaUppgskhOrgtyp = _portalSosRepository.GetActiveReportObligationForOrgtype(orgtype.Id);
                foreach (var orgtyp in aktivaUppgskhOrgtyp)
                {
                    var aktivUppgskhNotIncludedOrgtype = _portalSosRepository.GetActiveReportObligationInformationForOrgAndSubDir(org.Id, orgtyp.DelregisterId);
                    //if not activeReportObligation belongs to includedOrgtype
                    var exceptionList = new List<AdmUppgiftsskyldighet>();
                    foreach (var includedOrgtyp in org.Organisationstyp)
                    {
                        var aktivaUppgskhValdOrgtypList = _portalSosRepository.GetActiveReportObligationForOrgtype(includedOrgtyp.OrganisationstypId);
                        foreach (var aktivaUppgskhValdOrgtyp in aktivaUppgskhValdOrgtypList)
                        {
                            var aktivUppgskhValdOrgtyp = _portalSosRepository.GetActiveReportObligationInformationForOrgAndSubDir(org.Id, aktivaUppgskhValdOrgtyp.DelregisterId);

                            if (aktivUppgskhValdOrgtyp != null)
                            {
                                exceptionList.Add(aktivUppgskhValdOrgtyp);
                            }
                        }
                    }
                    
                    if (aktivUppgskhNotIncludedOrgtype != null)
                    {
                        //Om org är uppgiftsskyldigt för aktuellt delregister via annan (vald) orgtyp, sätt ej tom-datum
                        var matches = exceptionList.Where(p => p.Id == aktivUppgskhNotIncludedOrgtype.Id);
                        if (!matches.Any())
                        {
                            aktivUppgskhNotIncludedOrgtype.SkyldigTom = DateTime.Now;
                            aktivUppgskhNotIncludedOrgtype.AndradAv = userName;
                            _portalSosRepository.UpdateReportObligation(aktivUppgskhNotIncludedOrgtype);
                            UppdateraRollForKontaktpersoner(org.Id, orgtyp.DelregisterId);
                            //enhetsrapportering?
                            if (aktivUppgskhNotIncludedOrgtype.RapporterarPerEnhet)
                            {
                                var aktivEnhetsUppgskhList = _portalSosRepository.GetActiveUnitReportObligationForReportObligation(aktivUppgskhNotIncludedOrgtype.Id).ToList();
                                if (aktivEnhetsUppgskhList.Any())
                                {
                                    foreach (var aktivEnhetsUppgskh in aktivEnhetsUppgskhList)
                                    {
                                        aktivEnhetsUppgskh.SkyldigTom = DateTime.Now;
                                        aktivEnhetsUppgskh.AndradAv = userName;
                                        _portalSosRepository.UpdateUnitReportObligation(aktivEnhetsUppgskh);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Check and set reportobligation depending on orgtypes
            foreach (var orgtyp in org.Organisationstyp)
            {
                var aktivaAdmUppgskhetOrgtyper = _portalSosRepository.GetActiveReportObligationForOrgtype(orgtyp.OrganisationstypId);
                foreach (var admUppgskhetOrgtyp in aktivaAdmUppgskhetOrgtyper)
                {
                    var orguppgskheter = _portalSosRepository.GetAllActiveReportObligationsForSubDirAndOrg(admUppgskhetOrgtyp.DelregisterId, org.Id);
                    if (!orguppgskheter.Any())
                    {
                        var newUppgskhet = new AdmUppgiftsskyldighet
                        {
                            OrganisationId = org.Id,
                            DelregisterId = admUppgskhetOrgtyp.DelregisterId,
                            SkyldigFrom = DateTime.Now
                        };
                        SkapaUppgiftsskyldighet(newUppgskhet, userName);
                    }
                }
            }

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

        public void UppdateraSFTPKonto(SFTPkonto account, string userName)
        {
            account.AndradDatum = DateTime.Now;
            account.AndradAv = userName;
            _portalSosRepository.UpdateSFTPAccount(account);
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

        public void UppdateraArendetyp(Arendetyp caseType, string userName)
        {
            //Sätt datum och användare
            caseType.AndradDatum = DateTime.Now;
            caseType.AndradAv = userName;
            _portalSosRepository.UpdateCaseType(caseType);
        }

        public void UppdateraArendeAnsvarig(ArendeAnsvarig caseManager, string userName)
        {
            //Sätt datum och användare
            caseManager.AndradDatum = DateTime.Now;
            caseManager.AndradAv = userName;
            _portalSosRepository.UpdateCaseManager(caseManager);
        }

        public void UppdateraUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSkyldighet, string userName)
        {
            //Sätt datum och användare
            uppgSkyldighet.AndradDatum = DateTime.Now;
            uppgSkyldighet.AndradAv = userName;
            _portalSosRepository.UpdateReportObligation(uppgSkyldighet);
        }

        public void UppdateraUndantagForvantadFil(List<UndantagForvantadfilDTO> undantagList, string userName)
        {
            foreach (var undantagForvFil in undantagList)
            {
                var undantagDb = _portalSosRepository.GetExceptionExpectedFile(undantagForvFil.OrganisationsId, undantagForvFil.DelregisterId, undantagForvFil.ForvantadfilId);

                if (undantagForvFil.Selected)
                {
                    //Lägg till i undantagstabellen om inte redan finns
                    if (undantagDb == null)
                    {
                        var exceptionDb = new UndantagForvantadfil
                        {
                            OrganisationsId = undantagForvFil.OrganisationsId,
                            DelregisterId = undantagForvFil.DelregisterId,
                            ForvantadfilId = undantagForvFil.ForvantadfilId,
                            SkapadAv = userName,
                            SkapadDatum = DateTime.Now,
                            AndradAv = userName,
                            AndradDatum = DateTime.Now
                        };
                        _portalSosRepository.SaveExceptionExpectedFile(exceptionDb);
                    }
                }
                else if (!undantagForvFil.Selected && undantagDb != null)
                {
                    //Ta bort från undantagslistan om finns
                    _portalSosRepository.DeleteExceptionExpectedFile(undantagForvFil.OrganisationsId, undantagForvFil.DelregisterId, undantagForvFil.ForvantadfilId);

                }
            }
        }

        public void UppdateraValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> registerList)
        {
            _portalSosRepository.UpdateChosenRegistersForUser(userId, userName, registerList);
        }

        public void UppdateraValdaOrganisationsenheterForAnvandareOchDelreg(string userId, string userName, List<OrganisationsenhetDTO> orgenhetsList, int subdirId, int orgId)
        {
            var availableOrgUnits = HamtaDelregistersAktuellaEnheter(subdirId, orgId).ToList();
            var orgenhetIdList = new List<int>();
            var roll = _portalSosRepository.GetRollForUserAndSubdir(userId, subdirId);
            foreach (var orgenhet in orgenhetsList)
            {
                if (orgenhet.Selected)
                {
                    orgenhetIdList.Add(orgenhet.Id);
                }
            }
            _portalSosRepository.UpdateChosenOrgUnitsForUserAndSubdir(userId, userName, orgenhetIdList, roll.Id, availableOrgUnits);
        }

        public void UppdateraEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSkyldighet, string userName)
        {
            //Sätt datum och användare
            enhetsUppgSkyldighet.AndradDatum = DateTime.Now;
            enhetsUppgSkyldighet.AndradAv = userName;
            _portalSosRepository.UpdateUnitReportObligation(enhetsUppgSkyldighet);
        }

        public void UppdateraUppgiftsskyldighetOrganisationstyp(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtypes,List<OrganisationstypDTO> listOfOrgtypes, string userName)
        {
            foreach (var organisationstyp in listOfOrgtypes)
            {
                subdirOrgtypes.OrganisationstypId = organisationstyp.Organisationstypid;
                var subdirOrgtypeDb = _portalSosRepository.GetReportObligationForSubDirAndOrgtype(subdirOrgtypes.DelregisterId, organisationstyp.Organisationstypid, subdirOrgtypes.Id);
                if (organisationstyp.Selected)
                {
                    subdirOrgtypes.AndradAv = userName;
                    subdirOrgtypes.AndradDatum = DateTime.Now;

                    //check if already exists, otherwise insert into db
                    if (subdirOrgtypeDb == null)
                    {
                        subdirOrgtypes.SkapadAv = userName;
                        subdirOrgtypes.SkapadDatum = DateTime.Now;
                        _portalSosRepository.CreateSubdirReportObligation(subdirOrgtypes);
                    }
                    else //Update
                    {
                        _portalSosRepository.UpdateSubdirReportObligation(subdirOrgtypes);

                    }
                }
                else
                {
                    //Check if exists, if so - delete
                    if (subdirOrgtypeDb != null)
                    {
                        _portalSosRepository.DeleteSubdirReportObligation(subdirOrgtypes);
                    }
                }
            }
        }

        public void UppdateraUppgiftsskyldighetForOrganisationer(AdmUppgiftsskyldighetOrganisationstypDTO subdirOrgtype,string userName)
        {
            var orgsWithCurrOrgType = _portalSosRepository.GetOrgByOrgtype(subdirOrgtype.OrganisationstypId);

            foreach (var org in orgsWithCurrOrgType)
            {
                //check if already exists (update), otherwise insert into db
                var reportObligationDb = _portalSosRepository.GetActiveReportObligationInformationForOrgAndSubDir(org.Id, subdirOrgtype.DelregisterId);

                if (reportObligationDb == null) //Create
                {
                    reportObligationDb = new AdmUppgiftsskyldighet();
                    reportObligationDb.OrganisationId = org.Id;
                    reportObligationDb.DelregisterId = subdirOrgtype.DelregisterId;
                    reportObligationDb.SkyldigFrom = subdirOrgtype.SkyldigFrom;
                    reportObligationDb.SkyldigTom = subdirOrgtype.SkyldigTom;
                    reportObligationDb.AndradAv = userName;
                    reportObligationDb.AndradDatum = DateTime.Now;
                    reportObligationDb.SkapadAv = userName;
                    reportObligationDb.SkapadDatum = DateTime.Now;

                    _portalSosRepository.CreateReportObligation(reportObligationDb);
                }
                else if (reportObligationDb != null) //Update
                {
                    reportObligationDb.SkyldigTom = subdirOrgtype.SkyldigFrom;
                    reportObligationDb.SkyldigTom = subdirOrgtype.SkyldigTom;
                    reportObligationDb.AndradAv = userName;
                    reportObligationDb.AndradDatum = DateTime.Now;
                    _portalSosRepository.UpdateReportObligation(reportObligationDb); 
                    //Enhetsrapportering?
                    if (reportObligationDb.RapporterarPerEnhet)
                    {
                        var unitReportObligations = _portalSosRepository.GetActiveUnitReportObligationForReportObligation(reportObligationDb.Id);
                        foreach (var unitReportObligationDB in unitReportObligations)
                        {
                            unitReportObligationDB.SkyldigFrom = subdirOrgtype.SkyldigFrom;
                            unitReportObligationDB.SkyldigTom = subdirOrgtype.SkyldigTom;
                            UppdateraEnhetsUppgiftsskyldighet(unitReportObligationDB, userName);
                        }
                    }
                    if (subdirOrgtype.SkyldigTom != null)
                    {
                        if (subdirOrgtype.SkyldigTom > DateTime.Now)
                        {
                            UppdateraRollForKontaktpersoner(org.Id, subdirOrgtype.DelregisterId);
                        }
                    }
                }
            }
        }

        public void UppdateraRollForKontaktpersoner(int orgId, int delregId)
        {
            var contacts = _portalSosRepository.GetContactPersonsForOrg(orgId);
            foreach (var contact in contacts)
            {
                var rollList = _portalSosRepository.GetChosenDelRegistersForUser(contact.Id);
                if (rollList.Any())
                {
                    foreach (var roll in rollList)
                    {
                        if (roll.DelregisterId == delregId)
                        {
                            _portalSosRepository.DeleteChosenSubDirectoriesForUser(contact.Id);
                        }
                    }
                }
            }
        }

        public void UppdateraUppgiftsskyldighetOrganisationstyp(AdmUppgiftsskyldighetOrganisationstypDTO subdirOrgtype, string userName)
        {
            //check if already exists (update or delete), otherwise insert into db
            var subdirOrgtypeDb = _portalSosRepository.GetReportObligationForSubDirAndOrgtype(subdirOrgtype.DelregisterId, subdirOrgtype.OrganisationstypId, subdirOrgtype.Id);
               
            if (subdirOrgtypeDb == null) //Create
            {
                subdirOrgtypeDb = new AdmUppgiftsskyldighetOrganisationstyp();
                subdirOrgtypeDb.DelregisterId = subdirOrgtype.DelregisterId;
                subdirOrgtypeDb.OrganisationstypId = subdirOrgtype.Id;
                subdirOrgtypeDb.SkyldigFrom = subdirOrgtype.SkyldigFrom;
                subdirOrgtypeDb.SkyldigTom = subdirOrgtype.SkyldigTom;
                subdirOrgtypeDb.AndradAv = userName;
                subdirOrgtypeDb.AndradDatum = DateTime.Now;
                subdirOrgtypeDb.SkapadAv = userName;
                subdirOrgtypeDb.SkapadDatum = DateTime.Now;

                 _portalSosRepository.CreateSubdirReportObligation(subdirOrgtypeDb);
            }
            else if (subdirOrgtypeDb != null) //Update
            {
                subdirOrgtypeDb.SkyldigFrom = subdirOrgtype.SkyldigFrom;
                subdirOrgtypeDb.SkyldigTom = subdirOrgtype.SkyldigTom;
                subdirOrgtypeDb.AndradAv = userName;
                subdirOrgtypeDb.AndradDatum = DateTime.Now;
                _portalSosRepository.UpdateSubdirReportObligation(subdirOrgtypeDb);
            }

            UppdateraUppgiftsskyldighetForOrganisationer(subdirOrgtype, userName);
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

        public void UppdateraForeskrift(AdmForeskrift foreskrift, string userName)
        {
            foreskrift.AndradAv = userName;
            foreskrift.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateRegulation(foreskrift);

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

        public void UppdateraPrivatEpostAdress(UndantagEpostadress privEpostDoman, string userName)
        {
            privEpostDoman.AndradAv = userName;
            privEpostDoman.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdatePrivateEmail(privEpostDoman);
        }

        public void UppdateraArende(ArendeDTO arende,  string userName)
        {
            var arendeDb = ConvertArendeDTOToDb(arende);
            arendeDb.AndradAv = userName;
            arendeDb.AndradDatum = DateTime.Now;
            _portalSosRepository.UpdateCase(arendeDb);

            
            try
            {
                //Lägg till/ta bort arendets registrerade kontaktpersoner
                foreach (var contact in arende.Kontaktpersoner)
                {
                    if (contact.Selected)
                    {
                        KopplaKontaktpersonTillArende(userName, arende.Id, contact.Id);
                    }
                    else
                    {
                        TaBortKontaktpersonFranArende(userName, arende.Id, contact.Id);
                    }
                }
                //Lägg till/ta bort ej registrerade kontaktpersoner
                HanteraArendesEjReggadeKontaktpersoner(arende, userName);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
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

        public void SparaTillDatabasFillogg(string userName, string ursprungligtFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber)
        {
            _portalSosRepository.SaveToFilelogg(userName, ursprungligtFilNamn, nyttFilNamn, leveransId, sequenceNumber);
        }

        public void SparaValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> regIdList)
        {
            _portalSosRepository.SaveChosenRegistersForUser(userId, userName, regIdList);
        }

        public void SparaRollOrganisationsenhet(RollOrganisationsenhet rollOrgenhet)
        {
            _portalSosRepository.CreateRollOrganisationsenhet(rollOrgenhet);
        }

        public void SaveToLoginLog(string userid, string userName)
        {
            _portalSosRepository.SaveToLoginLog(userid, userName);
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

                //Get Email for users for current subdir and org
                if (String.IsNullOrEmpty(rappResRadVM.Email))
                {
                    var contacts = _portalSosRepository.GetContactPersonsForOrg(rappResRadVM.OrganisationsId);
                    foreach (var contact in contacts)
                    {
                        var subDirs = _portalSosRepository.GetChosenDelRegistersForUser(contact.Id);
                        foreach (var subDir in subDirs)
                        {
                            if (subDir.DelregisterId == rappResRadVM.DelregisterId)
                            {
                                if (String.IsNullOrEmpty(rappResRadVM.Email))
                                {
                                    rappResRadVM.Email = contact.Email;
                                }
                                else
                                {
                                    rappResRadVM.Email = rappResRadVM.Email + ", " +  contact.Email;
                                }
                            }
                        }
                    }
                }
                

                //If user is inactive, remove emailadress from list
                if (!String.IsNullOrEmpty(rappResRadVM.Email))
                {
                    var newEmailList = "";
                    //Check if more than one emailadress
                    if (rappResRadVM.Email.IndexOf(",", StringComparison.Ordinal) > 0)
                    {
                        //Fler epostadresser finns för raden, splitta
                        var newEmailStr = rappResRadVM.Email.Split(',');
                        foreach (var email in newEmailStr)
                        {
                            if (UserIsActive(email))
                            {
                                newEmailList = AddToEmailList(email, newEmailList);
                            }
                        }
                    }
                    else //Bara en epostadress
                    {
                        if (UserIsActive(rappResRadVM.Email))
                        {
                            newEmailList = AddToEmailList(rappResRadVM.Email, newEmailList);
                        }
                    }
                    rappResRadVM.Email = newEmailList;
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

        private bool UserIsActive(string email)
        {
            var active = false;
            var user = _portalSosRepository.GetUserByEmail(email.Trim());
            if (user.AktivTom == null || user.AktivTom > DateTime.Now.Date)
            {
                active = true;
            }
            return active;
        }

        private string AddToEmailList(string email, string emailList)
        {
            if (!String.IsNullOrEmpty(emailList))
            {
                emailList = emailList + ", " + email.Trim();
            }
            else
            {
                emailList = email.Trim();
            }
            return emailList;
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
                ArendeansvarId = arendeDto.ArendeanvsarId,
                Aktiv = arendeDto.Aktiv
            };
            return arende;
        }

        private List<AdmForvantadfilDTO> ConvertForvFiltoDTO(List<AdmForvantadfil> forvantadFilList)
        {
            var dtoList = new List<AdmForvantadfilDTO>();
            foreach (var forvantadFil in forvantadFilList)
            {
                var forvFilDTO = new AdmForvantadfilDTO
                {
                    Id = forvantadFil.Id,
                    FilkravId = forvantadFil.FilkravId,
                    DelregisterId = _portalSosRepository.GetFileRequirementById(forvantadFil.FilkravId).DelregisterId,
                    ForeskriftsId = forvantadFil.ForeskriftsId,
                    Filmask = forvantadFil.Filmask
                };
                dtoList.Add(forvFilDTO);
            }
           
            return dtoList;
        }

        private AdmUppgiftsskyldighetOrganisationstypDTO ConvertUppgskOrgtypToDTO(AdmUppgiftsskyldighetOrganisationstyp uppgSk)
        {
            var uppgSkDTO = new AdmUppgiftsskyldighetOrganisationstypDTO
            {
                Id = uppgSk.Id,
                DelregisterId = uppgSk.DelregisterId,
                OrganisationstypId = uppgSk.OrganisationstypId,
                SkyldigTom = uppgSk.SkyldigTom
            };

            if (uppgSk.SkyldigFrom != null)
            {
                uppgSkDTO.SkyldigFrom = uppgSk.SkyldigFrom;
            }
        return uppgSkDTO;
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

        private List<FilloggDetaljDTO> AddHistorikListItemOLD(Leverans senasteLeverans, List<FilloggDetaljDTO> historikLista)
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


        private List<FilloggDetaljDTO> AddHistorikListItem(Leverans senasteLeverans, List<FilloggDetaljDTO> historikLista)
        {
            var filloggDetalj = new FilloggDetaljDTO();
            //Kolla om återkopplingsfil finns för aktuell leverans
            var aterkoppling = _portalSosRepository.GetAterkopplingForLeverans(senasteLeverans.Id);

            var filer = _portalSosRepository.GetFilerForLeveransId(senasteLeverans.Id).ToList();
            var registerKortnamn = _portalSosRepository.GetSubDirectoryShortName(senasteLeverans.DelregisterId);
            var sftpKonto = new SFTPkonto();
            var sftpkontoId = senasteLeverans.SFTPkontoId.GetValueOrDefault();
            if (sftpkontoId != 0)
            {
                sftpKonto = _portalSosRepository.GetSFTPAccount(sftpkontoId);
            }

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
                filloggDetalj.Period = senasteLeverans.AdmForvantadleverans.Period;
                filloggDetalj.SFTPkontoNamn = sftpKonto.Kontonamn;
                if (aterkoppling != null)
                {
                    filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus;
                    filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                    filloggDetalj.Aterkopplingskontakt =
                        _portalSosRepository.GetUserEmail(senasteLeverans.ApplicationUserId);
                }
                if (senasteLeverans.Organisationsenhet != null)
                {
                    filloggDetalj.Enhetskod = senasteLeverans.Organisationsenhet.Enhetskod;
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
                    filloggDetalj.Period = senasteLeverans.AdmForvantadleverans.Period;
                    filloggDetalj.SFTPkontoNamn = sftpKonto.Kontonamn;
                    if (aterkoppling != null)
                    {
                        //filloggDetalj.Leveransstatus = aterkoppling.Leveransstatus; //Skriv ej över leveransstatusen från återkopplingen. Beslut 20180912, ärende #128
                        filloggDetalj.Resultatfil = aterkoppling.Resultatfil;
                        filloggDetalj.Aterkopplingskontakt =
                            _portalSosRepository.GetUserEmail(senasteLeverans.ApplicationUserId);

                    }
                    if (senasteLeverans.Organisationsenhet != null)
                    {
                        filloggDetalj.Enhetskod = senasteLeverans.Organisationsenhet.Enhetskod;
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

        private List<string> HamtaFilnamnsstarterFranFilmasker(IEnumerable<string> filmasker)
        {
            var filnamnsStartList = new List<string>();
            foreach (var filmask in filmasker)
            {
                //Hantera ej filmasker med värdet "*"
                if (!filmask.Equals("*"))
                {
                    var firstUnderscorePos = filmask.IndexOf("_");
                    if (firstUnderscorePos != -1)
                    {
                        var fileNameStart = filmask.Substring(0, firstUnderscorePos);
                        filnamnsStartList.Add(fileNameStart);
                    }
                }
            }
            return filnamnsStartList.Distinct().ToList();
        }


        public List<RegisterInfo> HamtaOrgenheter(List<RegisterInfo> registerInfoList, int orgId)
        {
            foreach (var item in registerInfoList)
            {
                var uppgiftsskyldighet = HamtaAktivUppgiftsskyldighetForOrganisationOchRegister(orgId, item.Id);
                if (uppgiftsskyldighet.RapporterarPerEnhet)
                {
                    //Ger cirkulär reference, därav keyValuePair nedan
                    //item.Orgenheter = orgUnits.ToList();
                    item.Organisationsenheter = new List<KeyValuePair<string, string>>();
                    item.Orgenheter = new List<KeyValuePair<string, string>>();

                    item.RapporterarPerEnhet = true;
                    var enhetsuppgiftsskyldighetList =
                        _portalSosRepository.GetActiveUnitReportObligationForReportObligation(uppgiftsskyldighet.Id);
                    foreach (var enhetsuppgiftsskyldighet in enhetsuppgiftsskyldighetList)
                    {
                        var orgUnit =
                            _portalSosRepository.GetOrganisationUnit(enhetsuppgiftsskyldighet.OrganisationsenhetsId);
                        KeyValuePair<string, string> keyValuePair =
                            new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Enhetsnamn);
                        item.Organisationsenheter.Add(keyValuePair);
                        KeyValuePair<string, string> keyValuePairFilkod =
                            new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Filkod);
                        item.Orgenheter.Add(keyValuePairFilkod);
                    }
                }
            }

            return registerInfoList;
        }

        public List<RegisterInfo> HamtaOrgenheterForAnvandare(List<RegisterInfo> registerInfoList, int orgId, string userId)
        {
            var filteredRegisterInfoList = new List<RegisterInfo>();
            foreach (var item in registerInfoList)
            {
                var uppgiftsskyldighet = HamtaAktivUppgiftsskyldighetForOrganisationOchRegister(orgId, item.Id);
                if (uppgiftsskyldighet.RapporterarPerEnhet)
                {
                    var chosenBol = false;
                    var anvValdaOrgenheterForDelreg = HamtaAnvandarensValdaEnheterForDelreg(userId, item.Id).ToList();
                    //Ger cirkulär reference, därav keyValuePair nedan
                    //item.Orgenheter = orgUnits.ToList();
                    item.Organisationsenheter = new List<KeyValuePair<string, string>>();
                    item.Orgenheter = new List<KeyValuePair<string, string>>();

                    item.RapporterarPerEnhet = true;
                    var enhetsuppgiftsskyldighetList =
                        _portalSosRepository.GetActiveUnitReportObligationForReportObligation(uppgiftsskyldighet.Id);
                    foreach (var enhetsuppgiftsskyldighet in enhetsuppgiftsskyldighetList)
                    {
                        var orgUnit = _portalSosRepository.GetOrganisationUnit(enhetsuppgiftsskyldighet.OrganisationsenhetsId);
                        var chosen = anvValdaOrgenheterForDelreg.Where(x => x.Id == orgUnit.Id);
                        if (chosen.Any())
                        {
                            chosenBol = true;
                            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Enhetsnamn);
                            item.Organisationsenheter.Add(keyValuePair);
                            KeyValuePair<string, string> keyValuePairFilkod = new KeyValuePair<string, string>(orgUnit.Enhetskod, orgUnit.Filkod);
                            item.Orgenheter.Add(keyValuePairFilkod);
                        }
                    }
                    if (chosenBol)
                    {
                        filteredRegisterInfoList.Add(item);
                    }
                }
                else
                {
                    filteredRegisterInfoList.Add(item);
                }
            }

            return filteredRegisterInfoList;
        }

        private void HandleRegisteredCaseReporters(ArendeDTO arende, string userName)
        {
            foreach (var contact in arende.Kontaktpersoner)
            {
                if (contact.Selected)
                {
                    var caseContact = new ArendeKontaktperson
                    {
                        ArendeId = arende.Id,
                        ApplicationUserId = contact.Id,
                        SkapadAv = userName,
                        SkapadDatum = DateTime.Now,
                        AndradAv = userName,
                        AndradDatum = DateTime.Now
                    };
                    var arendeContactId = _portalSosRepository.SaveCaseContact(caseContact);

                    //Lägg till rollen ArendeUpp för redan reggad användare (om den inte redan är satt)
                    var roll = _portalSosRepository.GetFilipRoleByName("ArendeUpp");
                    var userRoles = _portalSosRepository.GetFilipUserRolesForUser(contact.Id);
                    var exists = userRoles.SingleOrDefault(x => x.RoleId == roll.Id);
                    if (exists == null)
                    {
                        var role = new ApplicationUserRole
                        {
                            UserId = contact.Id,
                            RoleId = roll.Id,
                            SkapadAv = userName,
                            SkapadDatum = DateTime.Now,
                            AndradAv = userName,
                            AndradDatum = DateTime.Now
                        };
                        _portalSosRepository.AddRoleToFilipUser(role);
                    }
                }
                else
                {
                    _portalSosRepository.DeleteCaseContact(contact.Id, arende.Id);
                }
            }
        }

        private void HandleUnRegisteredCaseReporters(ArendeDTO arende, string userName)
        {
            var reporters = arende.Rapportorer.Replace(' ', ',');
            var newEmailStr = reporters.Split(',');
            foreach (var email in newEmailStr)
            {
                if (!String.IsNullOrEmpty(email.Trim()))
                {
                    //Spara i undantagstabell
                    var undantag = new PreKontakt()
                    {
                        ArendeId = arende.Id,
                        Epostadress = email.Trim(),
                        SkapadAv = userName,
                        SkapadDatum = DateTime.Now,
                        AndradAv = userName,
                        AndradDatum = DateTime.Now
                    };
                    _portalSosRepository.CreatePreKontakt(undantag);
                }
            }
        }


        public List<List<Organisation>> SokOrganisation(string sokStr)
        {
            string[] searchstring = sokStr.Split(' ');
            var orgList = _portalSosRepository.SearchOrganisation(searchstring);
            return orgList;
        }
    }
}

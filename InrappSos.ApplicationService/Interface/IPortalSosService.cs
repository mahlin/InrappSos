using InrappSos.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using InrappSos.ApplicationService.DTOModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.ApplicationService.Interface
{
    public interface IPortalSosService
    {
        bool IsConnectedViaPrivateEmailadress(string email);

        UndantagEpostDoman HamtaUndantagEpostDoman(string email);
        void AktiveraKontaktperson(string userId);
        string ClosedComingWeek();
        string HelgdagComingWeek();
        string SpecialdagComingWeek();
        List<List<Organisation>> SokOrganisation(string sokStr);

        IEnumerable<FilloggDetaljDTO> FiltreraHistorikForAnvandare(string userId, List<RegisterInfo> valdaDelregisterList, List<FilloggDetaljDTO> historikForOrganisation);

        IdentityRole HamtaAstridRoll(string roleName);

        IEnumerable<AspNetPermissions> HamtaAllaAstridRattigheter();
            
        IEnumerable<AspNetRolesPermissions> HamtaAstridRattigheterForRoll(string rollId);

        IEnumerable<string> HamtaAstridRattighetersNamnForRoll(string rollId);

        IEnumerable<PermissionDTO> HamtaValdaAstridRattigheterForRoll(string rollId);

        void UppdateraAstridRollsRattigheter(string rollId, List<PermissionDTO> rattighetsLista);

        IdentityRole HamtaFilipRoll(string roleName);

        IEnumerable<AdmFAQKategori> HamtaAllaFAQs();
        IEnumerable<AdmOrganisationstyp> HamtaAllaOrganisationstyper();

        string HamtaAnvandaresNamn(string userId);

        ApplicationUser HamtaAnvandareMedEpost(string epost);

        string HamtaAnvandaresKontaktnummer(string userId);

        string HamtaAnvandaresMobilnummer(string userId);

        Arende HamtaArende(string arendeNr);

        Arende HamtaArendeById(int arendeId);

        int HamtaUserOrganisationId(string userId);

        Organisation HamtaOrganisation(int orgId);

        Organisation HamtaOrganisationForKommunkod(string kommunkod);

        Organisation HamtaOrgForEmailDomain(string modelEmail);
        string HamtaKommunkodForOrg(int orgId);
        string HamtaKommunKodForAnvandare(string userId);

        Organisation HamtaOrgForAnvandare(string userId);
        Organisation HamtaOrgForOrganisationsenhet(int orgUnitId);

        Organisation HamtaOrgForUppgiftsskyldighet(int uppgSkId);

        List<OrganisationstypDTO> HamtaOrgtyperForOrganisation(int orgId, List<AdmOrganisationstyp> orgtyperList);

        //List<UserRolesDTO> HamtaAstridRoller();

        IEnumerable<ApplicationUser> HamtaKontaktpersonerForOrg(int orgId);

        IEnumerable<UndantagEpostDoman> HamtaPrivataEpostadresserForOrg(int orgId);
        IEnumerable<Arende> HamtaArendenForOrg(int orgId);
        IEnumerable<Organisationsenhet> HamtaOrgEnheterForOrg(int orgId);
        Organisationsenhet HamtaOrganisationsenhetMedEnhetskod(string kod, int orgId);

        IEnumerable<AdmUppgiftsskyldighet> HamtaUppgiftsskyldighetForOrg(int orgId);
        AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrganisationOchRegister(int orgId, int delregid);

        AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrgOchDelreg(int orgId, int delregId);

        IEnumerable<AdmEnhetsUppgiftsskyldighet> HamtaEnhetsUppgiftsskyldighetForOrgEnhet(int orgenhetId);
        AdmEnhetsUppgiftsskyldighet HamtaEnhetsUppgiftsskyldighetForUppgiftsskyldighetOchOrgEnhet(int uppgskhId, int orgenhetId);

        IEnumerable<AdmFAQKategori> HamtaFAQkategorier();

        IEnumerable<AdmFAQ> HamtaFAQs(int faqCatId);

        IEnumerable<AdmHelgdag> HamtaAllaHelgdagar();

        IEnumerable<AdmSpecialdag> HamtaAllaSpecialdagar();

        AdmFAQ HamtaFAQ(int faqId);

        AdmForeskrift HamtaForeskrift(int foreskriftId);

        AdmFAQKategori HamtaFAQKategori(int faqCatId);

        IEnumerable<AdmInformation> HamtaInformationstexter();

        OpeningHoursInfoDTO HamtaOppettider();

        AdmInformation HamtaInfoText(string infoTyp);
        

        AdmInformation HamtaInfo(int infoId);

        IEnumerable<AdmRegister> HamtaRegister();

        AdmRegister HamtaRegisterMedKortnamn(string regKortNamn);

        AdmRegister HamtaRegisterMedId(int regId);

        IEnumerable<AdmDelregister> HamtaDelRegister();

        AdmDelregister HamtaDelregister(int delregId);

        IEnumerable<AdmDelregister> HamtaDelRegisterForRegister(int regId);

        IEnumerable<AdmForeskrift> HamtaForeskrifterForRegister(int regId);

        AdmDelregister HamtaDelRegisterForUppgiftsskyldighet(int uppgSkId);
        AdmDelregister HamtaDelRegisterForKortnamn(string shortName);

        IEnumerable<RegisterInfo> HamtaDelregisterOchFilkrav();
        IEnumerable<RegisterInfo> HamtaDelregisterOchAktuellaFilkrav();
        IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranser();

        IEnumerable<AdmForvantadfil> HamtaAllaForvantadeFiler();

        IEnumerable<AdmFilkrav> HamtaAllaFilkrav();

        IEnumerable<AdmForvantadfil> HamtaForvantadeFilerForRegister(int regId);

        IEnumerable<AdmForvantadfil> HamtaForvantadFil(int filkravId);

        IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForRegister(int regId);

        IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForDelregister(int delregId);

        int HamtaForvantadleveransIdForRegisterOchPeriod(int delregId, string period);
        List<string> HamtaGiltigaPerioderForDelregister(int delregId);

        IEnumerable<AdmFilkrav> HamtaFilkravForRegister(int regId);

        string HamtaHelgEllerSpecialdagsInfo();

        string HamtaHelgdagsInfo();

        string HamtaSpecialdagsInfo();

        IEnumerable<AdmRegister> HamtaAllaRegister();
        IEnumerable<AdmRegister> HamtaAllaRegisterForPortalen();

        IEnumerable<IdentityRole> HamtaAllaAstridRoller();
        IEnumerable<IdentityRole> HamtaAllaFilipRoller();

        IEnumerable<AdmDelregister> HamtaAllaDelregisterForPortalen();

        IEnumerable<AdmForeskrift> HamtaAllaForeskrifter();

        IEnumerable<AdmDelregister> HamtaDelregisterMedInsamlingsfrekvens(int insamlingsfrekvensId);

        IEnumerable<RegisterInfo> HamtaAllRegisterInformation();
        IEnumerable<AdmInsamlingsfrekvens> HamtaAllaInsamlingsfrekvenser();

        IEnumerable<Arendetyp> HamtaAllaArendetyper();

        IEnumerable<ArendeStatus> HamtaAllaArendestatusar();

        IEnumerable<Organisation> HamtaAllaOrganisationer();

        string HamtaKortnamnForDelregisterMedFilkravsId(int filkravsId);

        string HamtaKortnamnForDelregister(int delregId);

        AdmInsamlingsfrekvens HamtaInsamlingsfrekvens(int insamlingsid);

        string HamtaKortnamnForRegister(int regId);

        string HamtaNamnForFilkrav(int filkravId);
        int HamtaNyttLeveransId(string userId, string userName, int orgId, int registerId, int orgenhetsId, int forvLevId, string status);


        AdmForeskrift HamtaForeskriftByFilkrav(int filkravId);

        IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisation(int orgId);

        IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisation(int orgId);

        IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndUser(int orgId, string userId);

        IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndDelreg(int orgId, List<RegisterInfo> valdaDelregister);

        IEnumerable<AppUserAdmin> HamtaAdminUsers();

        IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForDelregOchPeriod(int delRegId, string period);

        IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForRegOchPeriod(int regId, string period);

        IEnumerable<AdmRegister> HamtaRegisterForOrg(int orgId);

        IEnumerable<string> HamtaDelregistersPerioderForAr(int delregId, int ar);

        List<int> HamtaValbaraAr(int delregId);
        DateTime HamtaRapporteringsstartForRegisterOchPeriod(int regId, string period);

        DateTime HamtaRapporteringsstartForRegisterOchPeriodSpecial(int regId, string period);
        DateTime HamtaSenasteRapporteringForRegisterOchPeriodSpecial(int regId, string period);

        DateTime HamtaSenasteRapporteringForRegisterOchPeriod(int regId, string period);

        IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisationRegisterPeriod(int orgId, int regId, string periodForReg);

        string HamtaSammanlagdStatusForPeriod(IEnumerable<FilloggDetaljDTO> historikLista);

        //IEnumerable<RegisterInfo> HamtaValdaRegistersForAnvandare(string userId, int orgId);

        List<RegisterInfo> HamtaValdaDelregisterForAnvandare(string userId, int orgId);

        IEnumerable<RegisterInfo> HamtaRegistersMedAnvandaresVal(string userId, int orgId);

        IEnumerable<AdmRegister> HamtaRegisterForAnvandare(string userId, int orgId);

        Arendetyp HamtaArendetyp(int arendetypId);

        ArendeStatus HamtaArendestatus(int arendestatusId);

        string HamtaArendesRapportorer(int orgId, int arendeId);

        void InaktiveraKontaktperson(string userId);

        bool IsOpen();
        bool IsHelgdag();
        bool IsSpecialdag();

        void SkapaAstridRoll(string rollNamn);

        void SkapaFilipRoll(string rollNamn);

        int SkapaOrganisation(Organisation org, ICollection<Organisationstyp> orgtyperForOrg, string userName);

        void SkapaOrganisationsenhet(Organisationsenhet orgUnit, string userName);

        void SkapaPrivatEpostadress(UndantagEpostDoman privEmail, string userName);

        void SkapaArende(ArendeDTO arende, string userName);
        void SkapaOrganisationstyp(AdmOrganisationstyp orgtyp, string userName);

        void SkapaFAQKategori(AdmFAQKategori faqKategori, string userName);

        void SkapaFAQ(AdmFAQ faq, string userName);

        void SkapaHelgdag(AdmHelgdag helgdag, string userName);

        void SkapaSpecialdag(AdmSpecialdag specialdag, string userName);

        void SkapaInformationsText(AdmInformation infoText, string userName);

        void SkapaUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSk, string userName);

        void SkapaEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSk, string userName);

        void SkapaRegister(AdmRegister reg, string userName);

        void SkapaDelregister(AdmDelregister delReg, string userName);

        void SkapaForeskrift(AdmForeskrift foreskrift, string userName);

        void SkapaForvantadLeverans(AdmForvantadleverans forvLev, string userName);
        void SkapaForvantadLeveranser(IEnumerable<AdmForvantadleverans> forvLevList, string userName);
        IEnumerable<ForvantadLeveransDTO> SkapaForvantadeLeveranserUtkast(int selectedYear, int selectedDelRegId, int selectedFilkravId);

        void SkapaForvantadFil(AdmForvantadfil forvFil, string userName);

        void SkapaFilkrav(AdmFilkrav filkrav, string userName);

        void SkapaInsamlingsfrekvens(AdmInsamlingsfrekvens insamlingsfrekvens, string userName);

        void UppdateraAstridRoll(IdentityRole role);
        void UppdateraFilipRoll(IdentityRole role);

        void UppdateraOrganisation(Organisation org, string userName);

        void UppdateraKontaktperson(ApplicationUser user, string userName);
        void UppdateraNamnForAnvandare(string userId, string userName);
        void UppdateraKontaktnummerForAnvandare(string userId, string tfnnr);

        void UppdateraAktivFromForAnvandare(string userId);
        void UppdateraAnvandarInfo(ApplicationUser user);

        void UppdateraAdminAnvandare(AppUserAdmin user, string userName);

        void UppdateraOrganisationsenhet(Organisationsenhet orgUnit, string userName);
        void UppdateraOrganisationstyp(AdmOrganisationstyp orgtyp, string userName);

        void UppdateraUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSkyldighet, string userName);


        void UppdateraValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> registerList);


        void UppdateraEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSkyldighet, string userName);

        void UppdateraFAQKategori(AdmFAQKategori faqKategori, string userName);

        void UppdateraFAQ(AdmFAQ faq, string userName);

        void UppdateraHelgdag(AdmHelgdag holiday, string userName);

        void UppdateraSpecialdag(AdmSpecialdag specialDay, string userName);

        void UppdateraInformationstext(AdmInformation infoText, string userName);

        void UppdateraRegister(AdmRegister register, string userName);

        void UppdateraDelregister(AdmDelregister delregister, string userName);
        void UppdateraForeskrift(AdmForeskrift foreskrift, string userName);

        void UppdateraForvantadLeverans(AdmForvantadleverans forvLev, string userName);

        void UppdateraForvantadFil(AdmForvantadfil forvFil, string userName);

        void UppdateraFilkrav(AdmFilkrav filkrav, string userName);

        void UppdateraInsamlingsfrekvens(AdmInsamlingsfrekvens insamlingsfrekvens, string userName);

        void UppdateraAnvandarInfo(AppUserAdmin user, string userName);

        void UppdateraPrivatEpostAdress(UndantagEpostDoman privEpostDoman, string userName);

        void UppdateraArende(ArendeDTO arende, string userName, string rapportorer);

        void SparaOppettider(OpeningHoursInfoDTO oppetTider, string userName);
        void SparaTillDatabasFillogg(string userName, string ursprungligtFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber);
        void SparaValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> registerList);
        void SaveToLoginLog(string userid, string userName);

        void TaBortAstridRoll(string roleName);

        void TaBortFilipRoll(string roleName);

        void TaBortFAQKategori(int faqKategoriId);

        void TaBortFAQ(int faqId);
        void TaBortOrganisationstyp(int orgTypeId);

        void TaBortHelgdag(int holidayId);

        void TaBortSpecialdag(int specialDayId);

        void TaBortKontaktperson(string contactId);

        void TaBortAdminAnvandare(string userId);

        List<OpeningDay> MarkeraStangdaDagar(List<string> closedDays);
        string MaskPhoneNumber(string phoneNumber);

        void SkickaPaminnelse(IEnumerable<RapporteringsresultatDTO> rappResList, string userId);

    }
}

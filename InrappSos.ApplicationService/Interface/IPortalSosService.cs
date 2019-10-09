using InrappSos.DomainModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        bool IsConnectedViaArende(string email);

        UndantagEpostadress HamtaUndantagEpostadress(string email);
        PreKontakt HamtaPrekontakt(string email);
        void AktiveraKontaktperson(string userId);
        string ClosedComingWeek();

        string DeviatingOpeningHoursNextThreeWeeks();
        string HelgdagComingWeek();
        string SpecialdagComingWeek();
        List<List<Organisation>> SokOrganisation(string sokStr);

        List<List<Organisation>> SokCaseOrganisation(string sokStr);

        List<List<Arende>> SokArende(string sokStr);

        List<List<ApplicationUser>> SokKontaktperson(string sokStr);

        IEnumerable<FilloggDetaljDTO> FiltreraHistorikForAnvandare(string userId, List<RegisterInfo> valdaDelregisterList, List<FilloggDetaljDTO> historikForOrganisation);

        ApplicationRole HamtaAstridRoll(string roleName);

        List<string> HamtaFilipRollerForAnvandare(string userId);

        ApplicationRole HamtaFilipRoll(string roleName);

        void HandlePrekontaktUserRegistration(ApplicationUser user, PreKontakt preKontakt);

        IEnumerable<AdmFAQKategori> HamtaAllaFAQs();
        IEnumerable<AdmOrganisationstyp> HamtaAllaOrganisationstyper();

        string HamtaAnvandaresNamn(string userId);

        ApplicationUser HamtaAnvandareMedEpost(string epost);

        string HamtaAnvandaresKontaktnummer(string userId);

        IEnumerable<Arende> HamtaAnvandaresArenden(string userId);

        IEnumerable<Leverans> HamtaAnvandaresLeveranser(string userId);
        IEnumerable<Arende> HamtaAnvandaresOppnaArenden(string userId);

        string HamtaAnvandaresMobilnummer(string userId);

        Arende HamtaArende(string arendeNr);

        Arende HamtaArendeById(int arendeId);

        int HamtaUserOrganisationId(string userId);

        Organisation HamtaOrganisation(int orgId);

        Organisation HamtaOrganisationForKommunkod(string kommunkod);

        Organisation HamtaOrgForEmailDomain(string modelEmail);
        string HamtaKommunkodForOrg(int orgId);
        string HamtaKommunKodForAnvandare(string userId);

        string HamtaLandstingsKodForAnvandare(string userId);

        string HamtaLandstingskodForOrg(int orgId);

        string HamtaInrapporteringskodKodForAnvandare(string userId);

        Organisation HamtaOrgForAnvandare(string userId);
        Organisation HamtaOrgForOrganisationsenhet(int orgUnitId);

        Organisation HamtaOrgForUppgiftsskyldighet(int uppgSkId);

        AdmOrganisationstyp HamtaOrgtyp(string orgtypnamn);

        List<OrganisationstypDTO> HamtaOrgtyperForOrganisation(int orgId, List<AdmOrganisationstyp> orgtyperList);

        List<OrganisationstypDTO> HamtaOrgtyperForOrganisation(int orgId);

        List<OrganisationstypDTO> HamtaOrgtyperForDelregister(int delregId);

        AdmOrganisationstyp HamtaOrganisationstyp(int orgtypId);

        IEnumerable<ApplicationUser> HamtaKontaktpersonerForOrg(int orgId);

        IEnumerable<ApplicationUser> HamtaAktivaKontaktpersonerForOrg(int orgId);

        IEnumerable<SFTPkonto> HamtaSFTPkontonForOrg(int orgId);

        IEnumerable<ApplicationUser> HamtaKontaktpersonerForSFTPKonto(int sftpKontoId);

        List<string> HamtaEpostadresserForSFTPKonto(int sftpKontoId);

        IEnumerable<UndantagEpostadress> HamtaPrivataEpostadresserForOrg(int orgId);

        IEnumerable<UndantagForvantadfil> HamtaUndantagnaForvantadeFilerForOrg(int orgId);
        IEnumerable<Arende> HamtaArendenForOrg(int orgId);
        IEnumerable<Organisationsenhet> HamtaOrgEnheterForOrg(int orgId);

        IEnumerable<Organisationsenhet> HamtaOrganisationsenheterMedUppgSkyldighetsId(int uppgSkyldighetsid);

        IEnumerable<Organisationsenhet> HamtaOrganisationsenheterMedUppgSkyldighetInomPerioden(int uppgSkyldighetsid, string period);
        Organisationsenhet HamtaOrganisationsenhetMedEnhetskod(string kod, int orgId);
        Organisationsenhet HamtaOrganisationsenhetMedFilkod(string kod, int orgId);

        IEnumerable<AdmUppgiftsskyldighet> HamtaUppgiftsskyldighetForOrg(int orgId);

        IEnumerable<AdmUppgiftsskyldighet> HamtaAktivUppgiftsskyldighetForOrg(int orgId);
        AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrganisationOchRegister(int orgId, int delregid);

        AdmUppgiftsskyldighet HamtaAktivUppgiftsskyldighetForOrganisationOchRegister(int orgId, int delregid);

        AdmUppgiftsskyldighet HamtaUppgiftsskyldighetForOrgOchDelreg(int orgId, int delregId);

        IEnumerable<AdmEnhetsUppgiftsskyldighet> HamtaEnhetsUppgiftsskyldighetForOrgEnhet(int orgenhetId);
        AdmEnhetsUppgiftsskyldighet HamtaEnhetsUppgiftsskyldighetForUppgiftsskyldighetOchOrgEnhet(int uppgskhId, int orgenhetId);

        AdmEnhetsUppgiftsskyldighet HamtaAktivEnhetsUppgiftsskyldighetForUppgiftsskyldighetOchOrgEnhet(int uppgskhId, int orgenhetId);

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

        IEnumerable<Inloggning> HamtaInloggning(string userId);

        IEnumerable<AdmRegister> HamtaRegister();

        AdmRegister HamtaRegisterMedKortnamn(string regKortNamn);

        AdmRegister HamtaRegisterMedId(int regId);

        IEnumerable<AdmDelregister> HamtaDelRegister();

        AdmDelregister HamtaDelregister(int delregId);

        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> HamtaAllaDelRegistersOrganisationstyper();

        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> HamtaDelRegistersOrganisationstyper(int delregId);

        List<string> HamtaOrganisationstyperForDelregister(int delregId);

        IEnumerable<AdmDelregister> HamtaDelRegisterForRegister(int regId);

        IEnumerable<AdmDelregister> HamtaDelRegisterMedUndertabellerForRegister(int regId);

        IEnumerable<AdmForeskrift> HamtaForeskrifterForRegister(int regId);

        AdmDelregister HamtaDelRegisterForUppgiftsskyldighet(int uppgSkId);
        AdmDelregister HamtaDelRegisterForKortnamn(string shortName);

        IEnumerable<RegisterInfo> HamtaDelregisterOchFilkrav();
        IEnumerable<RegisterInfo> HamtaDelregisterOchAktuellaFilkrav();
        IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranser();

        IEnumerable<AdmForvantadfil> HamtaAllaForvantadeFiler();

        IEnumerable<AdmForvantadfil> HamtaAllaAktivaForvantadeFiler();

        IEnumerable<AdmFilkrav> HamtaAllaFilkrav();

        IEnumerable<AdmFilkrav> HamtaAllaAktivaFilkrav();

        IEnumerable<AdmForvantadfil> HamtaForvantadeFilerForRegister(int regId);
        IEnumerable<AdmForvantadfil> HamtaAktivaForvantadeFilerForRegister(int regId);

        IEnumerable<AdmForvantadfil> HamtaForvantadFil(int filkravId);

        IEnumerable<AdmForvantadfilDTO> HamtaAllaForvantadeFilerForOrg(int orgId);

        IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForRegister(int regId);

        IEnumerable<AdmForvantadleverans> HamtaForvantadeLeveranserForDelregister(int delregId);

        int HamtaForvantadleveransIdForRegisterOchPeriod(int delregId, string period);
        List<string> HamtaGiltigaPerioderForDelregister(int delregId);

        List<string> HamtaGiltigaFilkoderForOrganisation(int orgId);

        List<string> HamtaGiltigaFilkoderForSFTPKonto(int sftpAccountId);

        IEnumerable<AdmFilkrav> HamtaFilkravForRegister(int regId);

        IEnumerable<AdmFilkrav> HamtaAktivaFilkravForRegister(int regId);

        AdmFilkrav HamtaFilkravById(int filkravsId);

        string HamtaHelgEllerSpecialdagsInfo();

        string HamtaHelgdagsInfo();

        string HamtaSpecialdagsInfo();

        SFTPkonto HamtaFtpKontoByName(string name);

        IEnumerable<AdmRegister> HamtaAllaRegister();
        IEnumerable<AdmRegister> HamtaAllaRegisterForPortalen();

        IEnumerable<ApplicationRole> HamtaAllaAstridRoller();
        IEnumerable<ApplicationRole> HamtaAllaFilipRoller();

        IEnumerable<AdmDelregister> HamtaAllaDelregisterForPortalen();

        IEnumerable<AdmDelregister> HamtaAllaDelregisterForOrganisationen(int orgId);

        IEnumerable<RegisterInfo> HamtaAllRegisterInformationForOrganisation(int orgId);

        IEnumerable<AppUserAdmin> HamtaAllaArendeadministratorerForOrg(int orgId);

        IEnumerable<ArendeAnsvarig> HamtaAllaArendeansvarigaForOrg(int orgId);

        IEnumerable<ArendeAnsvarig> HamtaAllaArendeansvariga();

        IEnumerable<AdmForeskrift> HamtaAllaForeskrifter();

        IEnumerable<AdmDelregister> HamtaDelregisterMedInsamlingsfrekvens(int insamlingsfrekvensId);

        //IEnumerable<RegisterInfo> HamtaAllRegisterInformation();
        IEnumerable<AdmInsamlingsfrekvens> HamtaAllaInsamlingsfrekvenser();

        IEnumerable<Arendetyp> HamtaAllaArendetyper();

        IEnumerable<Organisation> HamtaAllaOrganisationer();

        string HamtaKortnamnForDelregisterMedFilkravsId(int filkravsId);

        string HamtaKortnamnForDelregister(int delregId);

        AdmInsamlingsfrekvens HamtaInsamlingsfrekvens(int insamlingsid);

        List<string> HamtaGodkandaFilnamnsStarter();

        string HamtaKortnamnForRegister(int regId);

        string HamtaNamnForFilkrav(int filkravId);
        int HamtaNyttLeveransId(string userId, string userName, int orgId, int registerId, int orgenhetsId, int forvLevId, string status);


        AdmForeskrift HamtaForeskriftByFilkrav(int filkravId);

        IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisation(int orgId);

        IEnumerable<FildroppDetaljDTO> HamtaFildroppsHistorikForAnvandaresArenden(string userId);

        IEnumerable<FildroppDetaljDTO> HamtaFildroppsHistorikForValtArende(int arendeId);

        IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisation(int orgId);

        IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndUser(int orgId, string userId);

        IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndDelreg(int orgId, List<RegisterInfo> valdaDelregister);

        IEnumerable<FilloggDetaljDTO> HamtaTop100HistorikForOrganisationAndDelreg(int orgId, List<RegisterInfo> valdaDelregister);

        IEnumerable<FilloggDetaljDTO> HamtaTop100HistorikForOrganisationAndDelregAndOrgenheter(int orgId, List<RegisterInfo> valdaDelregister, List<Organisationsenhet> orgenhetList);


        IEnumerable<FilloggDetaljDTO> HamtaTop10HistorikForOrganisationAndDelregOchValdaOrgenheter(int orgId, List<RegisterInfo> valdaDelregister, string userId);

        IEnumerable<AppUserAdmin> HamtaAdminUsers();

        IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForDelregOchPeriod(int delRegId, string period);

        IEnumerable<RapporteringsresultatDTO> HamtaRapporteringsresultatForRegOchPeriod(int regId, string period);

        IEnumerable<AdmRegister> HamtaRegisterForOrg(int orgId);

        IEnumerable<AdmRegister> HamtaRegisterEjKoppladeTillSFTPKontoForOrg(int orgId);

        IEnumerable<string> HamtaDelregistersPerioderForAr(int delregId, int ar);

        IEnumerable<string> HamtaDelregistersPerioderForAr(AdmDelregister delreg, int ar);

        List<int> HamtaValbaraAr(int delregId);

        List<int> HamtaValbaraAr(AdmDelregister delreg);

        DateTime HamtaRapporteringsstartForRegisterOchPeriod(int regId, string period);

        IEnumerable<AdmForvantadleverans> HamtaForvLevForRegisterOchPerioder(List<AdmDelregister> delregList, List<string> perioder);

        DateTime HamtaRapporteringsstartForRegisterOchPeriodSpecial(int regId, string period);
        DateTime HamtaSenasteRapporteringForRegisterOchPeriodSpecial(int regId, string period);

        DateTime HamtaSenasteRapporteringForRegisterOchPeriod(int regId, string period);

        IEnumerable<FilloggDetaljDTO> HamtaHistorikForOrganisationRegisterPeriod(int orgId, List<AdmDelregister> delregisterList, string periodForReg, List<Leverans> levstatusRapportList);

        string HamtaSammanlagdStatusForPeriod(IEnumerable<FilloggDetaljDTO> historikLista);

        string KontrolleraOmKomplettaEnhetsleveranser(int orgId, LeveransStatusDTO leveransStatusObj, List<AdmDelregister> delregisterList);

        void KopplaAstridAnvändareTillAstridRoll(string userName, string astridUserId, string rollId);

        void KopplaFilipAnvändareTillFilipRoll(string userName, string filipUserId, string rollId);

        void KopplaFilipAnvändareTillFilipRollNamn(string userName, string filipUserId, string rollnamn);

        void KopplaKontaktpersonTillArende(string userName, int arendeId, string kontaktId);

        void HanteraArendesEjReggadeKontaktpersoner(ArendeDTO arende, string userName);

        void TaBortFilipRollForanvandare(string userName, string filipUserId, string rollId);

        void TaBortKontaktpersonFranArende(string userName, int arendeId, string kontaktId);

        IEnumerable<ApplicationRole> HamtaAstridAnvandaresRoller(string userId);

        //IEnumerable<RegisterInfo> HamtaValdaRegistersForAnvandare(string userId, int orgId);

        List<RegisterInfo> HamtaValdaDelregisterForAnvandare(string userId, int orgId);

        AdmDelregister HamtaValtDelRegisterMedFilnamnsstart(string filnamnsStart);
  
        List<RegisterInfo> HamtaRelevantaDelregisterForSFTPKonto(SFTPkonto sftpKonto);

        IEnumerable<RegisterInfo> HamtaRegistersMedAnvandaresVal(string userId, int orgId);

        IEnumerable<Organisationsenhet> HamtaDelregistersAktuellaEnheter(int delregId, int orgId);

        IEnumerable<Organisationsenhet> HamtaAnvandarensValdaEnheterForDelreg(string userId, int subdirId);

        IEnumerable<AdmRegister> HamtaRegisterForAnvandare(string userId, int orgId);

        Arendetyp HamtaArendetyp(int arendetypId);

        ArendeAnsvarig HamtaArendeAnsvarig(int arendeAnsvId);

        string HamtaArendesKontaktpersoner(int orgId, int arendeId);

        IEnumerable<ArendeKontaktperson> HamtaArendesKontaktpersoner(int arendeid);

        string HamtaArendesEjRegistreradeKontaktpersoner(int orgId, int arendeId);

        IEnumerable<Leverans> HamtaLeveransStatusRapporterForOrgDelregPerioder( int orgId, List<AdmDelregister> delregisterList, List<string> periodsForRegister);

        void InaktiveraKontaktperson(string userId);

        IEnumerable<AdmOrganisationstyp> HamtaAllaValbaraOrganisationstyperForDelregister(int subdirId);

        bool IsOpen();
        bool IsHelgdag();
        bool IsSpecialdag();

        void SkapaFilipRoll(ApplicationRole filipRoll, string userName);

        int SkapaOrganisation(Organisation org, ICollection<Organisationstyp> orgtyperForOrg, string userName);

        int SkapaSFTPkonto(SFTPkonto account, ICollection<KontaktpersonSFTPkonto> contacts, string userName);

        void SkapaOrganisationsenhet(Organisationsenhet orgUnit, string userName);

        void SkapaPrivatEpostadress(UndantagEpostadress privEmail, string userName);

        void SkapaArende(ArendeDTO arende, string userName);
        void SkapaOrganisationstyp(AdmOrganisationstyp orgtyp, string userName);

        void SkapaArendetyp(Arendetyp arendetyp, string userName);

        void SkapaArendeAnsvarig(ArendeAnsvarig arendeAnsvarig, string userName);

        void SkapaFAQKategori(AdmFAQKategori faqKategori, string userName);

        void SkapaFAQ(AdmFAQ faq, string userName);

        void SkapaHelgdag(AdmHelgdag helgdag, string userName);

        void SkapaSpecialdag(AdmSpecialdag specialdag, string userName);

        void SkapaInformationsText(AdmInformation infoText, string userName);

        void SkapaUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSk, string userName);
        void SkapaUppgiftsskyldighetOrgtyp(AdmUppgiftsskyldighetOrganisationstyp uppgSk, string userName);

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

        void UppdateraAstridRoll(ApplicationRole role, string userName);
        void UppdateraFilipRoll(ApplicationRole role, string userName);

        void UppdateraOrganisation(Organisation org, string userName);

        void UppdateraKontaktperson(ApplicationUser user, string userName);
        void UppdateraSFTPKonto(SFTPkonto account, string userName);
        void UppdateraNamnForAnvandare(string userId, string userName);
        void UppdateraKontaktnummerForAnvandare(string userId, string tfnnr);

        void UppdateraAktivFromForAnvandare(string userId);
        void UppdateraAnvandarInfo(ApplicationUser user);

        void UppdateraAdminAnvandare(AppUserAdmin user, string userName);

        void UppdateraOrganisationsenhet(Organisationsenhet orgUnit, string userName);
        void UppdateraOrganisationstyp(AdmOrganisationstyp orgtyp, string userName);

        void UppdateraArendetyp(Arendetyp caseType, string userName);

        void UppdateraArendeAnsvarig(ArendeAnsvarig caseManager, string userName);

        void UppdateraUppgiftsskyldighet(AdmUppgiftsskyldighet uppgSkyldighet, string userName);

        void UppdateraUndantagForvantadFil(List<UndantagForvantadfilDTO> undantagList, string userName);

        void UppdateraValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> registerList);
        void UppdateraValdaOrganisationsenheterForAnvandareOchDelreg(string userId, string userName, List<OrganisationsenhetDTO> orgenhetsList, int subdirId, int orgId);

        void UppdateraEnhetsUppgiftsskyldighet(AdmEnhetsUppgiftsskyldighet enhetsUppgSkyldighet, string userName);

        void UppdateraUppgiftsskyldighetOrganisationstyp(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtypes, List<OrganisationstypDTO> listOfOrgtypes, string userName);

        void UppdateraUppgiftsskyldighetForOrganisationer(AdmUppgiftsskyldighetOrganisationstypDTO subdirOrgtype, string userName);

        void UppdateraRollForKontaktpersoner(int orgId, int delregId);

        void UppdateraUppgiftsskyldighetOrganisationstyp(AdmUppgiftsskyldighetOrganisationstypDTO subdirOrgtype, string userName);

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

        void UppdateraPrivatEpostAdress(UndantagEpostadress privEpostDoman, string userName);

        void UppdateraArende(ArendeDTO arende, string userName);

        void SparaOppettider(OpeningHoursInfoDTO oppetTider, string userName);
        void SparaTillDatabasFillogg(string userName, string ursprungligtFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber);
        void SparaValdaRegistersForAnvandare(string userId, string userName, List<RegisterInfo> registerList);
        void SaveToLoginLog(string userid, string userName);

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

        bool RapporterarPerEnhet(int delregId, int orgId);

    }
}

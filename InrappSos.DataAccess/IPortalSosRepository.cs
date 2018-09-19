using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.DomainModel;

namespace InrappSos.DataAccess
{
    public interface IPortalAdminRepository
    {

        IEnumerable<Leverans> GetLeveranserForOrganisation(int orgId);

        Aterkoppling GetAterkopplingForLeverans(int levId);

        Organisation GetOrganisation(int orgId);

        Organisation GetOrganisationFromKommunkod(string kommunkod);

        string GetKommunkodForOrg(int orgId);

        Organisation GetOrgForUser(string userId);

        Organisation GetOrgForOrgUnit(int orgUnitId);

        Organisation GetOrgForReportObligation(int repObligationId);

        int GetUserOrganisationId(string userId);

        int GetOrgUnitOrganisationId(int orgUnitId);

        int GetReportObligationOrganisationId(int repObligationId);


        IEnumerable<ApplicationUser> GetContactPersonsForOrg(int orgId);
        IEnumerable<ApplicationUser> GetContactPersonsForOrgAndSubdir(int orgId, int subdirId);

        IEnumerable<AppUserAdmin> GetAdminUsers();

        IEnumerable<Organisationsenhet> GetOrgUnitsForOrg(int orgId);

        IEnumerable<AdmUppgiftsskyldighet> GetReportObligationInformationForOrg(int orgId);

        AdmUppgiftsskyldighet GetReportObligationInformationForOrgAndSubDir(int orgId, int subdirId);

        IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationInformationForOrgUnit(int orgUnitId);

        IEnumerable<AdmFAQKategori> GetFAQCategories();

        IEnumerable<AdmFAQ> GetFAQs(int faqCatId);
        IEnumerable<AdmHelgdag> GetAllHolidays();

        IEnumerable<AdmSpecialdag> GetAllSpecialDays();

        AdmFAQ GetFAQ(int faqId);

        AdmFAQKategori GetFAQCategory(int faqCatId);

        IEnumerable<AdmInformation> GetInformationTexts();

        IEnumerable<AdmKonfiguration> GetAdmConfiguration();

        AdmInformation GetInfoText(string infoType);

        AdmInformation GetInfoText(int infoId);

        AdmInsamlingsfrekvens GetInsamlingsfrekvens(int insamlingsid);

        int GetPageInfoTextId(string pageType);

        AdmRegister GetDirectoryByShortName(string shortName);

        AdmRegister GetDirectoryById(int dirId);

        AdmDelregister GetSubDirectoryById(int subdirId);

        IEnumerable<AdmRegister> GetDirectories();

        IEnumerable<AdmDelregister> GetSubDirectories();

        IEnumerable<AdmDelregister> GetSubDirectoriesForDirectory(int dirId);

        AdmDelregister GetSubDirectoryByShortName(string shortName);

        IEnumerable<AdmForvantadleverans> GetExpectedDeliveries();

        IEnumerable<AdmForvantadfil> GetAllExpectedFiles();

        IEnumerable<AdmFilkrav> GetAllFileRequirements();

        IEnumerable<AdmInsamlingsfrekvens> GetAllCollectionFrequencies();

        IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForDirectory(int dirId);
        IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForSubDirectory(int subdirId);
        IEnumerable<AdmFilkrav> GetFileRequirementsForDirectory(int dirId);

        IEnumerable<AdmFilkrav> GetFileRequirementsForSubDirectory(int subdirId);
        AdmFilkrav GetFileRequirementsForSubDirectoryAndFileReqId(int subdirId, int filereqId);

        IEnumerable<AdmForvantadfil> GetExpectedFilesForDirectory(int dirId);

        IEnumerable<AdmRegister> GetAllRegisters();

        IEnumerable<Organisation> GetAllOrganisations();

        IEnumerable<AdmRegister> GetAllRegistersForPortal();

        IEnumerable<AdmDelregister> GetAllSubDirectoriesForPortal();

        string GetDirectoryShortName(int dirId);

        string GetSubDirectoryShortNameForExpectedFile(int filKravId);

        string GetSubDirectoryShortName(int subDirId);

        string GetFileRequirementName(int filereqId);

        string GetEnhetskodForLeverans(int orgenhetsid);

        string GetPeriodForAktuellLeverans(int forvLevid);

        AdmForeskrift GetForeskriftByFileReq(int fileReqId);

        IEnumerable<LevereradFil> GetFilerForLeveransId(int leveransId);

        IEnumerable<Rapporteringsresultat> GetReportResultForSubdirAndPeriod(int delRegId, string period);

        IEnumerable<Rapporteringsresultat> GetReportResultForDirAndPeriod(int delRegId, string period);

        //IEnumerable<AdmRegister> GetDirectoriesForOrg(int orgId);

        IEnumerable<string> GetSubDirectoysPeriodsForAYear(int subdirId, int year);
        List<DateTime> GetTaskStartForSubdir(int subdirId);
        DateTime GetReportstartForRegisterAndPeriod(int dirId, string period);
        DateTime GetReportstartForRegisterAndPeriodSpecial(int dirId, string period);
        DateTime GetLatestReportDateForRegisterAndPeriod(int dirId, string period);
        DateTime GetLatestReportDateForRegisterAndPeriodSpecial(int dirId, string period);

        IEnumerable<AdmDelregister> GetSubdirsForDirectory(int dirId);

        AdmUppgiftsskyldighet GetReportObligationById(int repOblId);
        int GetExpextedDeliveryIdForSubDirAndPeriod(int subDirId, string period);

        AdmForvantadleverans GetExpectedDeliveryBySubDirAndFileReqIdAndPeriod(int subDirId, int fileReqId, string period);

        Leverans GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(int orgId, int subdirId, int forvlevId);

        Leverans GetLatestDeliveryForOrganisationSubDirectoryPeriodAndOrgUnit(int orgId, int subdirId, int forvlevId, int orgUnitId);

        string GetUserEmail(string userId);

        ApplicationUser GetUserByEmail(string email);

        IEnumerable<Roll> GetChosenDelRegistersForUser(string userId);

        IEnumerable<RegisterInfo> GetAllRegisterInformationForOrganisation(int orgId);

        AdmUppgiftsskyldighet GetUppgiftsskyldighetForOrganisationAndRegister(int orgId, int delregid);

        IEnumerable<Organisationsenhet> GetOrganisationUnits(int orgId);

        int CreateOrganisation(Organisation org);

        void CreateOrgUnit(Organisationsenhet orgUnit);

        void CreateFAQCategory(AdmFAQKategori faqCategory);

        void CreateFAQ(AdmFAQ faq);
        void CreateHoliday(AdmHelgdag holiday);
        void CreateSpecialDay(AdmSpecialdag specialDay);
        void CreateInformationText(AdmInformation infoText);
        void CreateReportObligation(AdmUppgiftsskyldighet uppgSk);

        void CreateUnitReportObligation(AdmEnhetsUppgiftsskyldighet enhetsUppgSk);

        void CreateDirectory(AdmRegister dir);

        void CreateSubDirectory(AdmDelregister subDir);

        void CreateExpectedDelivery(AdmForvantadleverans forvLev);

        void CreateExpectedFile(AdmForvantadfil forvFil);

        void CreateFileRequirement(AdmFilkrav filkrav);

        void CreateCollectFrequence(AdmInsamlingsfrekvens colFreq);

        void UpdateOrganisation(Organisation org);

        void UpdateContactPerson(ApplicationUser user);

        void UpdateAdminUser(AppUserAdmin user);
        void UpdateOrgUnit(Organisationsenhet orgUnit);

        void UpdateReportObligation(AdmUppgiftsskyldighet repObligation);

        void UpdateUnitReportObligation(AdmEnhetsUppgiftsskyldighet unitRepObligation);

        void UpdateFAQCategory(AdmFAQKategori faqCategory);

        void UpdateFAQ(AdmFAQ faq);

        void UpdateHoliday(AdmHelgdag holiday);

        void UpdateSpecialDay(AdmSpecialdag specialDay);

        void UpdateInfoText(AdmInformation infoText);

        void UpdateDirectory(AdmRegister directory);

        void UpdateSubDirectory(AdmDelregister subDirectory);

        void UpdateExpectedDelivery(AdmForvantadleverans forvLev);

        void UpdateExpectedFile(AdmForvantadfil forvFil);

        void UpdateFileRequirement(AdmFilkrav filkrav);

        void UpdateCollectFrequency(AdmInsamlingsfrekvens insamlingsfrekvens);

        void UpdateUserInfo(AppUserAdmin user);

        void SaveOpeningHours(AdmKonfiguration admKonf);

        void DeleteFAQCategory(int faqCategoryId);

        void DeleteFAQ(int faqId);

        void DeleteHoliday(int holidayId);

        void DeleteSpecialDay(int specialDayId);

        void DeleteContact(string contactId);

        void DeleteAdminUser(string userId);
    }
}

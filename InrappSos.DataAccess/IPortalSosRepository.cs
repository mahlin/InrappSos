using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.DomainModel;

namespace InrappSos.DataAccess
{
    public interface IPortalSosRepository
    {
        //****************** Metoder för AtridDB *************************//
        IEnumerable<AppUserAdmin> GetAdminUsers();
        string GetAdminUserEmail(string userId);
        void UpdateAdminUser(AppUserAdmin user);

        void UpdateAdminUserInfo(AppUserAdmin user);

        void DeleteAdminUser(string userId);

        //****************************************************************//

        void DisableContact(string userId);
        void EnableContact(string userId);
        IEnumerable<Leverans> GetLeveranserForOrganisation(int orgId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisation(int orgId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndUser(int orgId, string userId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndDelreg(int orgId, int delregId);

        IEnumerable<int> GetLeveransIdnForOrganisation(int orgId);

        int GetNewLeveransId(string userId, string userName, int orgId, int regId, int orgenhetsId, int forvLevId, string status);

        Aterkoppling GetAterkopplingForLeverans(int levId);

        Organisation GetOrganisation(int orgId);

        Organisation GetOrganisationFromKommunkod(string kommunkod);

        string GetKommunkodForOrganisation(int orgId);

        Organisation GetOrgForUser(string userId);

        Organisation GetOrgForEmailDomain(string modelEmailDomain);

        Organisation GetOrgForOrgUnit(int orgUnitId);

        Organisation GetOrgForReportObligation(int repObligationId);

        int GetUserOrganisationId(string userId);

        int GetOrgUnitOrganisationId(int orgUnitId);

        int GetReportObligationOrganisationId(int repObligationId);


        IEnumerable<ApplicationUser> GetContactPersonsForOrg(int orgId);
        IEnumerable<ApplicationUser> GetContactPersonsForOrgAndSubdir(int orgId, int subdirId);

        //IEnumerable<AppUserAdmin> GetAdminUsers();

        IEnumerable<Organisationsenhet> GetOrgUnitsForOrg(int orgId);

        List<int> GetOrgTypesForOrg(int orgId);

        int GetOrganisationsenhetsId(string orgUnitCode, int orgId);
        Organisationsenhet GetOrganisationUnitByCode(string code, int orgId);

        IEnumerable<AdmUppgiftsskyldighet> GetReportObligationInformationForOrg(int orgId);

        AdmUppgiftsskyldighet GetReportObligationInformationForOrgAndSubDir(int orgId, int subdirId);

        IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationInformationForOrgUnit(int orgUnitId);
        AdmEnhetsUppgiftsskyldighet GetUnitReportObligationForReportObligationAndOrg(int oblId, int orgunitId);

        IEnumerable<AdmOrganisationstyp> GetAllOrgTypes();
        IEnumerable<AdmFAQKategori> GetAllFAQs();

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

        List<AdmFilkrav> GetFileRequirementsAndExpectedFilesForSubDirectory(int subDirId);

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

        void GetPeriodsForAktuellLeverans(AdmFilkrav filkrav, RegisterFilkrav regfilkrav);

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

        IEnumerable<AdmForvantadfil> GetExpectedFile(int fileReq);

        Leverans GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(int orgId, int subdirId, int forvlevId);

        Leverans GetLatestDeliveryForOrganisationSubDirectoryPeriodAndOrgUnit(int orgId, int subdirId, int forvlevId, int orgUnitId);

        //string GetUserEmail(string userId);

        string GetUserName(string userId);
        string GetUserPhoneNumber(string userId);

        string GetUserContactNumber(string userId);

        ApplicationUser GetUserByEmail(string email);

        string GetUserEmail(string userId);
        IEnumerable<AdmRegister> GetChosenRegistersForUser(string userId);

        string GetClosedDays();
        string GetClosedFromHour();
        string GetClosedFromMin();
        string GetClosedToHour();
        string GetClosedToMin();
        string GetClosedAnnyway();

        IEnumerable<AdmHelgdag> GetHolidays();
        IEnumerable<AdmSpecialdag> GetSpecialDays();

        IEnumerable<Roll> GetChosenDelRegistersForUser(string userId);
        IEnumerable<RegisterInfo> GetAllRegisterInformation();

        IEnumerable<RegisterInfo> GetAllRegisterInformationForOrganisation(int orgId);

        AdmUppgiftsskyldighet GetUppgiftsskyldighetForOrganisationAndRegister(int orgId, int delregid);

        IEnumerable<Organisationsenhet> GetOrganisationUnits(int orgId);

        int CreateOrganisation(Organisation org);

        void CreateOrgUnit(Organisationsenhet orgUnit);
        void CreateOrgType(AdmOrganisationstyp orgType);

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
        void UpdateContactNumberForUser(string userId, string number);

        void UpdateActiveFromForUser(string userId);

        //void UpdateUserInfo(ApplicationUser user);
        void UpdateChosenRegistersForUser(string userId, string userName, List<RegisterInfo> registerList);

        //void UpdateAdminUser(AppUserAdmin user);
        void UpdateNameForUser(string userId, string userName);
        void UpdateOrgUnit(Organisationsenhet orgUnit);
        void UpdateOrgType(AdmOrganisationstyp orgType);

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

        void UpdateUserInfo(ApplicationUser user);

        void SaveOpeningHours(AdmKonfiguration admKonf);
        void SaveToFilelogg(string userName, string ursprungligtFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber);

        void SaveToLoginLog(string userid, string userName);

        List<List<Organisation>> SearchOrganisation(string[] searchString);
        void SaveChosenRegistersForUser(string userId, string userName, List<RegisterInfo> registerList);

        void DeleteFAQCategory(int faqCategoryId);

        void DeleteFAQ(int faqId);
        void DeleteOrgType(int orgTypeId);

        void DeleteHoliday(int holidayId);

        void DeleteSpecialDay(int specialDayId);

        void DeleteContact(string contactId);

        //void DeleteAdminUser(string userId);

        void DeleteChosenSubDirectoriesForUser(string userId);

        void DeleteDelivery(int deliveryId);
    }
}

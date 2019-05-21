using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity.EntityFramework;

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

        IEnumerable<IdentityRole> GetAllAstridRoles();

        IEnumerable<IdentityRole> GetAllFilipRoles();

        void CreateAstridRole(string roleName);
        void CreateAstridRolePermission(string roleId, int permissionId);

        void DeleteAstridRolePermission(string roleId, int permissionId);

        void CreateFilipRole(string roleName);

        IdentityRole GetAstridRole(string roleName);

        void UpdateAstridRole(IdentityRole role);

        void DeleteAstridRole(string roleName);

        IEnumerable<AspNetPermissions> GetAllAstridPermissions();

        IEnumerable<AspNetRolesPermissions> GetAstridRolesPermissions(string roleId);

        string GetAstridPermissionName(int permissionId);
        IEnumerable<int> GetAstridRolesPermissionIds(string roleId);

        //****************************************************************//

        UndantagEpostadress GetUserFromUndantagEpostadress(string email);
        IdentityRole GetFilipRole(string roleName);
        void UpdateFilipRole(IdentityRole role);
        void DeleteFilipRole(string roleName);

        Arende GetArende(string arendeNr);

        Arende GetArendeById(int arendeId);
        void DisableContact(string userId);
        void EnableContact(string userId);
        IEnumerable<Leverans> GetLeveranserForOrganisation(int orgId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisation(int orgId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndUser(int orgId, string userId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndDelreg(int orgId, int delregId);

        IEnumerable<int> GetLeveransIdnForOrganisation(int orgId);

        int GetNewLeveransId(string userId, string userName, int orgId, int regId, int orgenhetsId, int forvLevId, string status, int sftpAccountId);

        Aterkoppling GetAterkopplingForLeverans(int levId);

        Organisation GetOrganisation(int orgId);

        Organisation GetOrganisationFromKommunkod(string kommunkod);

        string GetKommunkodForOrganisation(int orgId);

        string GetLandstingskodForOrganisation(int orgId);

        string GetInrapporteringskodForOrganisation(int orgId);

        Organisation GetOrgForUser(string userId);

        Organisation GetOrgForEmailDomain(string modelEmailDomain);

        Organisation GetOrgForOrgUnit(int orgUnitId);

        Organisation GetOrgForReportObligation(int repObligationId);

        int GetUserOrganisationId(string userId);

        int GetOrgUnitOrganisationId(int orgUnitId);

        int GetReportObligationOrganisationId(int repObligationId);

        IEnumerable<UndantagEpostadress> GetPrivateEmailAdressesForOrg(int orgId);

        IEnumerable<UndantagForvantadfil> GetExceptionsExpectedFilesforOrg(int orgId);

        UndantagForvantadfil GetExceptionExpectedFile(int orgId, int subdirId, int expectedFielId);

        IEnumerable<UndantagEpostadress> GetPrivateEmailAdressesForOrgAndCase(int orgId, int caseId);

        IEnumerable<Arende> GetCasesForOrg(int orgId);

        IEnumerable<string> GetCaseReporterIds(int caseId);
        

        Arendetyp GetCaseType(int casetypeId);

        ArendeStatus GetCaseStatus(int casestatusId);

        IEnumerable<ApplicationUser> GetContactPersonsForOrg(int orgId);

        IEnumerable<ApplicationUser> GetActiveContactPersonsForOrg(int orgId);

        IEnumerable<SFTPkonto> GetSFTPAccountsForOrg(int orgId);

        IEnumerable<ApplicationUser> GetContactPersonsForSFTPAccount(int sftpAccountId);

        IEnumerable<ApplicationUser> GetContactPersonsForOrgAndSubdir(int orgId, int subdirId);

        //IEnumerable<AppUserAdmin> GetAdminUsers();

        IEnumerable<Organisationsenhet> GetOrgUnitsForOrg(int orgId);

        IEnumerable<Organisationsenhet> GetOrgUnitsByRepOblId(int repOblId);

        IEnumerable<Organisationsenhet> GetOrgUnitsByRepOblWithInPeriod(int repOblId, string period);

        List<int> GetOrgTypesIdsForOrg(int orgId);

        List<int> GetOrgTypesIdsForSubDir(int subdirId);

        AdmOrganisationstyp GetOrgtype(int orgtypeId);

        int GetOrganisationsenhetsId(string orgUnitCode, int orgId);
        Organisationsenhet GetOrganisationUnitByCode(string code, int orgId);

        Organisationsenhet GetOrganisationUnitByFileCode(string code, int orgId);

        IEnumerable<AdmUppgiftsskyldighet> GetReportObligationInformationForOrg(int orgId);

        AdmUppgiftsskyldighet GetReportObligationInformationForOrgAndSubDir(int orgId, int subdirId);

        AdmUppgiftsskyldighetOrganisationstyp GetReportObligationForSubDirAndOrgtype(int subdirId, int orgtypeId);

        IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationInformationForOrgUnit(int orgUnitId);
        AdmEnhetsUppgiftsskyldighet GetUnitReportObligationForReportObligationAndOrg(int oblId, int orgunitId);

        IEnumerable<AdmOrganisationstyp> GetAllOrgTypes();
        IEnumerable<AdmFAQKategori> GetAllFAQs();

        IEnumerable<AdmFAQKategori> GetFAQCategories();

        IEnumerable<AdmFAQ> GetFAQs(int faqCatId);
        IEnumerable<AdmHelgdag> GetAllHolidays();

        IEnumerable<AdmSpecialdag> GetAllSpecialDays();

        AdmFAQ GetFAQ(int faqId);

        AdmForeskrift GetRegulation(int regulationId);

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

        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetAllSubDirectoriesOrgtypes();

        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetOrgTypesForSubDir(int subdirId);

        IEnumerable<AdmRegister> GetDirectories();

        IEnumerable<AdmDelregister> GetSubDirectories();

        IEnumerable<AdmDelregister> GetSubDirectoriesForDirectory(int dirId);
        IEnumerable<AdmDelregister> GetSubDirectoriesWithIncludesForDirectory(int dirId);

        IEnumerable<AdmDelregister> GetSubDirsObligatedForOrg(int orgId);

        IEnumerable<AdmForeskrift> GetRegulationsForDirectory(int dirId);


        AdmDelregister GetSubDirectoryByShortName(string shortName);

        IEnumerable<AdmForvantadleverans> GetExpectedDeliveries();

        IEnumerable<LevereradFil> GetDeliveredFiles(int deliveryId);

        IEnumerable<AdmForvantadfil> GetAllExpectedFiles();

        IEnumerable<AdmFilkrav> GetAllFileRequirements();

        IEnumerable<string> GetAllFileMasks();

        IEnumerable<AdmInsamlingsfrekvens> GetAllCollectionFrequencies();

        IEnumerable<Arendetyp> GetAllCaseTypes();

        IEnumerable<ArendeStatus> GetAllCaseStatuses();

        IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForDirectory(int dirId);
        IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForSubDirectory(int subdirId);

        AdmDokument GetFile(int fileId);

        IEnumerable<AdmDokument> GetAllFiles();

        void SaveFile(AdmDokument file);

        void UpdateFile(AdmDokument file);

        void DeleteFile(AdmDokument file);

        IEnumerable<AdmFilkrav> GetFileRequirementsForDirectory(int dirId);

        AdmFilkrav GetFileRequirementById(int fileReqId);

        IEnumerable<AdmFilkrav> GetFileRequirementsForSubDirectory(int subdirId);
        AdmFilkrav GetFileRequirementsForSubDirectoryAndFileReqId(int subdirId, int filereqId);

        List<AdmFilkrav> GetFileRequirementsAndExpectedFilesForSubDirectory(int subDirId);

        IEnumerable<AdmForvantadfil> GetExpectedFilesForDirectory(int dirId);

        IEnumerable<AdmRegister> GetAllRegisters();

        IEnumerable<Organisation> GetAllOrganisations();

        IEnumerable<AdmRegister> GetAllRegistersForPortal();

        IEnumerable<AdmDelregister> GetAllSubDirectoriesForPortal();

        IEnumerable<AdmForeskrift> GetAllRegulations();

        string GetDirectoryShortName(int dirId);

        IEnumerable<LeveransstatusRapport> GetDeliveryStatusReport(int orgId, List<int> delregIdList, string period);

        string GetSubDirectoryShortNameForExpectedFile(int filKravId);

        string GetSubDirectoryShortName(int subDirId);

        string GetFileRequirementName(int filereqId);

        string GetEnhetskodForLeverans(int orgenhetsid);

        string GetPeriodForAktuellLeverans(int forvLevid);

        void GetPeriodsForAktuellLeverans(AdmFilkrav filkrav, RegisterFilkrav regfilkrav);

        AdmForeskrift GetForeskriftByFileReq(int fileReqId);

        AdmForeskrift GetForeskriftById(int foreskriftsId);

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

        ApplicationUser GetUserBySFTPAccountId(int ftpAccountId);

        SFTPkonto GetSFTPAccount(int ftpAccountId);

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

        SFTPkonto GetSFTPAccountByName(string name);

        IEnumerable<Roll> GetChosenDelRegistersForUser(string userId);
        //IEnumerable<RegisterInfo> GetAllRegisterInformation();

        IEnumerable<RegisterInfo> GetAllRegisterInformationForOrganisation(int orgId);

        AdmUppgiftsskyldighet GetUppgiftsskyldighetForOrganisationAndRegister(int orgId, int delregid);

        IEnumerable<Organisationsenhet> GetOrganisationUnits(int orgId);
        IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationForReportObligation(int uppgSkyldighetId);

        Organisationsenhet GetOrganisationUnit(int orgunitId);

        int CreateOrganisation(Organisation org, ICollection<Organisationstyp> orgtyperForOrg);

        int CreateSFTPAccount(SFTPkonto account, ICollection<KontaktpersonSFTPkonto> contacts);

        void CreateOrgUnit(Organisationsenhet orgUnit);

        void CreatePrivateEmail(UndantagEpostadress privEmail);

        void CreateCase(Arende arende);
        void CreateOrgType(AdmOrganisationstyp orgType);

        void CreateFAQCategory(AdmFAQKategori faqCategory);

        void CreateFAQ(AdmFAQ faq);
        void CreateHoliday(AdmHelgdag holiday);
        void CreateSpecialDay(AdmSpecialdag specialDay);
        void CreateInformationText(AdmInformation infoText);
        void CreateReportObligation(AdmUppgiftsskyldighet uppgSk);

        void CreateUnitReportObligation(AdmEnhetsUppgiftsskyldighet enhetsUppgSk);

        void CreateSubdirReportObligation(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtype);

        void CreateDirectory(AdmRegister dir);

        void CreateSubDirectory(AdmDelregister subDir);

        void CreateRegulation(AdmForeskrift regulation);

        void CreateExpectedDelivery(AdmForvantadleverans forvLev);

        void CreateExpectedFile(AdmForvantadfil forvFil);

        void CreateFileRequirement(AdmFilkrav filkrav);

        void CreateCollectFrequence(AdmInsamlingsfrekvens colFreq);

        void UpdateOrganisation(Organisation org);

        void UpdateContactPerson(ApplicationUser user);

        void UpdateSFTPAccount(SFTPkonto account);
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

        void UpdateRegulation(AdmForeskrift regulation);

        void UpdateExpectedDelivery(AdmForvantadleverans forvLev);

        void UpdateExpectedFile(AdmForvantadfil forvFil);

        void UpdateFileRequirement(AdmFilkrav filkrav);

        void UpdateCollectFrequency(AdmInsamlingsfrekvens insamlingsfrekvens);

        void UpdateUserInfo(ApplicationUser user);

        void UpdatePrivateEmail(UndantagEpostadress privEmail);

        void UpdateCase(Arende arende);

        void UpdateCaseReporters(int caseId, List<string> userIdList, string userName);

        void UpdateSubdirReportObligation(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtype);

        void AddRoleToFilipUser(string userId, string roleName);

        void UpdateCaseUnregisteredReporters(int caseId, List<UndantagEpostadress> userList, string userName);

        void SaveOpeningHours(AdmKonfiguration admKonf);
        void SaveToFilelogg(string userName, string ursprungligtFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber);

        void SaveToLoginLog(string userid, string userName);

        List<List<Organisation>> SearchOrganisation(string[] searchString);

        List<List<ApplicationUser>> SearchContact(string[] searchString);

        void SaveChosenRegistersForUser(string userId, string userName, List<RegisterInfo> registerList);

        void SaveExceptionExpectedFile(UndantagForvantadfil exception);

        void DeleteFAQCategory(int faqCategoryId);

        void DeleteFAQ(int faqId);
        void DeleteOrgType(int orgTypeId);

        void DeleteHoliday(int holidayId);

        void DeleteSpecialDay(int specialDayId);

        void DeleteContact(string contactId);

        //void DeleteAdminUser(string userId);

        void DeleteChosenSubDirectoriesForUser(string userId);

        void DeleteDelivery(int deliveryId);

        void DeleteExceptionExpectedFile(int orgId, int subdirId, int expectedFileId);

        void DeleteSubdirReportObligation(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtype);
    }
}

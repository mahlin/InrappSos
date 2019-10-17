using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        IEnumerable<ApplicationRole> GetAllAstridRoles();

        IEnumerable<ApplicationRole> GetAllFilipRoles();
        
        void CreateFilipRole(ApplicationRole filipRole);

        ApplicationRole GetAstridRole(string roleName);

        void UpdateAstridRole(ApplicationRole role);
        ApplicationRole GetAstridRoleById(string roleId);


        //****************************************************************//

        UndantagEpostadress GetUserFromUndantagEpostadress(string email);

        PreKontakt GetUserFromPreKontakt(string email);

        IEnumerable<ApplicationUserRole> GetFilipUserRolesForUser(string userId);

        ApplicationRole GetFilipRoleById(string roleId);
        ApplicationRole GetFilipRoleByName(string name);

        void UpdateFilipRole(ApplicationRole role);

        void DeleteFilipRole(string roleName);

        void DeleteAllNotRegistredContactsForCase(int caseId);

        void DeleteRoleFromFilipUser(string userId, string roleId);

        void DeleteContactFromCase(int caseId, string userId);

        Arende GetArende(string arendeNr);

        Arende GetArendeById(int arendeId);
        void DisableContact(string userId);
        void EnableContact(string userId);
        IEnumerable<Leverans> GetLeveranserForOrganisation(int orgId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisation(int orgId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndUser(int orgId, string userId);

        IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndDelreg(int orgId, int delregId);

        IEnumerable<Leverans> GetTop100LeveranserForOrganisationAndDelreg(int orgId, int delregId);

        IEnumerable<int> GetLeveransIdnForOrganisation(int orgId);

        int GetNewLeveransId(string userId, string userName, int orgId, int regId, int orgenhetsId, int forvLevId, string status, int sftpAccountId);

        int SaveFiledropFile(string filename, string sosFilename, Int64 filesize, int caseId, string userId, string userName);

        int SaveCaseContact(ArendeKontaktperson contact);

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

        //IEnumerable<RollOrganisationsenhet> GetUsersChosenOrgUnitsForSubdir(string userId, int subdirId);

        Roll GetRollForUserAndSubdir(string userId, int subdirId);

        RollOrganisationsenhet GetRollOrganisationsenhet(int rollId);

        int GetOrgUnitOrganisationId(int orgUnitId);

        int GetReportObligationOrganisationId(int repObligationId);

        IEnumerable<UndantagEpostadress> GetPrivateEmailAdressesForOrg(int orgId);

        IEnumerable<UndantagForvantadfil> GetExceptionsExpectedFilesforOrg(int orgId);

        UndantagForvantadfil GetExceptionExpectedFile(int orgId, int subdirId, int expectedFielId);

        IEnumerable<Arende> GetCasesForOrg(int orgId);

        List<Arende> GetCasesForContact(string userId);

        List<Arende> GetCasesForPreContact(int preContactId);

        List<Arende> GetCasesForCaseManager(int caseManagerId);

        List<Arende> GetCasesByCaseType(int caseTypeId);

        IEnumerable<string> GetCaseRegisteredContactIds(int caseId);

        IEnumerable<PreKontakt> GetCaseNotRegisteredContact(int caseId);

        ArendeAnsvarig GetCaseResponsible(int respId);

        IEnumerable<ArendeAnsvarig> GetAllCaseResponsibles();

        Arende GetCase(int caseId);

        Arendetyp GetCaseType(int casetypeId);

        IEnumerable<ArendeKontaktperson> GetCaseContacts(int caseId);
        IEnumerable<ApplicationUser> GetContactPersonsForOrg(int orgId);

        IEnumerable<ApplicationUser> GetActiveContactPersonsForOrg(int orgId);

        IEnumerable<SFTPkonto> GetSFTPAccountsForOrg(int orgId);

        IEnumerable<ApplicationUser> GetContactPersonsForSFTPAccount(int sftpAccountId);

        IEnumerable<ApplicationUser> GetContactPersonsForOrgAndSubdir(int orgId, int subdirId);

        //IEnumerable<AppUserAdmin> GetAdminUsers();

        IEnumerable<Organisationsenhet> GetOrgUnitsForOrg(int orgId);

        IEnumerable<Organisationsenhet> GetOrgUnitsByRepOblId(int repOblId);

        IEnumerable<Organisationsenhet> GetActiveOrgUnitsByRepOblId(int repOblId);

        IEnumerable<Organisationsenhet> GetOrgUnitsByRepOblWithInPeriod(int repOblId, string period);

        List<int> GetOrgTypesIdsForOrg(int orgId);

        List<int> GetOrgTypesIdsForSubDir(int subdirId);

        IEnumerable<int> GetSubdirIdsForOrgtype(int orgtypeId);

        AdmOrganisationstyp GetOrgtype(int orgtypeId);
        AdmOrganisationstyp GetOrgtypeByName(string orgtypeName);

        int GetOrganisationsenhetsId(string orgUnitCode, int orgId);
        Organisationsenhet GetOrganisationUnitByCode(string code, int orgId);

        Organisationsenhet GetOrganisationUnitByFileCode(string code, int orgId);

        IEnumerable<AdmUppgiftsskyldighet> GetReportObligationInformationForOrg(int orgId);

        IEnumerable<AdmUppgiftsskyldighet> GetActiveReportObligationInformationForOrg(int orgId);

        AdmUppgiftsskyldighet GetReportObligationInformationForOrgAndSubDir(int orgId, int subdirId);

        AdmUppgiftsskyldighet GetActiveReportObligationInformationForOrgAndSubDir(int orgId, int subdirId);

        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetAllActiveReportObligationsForSubDir(int subdirId);
        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetAllInactiveReportObligationsForSubDir(int subdirId);


        AdmUppgiftsskyldighetOrganisationstyp GetReportObligationForSubDirAndOrgtype(int subdirId, int orgtypeId, int subdirorgtypeId);

        IEnumerable<AdmUppgiftsskyldighet> GetAllReportObligationsForSubDirAndOrg(int subdirId, int orgId);

        IEnumerable<AdmUppgiftsskyldighet> GetAllActiveReportObligationsForSubDirAndOrg(int subdirId, int orgId);

        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetReportObligationForOrgtype(int orgtypeId);
        IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetActiveReportObligationForOrgtype(int orgtypeId);
        IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationInformationForOrgUnit(int orgUnitId);
        AdmEnhetsUppgiftsskyldighet GetUnitReportObligationForReportObligationAndOrg(int oblId, int orgunitId);

        AdmEnhetsUppgiftsskyldighet GetActiveUnitReportObligationForReportObligationAndOrg(int oblId, int orgunitId);

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

        IEnumerable<Inloggning> GetLogins(string userId);

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
        AdmDelregister GetSubDirectoryWithIncludes(int subdirId);
        IEnumerable<AdmDelregister> GetSubDirsObligatedForOrg(int orgId);

        IEnumerable<AdmDelregister> GetActiveSubDirsObligatedForOrg(int orgId);

        IEnumerable<AdmForeskrift> GetRegulationsForDirectory(int dirId);


        AdmDelregister GetSubDirectoryByShortName(string shortName);

        IEnumerable<AdmForvantadleverans> GetExpectedDeliveries();

        IEnumerable<LevereradFil> GetDeliveredFiles(int deliveryId);

        IEnumerable<DroppadFil> GetDroppedFilesForCase(int caseId);

        IEnumerable<AdmForvantadfil> GetAllExpectedFiles();

        IEnumerable<AdmFilkrav> GetAllFileRequirements();

        IEnumerable<string> GetAllFileMasks();

        IEnumerable<AdmInsamlingsfrekvens> GetAllCollectionFrequencies();

        IEnumerable<Arendetyp> GetAllCaseTypes();

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

        IEnumerable<Leverans> GetDeliveryStatusReport(int orgId, List<int> delregIdList, List<int> forvlevIdList);

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

        IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForSubdirsAndPeriods(List<int> subdirIds, List<string> periods);

        IEnumerable<AdmForvantadfil> GetExpectedFile(int fileReq);

        Leverans GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(int orgId, int subdirId, int forvlevId);

        Leverans GetLatestDeliveryForOrganisationSubDirectoryPeriodAndOrgUnit(int orgId, int subdirId, int forvlevId, int orgUnitId);

        //string GetUserEmail(string userId);

        string GetUserName(string userId);
        string GetUserPhoneNumber(string userId);

        string GetUserContactNumber(string userId);

        IEnumerable<Arende> GetUserCases(string userId);

        IEnumerable<Leverans> GetUserDeliveries(string userId);

        ApplicationUser GetUserByEmail(string email);

        ApplicationUser GetUserById(string userId);

        ApplicationUser GetUserBySFTPAccountId(int ftpAccountId);

        ApplicationUser GetFirstUserForSFTPAccount(int ftpAccountId);

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
        IEnumerable<RegisterInfo> GetAllActiveRegisterInformationForOrganisation(int orgId);

        AdmUppgiftsskyldighet GetUppgiftsskyldighetForOrganisationAndRegister(int orgId, int delregid);

        AdmUppgiftsskyldighet GetActiveUppgiftsskyldighetForOrganisationAndRegister(int orgId, int delregid);

        IEnumerable<Organisationsenhet> GetOrganisationUnits(int orgId);
        IEnumerable<Organisation> GetOrgByOrgtype(int orgtypeId);
        IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationForReportObligation(int uppgSkyldighetId);

        IEnumerable<AdmEnhetsUppgiftsskyldighet> GetActiveUnitReportObligationForReportObligation(int uppgSkyldighetId);

        Organisationsenhet GetOrganisationUnit(int orgunitId);

        int CreateOrganisation(Organisation org, ICollection<Organisationstyp> orgtyperForOrg);

        int CreateSFTPAccount(SFTPkonto account, ICollection<KontaktpersonSFTPkonto> contacts);

        void CreateOrgUnit(Organisationsenhet orgUnit);

        void CreatePrivateEmail(UndantagEpostadress privEmail);

        void CreatePreKontakt(PreKontakt preContact);

        int CreateCase(Arende arende);
        void CreateOrgType(AdmOrganisationstyp orgType);

        void CreateCaseType(Arendetyp caseType);

        void CreateCaseManager(ArendeAnsvarig caseManager);

        void CreateFAQCategory(AdmFAQKategori faqCategory);

        void CreateFAQ(AdmFAQ faq);
        void CreateHoliday(AdmHelgdag holiday);
        void CreateSpecialDay(AdmSpecialdag specialDay);
        void CreateInformationText(AdmInformation infoText);
        void CreateReportObligation(AdmUppgiftsskyldighet uppgSk);

        void CreateReportObligationForOrgtype(AdmUppgiftsskyldighetOrganisationstyp uppgSk);

        void CreateRollOrganisationsenhet(RollOrganisationsenhet rollOrgUnit);

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
        void UpdateChosenOrgUnitsForUserAndSubdir (string userId, string userName, List<int> orgUnitIdList, int rollId, List<Organisationsenhet> availableOrgUnits);

        void UpdateNameForUser(string userId, string userName);
        void UpdateOrgUnit(Organisationsenhet orgUnit);
        void UpdateOrgType(AdmOrganisationstyp orgType);

        void UpdateCaseType(Arendetyp caseType);

        void UpdateCaseManager(ArendeAnsvarig caseManager);

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

        void AddRoleToFilipUser(ApplicationUserRole userRole);

        void SaveOpeningHours(AdmKonfiguration admKonf);

        void SetCaseContact(ArendeKontaktperson caseContact);

        void SaveToFilelogg(string userName, string ursprungligtFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber);

        void SaveToLoginLog(string userid, string userName);

        List<List<Organisation>> SearchOrganisation(string[] searchString);

        List<List<Arende>> SearchCase(string[] searchString);

        List<List<ApplicationUser>> SearchContact(string[] searchString);

        List<List<PreKontakt>> SearchPreContact(string[] searchString);
        List<List<ArendeAnsvarig>> SearchCaseManager(string[] searchString);

        List<List<Arendetyp>> SearchCaseType(string[] searchString);

        void SetAstridRoleForAstridUser(ApplicationUserRole appUserRole);

        void SetFilipRoleForFilipUser(ApplicationUserRole applicationUserRole);

        IEnumerable<ApplicationRole> GetAstridUsersRoles(string userId);
        IEnumerable<ApplicationUserRole> GetAstridUsersInRole(string roleId);
        AppUserAdmin GetAstridUserById(string userId);

        void SaveChosenRegistersForUser(string userId, string userName, List<RegisterInfo> registerList);

        void SaveExceptionExpectedFile(UndantagForvantadfil exception);

        void DeleteFAQCategory(int faqCategoryId);

        void DeleteFAQ(int faqId);

        void DeletePreKontakt(int prekontaktId);
        void DeleteOrgType(int orgTypeId);

        void DeleteHoliday(int holidayId);

        void DeleteSpecialDay(int specialDayId);

        void DeleteContact(string contactId);

        void DeleteCaseContact(string contactId, int caseId);

        void DeleteChosenSubDirectoriesForUser(string userId);

        void DeleteChosenSubDirectoryForUser(string userId, int dirId);

        void DeleteDelivery(int deliveryId);

        void DeleteExceptionExpectedFile(int orgId, int subdirId, int expectedFileId);

        void DeleteSubdirReportObligation(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtype);
    }
}

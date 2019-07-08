using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace InrappSos.DataAccess
{
    public class PortalSosRepository : IPortalSosRepository
    {

        private InrappSosDbContext DbContext { get; }
        private InrappSosAstridDbContext AstridDbContext { get; }


        public PortalSosRepository(InrappSosDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public PortalSosRepository(InrappSosDbContext dbContext, InrappSosAstridDbContext astridDbContext)
        {
            DbContext = dbContext;
            AstridDbContext = astridDbContext;
        }


        //************************ Metoder för AstridDB *****************************************************************//
        public IEnumerable<AppUserAdmin> GetAdminUsers()
        {
            var adminUsers = AstridDbContext.Users.OrderBy(x => x.Email).ToList();
            return adminUsers;
        }

        public string GetAdminUserEmail(string userId)
        {
            var email = AstridDbContext.Users.Where(x => x.Id == userId).Select(x => x.Email).SingleOrDefault();
            return email;
        }

        public void UpdateAdminUser(AppUserAdmin user)
        {
            var usrDb = AstridDbContext.Users.Where(u => u.Id == user.Id).Select(u => u).SingleOrDefault();
            usrDb.PhoneNumber = user.PhoneNumber;
            usrDb.Email = user.Email;
            usrDb.AndradDatum = user.AndradDatum;
            usrDb.AndradAv = user.AndradAv;
            AstridDbContext.SaveChanges();
        }

        public void UpdateAdminUserInfo(AppUserAdmin user)
        {
            var userDb = AstridDbContext.Users.SingleOrDefault(x => x.Id == user.Id);
            userDb.AndradAv = user.AndradAv;
            userDb.AndradDatum = user.AndradDatum;
            AstridDbContext.SaveChanges();
        }

        public void DeleteAdminUser(string userId)
        {
            var cuserToDelete = AstridDbContext.Users.SingleOrDefault(x => x.Id == userId);
            AstridDbContext.Users.Remove(cuserToDelete);
            AstridDbContext.SaveChanges();
        }

        public IEnumerable<IdentityRole> GetAllAstridRoles()
        {
            var astridRoles = AstridDbContext.Roles.OrderBy(r => r.Name).ToList();
            return astridRoles;
            //AstridDbContext.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
            //    new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

        }

        public IEnumerable<IdentityRole> GetAllFilipRoles()
        {
            var filipRoles = DbContext.Roles.OrderBy(r => r.Name).ToList();
            return filipRoles;
        }

        public void CreateAstridRole(string roleName)
        {
            AstridDbContext.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
            {
                Name = roleName
            });
            AstridDbContext.SaveChanges();
        }

        public void CreateAstridRolePermission(string roleId, int permissionId)
        {
            AstridDbContext.AspNetRolesPermissions.Add(new AspNetRolesPermissions()
            {
                PermissionId = permissionId,
                RoleId = roleId
            });
            AstridDbContext.SaveChanges();
        }

        public void DeleteAstridRolePermission(string roleId, int permissionId)
        {
            var thisPermission = AstridDbContext.AspNetRolesPermissions.FirstOrDefault(r => r.RoleId.Equals(roleId) && r.PermissionId == permissionId);
            AstridDbContext.AspNetRolesPermissions.Remove(thisPermission);
            AstridDbContext.SaveChanges();
        }

        public void CreateFilipRole(string roleName)
        {
            DbContext.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
            {
                Name = roleName
            });
            DbContext.SaveChanges();
        }

        public IdentityRole GetAstridRole(string roleName)
        {
            var thisRole = AstridDbContext.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
            return thisRole;
        }

        public void UpdateAstridRole(IdentityRole role)
        {
            AstridDbContext.Entry(role).State = System.Data.Entity.EntityState.Modified;
            AstridDbContext.SaveChanges();
        }

        public void DeleteAstridRole(string roleName)
        {
            var thisRole = AstridDbContext.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
            AstridDbContext.Roles.Remove(thisRole);
            AstridDbContext.SaveChanges();
        }

        public IEnumerable<AspNetPermissions> GetAllAstridPermissions()
        {
            var permissions = AstridDbContext.AspNetPermissions.ToList();
            return permissions;
        }

        public IEnumerable<AspNetRolesPermissions> GetAstridRolesPermissions(string roleId)
        {
            var permissionIdsForRole = AstridDbContext.AspNetRolesPermissions.Where(x => x.RoleId == roleId).ToList();
            return permissionIdsForRole;
        }

        public string GetAstridPermissionName(int permissionId)
        {
            var permissionName = AstridDbContext.AspNetPermissions.Where(x => x.Id == permissionId).Select(x => x.PermissionName).SingleOrDefault();
            return permissionName;
        }

        public IEnumerable<int> GetAstridRolesPermissionIds(string roleId)
        {
            var permissionIdsForRole = AstridDbContext.AspNetRolesPermissions.Where(x => x.RoleId == roleId).Select(x => x.PermissionId).ToList();
            return permissionIdsForRole;
        }

        public UndantagEpostadress GetUserFromUndantagEpostadress(string email)
        {
            var userException = DbContext.UndantagEpostadress.FirstOrDefault(x => x.PrivatEpostAdress == email);
            return userException;
        }

        public IdentityRole GetFilipRole(string roleName)
        {
            var thisRole = DbContext.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
            return thisRole;
        }

        public void UpdateFilipRole(IdentityRole role)
        {
            DbContext.Entry(role).State = System.Data.Entity.EntityState.Modified;
            DbContext.SaveChanges();
        }

        public void DeleteFilipRole(string roleName)
        {
            var thisRole = DbContext.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
            DbContext.Roles.Remove(thisRole);
            DbContext.SaveChanges();
        }

        public Arende GetArende(string arendeNr)
        {
            var arende = DbContext.Arende.SingleOrDefault(x => x.Arendenr == arendeNr);
            return arende;
        }

        public Arende GetArendeById(int arendeId)
        {
            var arende = DbContext.Arende.SingleOrDefault(x => x.Id == arendeId);
            return arende;
        }

        //*************************************************************************************************************************//

        private IEnumerable<Leverans> AllaLeveranser()
        {
            return DbContext.Leverans;
        }

        public void DisableContact(string userId)
        {
            var contactDb = DbContext.Users.SingleOrDefault(x => x.Id == userId);
            contactDb.AktivTom = DateTime.Now;
            DbContext.SaveChanges();
        }

        public void EnableContact(string userId)
        {
            var contactDb = DbContext.Users.SingleOrDefault(x => x.Id == userId);
            contactDb.AktivTom = null;
            DbContext.SaveChanges();
        }

        public IEnumerable<Leverans> GetLeveranserForOrganisation(int orgId)
        {
            var levIdnForOrg = AllaLeveranser().Where(a => a.OrganisationId == orgId).ToList();
            return levIdnForOrg;
        }

        public IEnumerable<Leverans> GetTop10LeveranserForOrganisation(int orgId)
        {
            var levIdnForOrg = DbContext.Leverans.Where(a => a.OrganisationId == orgId).OrderByDescending(a => a.Leveranstidpunkt).Take(10).ToList();
            return levIdnForOrg;
        }

        public IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndUser(int orgId, string userId)
        {
            var levIdnForOrg = DbContext.Leverans.Where(a => a.OrganisationId == orgId && a.ApplicationUserId == userId).OrderByDescending(a => a.Leveranstidpunkt).Take(10).ToList();
            return levIdnForOrg;
        }

        public IEnumerable<Leverans> GetTop10LeveranserForOrganisationAndDelreg(int orgId, int delregId)
        {
            var levIdnForOrg = DbContext.Leverans.Where(a => a.OrganisationId == orgId && a.DelregisterId == delregId).OrderByDescending(a => a.Leveranstidpunkt).Take(10).ToList();
            return levIdnForOrg;
        }

        public IEnumerable<int> GetLeveransIdnForOrganisation(int orgId)
        {
            var levIdnForOrg = AllaLeveranser().Where(a => a.OrganisationId == orgId).Select(a => a.Id).ToList();
            return levIdnForOrg;
        }


        public int SaveFiledropFile(string filename, string sosFilename, int caseId, string userId, string userName)
        {
            var filedrop = new DroppadFil()
            {
                ArendeId = caseId,
                ApplicationUserId = userId,
                Filnamn = filename,
                NyttFilnamn = sosFilename,
                SkapadDatum = DateTime.Now,
                SkapadAv = userName,
                AndradDatum = DateTime.Now,
                AndradAv = userName
            };

            DbContext.DroppadFil.Add(filedrop);

            DbContext.SaveChanges();
            return filedrop.Id;
        }

        public Aterkoppling GetAterkopplingForLeverans(int levId)
        {
            var aterkoppling = DbContext.Aterkoppling.FirstOrDefault(x => x.LeveransId == levId);
            return aterkoppling;
        }

        public string GetEnhetskodForLeverans(int orgenhetsid)
        {
            var enhetskod = DbContext.Organisationsenhet.Where(x => x.Id == orgenhetsid).Select(x => x.Enhetskod).SingleOrDefault();
            return enhetskod;
        }

        public string GetPeriodForAktuellLeverans(int forvLevid)
        {
            var period = DbContext.AdmForvantadleverans.Where(x => x.Id == forvLevid).Select(x => x.Period).SingleOrDefault();
            return period;
        }

        public AdmForeskrift GetForeskriftById(int foreskriftsId)
        {
            var foreskrift = DbContext.AdmForeskrift.SingleOrDefault(x => x.Id == foreskriftsId);
            return foreskrift;
        }

        public IEnumerable<LevereradFil> GetFilerForLeveransId(int leveransId)
        {
            
            var filInfo = DbContext.LevereradFil.Where(a => a.LeveransId == leveransId).OrderByDescending(x => x.LeveransId); ;

            return filInfo;
        }


        public Organisation GetOrganisationFromKommunkod(string kommunkod)
        {
            var org = DbContext.Organisation.Single(a => a.Kommunkod == kommunkod);
            return org;
        }

        public Organisation GetOrganisation(int orgId)
        {
            var org = DbContext.Organisation.Where(x => x.Id == orgId).Include(x=> x.Organisationstyp).Select(x => x).SingleOrDefault();
            return org;
        }

        public string GetLandstingskodForOrganisation(int orgId)
        {
            var landstingskod = DbContext.Organisation.Where(x => x.Id == orgId).Select(x => x.Landstingskod).SingleOrDefault();
            return landstingskod;
        }

        public string GetInrapporteringskodForOrganisation(int orgId)
        {
            var inrapporteringskod = DbContext.Organisation.Where(x => x.Id == orgId).Select(x => x.Inrapporteringskod).SingleOrDefault();
            return inrapporteringskod;
        }

        public Organisation GetOrgForUser(string userId)
        {
            var orgId = GetUserOrganisationId(userId);
            var org = DbContext.Organisation.Where(o => o.Id == orgId).Select(o => o).FirstOrDefault();
            return org;
        }

        public Organisation GetOrgForEmailDomain(string modelEmailDomain)
        {
            var organisation = DbContext.Organisation.Where(a => a.Epostdoman == modelEmailDomain).Select(o => o).FirstOrDefault();
            return organisation;
        }

        public Organisation GetOrgForOrgUnit(int orgUnitId)
        {
            var orgId = GetOrgUnitOrganisationId(orgUnitId);
            var org = DbContext.Organisation.Where(o => o.Id == orgId).Select(o => o).FirstOrDefault();

            return org;
        }

        public Organisation GetOrgForReportObligation(int repObligationId)
        {
            var orgId = GetReportObligationOrganisationId(repObligationId);
            var org = DbContext.Organisation.Where(o => o.Id == orgId).Select(o => o).FirstOrDefault();

            return org;

        }

        public int GetUserOrganisationId(string userId)
        {
            var orgId = DbContext.Users.Where(u => u.Id == userId).Select(o => o.OrganisationId).SingleOrDefault();
            return orgId;
        }

        public int GetOrgUnitOrganisationId(int orgUnitId)
        {
            var orgId = DbContext.Organisationsenhet.Where(u => u.Id == orgUnitId).Select(o => o.OrganisationsId).SingleOrDefault();
            return orgId;
        }

        public int GetReportObligationOrganisationId(int repObligationId)
        {
            var orgId = DbContext.AdmUppgiftsskyldighet.Where(u => u.Id == repObligationId).Select(o => o.OrganisationId).SingleOrDefault();
            return orgId;
        }

        public IEnumerable<UndantagEpostadress> GetPrivateEmailAdressesForOrg(int orgId)
        {
            var privEmails = DbContext.UndantagEpostadress.Where(x => x.OrganisationsId == orgId).ToList();
            return privEmails;
        }

        public IEnumerable<UndantagForvantadfil> GetExceptionsExpectedFilesforOrg(int orgId)
        {
            var exceptionsExpectedFiles = DbContext.UndantagForvantadfil.Where(x => x.OrganisationsId == orgId).ToList();
            return exceptionsExpectedFiles;
        }

        public UndantagForvantadfil GetExceptionExpectedFile(int orgId, int subdirId, int expectedFielId)
        {
            var exception = DbContext.UndantagForvantadfil
                .SingleOrDefault(x => x.OrganisationsId == orgId && x.DelregisterId == subdirId && x.ForvantadfilId == expectedFielId);
            return exception;
        }

        public IEnumerable<UndantagEpostadress> GetPrivateEmailAdressesForOrgAndCase(int orgId, int caseId)
        {
            var privEmails = DbContext.UndantagEpostadress.Where(x => x.OrganisationsId == orgId && x.ArendeId == caseId).ToList();
            return privEmails;
        }

        public IEnumerable<Arende> GetCasesForOrg(int orgId)
        {
            var cases = DbContext.Arende.Where(x => x.OrganisationsId == orgId).ToList();
            return cases;
        }

        public IEnumerable<string> GetCaseReporterIds(int caseId)
        {
            var caseRepIds = DbContext.ArendeKontaktperson.Where(x => x.ArendeId == caseId).Select(x => x.ApplicationUserId).ToList();
            return caseRepIds;
        }

        public Arende GetCase(int caseId)
        {
            var arende = DbContext.Arende.SingleOrDefault(x => x.Id == caseId);
            return arende;
        }

        public Arendetyp GetCaseType(int casetypeId)
        {
            var caseType = DbContext.Arendetyp.SingleOrDefault(x => x.Id == casetypeId);
            return caseType;
        }

        public ArendeStatus GetCaseStatus(int casestatusId)
        {
            var caseStatus = DbContext.ArendeStatus.SingleOrDefault(x => x.Id == casestatusId);
            return caseStatus;
        }

        public AdmUppgiftsskyldighet GetReportObligationById(int repOblId)
        {
            var repObl = DbContext.AdmUppgiftsskyldighet.SingleOrDefault(x => x.Id == repOblId);
            return repObl;
        }

        public IEnumerable<ApplicationUser> GetContactPersonsForOrg(int orgId)
        {
            var contacts = DbContext.Users.Where(x => x.OrganisationId == orgId).ToList();
            //var contacts = DbContext.Users.Where(x => x.OrganisationId == orgId).ToList().OrderBy(x => x.AktivTom);
            return contacts;
        }

        public IEnumerable<ApplicationUser> GetActiveContactPersonsForOrg(int orgId)
        {
            var today = DateTime.Now.Date;
            var activeContacts = new List<ApplicationUser>();
            var contacts = DbContext.Users.Where(x => x.OrganisationId == orgId).ToList();

            foreach (var contact in contacts)
            {
                var tmp = contact.AktivTom;
               if (contact.AktivTom == null) 
                    activeContacts.Add(contact);
               else if (contact.AktivTom >= DateTime.Now.Date)
                    activeContacts.Add(contact);
            }
            return activeContacts;
        }

        public IEnumerable<SFTPkonto> GetSFTPAccountsForOrg(int orgId)
        {
            var accounts = DbContext.SFTPkonto.Where(x => x.OrganisationsId == orgId).ToList();
            return accounts;
        }

        public IEnumerable<ApplicationUser> GetContactPersonsForSFTPAccount(int sftpAccountId)
        {
            var contactsList = new List<ApplicationUser>();
            var contactIds = DbContext.KontaktpersonSFTPkonto.Where(x => x.SFTPkontoId == sftpAccountId).Select(x => x.ApplicationUserId).ToList();
            foreach (var contactId in contactIds)
            {
                var contact = DbContext.Users.SingleOrDefault(x => x.Id == contactId);
                if (contact != null)
                    contactsList.Add(contact);
            }
            return contactsList;
        }

        public IEnumerable<ApplicationUser> GetContactPersonsForOrgAndSubdir(int orgId, int subdirId)
        {
            var contactList = new List<ApplicationUser>();
            var contacts = DbContext.Users.Where(x => x.OrganisationId == orgId).ToList();
            foreach (var contact in contacts)
            {
                var role = DbContext.Roll.FirstOrDefault(x => x.ApplicationUserId == contact.Id && x.DelregisterId == subdirId);
                if (role != null)
                {
                    contactList.Add(contact);
                }
            }
            return contactList;
        }

        //public IEnumerable<AppUserAdmin> GetAdminUsers()
        //{
        //    var adminUsers = IdentityDbContext.Users.ToList();
        //    return adminUsers;
        //}

        public IEnumerable<Organisationsenhet> GetOrgUnitsForOrg(int orgId)
        {
            var orgUnits = DbContext.Organisationsenhet.Where(x => x.OrganisationsId == orgId).ToList();
            return orgUnits;
        }

        public IEnumerable<Organisationsenhet> GetOrgUnitsByRepOblId(int repOblId)
        {
            var orgUnitRepOligations = DbContext.AdmEnhetsUppgiftsskyldighet.Where(x => x.UppgiftsskyldighetId == repOblId).ToList();
            var orgUnits = new List<Organisationsenhet>();
            foreach (var orgUnitRepObligation in orgUnitRepOligations)
            {
                var orgUnit = DbContext.Organisationsenhet.SingleOrDefault(x => x.Id == orgUnitRepObligation.OrganisationsenhetsId);
                if (orgUnit != null)
                {
                    orgUnits.Add(orgUnit);
                }
            }

            return orgUnits;
        }

        public IEnumerable<Organisationsenhet> GetOrgUnitsByRepOblWithInPeriod(int repOblId, string period)
        {
            var orgUnitRepOligations = DbContext.AdmEnhetsUppgiftsskyldighet.Where(x => x.UppgiftsskyldighetId == repOblId).ToList();
            var year = period.Substring(0, 4);
            var month = "01";
            if (period.Length == 6)
            {
                month = period.Substring(4, 2);
            }
            var periodDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
            var activeUnitRepObligations = new List<AdmEnhetsUppgiftsskyldighet>();
            //var from = 
            //Check if still active reportobligation
            foreach (var orgUnitRepOligation in orgUnitRepOligations)
            {
                if (orgUnitRepOligation.SkyldigFrom <= periodDate)
                {
                    if (orgUnitRepOligation.SkyldigTom != null)
                    {
                        if (orgUnitRepOligation.SkyldigTom.Value >= periodDate)
                        {
                            activeUnitRepObligations.Add(orgUnitRepOligation);
                        }
                    }
                    else
                    {
                        activeUnitRepObligations.Add(orgUnitRepOligation);
                    }
                }
            }
            var orgUnits = new List<Organisationsenhet>();
            foreach (var orgUnitRepObligation in activeUnitRepObligations)
            {
                var orgUnit = DbContext.Organisationsenhet.SingleOrDefault(x => x.Id == orgUnitRepObligation.OrganisationsenhetsId);
                if (orgUnit != null)
                {
                    orgUnits.Add(orgUnit);
                }
            }
            return orgUnits;
        }

        public List<int> GetOrgTypesIdsForOrg(int orgId)
        {
            var orgTypesIdsForOrg = DbContext.Organisationstyp.Where(x => x.OrganisationsId == orgId).Select(x => x.OrganisationstypId).ToList();
            return orgTypesIdsForOrg;
        }

        public List<int> GetOrgTypesIdsForSubDir(int subdirId)
        {
            var orgTypesIdsForSubDir = new List<int>();
            var today = DateTime.Now;
            var uppgiftsskyldighetOrgtypeList = DbContext.AdmUppgiftsskyldighetOrganisationstyp.Where(x => x.DelregisterId == subdirId && x.SkyldigFrom <= today).ToList();
            foreach (var item in uppgiftsskyldighetOrgtypeList)
            {
                if (item.SkyldigTom != null)
                {
                    //If date passed, exlude from list
                    if (item.SkyldigTom >= today) 
                    {
                        orgTypesIdsForSubDir.Add(item.OrganisationstypId);
                    }
                }
                else
                {
                    orgTypesIdsForSubDir.Add(item.OrganisationstypId);
                }
            }
            return orgTypesIdsForSubDir;
        }

        public AdmOrganisationstyp GetOrgtype(int orgtypeId)
        {
            var orgtype = DbContext.AdmOrganisationstyp.Where(x => x.Id == orgtypeId).SingleOrDefault();
            return orgtype;
        }

        public int GetOrganisationsenhetsId(string orgUnitCode, int orgId)
        {
            var orgenhetsId = DbContext.Organisationsenhet.Where(x => x.Enhetskod == orgUnitCode && x.OrganisationsId == orgId).Select(x => x.Id).FirstOrDefault();
            return orgenhetsId;
        }

        public Organisationsenhet GetOrganisationUnitByCode(string code, int orgId)
        {
            var orgUnit = DbContext.Organisationsenhet.SingleOrDefault(x => x.Enhetskod == code && x.OrganisationsId == orgId);
            return orgUnit;
        }

        public Organisationsenhet GetOrganisationUnitByFileCode(string code, int orgId)
        {
            var orgUnit = DbContext.Organisationsenhet.SingleOrDefault(x => x.Filkod == code && x.OrganisationsId == orgId);
            return orgUnit;
        }

        public IEnumerable<AdmUppgiftsskyldighet> GetReportObligationInformationForOrg(int orgId)
        {
            var reportObligationInfo = DbContext.AdmUppgiftsskyldighet.Where(x => x.OrganisationId == orgId).Include(x => x.AdmDelregister).ToList();
            return reportObligationInfo;
        }

        public AdmUppgiftsskyldighet GetReportObligationInformationForOrgAndSubDir(int orgId, int subdirId)
        {
            var reportObligation = DbContext.AdmUppgiftsskyldighet.SingleOrDefault(x => x.OrganisationId == orgId && x.DelregisterId == subdirId);
            return reportObligation;
        }

        public AdmUppgiftsskyldighetOrganisationstyp GetReportObligationForSubDirAndOrgtype(int subdirId, int orgtypeId)
        {
            var reportObligationForSubdirAndOrgtype = DbContext.AdmUppgiftsskyldighetOrganisationstyp.SingleOrDefault(x => x.DelregisterId == subdirId && x.OrganisationstypId == orgtypeId);
            return reportObligationForSubdirAndOrgtype;
        }

        public IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationInformationForOrgUnit(int orgUnitId)
        {
            var unitReportObligationInfo = DbContext.AdmEnhetsUppgiftsskyldighet.Where(x => x.OrganisationsenhetsId == orgUnitId).ToList();
            return unitReportObligationInfo;
        }

        public AdmEnhetsUppgiftsskyldighet GetUnitReportObligationForReportObligationAndOrg(int oblId, int orgunitId)
        {
            var unitReportObigation = DbContext.AdmEnhetsUppgiftsskyldighet.SingleOrDefault(x => x.UppgiftsskyldighetId == oblId && x.OrganisationsenhetsId == orgunitId);
            return unitReportObigation;
        }

        public string GetKommunkodForOrganisation(int orgId)
        {
            var kommunkod = DbContext.Organisation.Where(x => x.Id == orgId).Select(x => x.Kommunkod).SingleOrDefault();
            return kommunkod;
        }

        public int GetNewLeveransId(string userId, string userName, int orgId, int regId, int orgenhetsId, int forvLevId, string status, int sftpkontoId = 0)
        {
            var dbStatus = "Levererad";
            if (!String.IsNullOrEmpty(status))
            {
                dbStatus = status;
            }
            var leverans = new Leverans
            {
                ForvantadleveransId = forvLevId,
                OrganisationId = orgId,
                ApplicationUserId = userId,
                DelregisterId = regId,
                Leveranstidpunkt = DateTime.Now,
                Leveransstatus = dbStatus,
                SkapadDatum = DateTime.Now,
                SkapadAv = userName,
                AndradDatum = DateTime.Now,
                AndradAv = userName
            };

            if (orgenhetsId != 0)
            {
                leverans.OrganisationsenhetsId = orgenhetsId;
            }
            if (sftpkontoId != 0)
            {
                leverans.SFTPkontoId = sftpkontoId;
            }


            DbContext.Leverans.Add(leverans);

            DbContext.SaveChanges();
            return leverans.Id;
        }
        
        public IEnumerable<AdmOrganisationstyp> GetAllOrgTypes()
        {
            var orgTypes = DbContext.AdmOrganisationstyp.OrderBy(x => x.Typnamn).ToList();

            return orgTypes;
        }

        public IEnumerable<AdmFAQKategori> GetAllFAQs()
        {
            //var faqs = DbContext.AdmFAQKategori.Include(x => x.AdmFAQ).OrderBy(x => x.Sortering).ToList();
            var faqs = DbContext.AdmFAQKategori.OrderBy(x => x.Sortering).ToList();

            //Hämta FAQs per kategori separat (pga orderby)
            foreach (var faqCat in faqs)
            {
                faqCat.AdmFAQ = DbContext.AdmFAQ.Where(x => x.FAQkategoriId == faqCat.Id).OrderBy(x => x.Sortering).ToList();
            }

            return faqs;
        }

        public IEnumerable<AdmFAQKategori> GetFAQCategories()
        {
            //var faqCats = DbContext.AdmFAQKategori.Include(x => x.AdmFAQ).ToList();
            var faqCats = DbContext.AdmFAQKategori.OrderBy(x => x.Sortering).ToList();
            return faqCats;
        }

        public IEnumerable<AdmFAQ> GetFAQs(int faqCatId)
        {
            var faqs = DbContext.AdmFAQ.Where(x => x.FAQkategoriId == faqCatId).OrderBy(x => x.Sortering).ToList();
            return faqs;
        }

        public IEnumerable<AdmHelgdag> GetAllHolidays()
        {
            var holidays = DbContext.AdmHelgdag.ToList();
            return holidays;
        }

        public IEnumerable<AdmSpecialdag> GetAllSpecialDays()
        {
            var specialDays = DbContext.AdmSpecialdag.ToList();
            return specialDays;
        }

        public IEnumerable<AdmInformation> GetInformationTexts()
        {
            var infoTexts = DbContext.AdmInformation.ToList();
            return infoTexts;
        }

        public IEnumerable<AdmKonfiguration> GetAdmConfiguration()
        {
            var configInfo = DbContext.AdmKonfiguration.ToList();
            return configInfo;
        }

        public AdmInformation GetInfoText(string infoType)
        {
            var infoText = DbContext.AdmInformation.SingleOrDefault(x => x.Informationstyp == infoType);
            return infoText;
        }

        public AdmInformation GetInfoText(int infoId)
        {
            var infoText = DbContext.AdmInformation.SingleOrDefault(x => x.Id == infoId);
            return infoText;
        }

        public int GetPageInfoTextId(string pageType)
        {
            var pageInfoId = DbContext.AdmInformation.Where(x => x.Informationstyp == pageType).Select(x => x.Id).SingleOrDefault();
            return pageInfoId;
        }

        public AdmInsamlingsfrekvens GetInsamlingsfrekvens(int insamlingsid)
        {
            var insamlingsfrekvens = DbContext.AdmInsamlingsfrekvens.SingleOrDefault(x => x.Id == insamlingsid);
            return insamlingsfrekvens;
        }

        public IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetAllSubDirectoriesOrgtypes()
        {
            var subDirOrgtypes = DbContext.AdmUppgiftsskyldighetOrganisationstyp.ToList();
            return subDirOrgtypes;
        }

        public IEnumerable<AdmUppgiftsskyldighetOrganisationstyp> GetOrgTypesForSubDir(int subdirId)
        {
            var subdirOrgtypes = DbContext.AdmUppgiftsskyldighetOrganisationstyp.Where(x => x.DelregisterId == subdirId).ToList();
            return subdirOrgtypes;
        }

        public IEnumerable<AdmRegister> GetDirectories()
        {
            var registers = DbContext.AdmRegister.ToList();
            return registers;
        }

        public AdmRegister GetDirectoryByShortName(string shortName)
        {
            var register = DbContext.AdmRegister.SingleOrDefault(x => x.Kortnamn == shortName);
            return register;
        }

        public AdmRegister GetDirectoryById(int dirId)
        {
            var register = DbContext.AdmRegister.SingleOrDefault(x => x.Id == dirId);
            return register;
        }

        public AdmDelregister GetSubDirectoryById(int subdirId)
        {
            var delregister = DbContext.AdmDelregister.SingleOrDefault(x => x.Id == subdirId);
            return delregister;
        }

        public IEnumerable<AdmDelregister> GetSubDirectories()
        {
            var subDirectories = DbContext.AdmDelregister.ToList();
            return subDirectories;
        }

        public IEnumerable<AdmDelregister> GetSubDirectoriesForDirectory(int dirId)
        {
            var subDirectories = DbContext.AdmDelregister.Where(x => x.RegisterId == dirId).ToList();
            return subDirectories;
        }

        //public IEnumerable<AdmDelregister> GetSubDirectoriesWithIncludesForDirectory(int dirId)
        //{
        //    var subDirectories = DbContext.AdmDelregister.Where(x => x.RegisterId == dirId)
        //        .Include(x => x.AdmForvantadleverans)
        //        .Include(x => x.AdmUppgiftsskyldighet)
        //        .ToList();
        //    return subDirectories;
        //}

        public IEnumerable<AdmDelregister> GetSubDirectoriesWithIncludesForDirectory(int regId)
        {
            var reg = DbContext.AdmDelregister.Where(x => x.RegisterId == regId).Include(x => x.AdmUppgiftsskyldighet).Include(x => x.AdmForvantadleverans).ToList();
            return reg;
        }

        public IEnumerable<AdmDelregister> GetSubDirsObligatedForOrg(int orgId)
        {
            var uppgSkyldighetDelRegIds = DbContext.AdmUppgiftsskyldighet.Where(x => x.OrganisationId == orgId).Select(x => x.DelregisterId).ToList();

            var delregister = DbContext.AdmDelregister
                .Include(z => z.AdmRegister)
                .Include(f => f.AdmFilkrav.Select(q => q.AdmForvantadfil))
                .Where(x => x.Inrapporteringsportal && uppgSkyldighetDelRegIds.Contains(x.Id))
                .Include(f => f.AdmFilkrav.Select(q => q.AdmForvantadleverans))
                .ToList();

            return delregister;
        }

        public IEnumerable<AdmForeskrift> GetRegulationsForDirectory(int dirId)
        {
            var regulations = DbContext.AdmForeskrift.Where(x => x.RegisterId == dirId).ToList();
            return regulations;
        }


        public AdmDelregister GetSubDirectoryByShortName(string shortName)
        {
            var subDirectory = DbContext.AdmDelregister.Where(x => x.Kortnamn == shortName).FirstOrDefault();
            return subDirectory;
        }

        public IEnumerable<AdmForvantadleverans> GetExpectedDeliveries()
        {
            var expDeliveries = DbContext.AdmForvantadleverans.OrderBy(x => x.Uppgiftsstart).ToList();
            return expDeliveries;
        }

        public IEnumerable<LevereradFil> GetDeliveredFiles(int deliveryId)
        {
            var deliveredFiles = DbContext.LevereradFil.Where(x => x.LeveransId == deliveryId).ToList();
            return deliveredFiles;
        }

        public IEnumerable<DroppadFil> GetDroppedFilesForCase(int caseId)
        {
            var deliveredFiles = DbContext.DroppadFil.Where(x => x.ArendeId == caseId).ToList();
            return deliveredFiles;
        }

        public IEnumerable<AdmForeskrift> GetAllRegulations()
        {
            var regulations = DbContext.AdmForeskrift.ToList();
            return regulations;
        }

        public string GetDirectoryShortName(int dirId)
        {
            var dirShortName = DbContext.AdmRegister.Where(x => x.Id == dirId).Select(x => x.Kortnamn).SingleOrDefault();
            return dirShortName;
        }

        //public IEnumerable<LeveransstatusRapport> GetDeliveryStatusReport(int orgId, List<int> delregIdList, string period)
        //{
        //    //create parameters to pass to the stored procedure  
        //    //First input Parameter
        //    var param1 = new SqlParameter
        //    {
        //        ParameterName = "@org",
        //        SqlDbType = SqlDbType.Int,
        //        Direction = ParameterDirection.Input,
        //        Value = orgId
        //    };

        //    //Second input parameter
        //    var param2 = new SqlParameter
        //    {
        //        ParameterName = "@per",
        //        SqlDbType = SqlDbType.VarChar,
        //        Direction = ParameterDirection.Input,
        //        Value = "2019-04-01"
        //    };

        //    //third input parameter
        //    var param3 = new SqlParameter
        //    {
        //        ParameterName = "@String",
        //        SqlDbType = SqlDbType.VarChar,
        //        Direction = ParameterDirection.Input,
        //        Value = "12,13,14,31,35"
        //    };

        //    //compose the SQL
        //    var SQLString = "EXEC [dbo].[leveransstatus_rapport] @org, @per, @String";

        //    //Execute the stored procedure 
        //    var rapport = DbContext.LeveransstatusRapport.SqlQuery(SQLString, param1, param2, param3).ToList();
        //    return rapport;
        //}

        //public IEnumerable<LeveransstatusRapport> GetDeliveryStatusReport2(int orgId, List<int> delregIdList, List<int> forvlevIdList)
        //{
        //    //TODO - ta bort
        //    var rapport = new List<LeveransstatusRapport>();
        //    //create parameters to pass to the stored procedure  
        //    var delregIdStr = String.Empty;
        //    var forvlevIdStr = String.Empty;
        //    foreach (var id in delregIdList)
        //    {
        //        if (!String.IsNullOrEmpty(delregIdStr))
        //        {
        //            delregIdStr = delregIdStr + "," + id.ToString();
        //        }
        //        else
        //        {
        //            delregIdStr = id.ToString();
        //        }
        //    }
        //    foreach (var id in forvlevIdList)
        //    {
        //        if (!String.IsNullOrEmpty(forvlevIdStr))
        //        {
        //            forvlevIdStr = forvlevIdStr + "," + id.ToString();
        //        }
        //        else
        //        {
        //            forvlevIdStr = id.ToString();
        //        }
        //    }

        //    //var forvlevIdStr = forvlevIdList.ConvertAll(Convert.ToString);
        //    //First input Parameter
        //    var param1 = new SqlParameter
        //    {
        //        ParameterName = "@org",
        //        SqlDbType = SqlDbType.Int,
        //        Direction = ParameterDirection.Input,
        //        Value = orgId
        //    };

        //    //Second input parameter
        //    var param2 = new SqlParameter
        //    {
        //        ParameterName = "@String",
        //        SqlDbType = SqlDbType.VarChar,
        //        Direction = ParameterDirection.Input,
        //        Value = "12"
        //    };

        //    //Third input parameter
        //    var param3 = new SqlParameter
        //    {
        //        ParameterName = "@String2",
        //        SqlDbType = SqlDbType.VarChar,
        //        Direction = ParameterDirection.Input,
        //        Value = "173"
        //    };

        //    ////compose the SQL
        //    //var SQLString = "EXEC [dbo].[leveransRapport] @org, @String, @String2";

        //    ////Execute the stored procedure 
        //    //var rapport = DbContext.LeveransstatusRapport.SqlQuery(SQLString, param1, param2, param3).ToList();

        //    //TODO - test
        //    //var SQLStringA = "EXEC [dbo].[leveransRapportA] @org, @String, @String2";
        //    //var rappA= DbContext.TestA.SqlQuery(SQLStringA, param1, param2, param3).ToList();

        //    //var SQLStringA = "EXEC [dbo].[leveransRapportB] @org, @String, @String2";
        //    //var rappA = DbContext.TestB.SqlQuery(SQLStringA, param1, param2, param3).ToList();

        //    var dynamicSearchQuery =
        //        "SELECT Leverans.leveransid, Organisationsenhet.enhetskod, admRegister.registerid, admRegister.kortnamn, Levereradfil.filnamn, Levereradfil.filstatus, Leverans.organisationsid, Leverans.organisationsenhetsid," +
        //        "Leverans.kontaktpersonid, Leverans.delregisterid, admDelregister.kortnamn as DelregKortnamn," +
        //        "Leverans.forvantadleveransid, Leverans.leveranstidpunkt, Leverans.leveransstatus, admForvantadleverans.period, admForvantadleverans.rapporteringsstart, admForvantadleverans.rapporteringsenast " +
        //        "FROM(" +
        //        "SELECT Max(Produktionsleveranser.leveransid) AS senasteleverans " +
        //        "FROM(" +
        //        "SELECT Leverans.leveransid, Leverans.organisationsid, Leverans.organisationsenhetsid, Leverans.delregisterid, Leverans.forvantadleveransid " +
        //        "FROM Leverans WHERE(Leverans.organisationsid = 312 " +
        //        "and Leverans.delregisterid IN(12) " +
        //        "and Leverans.forvantadleveransid IN(173))) as produktionsleveranser " +
        //        "GROUP BY Produktionsleveranser.organisationsid, Produktionsleveranser.organisationsenhetsid, Produktionsleveranser.delregisterid, Produktionsleveranser.forvantadleveransid) as t " +
        //        "INNER JOIN Leverans ON senasteleverans = Leverans.leveransid " +
        //        "LEFT JOIN Organisationsenhet ON Organisationsenhet.organisationsenhetsid = Leverans.organisationsenhetsid " +
        //        "JOIN admDelregister ON admDelregister.delregisterid = Leverans.delregisterid " +
        //        "JOIN admRegister ON admRegister.registerid = admDelregister.registerid " +
        //        "JOIN Levereradfil ON Levereradfil.leveransid = Leverans.leveransid " +
        //        "JOIN admForvantadleverans ON admForvantadleverans.forvantadleveransid = Leverans.forvantadleveransid";

        //    var dynamicSearchQuery2 =
        //        "SELECT Max(Produktionsleveranser.leveransid) AS SenasteLeverans " +
        //        "FROM(" +
        //        "SELECT Leverans.leveransid, Leverans.organisationsid, Leverans.organisationsenhetsid, Leverans.delregisterid, Leverans.forvantadleveransid " +
        //        "FROM Leverans WHERE(Leverans.organisationsid = 312 " +
        //        "and Leverans.delregisterid IN(12) " +
        //        "and Leverans.forvantadleveransid IN(173))) as produktionsleveranser " +
        //        "GROUP BY Produktionsleveranser.organisationsid, Produktionsleveranser.organisationsenhetsid, Produktionsleveranser.delregisterid, Produktionsleveranser.forvantadleveransid";

        //    var dynamicSearchQuery4 =
        //        "SELECT Leverans.leveransid, Leverans.organisationsid, Leverans.organisationsenhetsid," +
        //        "Leverans.kontaktpersonid, Leverans.delregisterid, " +
        //        "Leverans.forvantadleveransid, Leverans.leveranstidpunkt, Leverans.leveransstatus " +
        //        "FROM(" +
        //        "SELECT Max(Produktionsleveranser.leveransid) AS SenasteLeverans " +
        //        "FROM(" +
        //        "SELECT Leverans.leveransid, Leverans.organisationsid, Leverans.organisationsenhetsid, Leverans.delregisterid, Leverans.forvantadleveransid " +
        //        "FROM Leverans WHERE(Leverans.organisationsid = 312 " +
        //        "and Leverans.delregisterid IN(12) " +
        //        "and Leverans.forvantadleveransid IN(173))) as produktionsleveranser " +
        //        "GROUP BY Produktionsleveranser.organisationsid, Produktionsleveranser.organisationsenhetsid, Produktionsleveranser.delregisterid, Produktionsleveranser.forvantadleveransid) as t " +
        //        "INNER JOIN Leverans ON Leverans.leveransid = t.senasteleverans";

        //    var dynamicSearchQuery3 =
        //        "SELECT Leverans.leveransid, Organisationsenhet.enhetskod, admRegister.registerid, admRegister.kortnamn, Levereradfil.filnamn, Levereradfil.filstatus, Leverans.organisationsid, Leverans.organisationsenhetsid," +
        //        "Leverans.kontaktpersonid, Leverans.delregisterid, admDelregister.kortnamn as DelregKortnamn," +
        //        "Leverans.forvantadleveransid, Leverans.leveranstidpunkt, Leverans.leveransstatus, admForvantadleverans.period, admForvantadleverans.rapporteringsstart, admForvantadleverans.rapporteringsenast " +
        //        "FROM Leverans " +
        //        "LEFT JOIN Organisationsenhet ON Organisationsenhet.organisationsenhetsid = Leverans.organisationsenhetsid " +
        //        "JOIN admDelregister ON admDelregister.delregisterid = Leverans.delregisterid " +
        //        "JOIN admRegister ON admRegister.registerid = admDelregister.registerid " +
        //        "JOIN Levereradfil ON Levereradfil.leveransid = Leverans.leveransid " +
        //        "JOIN admForvantadleverans ON admForvantadleverans.forvantadleveransid = Leverans.forvantadleveransid " +
        //        "WHERE Leverans.leveransid IN (38742,38741)";

        //    var idList = new List<int> {38742, 38741};

        //    var test2 = DbContext.Leverans.Where(x => idList.Contains(x.Id))
        //        .Include(x => x.AdmForvantadleverans)
        //        .Include(x => x.LevereradeFiler)
        //        .Include(x => x.Organisationsenhet)
        //        .ToList();


        //    var rappB = DbContext.TestD.SqlQuery(dynamicSearchQuery4).ToList();

        //    //var SQLStringA = "EXEC [dbo].[leveransRapportE] @org, @String, @String2";
        //    //var rappA = DbContext.TestE.SqlQuery(SQLStringA, param1, param2, param3).ToList();




        //    return rapport;
        //}

        public IEnumerable<Leverans> GetDeliveryStatusReport(int orgId, List<int> delregIdList, List<int> forvlevIdList)
        {

            var delregIdStr = String.Empty;
            var forvlevIdStr = String.Empty;
            foreach (var id in delregIdList)
            {
                if (!String.IsNullOrEmpty(delregIdStr))
                {
                    delregIdStr = delregIdStr + "," + id.ToString();
                }
                else
                {
                    delregIdStr = id.ToString();
                }
            }
            foreach (var id in forvlevIdList)
            {
                if (!String.IsNullOrEmpty(forvlevIdStr))
                {
                    forvlevIdStr = forvlevIdStr + "," + id.ToString();
                }
                else
                {
                    forvlevIdStr = id.ToString();
                }
            }


            var sqlLevIdn =
                "SELECT Max(Produktionsleveranser.leveransid) AS Id " +
                "FROM(" +
                "SELECT Leverans.leveransid, Leverans.organisationsid, Leverans.organisationsenhetsid, Leverans.delregisterid, Leverans.forvantadleveransid " +
                "FROM Leverans WHERE(Leverans.organisationsid = "+ orgId + " " +
                "and Leverans.delregisterid IN(" + delregIdStr + ") " +
                "and Leverans.forvantadleveransid IN(" + forvlevIdStr + "))) as produktionsleveranser " +
                "GROUP BY Produktionsleveranser.organisationsid, Produktionsleveranser.organisationsenhetsid, Produktionsleveranser.delregisterid, Produktionsleveranser.forvantadleveransid";

            var levIdList = DbContext.LevId.SqlQuery(sqlLevIdn).ToList();

            var idList= levIdList.Select(x => x.Id).ToList();

            var leveranser = DbContext.Leverans.Where(x => idList.Contains(x.Id))
                .Include(x => x.AdmForvantadleverans)
                .Include(x => x.LevereradeFiler)
                .Include(x => x.Organisationsenhet)
                .ToList();

            return leveranser;
        }

        public IEnumerable<AdmForvantadfil> GetAllExpectedFiles()
        {
            var expFiles = DbContext.AdmForvantadfil.ToList();
            return expFiles;
        }

        public IEnumerable<AdmFilkrav> GetAllFileRequirements()
        {
            var fileReqs = DbContext.AdmFilkrav.ToList();
            return fileReqs;
        }

        public IEnumerable<string> GetAllFileMasks()
        {
            var allFilemasks = DbContext.AdmForvantadfil.Select(x => x.Filmask).ToList();
            return allFilemasks;
        }

        public IEnumerable<AdmInsamlingsfrekvens> GetAllCollectionFrequencies()
        {
            var collFreq = DbContext.AdmInsamlingsfrekvens.ToList();
            return collFreq;
        }

        public IEnumerable<Arendetyp> GetAllCaseTypes()
        {
            var caseTypes = DbContext.Arendetyp.ToList();
            return caseTypes;
        }

        public IEnumerable<ArendeStatus> GetAllCaseStatuses()
        {
            var caseStatuses = DbContext.ArendeStatus.ToList();
            return caseStatuses;
        }

        public string GetSubDirectoryShortNameForExpectedFile(int filkravId)
        {
            var subDirId = DbContext.AdmFilkrav.Where(x => x.Id == filkravId).Select(x => x.DelregisterId).SingleOrDefault();
            var subDirShortName = DbContext.AdmDelregister.Where(x => x.Id == subDirId).Select(x => x.Kortnamn).SingleOrDefault();
            return subDirShortName;
        }

        public string GetSubDirectoryShortName(int subDirId)
        {
            var subDirShortName = DbContext.AdmDelregister.Where(x => x.Id == subDirId).Select(x => x.Kortnamn).SingleOrDefault();
            return subDirShortName;
        }

        public string GetFileRequirementName(int fileReqId)
        {
            var fileReqName = DbContext.AdmFilkrav.Where(x => x.Id == fileReqId).Select(x => x.Namn).SingleOrDefault();
            return fileReqName;
        }

        public IEnumerable<AdmForvantadfil> GetExpectedFilesForDirectory(int dirId)
        {
            var expectedFileList = new List<AdmForvantadfil>();
            var regulationsForDirectory = DbContext.AdmForeskrift.Where(x => x.RegisterId == dirId).ToList();
            foreach (var regulation in regulationsForDirectory)
            {
                var expectedFileListForRegulation = DbContext.AdmForvantadfil.Where(x => x.ForeskriftsId == regulation.Id).ToList();
                expectedFileList.AddRange(expectedFileListForRegulation);
            }
            return expectedFileList;
        }

        public IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForDirectory(int dirId)
        {
            var expectedDeliveriesList = new List<AdmForvantadleverans>();
            var subDirectoriesForDirectory = DbContext.AdmDelregister.Where(x => x.RegisterId == dirId).ToList();
            foreach (var subDir in subDirectoriesForDirectory)
            {
                var expectedDeliveryList = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == subDir.Id).OrderBy(x => x.Uppgiftsstart).ToList();
                expectedDeliveriesList.AddRange(expectedDeliveryList);
            }
            return expectedDeliveriesList;
        }

        public IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForSubDirectory(int subdirId)
        {
            var expectedDeliveriesList = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == subdirId).OrderBy(x => x.Uppgiftsstart).ToList();
            return expectedDeliveriesList;
        }

        public AdmDokument GetFile(int fileId)
        {
            var file = DbContext.AdmDokument.SingleOrDefault(x => x.Id == fileId);
            return file;
        }

        public IEnumerable<AdmDokument> GetAllFiles()
        {
            var fileList = DbContext.AdmDokument.ToList();
            return fileList;
        }

        public void SaveFile(AdmDokument file)
        {
            DbContext.AdmDokument.Add(file);
            DbContext.SaveChanges();
        }

        public void UpdateFile(AdmDokument file)
        {
            var fileFromDb = DbContext.AdmDokument.SingleOrDefault(x => x.Id == file.Id);
            if (fileFromDb != null)
            {
                fileFromDb.Fil = file.Fil;
                fileFromDb.AndradAv = file.AndradAv;
                fileFromDb.AndradDatum = file.AndradDatum;
                DbContext.SaveChanges();
            }
        }

        public void DeleteFile(AdmDokument file)
        {
            var fileToDelete = DbContext.AdmDokument.SingleOrDefault(x => x.Id == file.Id);
            if (fileToDelete != null)
            {
                DbContext.AdmDokument.Remove(fileToDelete);
                DbContext.SaveChanges();
            }
        }

        public IEnumerable<AdmForvantadleverans> GetExpectedDeliveriesForSubdirsAndPeriods(List<int> subdirIds, List<string> periods)
        {
            var forvlevList = DbContext.AdmForvantadleverans
                .Where(x => subdirIds.Contains(x.DelregisterId) && periods.Contains(x.Period)).Distinct().ToList();
            return forvlevList;
        }

        public IEnumerable<AdmForvantadfil> GetExpectedFile(int fileReq)
        {
            var expectedFiles = DbContext.AdmForvantadfil.Where(x => x.FilkravId == fileReq).ToList();
            return expectedFiles;
        }

        public IEnumerable<AdmFilkrav> GetFileRequirementsForDirectory(int dirId)
        {
            var fileRequirementsList = new List<AdmFilkrav>();
            var subDirectoriesForDirectory = DbContext.AdmDelregister.Where(x => x.RegisterId == dirId).ToList();
            foreach (var subDir in subDirectoriesForDirectory)
            {
                var fileRequirementList = DbContext.AdmFilkrav.Where(x => x.DelregisterId == subDir.Id).ToList();
                fileRequirementsList.AddRange(fileRequirementList);
            }
            return fileRequirementsList;
        }

        public AdmFilkrav GetFileRequirementById(int fileReqId)
        {
            var fileReq = DbContext.AdmFilkrav.SingleOrDefault(x => x.Id == fileReqId);
            return fileReq;
        }

        public IEnumerable<AdmFilkrav> GetFileRequirementsForSubDirectory(int subdirId)
        {
            var fileRequirementList = DbContext.AdmFilkrav.Where(x => x.DelregisterId == subdirId).ToList();
            return fileRequirementList;
        }

        public AdmFilkrav GetFileRequirementsForSubDirectoryAndFileReqId(int subdirId, int filereqId)
        {
            var fileRequirement = DbContext.AdmFilkrav.SingleOrDefault(x => x.DelregisterId == subdirId && x.Id == filereqId);
            return fileRequirement;
        }

        public List<AdmFilkrav> GetFileRequirementsAndExpectedFilesForSubDirectory(int subDirId)
        {
            var fileReqAndExpectedFileList = DbContext.AdmFilkrav.Where(x => x.DelregisterId == subDirId)
                .Include(x => x.AdmForvantadfil).ToList();
            return fileReqAndExpectedFileList;
        }

        public IEnumerable<AdmRegister> GetAllRegisters()
        {
            var registersList = DbContext.AdmRegister.ToList();
            return registersList;
        }

        public IEnumerable<Organisation> GetAllOrganisations()
        {
            var orgList = DbContext.Organisation.ToList();
            return orgList;
        }

        public IEnumerable<AdmRegister> GetAllRegistersForPortal()
        {
            var registersList = DbContext.AdmRegister.Where(x => x.Inrapporteringsportal).ToList();
            return registersList;
        }

        public IEnumerable<AdmDelregister> GetAllSubDirectoriesForPortal()
        {
            var delregistersList = DbContext.AdmDelregister.Where(x => x.Inrapporteringsportal).Include(x => x.AdmFilkrav).ToList();
            return delregistersList;
        }

        public AdmForeskrift GetRegulation(int regulationId)
        {
            var foreskrift = DbContext.AdmForeskrift.SingleOrDefault(x => x.Id == regulationId);
            return foreskrift;
        }

        public AdmFAQKategori GetFAQCategory(int faqCatId)
        {
            var faqCat = DbContext.AdmFAQKategori.SingleOrDefault(x => x.Id == faqCatId);
            return faqCat;
        }

        public AdmFAQ GetFAQ(int faqId)
        {
            var faq = DbContext.AdmFAQ.SingleOrDefault(x => x.Id == faqId);
            return faq;
        }

        public AdmForeskrift GetForeskriftByFileReq(int fileReqId)
        {
            var filkrav = DbContext.AdmFilkrav.FirstOrDefault(x => x.Id == fileReqId);
            var foreskrift = DbContext.AdmForeskrift.FirstOrDefault(x => x.Id == filkrav.ForeskriftsId);
            return foreskrift;
        }

        public IEnumerable<Rapporteringsresultat> GetReportResultForSubdirAndPeriod(int delRegId, string period)
        {
            var repResults = DbContext.RapporteringsResultat.Where(x => x.DelregisterId == delRegId && x.Period == period).ToList();
            return repResults;
        }

        public IEnumerable<Rapporteringsresultat> GetReportResultForDirAndPeriod(int regId, string period)
        {
            var repResults = DbContext.RapporteringsResultat.Where(x => x.RegisterId == regId && x.Period == period).ToList();
            return repResults;
        }

        public IEnumerable<string> GetSubDirectoysPeriodsForAYear(int subdirId, int year)
        {
            var dateFrom = new DateTime(year, 01, 01);
            var dateTom = new DateTime(year, 12, 31).Date;
            var periods = DbContext.AdmForvantadleverans
                .Where(x => x.DelregisterId == subdirId && x.Uppgiftsstart >= dateFrom && x.Uppgiftsslut <= dateTom)
                .Select(x => x.Period).ToList();

            return periods;
        }

        public List<DateTime> GetTaskStartForSubdir(int subdirId)
        {
            var taskstartList = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == subdirId)
                .Select(x => x.Uppgiftsstart).ToList();

            return taskstartList;
        }

        //TODO - special för EKB-År, Lös på annat sätt. Db?
        public DateTime GetReportstartForRegisterAndPeriodSpecial(int dirId, string period)
        {
            var subDir = DbContext.AdmDelregister.FirstOrDefault(x => x.RegisterId == dirId && x.Kortnamn == "EKB-År");
            var reportstart = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == subDir.Id && x.Period == period)
                .Select(x => x.Rapporteringsstart).FirstOrDefault();

            return reportstart;
        }
        public DateTime GetReportstartForRegisterAndPeriod(int dirId, string period)
        {
            var firstSubDirForReg = DbContext.AdmDelregister.FirstOrDefault(x => x.RegisterId == dirId);
            var reportstart = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == firstSubDirForReg.Id && x.Period == period)
                .Select(x => x.Rapporteringsstart).FirstOrDefault();

            return reportstart;
        }

        public DateTime GetLatestReportDateForRegisterAndPeriod(int dirId, string period)
        {
            var firstSubDirForReg = DbContext.AdmDelregister.FirstOrDefault(x => x.RegisterId == dirId);
            var reportstart = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == firstSubDirForReg.Id && x.Period == period)
                .Select(x => x.Rapporteringsenast).FirstOrDefault();

            return reportstart;
        }

        public DateTime GetLatestReportDateForRegisterAndPeriodSpecial(int dirId, string period)
        {
            var subDir = DbContext.AdmDelregister.FirstOrDefault(x => x.RegisterId == dirId && x.Kortnamn == "EKB-År");
            var reportstart = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == subDir.Id && x.Period == period)
                .Select(x => x.Rapporteringsenast).FirstOrDefault();

            return reportstart;
        }

        public IEnumerable<AdmDelregister> GetSubdirsForDirectory(int dirId)
        {
            var subdirectories = DbContext.AdmDelregister.Where(x => x.RegisterId == dirId).ToList();
            return subdirectories;
        }

        public int GetExpextedDeliveryIdForSubDirAndPeriod(int subDirId, string period)
        {
            var forvLevId = DbContext.AdmForvantadleverans.Where(x => x.DelregisterId == subDirId && x.Period == period)
                .Select(x => x.Id).SingleOrDefault();
            return forvLevId;
        }

        public AdmForvantadleverans GetExpectedDeliveryBySubDirAndFileReqIdAndPeriod(int subDirId, int fileReqId, string period)
        {
            var forvLev = DbContext.AdmForvantadleverans.SingleOrDefault(x => x.DelregisterId == subDirId && x.Period == period && x.FilkravId == fileReqId);
            return forvLev;
        }

        public Leverans GetLatestDeliveryForOrganisationSubDirectoryAndPeriod(int orgId, int subdirId, int forvlevId)
        {
            var latestsDeliveryForOrgAndSubdirectory = AllaLeveranser()
                .Where(a => a.OrganisationId == orgId && a.DelregisterId == subdirId &&
                            a.ForvantadleveransId == forvlevId).OrderByDescending(x => x.Id).FirstOrDefault();
            return latestsDeliveryForOrgAndSubdirectory;
        }

        public Leverans GetLatestDeliveryForOrganisationSubDirectoryPeriodAndOrgUnit(int orgId, int subdirId, int forvlevId,int orgUnitId)
        {
            var latestsDeliveryForOrgAndSubdirectory = AllaLeveranser()
                .Where(a => a.OrganisationId == orgId && a.DelregisterId == subdirId &&
                            a.ForvantadleveransId == forvlevId && a.OrganisationsenhetsId == orgUnitId).OrderByDescending(x => x.Id).FirstOrDefault();
            return latestsDeliveryForOrgAndSubdirectory;
        }

        //public string GetUserEmail(string userId)
        //{
        //    var email = IdentityDbContext.Users.Where(x => x.Id == userId).Select(x => x.Email).SingleOrDefault();
        //    return email;
        //}

        public string GetUserName(string userId)
        {
            var userName = DbContext.Users.Where(u => u.Id == userId).Select(u => u.Namn).SingleOrDefault();
            return userName;
        }

        public string GetUserContactNumber(string userId)
        {
            var userContactNumber = DbContext.Users.Where(u => u.Id == userId).Select(u => u.Kontaktnummer).SingleOrDefault();
            return userContactNumber;
        }

        public IEnumerable<Arende> GetUserCases(string userId)
        {
            var userCases = new List<Arende>();
            var casesIdsForUser = DbContext.ArendeKontaktperson.Where(x => x.ApplicationUserId == userId).Select(x => x.ArendeId).ToList();
            foreach (var caseId in casesIdsForUser)
            {
                var arende = DbContext.Arende.SingleOrDefault(x => x.Id == caseId);
                userCases.Add(arende);
            }
            return userCases;
        }

        public string GetUserPhoneNumber(string userId)
        {
            var phoneNumber = DbContext.Users.Where(u => u.Id == userId).Select(u => u.PhoneNumber).SingleOrDefault();
            return phoneNumber;
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            var user = DbContext.Users.SingleOrDefault(x => x.Email == email);
            return user;
        }

        public ApplicationUser GetUserById(string userId)
        {
            var user = DbContext.Users.SingleOrDefault(x => x.Id == userId);
            return user;
        }

        public ApplicationUser GetUserBySFTPAccountId(int ftpAccountId)
        {
            var userId = DbContext.KontaktpersonSFTPkonto.Where(x => x.SFTPkontoId == ftpAccountId)
                .Select(x => x.ApplicationUserId).SingleOrDefault();
            var user = DbContext.Users.SingleOrDefault(x => x.Id == userId);
            return user;
        }

        public ApplicationUser GetFirstUserForSFTPAccount(int ftpAccountId)
        {
            var userId = DbContext.KontaktpersonSFTPkonto.Where(x => x.SFTPkontoId == ftpAccountId)
                .Select(x => x.ApplicationUserId).FirstOrDefault();
            var user = DbContext.Users.SingleOrDefault(x => x.Id == userId);
            return user;
        }

        public SFTPkonto GetSFTPAccount(int ftpAccountId)
        {
            var sftpAccount = DbContext.SFTPkonto.SingleOrDefault(x => x.Id == ftpAccountId);
            return sftpAccount;
        }

        public string GetUserEmail(string userId)
        {
            var email = DbContext.Users.Where(x => x.Id == userId).Select(x => x.Email).SingleOrDefault();
            return email;
        }

        public SFTPkonto GetSFTPAccountByName(string name)
        {
            var sftpAccount = DbContext.SFTPkonto.Where(x => x.Kontonamn == name).Include(x => x.KontaktpersonSFTPkonto).SingleOrDefault();
            return sftpAccount;
        }

        public IEnumerable<Roll> GetChosenDelRegistersForUser(string userId)
        {
            var rollList = new List<Roll>();
            rollList = DbContext.Roll.Where(x => x.ApplicationUserId == userId).ToList();
            return rollList;
        }

        public IEnumerable<AdmRegister> GetChosenRegistersForUser(string userId)
        {
            var rollList = new List<Roll>();
            var registerList = new List<AdmRegister>();
            var regIdList = new List<int>();

            rollList = DbContext.Roll.Where(x => x.ApplicationUserId == userId).ToList();

            foreach (var roll in rollList)
            {
                var tmp = DbContext.AdmDelregister.Where(x => x.Id == roll.DelregisterId).FirstOrDefault();
                var delregister = DbContext.AdmDelregister.SingleOrDefault(x => x.Id == roll.DelregisterId);
                var registerId = DbContext.AdmRegister.Where(x => x.Id == delregister.RegisterId).Select(x => x.Id)
                    .SingleOrDefault();
                regIdList.Add(registerId);
            }

            //rensa bort dubbletter och hämta delregister för varje register
            var regIdListDistinct = regIdList.Distinct();
            foreach (var registerId in regIdListDistinct)
            {
                var register = DbContext.AdmRegister.Where(x => x.Id == registerId).Include(x => x.AdmDelregister).SingleOrDefault();
                registerList.Add(register);
            }
            return registerList;
        }

        public string GetClosedDays()
        {
            var closedDays = String.Empty;
            closedDays = DbContext.AdmKonfiguration.Where(x => x.Typ == "ClosedDays").Select(x => x.Varde).SingleOrDefault();
            return closedDays;
        }
        public string GetClosedFromHour()
        {
            var closedFromHour = "0";
            closedFromHour = DbContext.AdmKonfiguration.Where(x => x.Typ == "ClosedFromHour").Select(x => x.Varde).SingleOrDefault();
            return closedFromHour;
        }

        public string GetClosedFromMin()
        {
            var closedFromMin = "0";
            closedFromMin = DbContext.AdmKonfiguration.Where(x => x.Typ == "ClosedFromMin").Select(x => x.Varde).SingleOrDefault();
            return closedFromMin;
        }

        public string GetClosedToHour()
        {
            var closedToHour = "0";
            closedToHour = DbContext.AdmKonfiguration.Where(x => x.Typ == "ClosedToHour").Select(x => x.Varde).SingleOrDefault();
            return closedToHour;
        }

        public string GetClosedToMin()
        {
            var closedToMin = "0";
            closedToMin = DbContext.AdmKonfiguration.Where(x => x.Typ == "ClosedToMin").Select(x => x.Varde).SingleOrDefault();
            return closedToMin;
        }

        public string GetClosedAnnyway()
        {
            var closedAnyway = "false";
            closedAnyway = DbContext.AdmKonfiguration.Where(x => x.Typ == "ClosedAnyway").Select(x => x.Varde).SingleOrDefault();
            return closedAnyway;
        }

        public IEnumerable<AdmHelgdag> GetHolidays()
        {
            var holidays = DbContext.AdmHelgdag.OrderBy(x => x.Helgdatum).ToList();
            return holidays;
        }

        public IEnumerable<AdmSpecialdag> GetSpecialDays()
        {
            var specialDays = DbContext.AdmSpecialdag.OrderBy(x => x.Specialdagdatum).ToList();
            return specialDays;
        }

        //public IEnumerable<RegisterInfo> GetAllRegisterInformation()
        //{
        //    var registerInfoList = new List<RegisterInfo>();

        //    var delregister = DbContext.AdmDelregister
        //        .Include(z => z.AdmRegister)
        //        .Include(b => b.AdmForvantadleverans)
        //        .Include(f => f.AdmFilkrav.Select(q => q.AdmForvantadfil))
        //        .Where(x => x.Inrapporteringsportal)
        //        .ToList();


        //    foreach (var item in delregister)
        //    {
        //        var regInfoObj = CreateRegisterInfoObj(item);
        //        registerInfoList.Add(regInfoObj);
        //    }

        //    return registerInfoList;
        //}

        public IEnumerable<RegisterInfo> GetAllRegisterInformationForOrganisation(int orgId)
        {
            var registerInfoList = new List<RegisterInfo>();

            //Get all subdirs thar org are obligated to report for
            var delregister = GetSubDirsObligatedForOrg(orgId);

            foreach (var item in delregister)
            {
                var regInfoObj = CreateRegisterInfoObj(item, orgId);
                registerInfoList.Add(regInfoObj);
            }

            return registerInfoList;
        }

        public AdmUppgiftsskyldighet GetUppgiftsskyldighetForOrganisationAndRegister(int orgId, int delregid)
        {
            var uppgiftsskyldighet = DbContext.AdmUppgiftsskyldighet.SingleOrDefault(x => x.OrganisationId == orgId && x.DelregisterId == delregid);

            return uppgiftsskyldighet;
        }


        public IEnumerable<Organisationsenhet> GetOrganisationUnits(int orgId)
        {
            var orgUnits = DbContext.Organisationsenhet.Where(x => x.OrganisationsId == orgId).ToList();
            return orgUnits;
        }



        public IEnumerable<AdmEnhetsUppgiftsskyldighet> GetUnitReportObligationForReportObligation(int uppgSkyldighetId)
        {
            var unitReportObligations = DbContext.AdmEnhetsUppgiftsskyldighet.Where(x => x.UppgiftsskyldighetId == uppgSkyldighetId).ToList();
            return unitReportObligations;
        }

        public Organisationsenhet GetOrganisationUnit(int orgunitId)
        {
            var orgUnit = DbContext.Organisationsenhet.SingleOrDefault(x => x.Id == orgunitId);
            return orgUnit;
        }


        public int CreateOrganisation(Organisation org, ICollection<Organisationstyp> orgtyperForOrg)
        {
            DbContext.Organisation.Add(org);
            DbContext.SaveChanges();

            //Insert new orgtypes
            foreach (var orgtyp in orgtyperForOrg)
            {
                orgtyp.OrganisationsId = org.Id;
                DbContext.Organisationstyp.Add(orgtyp);
            }
            DbContext.SaveChanges();

            return org.Id;
        }

        public int CreateSFTPAccount(SFTPkonto account, ICollection<KontaktpersonSFTPkonto> contacts)
        {
            DbContext.SFTPkonto.Add(account);
            DbContext.SaveChanges();

            //Insert contacts
            foreach (var contact in contacts)
            {
                contact.SFTPkontoId = account.Id;
                DbContext.KontaktpersonSFTPkonto.Add(contact);
            }
            DbContext.SaveChanges();

            return account.Id;
        }

        public void CreateOrgUnit(Organisationsenhet orgUnit)
        {
            DbContext.Organisationsenhet.Add(orgUnit);
            DbContext.SaveChanges();
        }

        public void CreatePrivateEmail(UndantagEpostadress privEmail)
        {
            DbContext.UndantagEpostadress.Add(privEmail);
            DbContext.SaveChanges();
        }

        public void CreateCase(Arende arende)
        {
            DbContext.Arende.Add(arende);
            DbContext.SaveChanges();
        }

        public void CreateOrgType(AdmOrganisationstyp orgType)
        {
            DbContext.AdmOrganisationstyp.Add(orgType);
            DbContext.SaveChanges();
        }

        public void CreateFAQCategory(AdmFAQKategori faqCategory)
        {
            DbContext.AdmFAQKategori.Add(faqCategory);
            DbContext.SaveChanges();
        }

        public void CreateFAQ(AdmFAQ faq)
        {
            DbContext.AdmFAQ.Add(faq);
            DbContext.SaveChanges();
        }

        public void CreateHoliday(AdmHelgdag holiday)
        {
            DbContext.AdmHelgdag.Add(holiday);
            DbContext.SaveChanges();
        }

        public void CreateSpecialDay(AdmSpecialdag specialDay)
        {
            DbContext.AdmSpecialdag.Add(specialDay);
            DbContext.SaveChanges();
        }
        public void CreateInformationText(AdmInformation infoText)
        {
            DbContext.AdmInformation.Add(infoText);
            DbContext.SaveChanges();
        }

        public void CreateReportObligation(AdmUppgiftsskyldighet uppgSk)
        {
            DbContext.AdmUppgiftsskyldighet.Add(uppgSk);
            DbContext.SaveChanges();
        }

        public void CreateUnitReportObligation(AdmEnhetsUppgiftsskyldighet enhetsUppgSk)
        {
            DbContext.AdmEnhetsUppgiftsskyldighet.Add(enhetsUppgSk);
            DbContext.SaveChanges();
        }

        public void CreateSubdirReportObligation(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtype)
        {
            DbContext.AdmUppgiftsskyldighetOrganisationstyp.Add(subdirOrgtype);
            DbContext.SaveChanges();
        }

        public void CreateDirectory(AdmRegister dir)
        {
            DbContext.AdmRegister.Add(dir);
            DbContext.SaveChanges();
        }

        public void CreateSubDirectory(AdmDelregister subDir)
        {
            DbContext.AdmDelregister.Add(subDir);
            DbContext.SaveChanges();
        }

        public void CreateRegulation(AdmForeskrift regulation)
        {
            DbContext.AdmForeskrift.Add(regulation);
            DbContext.SaveChanges();
        }

        public void CreateExpectedDelivery(AdmForvantadleverans forvLev)
        {
            DbContext.AdmForvantadleverans.Add(forvLev);
            DbContext.SaveChanges();
        }

        public void CreateExpectedFile(AdmForvantadfil forvFil)
        {
            DbContext.AdmForvantadfil.Add(forvFil);
            DbContext.SaveChanges();
        }

        public void CreateFileRequirement(AdmFilkrav filkrav)
        {
            DbContext.AdmFilkrav.Add(filkrav);
            DbContext.SaveChanges();
        }

        public void CreateCollectFrequence(AdmInsamlingsfrekvens colFreq)
        {
            DbContext.AdmInsamlingsfrekvens.Add(colFreq);
            DbContext.SaveChanges();
        }

        public void UpdateOrganisation(Organisation org)
        {
            var orgDb = DbContext.Organisation.Where(u => u.Id == org.Id).Select(u => u).SingleOrDefault();
            orgDb.Landstingskod = org.Landstingskod;
            orgDb.Kommunkod = org.Kommunkod;
            orgDb.Inrapporteringskod = org.Inrapporteringskod;
            orgDb.Organisationsnr = org.Organisationsnr;
            orgDb.Organisationsnamn = org.Organisationsnamn;
            orgDb.Hemsida = org.Hemsida;
            orgDb.EpostAdress = org.EpostAdress;
            orgDb.Telefonnr = org.Telefonnr;
            orgDb.Postnr = org.Postnr;
            orgDb.Postort = org.Postort;
            orgDb.Adress = org.Adress;
            orgDb.Epostdoman = org.Epostdoman;
            orgDb.AktivFrom = org.AktivFrom;
            orgDb.AktivTom = org.AktivTom;
            orgDb.AndradDatum = org.AndradDatum;
            orgDb.AndradAv = org.AndradAv;

            //Organisationstype
            //delete prevoious orgtypes
            DbContext.Organisationstyp.RemoveRange(DbContext.Organisationstyp.Where(x => x.OrganisationsId == org.Id));

            DbContext.SaveChanges();

            //Insert new orgtypes
            foreach (var orgtyp in org.Organisationstyp)
            {
                var orgtypDB = new Organisationstyp
                {
                    OrganisationsId = org.Id,
                    OrganisationstypId = orgtyp.OrganisationstypId,
                    SkapadAv = org.AndradAv,
                    SkapadDatum = org.AndradDatum,
                    AndradAv = org.AndradAv,
                    AndradDatum = org.AndradDatum
                };
                DbContext.Organisationstyp.Add(orgtypDB);
            }
            DbContext.SaveChanges();
        }

        //public void UpdateAdminUser(AppUserAdmin user)
        //{
        //    var usrDb = IdentityDbContext.Users.Where(u => u.Id == user.Id).Select(u => u).SingleOrDefault();
        //    usrDb.PhoneNumber= user.PhoneNumber;
        //    usrDb.Email = user.Email;
        //    usrDb.AndradDatum = user.AndradDatum;
        //    usrDb.AndradAv = user.AndradAv;
        //    IdentityDbContext.SaveChanges(); 
        //}

        public void UpdateContactPerson(ApplicationUser user)
        {
            var usrDb = DbContext.Users.Where(u => u.Id == user.Id).Select(u => u).SingleOrDefault();
            usrDb.PhoneNumber = user.PhoneNumber;
            usrDb.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            usrDb.Kontaktnummer = user.Kontaktnummer;
            usrDb.AktivFrom = user.AktivFrom;
            usrDb.AktivTom = user.AktivTom;
            usrDb.AndradDatum = user.AndradDatum;
            usrDb.AndradAv = user.AndradAv;
            DbContext.SaveChanges();
        }

        public void UpdateSFTPAccount(SFTPkonto account)
        {
            var accountDb = DbContext.SFTPkonto.Where(u => u.Id == account.Id).Select(u => u).SingleOrDefault();
            accountDb.RegisterId = account.RegisterId;
            accountDb.Kontonamn = account.Kontonamn;
            accountDb.AndradDatum = account.AndradDatum;
            accountDb.AndradAv = account.AndradAv;

            //delete prevoious contacts
            var y = DbContext.KontaktpersonSFTPkonto.Where(x => x.SFTPkontoId == account.Id).ToList();
            foreach (var item in y)
            {
                DbContext.KontaktpersonSFTPkonto.Remove(item);
                DbContext.SaveChanges();
            }
            var tmp = DbContext.KontaktpersonSFTPkonto.Where(x => x.SFTPkontoId == account.Id).ToList();
            DbContext.KontaktpersonSFTPkonto.RemoveRange(tmp);
            DbContext.SaveChanges();

            //Insert new contacts
            foreach (var contact in account.KontaktpersonSFTPkonto)
            {
                var contactDB = new KontaktpersonSFTPkonto
                {
                    ApplicationUserId = contact.ApplicationUserId,
                    SFTPkontoId = contact.SFTPkontoId,
                    SkapadAv = account.AndradAv,
                    SkapadDatum = account.AndradDatum,
                    AndradAv = account.AndradAv,
                    AndradDatum = account.AndradDatum
                };
                DbContext.KontaktpersonSFTPkonto.Add(contactDB);
            }
            DbContext.SaveChanges();


        }

        public void UpdateChosenRegistersForUser(string userId, string userName, List<RegisterInfo> registerList)
        {
            //delete prevoious choices
            DbContext.Roll.RemoveRange(DbContext.Roll.Where(x => x.ApplicationUserId == userId));

            //Insert new choices
            foreach (var register in registerList)
            {
                if (register.Selected)
                {
                    var roll = new Roll
                    {
                        DelregisterId = register.Id,
                        ApplicationUserId = userId,
                        SkapadDatum = DateTime.Now,
                        SkapadAv = userName,
                        AndradDatum = DateTime.Now,
                        AndradAv = userName
                    };

                    DbContext.Roll.Add(roll);
                }
            }
            DbContext.SaveChanges();
        }

        public void UpdateNameForUser(string userId, string userName)
        {
            var user = DbContext.Users.Where(u => u.Id == userId).Select(u => u).SingleOrDefault();
            user.Namn = userName;
            DbContext.SaveChanges();
        }

        public void UpdateContactNumberForUser(string userId, string number)
        {
            var user = DbContext.Users.Where(u => u.Id == userId).Select(u => u).SingleOrDefault();
            user.Kontaktnummer = number;
            DbContext.SaveChanges();
        }

        public void UpdateActiveFromForUser(string userId)
        {
            var user = DbContext.Users.Where(u => u.Id == userId).Select(u => u).SingleOrDefault();
            user.AktivFrom = DateTime.Now;
            DbContext.SaveChanges();
        }

        public void UpdateUserInfo(ApplicationUser user)
        {
            var userDb = DbContext.Users.SingleOrDefault(x => x.Id == user.Id);
            userDb.AndradAv = user.AndradAv;
            userDb.AndradDatum = user.AndradDatum;
            DbContext.SaveChanges();
        }

        public void UpdatePrivateEmail(UndantagEpostadress privEmail)
        {
            var privEmailDb = DbContext.UndantagEpostadress.SingleOrDefault(x => x.Id == privEmail.Id);
            privEmailDb.ArendeId = privEmail.ArendeId;
            privEmailDb.OrganisationsId = privEmail.OrganisationsId;
            privEmailDb.PrivatEpostAdress = privEmail.PrivatEpostAdress;
            privEmailDb.Status = privEmail.Status;
            privEmailDb.AktivFrom = privEmail.AktivFrom;
            privEmailDb.AktivTom = privEmail.AktivTom;
            privEmailDb.AndradAv = privEmail.AndradAv;
            privEmailDb.AndradDatum = privEmail.AndradDatum;
            DbContext.SaveChanges();
        }

        public void UpdateCase(Arende arende)
        {
            var arendeDb = DbContext.Arende.SingleOrDefault(x => x.Id == arende.Id);
            arendeDb.Arendenamn = arende.Arendenamn;
            arendeDb.Arendenr = arende.Arendenr;
            arendeDb.StartDatum = arende.StartDatum;
            arendeDb.SlutDatum = arende.SlutDatum;
            arendeDb.AnsvarigEpost = arende.AnsvarigEpost;

            //If areandestatus and arendetyp changed, e.g. != 0, update theese as well
            if (arende.ArendetypId != 0)
            {
                arendeDb.ArendetypId = arende.ArendetypId;
            }
            if (arende.ArendestatusId != 0)
            {
                arendeDb.ArendestatusId= arende.ArendestatusId;
            }
            DbContext.SaveChanges();
        }

        public void UpdateCaseReporters(int caseId, List<string> userIdList, string userName)
        {
            //delete prevoious reporters for current case
            var currentUsersList = DbContext.ArendeKontaktperson.RemoveRange(DbContext.ArendeKontaktperson.Where(x => x.ArendeId == caseId));
            DbContext.SaveChanges();

            //Insert new reporters
            foreach (var userId in userIdList)
            {
                var caseContact = new ArendeKontaktperson
                {
                    ArendeId = caseId,
                    ApplicationUserId = userId,
                    SkapadAv = userName,
                    SkapadDatum = DateTime.Now,
                    AndradAv = userName,
                    AndradDatum = DateTime.Now
                };
                DbContext.ArendeKontaktperson.Add(caseContact);
            }
            DbContext.SaveChanges();
        }

        public void UpdateSubdirReportObligation(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtype)
        {
            var itemDb = DbContext.AdmUppgiftsskyldighetOrganisationstyp.SingleOrDefault(x => x.Id == subdirOrgtype.Id);

            itemDb.SkyldigFrom = subdirOrgtype.SkyldigFrom;
            itemDb.SkyldigTom = subdirOrgtype.SkyldigTom;
            itemDb.AndradAv = subdirOrgtype.AndradAv;
            itemDb.AndradDatum = subdirOrgtype.AndradDatum;
            DbContext.SaveChanges();
        }

        public void AddRoleToFilipUser(string userId, string roleName)
        {
            //Anv RoleManager istället? Flytta till svc-layer?
            //var user = DbContext.Users.Where(x => x.Id == userId).Include(u => u.);

            //var finns = false;
            //var existingRolesInFilip = DbContext.AspNetRoles.ToString();
            //var roleid = DbContext.AspNetRoles.Where(x => x.Name == roleName).Select(x => x.Id).ToString();
            //var currentRolesForUser = DbContext.AspNetUserRoles.Where(x => x.UserId == userId).ToList();
            //foreach (var userRole in currentRolesForUser)
            //{
            //    var role = DbContext.AspNetRoles.SingleOrDefault(x => x.Id == userRole.RoleId);
            //    if (role.Name == roleName)
            //    {
            //        finns = true;
            //    }
            //}
            //if (!finns)
            //{
            //    var userRole = new AspNetUserRoles
            //    {
            //        UserId = userId,
            //        RoleId = roleid
            //    };
            //    DbContext.AspNetUserRoles.Add(userRole);
            //    DbContext.SaveChanges();
            //}

        }

        public void UpdateCaseUnregisteredReporters(int caseId, List<UndantagEpostadress> userList, string userName)
        {
            //delete prevoious unregistered reporters for current case
            var currentUsersList = DbContext.UndantagEpostadress.RemoveRange(DbContext.UndantagEpostadress.Where(x => x.ArendeId == caseId));
            DbContext.SaveChanges();

            //Insert new unregistered reporters
            foreach (var user in userList)
            {
                DbContext.UndantagEpostadress.Add(user);
            }
            DbContext.SaveChanges();
        }

        public void UpdateOrgUnit(Organisationsenhet orgUnit)
        {
            var orgU = DbContext.Organisationsenhet.Where(u => u.Id == orgUnit.Id).Select(u => u).SingleOrDefault();
            orgU.Enhetsnamn = orgUnit.Enhetsnamn;
            orgU.Enhetskod = orgUnit.Enhetskod;
            orgU.AktivFrom = orgUnit.AktivFrom;
            orgU.AktivTom = orgUnit.AktivTom;
            orgU.Filkod = orgUnit.Filkod;
            orgU.AndradDatum = orgUnit.AndradDatum;
            orgU.AndradAv = orgUnit.AndradAv;
            DbContext.SaveChanges(); 
        }

        public void UpdateOrgType(AdmOrganisationstyp orgType)
        {
            var orgTypeDb = DbContext.AdmOrganisationstyp.Where(u => u.Id == orgType.Id).Select(u => u).SingleOrDefault();
            orgTypeDb.Typnamn = orgType.Typnamn;
            orgTypeDb.Beskrivning = orgType.Beskrivning;
            orgTypeDb.AndradDatum = orgType.AndradDatum;
            orgTypeDb.AndradAv = orgType.AndradAv;
            DbContext.SaveChanges();
        }

        public void UpdateReportObligation(AdmUppgiftsskyldighet repObligation)
        {
            var repObl = DbContext.AdmUppgiftsskyldighet.Where(u => u.Id == repObligation.Id).Select(u => u).SingleOrDefault();
            repObl.DelregisterId = repObligation.DelregisterId;
            repObl.RapporterarPerEnhet = repObligation.RapporterarPerEnhet;
            repObl.SkyldigFrom = repObligation.SkyldigFrom;
            repObl.SkyldigTom = repObligation.SkyldigTom;
            repObl.AndradDatum = repObligation.AndradDatum;
            repObl.AndradAv = repObligation.AndradAv;

            DbContext.SaveChanges();
        }

        public void UpdateUnitReportObligation(AdmEnhetsUppgiftsskyldighet unitRepObligation)
        {
            var unitRepOblDb = DbContext.AdmEnhetsUppgiftsskyldighet.Where(u => u.Id == unitRepObligation.Id).Select(u => u).SingleOrDefault();
            unitRepOblDb.UppgiftsskyldighetId = unitRepObligation.UppgiftsskyldighetId;
            unitRepOblDb.SkyldigFrom = unitRepObligation.SkyldigFrom;
            unitRepOblDb.SkyldigTom = unitRepObligation.SkyldigTom;
            unitRepOblDb.AndradDatum = unitRepObligation.AndradDatum;
            unitRepOblDb.AndradAv = unitRepObligation.AndradAv;

            DbContext.SaveChanges();
        }

        public void UpdateFAQCategory(AdmFAQKategori faqCategory)
        {
            var faqCatDb = DbContext.AdmFAQKategori.Where(x => x.Id == faqCategory.Id).Select(x => x).SingleOrDefault();
            faqCatDb.Kategori = faqCategory.Kategori;
            faqCatDb.Sortering = faqCategory.Sortering;
            faqCatDb.AndradDatum = faqCategory.AndradDatum;
            faqCatDb.AndradAv = faqCategory.AndradAv;

            DbContext.SaveChanges();
        }

        public void UpdateFAQ(AdmFAQ faq)
        {
            var faqDb = DbContext.AdmFAQ.Where(x => x.Id == faq.Id).Select(x => x).SingleOrDefault();
            faqDb.Fraga = faq.Fraga;
            faqDb.Svar = faq.Svar;
            faqDb.Sortering = faq.Sortering;
            faqDb.RegisterId = faq.RegisterId;
            faqDb.AndradDatum = faq.AndradDatum;
            faqDb.AndradAv = faq.AndradAv;

            DbContext.SaveChanges();
        }

        public void UpdateHoliday(AdmHelgdag holiday)
        {
            var holidayDb = DbContext.AdmHelgdag.Where(x => x.Id == holiday.Id).Select(x => x).SingleOrDefault();
            holidayDb.Helgdatum = holiday.Helgdatum;
            holidayDb.Helgdag = holiday.Helgdag;
            holidayDb.AndradDatum = holiday.AndradDatum;
            holidayDb.AndradAv = holiday.AndradAv;

            DbContext.SaveChanges();
        }

        public void UpdateSpecialDay(AdmSpecialdag specialDay)
        {
            var specialDayDb = DbContext.AdmSpecialdag.Where(x => x.Id == specialDay.Id).Select(x => x).SingleOrDefault();
            specialDayDb.Specialdagdatum = specialDay.Specialdagdatum;
            specialDayDb.Oppna = specialDay.Oppna;
            specialDayDb.Stang = specialDay.Stang;
            specialDayDb.Anledning = specialDay.Anledning;
            specialDayDb.AndradAv = specialDay.AndradAv;
            specialDayDb.AndradDatum = specialDay.AndradDatum;
            DbContext.SaveChanges();
        }

        public void UpdateInfoText(AdmInformation infoText)
        {
            var infoTextDb = DbContext.AdmInformation.Where(x => x.Id == infoText.Id).Select(x => x).SingleOrDefault();
            infoTextDb.Text = infoText.Text;
            infoTextDb.AndradAv = infoText.AndradAv;
            infoTextDb.AndradDatum = infoText.AndradDatum;

            DbContext.SaveChanges();
        }

        public void UpdateDirectory(AdmRegister directory)
        {
            var registerToUpdate = DbContext.AdmRegister.Where(x => x.Id == directory.Id).SingleOrDefault();
            registerToUpdate.Registernamn = directory.Registernamn;
            registerToUpdate.Beskrivning = directory.Beskrivning;
            registerToUpdate.Kortnamn = directory.Kortnamn;
            registerToUpdate.Inrapporteringsportal = directory.Inrapporteringsportal;
            registerToUpdate.GrupperaRegister = directory.GrupperaRegister;
            registerToUpdate.GrupperaKontaktpersoner = directory.GrupperaKontaktpersoner;
            registerToUpdate.AndradAv = directory.AndradAv;
            registerToUpdate.AndradDatum = directory.AndradDatum;

            DbContext.SaveChanges();
        }

        public void UpdateSubDirectory(AdmDelregister subDirectory)
        {
            var subDirectoryToUpdate = DbContext.AdmDelregister.SingleOrDefault(x => x.Id == subDirectory.Id);
            subDirectoryToUpdate.Delregisternamn = subDirectory.Delregisternamn;
            subDirectoryToUpdate.Beskrivning = subDirectory.Beskrivning;
            subDirectoryToUpdate.Kortnamn = subDirectory.Kortnamn;
            subDirectoryToUpdate.Inrapporteringsportal = subDirectory.Inrapporteringsportal;
            subDirectoryToUpdate.Slussmapp = subDirectory.Slussmapp;
            subDirectoryToUpdate.AndradAv = subDirectory.AndradAv;
            subDirectoryToUpdate.AndradDatum = subDirectory.AndradDatum;

            DbContext.SaveChanges();
        }

        public void UpdateRegulation(AdmForeskrift regulation)
        {
            var regulationToUpdate = DbContext.AdmForeskrift.SingleOrDefault(x => x.Id == regulation.Id);
            regulationToUpdate.Forfattningsnr = regulation.Forfattningsnr;
            regulationToUpdate.Forfattningsnamn = regulation.Forfattningsnamn;
            regulationToUpdate.GiltigFrom = regulation.GiltigFrom;
            regulationToUpdate.GiltigTom = regulation.GiltigTom;
            regulationToUpdate.Beslutsdatum = regulation.Beslutsdatum;
            regulationToUpdate.AndradAv = regulation.AndradAv;
            regulationToUpdate.AndradDatum = regulation.AndradDatum;

            DbContext.SaveChanges();
        }

        public void UpdateExpectedDelivery(AdmForvantadleverans forvLev)
        {
            var forvLevToUpdate = DbContext.AdmForvantadleverans.SingleOrDefault(x => x.Id == forvLev.Id);
            forvLevToUpdate.DelregisterId = forvLev.DelregisterId;
            forvLevToUpdate.FilkravId = forvLev.FilkravId;
            forvLevToUpdate.Period = forvLev.Period;
            forvLevToUpdate.Uppgiftsstart = forvLev.Uppgiftsstart;
            forvLevToUpdate.Uppgiftsslut = forvLev.Uppgiftsslut;
            forvLevToUpdate.Rapporteringsstart = forvLev.Rapporteringsstart;
            forvLevToUpdate.Rapporteringsslut = forvLev.Rapporteringsslut;
            forvLevToUpdate.Rapporteringsenast = forvLev.Rapporteringsenast;
            forvLevToUpdate.Paminnelse1 = forvLev.Paminnelse1;
            forvLevToUpdate.Paminnelse2 = forvLev.Paminnelse2;
            forvLevToUpdate.Paminnelse3 = forvLev.Paminnelse3;
            forvLevToUpdate.AndradAv = forvLev.AndradAv;
            forvLevToUpdate.AndradDatum = forvLev.AndradDatum;

            DbContext.SaveChanges();
        }


        public void UpdateExpectedFile(AdmForvantadfil forvFil)
        {
            var forvFileToUpdate = DbContext.AdmForvantadfil.SingleOrDefault(x => x.Id == forvFil.Id);
            forvFileToUpdate.Filmask = forvFil.Filmask;
            forvFileToUpdate.Regexp = forvFil.Regexp;
            forvFileToUpdate.Obligatorisk = forvFil.Obligatorisk;
            forvFileToUpdate.Tom= forvFil.Tom;
            forvFileToUpdate.AndradAv = forvFil.AndradAv;
            forvFileToUpdate.AndradDatum = forvFil.AndradDatum;

            DbContext.SaveChanges();
        }

        public void UpdateFileRequirement(AdmFilkrav filkrav)
        {
            var filereqToUpdate = DbContext.AdmFilkrav.SingleOrDefault(x => x.Id == filkrav.Id);
            filereqToUpdate.Namn = filkrav.Namn;
            filereqToUpdate.InsamlingsfrekvensId = filkrav.InsamlingsfrekvensId;
            filereqToUpdate.Uppgiftsstartdag = filkrav.Uppgiftsstartdag;
            filereqToUpdate.Uppgiftslutdag = filkrav.Uppgiftslutdag;
            filereqToUpdate.Rapporteringsstartdag = filkrav.Rapporteringsstartdag;
            filereqToUpdate.Rapporteringsslutdag = filkrav.Rapporteringsslutdag;
            filereqToUpdate.RapporteringSenastdag = filkrav.RapporteringSenastdag;
            filereqToUpdate.Paminnelse1dag = filkrav.Paminnelse1dag;
            filereqToUpdate.Paminnelse2dag = filkrav.Paminnelse2dag;
            filereqToUpdate.Paminnelse3dag = filkrav.Paminnelse3dag;
            filereqToUpdate.RapporteringEfterAntalManader = filkrav.RapporteringEfterAntalManader;
            filereqToUpdate.UppgifterAntalmanader = filkrav.UppgifterAntalmanader;
            filereqToUpdate.AndradAv = filkrav.AndradAv;
            filereqToUpdate.AndradDatum = filkrav.AndradDatum;

            DbContext.SaveChanges();
        }

        public void UpdateCollectFrequency(AdmInsamlingsfrekvens insamlingsfrekvens)
        {
            var collFreqToUpdate = DbContext.AdmInsamlingsfrekvens.SingleOrDefault(x => x.Id == insamlingsfrekvens.Id);
            collFreqToUpdate.Insamlingsfrekvens = insamlingsfrekvens.Insamlingsfrekvens;
            DbContext.SaveChanges();
        }

        //public void UpdateUserInfo(AppUserAdmin user)
        //{
        //    var userDb = IdentityDbContext.Users.SingleOrDefault(x => x.Id == user.Id);
        //    userDb.AndradAv = user.AndradAv;
        //    userDb.AndradDatum = user.AndradDatum;
        //    IdentityDbContext.SaveChanges();
        //}

        public void SaveOpeningHours(AdmKonfiguration admKonf)
        {
            var konfDb = DbContext.AdmKonfiguration.Where(x => x.Typ == admKonf.Typ).Select(x => x).FirstOrDefault();
            konfDb.Varde = admKonf.Varde;
            konfDb.AndradAv = admKonf.AndradAv;
            konfDb.AndradDatum = admKonf.AndradDatum;

            DbContext.SaveChanges();
        }
        public void SaveToFilelogg(string userName, string ursprungligtFilNamn, string nyttFilNamn, int leveransId, int sequenceNumber)
        {
            var logFil = new LevereradFil
            {
                LeveransId = leveransId,
                Filnamn = ursprungligtFilNamn,
                NyttFilnamn = nyttFilNamn,
                Ordningsnr = sequenceNumber,
                SkapadDatum = DateTime.Now,
                SkapadAv = userName,
                AndradDatum = DateTime.Now,
                AndradAv = userName,
                Filstatus = "Levererad"
            };

            DbContext.LevereradFil.Add(logFil);

            DbContext.SaveChanges();
        }

        public void SaveToLoginLog(string userid, string userName)
        {
            var inloggning = new Inloggning
            {
                ApplicationUserId = userid,
                SkapadDatum = DateTime.Now,
                SkapadAv = userName
            };

            DbContext.Inloggning.Add(inloggning);

            DbContext.SaveChanges();
        }

        public List<List<ApplicationUser>> SearchContact(string[] searchString)
        {
            var contactList = new List<List<ApplicationUser>>();

            foreach (string word in searchString)
            {
                contactList.Add(DbContext.Users.Where(x => x.Namn.Contains(word) || x.Email.Contains(word)).ToList());
            }
            return contactList;
        }

        public void SaveChosenRegistersForUser(string userId, string userName, List<RegisterInfo> registerList)
        {
            foreach (var register in registerList)
            {
                if (register.Selected)
                {
                    var roll = new Roll
                    {
                        DelregisterId = register.Id,
                        ApplicationUserId = userId,
                        SkapadDatum = DateTime.Now,
                        SkapadAv = userName,
                        AndradDatum = DateTime.Now,
                        AndradAv = userName
                    };

                    DbContext.Roll.Add(roll);
                }
            }
            DbContext.SaveChanges();
        }

        public void SaveExceptionExpectedFile(UndantagForvantadfil exception)
        {
            DbContext.UndantagForvantadfil.Add(exception);
            DbContext.SaveChanges();
        }

        public void DeleteFAQCategory(int faqCategoryId)
        {
            var faqCatToDelete = DbContext.AdmFAQKategori.SingleOrDefault(x => x.Id == faqCategoryId);

            //Delete all children
            if (faqCatToDelete != null)
            {
                var faqsToDelete = DbContext.AdmFAQ.Where(x => x.FAQkategoriId == faqCategoryId).ToList();
                foreach (var faq in faqsToDelete)
                {
                    DbContext.AdmFAQ.Remove(faq);
                }

                //Delete category
                DbContext.AdmFAQKategori.Remove(faqCatToDelete);
                DbContext.SaveChanges();
            }
        }

        public void DeleteFAQ(int faqId)
        {
            var faqToDelete = DbContext.AdmFAQ.SingleOrDefault(x => x.Id == faqId);
            if (faqToDelete != null)
            {
                DbContext.AdmFAQ.Remove(faqToDelete);
                DbContext.SaveChanges();
            }
        }

        public void DeleteOrgType(int orgTypeId)
        {
            var orgTypeToDelete = DbContext.AdmOrganisationstyp.SingleOrDefault(x => x.Id == orgTypeId);
            if (orgTypeToDelete != null)
            {
                DbContext.AdmOrganisationstyp.Remove(orgTypeToDelete);
                DbContext.SaveChanges();
            }
        }

        public void DeleteHoliday(int holidayId)
        {
            var holidayToDelete = DbContext.AdmHelgdag.SingleOrDefault(x => x.Id == holidayId);
            if (holidayToDelete != null)
            {
                DbContext.AdmHelgdag.Remove(holidayToDelete);
                DbContext.SaveChanges();
            }
        }

        public void DeleteSpecialDay(int specialDayId)
        {
            var specialDayToDelete = DbContext.AdmSpecialdag.SingleOrDefault(x => x.Id == specialDayId);
            if (specialDayToDelete != null)
            {
                DbContext.AdmSpecialdag.Remove(specialDayToDelete);
                DbContext.SaveChanges();
            }
        }

        public void DeleteContact(string contactId)
        {
            var contactToDelete = DbContext.Users.SingleOrDefault(x => x.Id == contactId);

            var contactRoles = DbContext.Roll.Where(x => x.ApplicationUserId == contactId).ToList();

            var contactTypes = DbContext.Kontaktpersonstyp.Where(x => x.KontaktpersonId == contactId).ToList();

 
            if (contactToDelete != null)
            {
                foreach (var role in contactRoles)
                {
                    DbContext.Roll.Remove(role);
                    DbContext.SaveChanges();
                }
                foreach (var type in contactTypes)
                {
                    DbContext.Kontaktpersonstyp.Remove(type);
                    DbContext.SaveChanges();
                }
                DbContext.Users.Remove(contactToDelete);
                DbContext.SaveChanges();
            }
        }

        //public void DeleteAdminUser(string userId)
        //{
        //    var cuserToDelete = IdentityDbContext.Users.SingleOrDefault(x => x.Id == userId);
        //    IdentityDbContext.Users.Remove(cuserToDelete);
        //    IdentityDbContext.SaveChanges();
        //}

        public void DeleteDelivery(int deliveryId)
        {
            var deliveryToDelete = DbContext.Leverans.SingleOrDefault(x => x.Id == deliveryId);
            DbContext.Leverans.Remove(deliveryToDelete);
            DbContext.SaveChanges();
        }

        public void DeleteExceptionExpectedFile(int orgId, int subdirId, int expectedFileId)
        {
            var exceptionToDelete = DbContext.UndantagForvantadfil
                .SingleOrDefault(x => x.OrganisationsId == orgId && x.DelregisterId == subdirId && x.ForvantadfilId == expectedFileId);
            DbContext.UndantagForvantadfil.Remove(exceptionToDelete);
            DbContext.SaveChanges();
        }

        public void DeleteSubdirReportObligation(AdmUppgiftsskyldighetOrganisationstyp subdirOrgtype)
        {
            var subdirOrgtypeToDelete = DbContext.AdmUppgiftsskyldighetOrganisationstyp.SingleOrDefault(x => x.Id == subdirOrgtype.Id);
            DbContext.AdmUppgiftsskyldighetOrganisationstyp.Remove(subdirOrgtypeToDelete);
            DbContext.SaveChanges();
        }

        public void DeleteChosenSubDirectoriesForUser(string userId)
        {
            var rollList = DbContext.Roll.Where(x => x.ApplicationUserId == userId).ToList();
            DbContext.Roll.RemoveRange(rollList);
            DbContext.SaveChanges();
        }


        private RegisterInfo CreateRegisterInfoObj(AdmDelregister delReg, int orgId)
        {
            var regInfo = new RegisterInfo
            {
                Id = delReg.Id,
                Namn = delReg.Delregisternamn,
                Kortnamn = delReg.Kortnamn,
                InfoText = delReg.AdmRegister.Beskrivning + "<br>" + delReg.Beskrivning,
                Slussmapp = delReg.Slussmapp
            };

            //Orgtyper som registret kan rapporteras för
            regInfo.Organisationstyper = new List<KeyValuePair<int,string>>();
            var orgTypeIdsForSubDir = GetOrgTypesIdsForSubDir(delReg.Id);
            foreach (var orgtypeId in orgTypeIdsForSubDir)
            {
                var orgtype = GetOrgtype(orgtypeId);
                KeyValuePair<int, string> keyValuePair =
                    new KeyValuePair<int, string>(orgtype.Id, orgtype.Typnamn);
                regInfo.Organisationstyper.Add(keyValuePair);
            }

            var filkravList = new List<RegisterFilkrav>();
            var i = 1;

            foreach (var filkrav in delReg.AdmFilkrav)
            {
                var regFilkrav = new RegisterFilkrav();
                if (filkrav.Namn != null)
                {
                    regFilkrav.Namn = filkrav.Namn;
                }
                else
                {
                    regFilkrav.Namn = "";
                }

                //Sök forväntad fil för varje filkrav 
                var forvantadeFilerDb = filkrav.AdmForvantadfil.ToList();
                var antObligatoriskaFiler = 0;
                var antEjObligatoriskaFiler = 0;
                var forvantadeFiler = new List<RegisterForvantadfil>();
                var undantagForvantadFil = new List<int>();
                //Kolla om någon förväntad fil ska undantas för aktuell organisation
                undantagForvantadFil = DbContext.UndantagForvantadfil.Where(x => x.OrganisationsId == orgId && x.DelregisterId == delReg.Id).Select(x => x.ForvantadfilId).ToList();


                foreach (var forvFilDb in forvantadeFilerDb)
                {
                    if (!undantagForvantadFil.Contains(forvFilDb.Id))
                    {
                        if (forvFilDb.Obligatorisk)
                        {
                            antObligatoriskaFiler++;
                        }
                        else
                        {
                            antEjObligatoriskaFiler++;
                        }
                        var forvFil = new RegisterForvantadfil
                        {
                            Id = forvFilDb.Id,
                            FilkravId = forvFilDb.FilkravId,
                            ForeskriftsId = forvFilDb.ForeskriftsId,
                            Filmask = forvFilDb.Filmask,
                            Regexp = forvFilDb.Regexp,
                            Obligatorisk = forvFilDb.Obligatorisk,
                            Tom = forvFilDb.Tom
                        };
                        forvantadeFiler.Add(forvFil);
                        regFilkrav.InfoText = regFilkrav.InfoText + "<br> Filformat: " + forvFil.Filmask;
                    }
                }
                //regFilkrav.AntalFiler = forvantadeFilerDb.Count;
                regFilkrav.AntalFiler = forvantadeFiler.Count;
                regFilkrav.AntalObligatoriskaFiler = antObligatoriskaFiler;
                regFilkrav.AntalEjObligatoriskaFiler = antEjObligatoriskaFiler;
                regFilkrav.ForvantadeFiler = forvantadeFiler;

                //get period och forvantadleveransId
                GetPeriodsForAktuellLeverans(filkrav, regFilkrav);
                regFilkrav.InfoText = regFilkrav.InfoText + "<br> Antal filer: " + regFilkrav.AntalFiler;
                regFilkrav.Id = i;

                //Om inga aktuella perioder finns för filkravet ska det inte läggas med i RegInfo
                if (regFilkrav.Perioder != null)
                {
                    if (regFilkrav.Perioder.ToList().Count != 0)
                    {
                        filkravList.Add(regFilkrav);
                        i++;
                    }
                }
            }

            regInfo.Filkrav = filkravList;
            return regInfo;
        }

        public void GetPeriodsForAktuellLeverans(AdmFilkrav filkrav, RegisterFilkrav regFilkrav)
        {
            string period = String.Empty;
            DateTime startDate;
            DateTime endDate;

            DateTime dagensDatum = DateTime.Now.Date;
            var perioder = new List<string>();

            //hämta varje förväntad leverans och sätt rätt period utifrån dagens datum
            var forvlevList = DbContext.AdmForvantadleverans.Where(x => x.FilkravId == filkrav.Id).ToList();

            foreach (var item in forvlevList)
            {
                if (item != null)
                {
                    startDate = item.Rapporteringsstart;
                    endDate = item.Rapporteringsslut;
                    if (dagensDatum >= startDate && dagensDatum <= endDate)
                    {
                        //regInfo.Period = item.Period;
                        perioder.Add(item.Period);
                        //regInfo.ForvantadLevransId = item.Id;
                    }
                }
                regFilkrav.Perioder = perioder;
            }
        }

        public List<List<Organisation>> SearchOrganisation(string[] searchString)
        {
            var orgList = new List<List<Organisation>>();

            foreach (string word in searchString)
            {
                orgList.Add(DbContext.Organisation.Where(x => x.Organisationsnamn.Contains(word) || x.Kommunkod.Contains(word) || x.Landstingskod.Contains(word)).ToList());
            }
            return orgList;
        }


    }
}

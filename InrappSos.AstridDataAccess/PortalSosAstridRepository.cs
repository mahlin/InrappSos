using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;

namespace InrappSos.AstridDataAccess
{
    public class PortalSosAstridRepository : IPortalSosAstridRepository
    {

        private InrappSosIdentityDbContext AstridIdentityDbContext { get; }


        public PortalSosAstridRepository(InrappSosIdentityDbContext identityDbContext)
        {
            AstridIdentityDbContext = identityDbContext;
        }

        public IEnumerable<AppUserAdmin> GetAdminUsers()
        {
            var adminUsers = AstridIdentityDbContext.Users.ToList();
            return adminUsers;
        }

        public string GetAdminUserEmail(string userId)
        {
            var email = AstridIdentityDbContext.Users.Where(x => x.Id == userId).Select(x => x.Email).SingleOrDefault();
            return email;
        }

        public void UpdateAdminUser(AppUserAdmin user)
        {
            var usrDb = AstridIdentityDbContext.Users.Where(u => u.Id == user.Id).Select(u => u).SingleOrDefault();
            usrDb.PhoneNumber = user.PhoneNumber;
            usrDb.Email = user.Email;
            usrDb.AndradDatum = user.AndradDatum;
            usrDb.AndradAv = user.AndradAv;
            AstridIdentityDbContext.SaveChanges();
        }

        public void UpdateAdminUserInfo(AppUserAdmin user)
        {
            var userDb = AstridIdentityDbContext.Users.SingleOrDefault(x => x.Id == user.Id);
            userDb.AndradAv = user.AndradAv;
            userDb.AndradDatum = user.AndradDatum;
            AstridIdentityDbContext.SaveChanges();
        }

        public void DeleteAdminUser(string userId)
        {
            var cuserToDelete = AstridIdentityDbContext.Users.SingleOrDefault(x => x.Id == userId);
            AstridIdentityDbContext.Users.Remove(cuserToDelete);
            AstridIdentityDbContext.SaveChanges();
        }
    }
}
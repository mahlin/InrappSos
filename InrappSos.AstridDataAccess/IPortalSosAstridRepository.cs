using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;

namespace InrappSos.AstridDataAccess
{
    public interface IPortalSosAstridRepository
    {
        IEnumerable<AppUserAdmin> GetAdminUsers();
        string GetAdminUserEmail(string userId);
        void UpdateAdminUser(AppUserAdmin user);

        void UpdateAdminUserInfo(AppUserAdmin user);

        void DeleteAdminUser(string userId);
    }
}
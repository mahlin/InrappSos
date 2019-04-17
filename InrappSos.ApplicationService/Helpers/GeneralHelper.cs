using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;

namespace InrappSos.ApplicationService.Helpers
{
    public class GeneralHelper
    {
        private readonly IPortalSosRepository _portalSosRepository;
        private readonly IPortalSosService _portalSosService;
        private readonly InrappSosDbContext db = new InrappSosDbContext();

        public GeneralHelper()
        {
            _portalSosRepository = new PortalSosRepository(db);
            _portalSosService = new PortalSosService(_portalSosRepository);
        }

        public bool IsTestUser(string userId)
        {
            var testTeamUserOrg = _portalSosService.HamtaOrgForAnvandare(userId);
            var isTestOrg = IsTestOrg(testTeamUserOrg.Id);
            return isTestOrg;
        }
        public bool IsTestOrg(int orgId)
        {
            var allTestOrgsStr = ConfigurationManager.AppSettings["TestOrgs"];
            string[] testOrgsArr = allTestOrgsStr.Split(',');
            var testOrg = false;
            foreach (var testOrgId in testOrgsArr)
            {
                if (Convert.ToInt32(testOrgId) == orgId)
                {
                    testOrg = true;
                }
            }
            return testOrg;
        }

    }
}

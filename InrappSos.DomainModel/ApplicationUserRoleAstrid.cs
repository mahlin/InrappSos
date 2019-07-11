using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DomainModel
{

    public class ApplicationUserRoleAstrid : IdentityUserRole
    {
        public ApplicationUserRoleAstrid()
            : base()
        {
        }

        public ApplicationRoleAstrid Role { get; set; }
    }
}


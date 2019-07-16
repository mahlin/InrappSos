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
        public ApplicationRoleAstrid Role { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
    }
}


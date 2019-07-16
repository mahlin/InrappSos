using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DomainModel
{
    public class ApplicationUserRole : IdentityUserRole
    {
        //public ApplicationRole Role { get; set; }
        public DateTime skapadDatum { get; set; }
        public string skapadAv { get; set; }
        public DateTime andradDatum { get; set; }
        public string andradAv { get; set; }
    }
}

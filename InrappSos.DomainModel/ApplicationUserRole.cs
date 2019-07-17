using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DomainModel
{
    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationRole ApplicationRole { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
    }
}

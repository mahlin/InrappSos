using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DomainModel
{
    public class ApplicationRole : IdentityRole
    {
        public virtual string BeskrivandeNamn { get; set; }
        public virtual string Beskrivning { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}

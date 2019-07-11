using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DomainModel
{
    public class ApplicationRoleAstrid : IdentityRole
    {
        public ApplicationRoleAstrid() : base() { }
        public virtual string BeskrivandeNamn { get; set; }
        public virtual string Beskrivning { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
    }
}

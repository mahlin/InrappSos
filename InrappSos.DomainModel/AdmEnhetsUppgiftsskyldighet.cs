using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmEnhetsUppgiftsskyldighet
    {
        public int Id { get; set; }
        public int OrganisationsenhetsId { get; set; }
        public int UppgiftsskyldighetId { get; set; }
        public DateTime? SkyldigFrom { get; set; }
        public DateTime? SkyldigTom { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisationsenhet Organisationsenhet { get; set; }
        public virtual AdmUppgiftsskyldighet AdmUppgiftsskyldighet { get; set; }

    }
}
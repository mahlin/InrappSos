using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmUppgiftsskyldighet
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public int DelregisterId { get; set; }
        public DateTime? SkyldigFrom { get; set; }
        public DateTime? SkyldigTom { get; set; }
        public bool RapporterarPerEnhet { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmDelregister AdmDelregister { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<AdmEnhetsUppgiftsskyldighet> AdmEnhetsUppgiftsskyldighet { get; set; }
    }
}
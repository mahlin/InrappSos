using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmDelregister
    {
        public int Id { get; set; }
        public int RegisterId { get; set; }
        public string Delregisternamn { get; set; }
        public string Kortnamn { get; set; }
        public string Beskrivning { get; set; }
        public string Slussmapp { get; set; }
        public bool Inrapporteringsportal { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmRegister AdmRegister { get; set; }
        public virtual ICollection<AdmFilkrav> AdmFilkrav { get; set; }
        public virtual ICollection<AdmForvantadleverans> AdmForvantadleverans { get; set; }
        public virtual ICollection<AdmUppgiftsskyldighet> AdmUppgiftsskyldighet { get; set; }
        public virtual ICollection<AdmUppgiftsskyldighetOrganisationstyp> AdmUppgiftsskyldighetOrganisationstyp { get; set; }
        public virtual ICollection<UndantagForvantadfil> UndantagForvantadfil { get; set; }


    }
}
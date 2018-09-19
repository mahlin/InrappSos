using System;
using System.Collections.Generic;

namespace InrappSos.DomainModel
{
    public class AdmFilkrav
    {
        public int Id { get; set; }
        public int DelregisterId { get; set; }
        public int? ForeskriftsId { get; set; }
        public int? InsamlingsfrekvensId { get; set; }
        public string Namn { get; set; }
        public int? Uppgiftsstartdag { get; set; }
        public int? Uppgiftslutdag { get; set; }
        public int? Rapporteringsstartdag { get; set; }
        public int? Rapporteringsslutdag { get; set; }
        public int? RapporteringSenastdag { get; set; }
        public int? Paminnelse1dag { get; set; }
        public int? Paminnelse2dag { get; set; }
        public int? Paminnelse3dag { get; set; }
        public int? RapporteringEfterAntalManader { get; set; }
        public int? UppgifterAntalmanader { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmDelregister AdmDelregister { get; set; }
        public virtual AdmForeskrift AdmForskrift { get; set; }
        public virtual AdmInsamlingsfrekvens AdmInsamlingsfrekvens { get; set; }
        public virtual ICollection<AdmForvantadfil> AdmForvantadfil { get; set; }
        public virtual ICollection<AdmForvantadleverans> AdmForvantadleverans { get; set; }

    }
}
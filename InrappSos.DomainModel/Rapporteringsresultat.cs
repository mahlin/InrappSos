using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;

namespace InrappSos.DomainModel
{
    public class Rapporteringsresultat
    {
        public string Lankod { get; set; }
        public string Kommunkod { get; set; }
        public string Organisationsnamn { get; set; }
        public int RegisterId { get; set; }
        public string Register { get; set; }
        public string Period { get; set; }
        public string RegisterKortnamn { get; set; }
        public string Enhetskod { get; set; }
        public int? AntalLeveranser { get; set; }
        public DateTime? Leveranstidpunkt { get; set; }
        public string Leveransstatus { get; set; }
        public string Email { get; set; }
        public string Filnamn { get; set; }
        public string NyttFilnamn { get; set; }
        public string Filstatus { get; set; }
        public DateTime SkyldigFrom { get; set; }
        public DateTime? SkyldigTom { get; set; }
        public DateTime? Uppgiftsstart { get; set; }
        public DateTime? Uppgiftsslut { get; set; }
        public DateTime? Rapporteringsstart { get; set; }
        public DateTime? Rapporteringsslut { get; set; }
        public DateTime? Rapporteringsenast { get; set; }
        [Key]
        [Column(Order = 3)]
        public int UppgiftsskyldighetId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int OrganisationsId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int DelregisterId { get; set; }
        [Key]
        [Column(Order = 4)]
        public int? ForvantadLeveransId { get; set; }
        [Key]
        [Column(Order = 5)]
        public int? OrganisationsenhetsId { get; set; }
        public int? LeveransId { get; set; }

        

    }
}
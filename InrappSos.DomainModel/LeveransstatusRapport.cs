using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;

namespace InrappSos.DomainModel
{
    public class LeveransstatusRapport
    {
        public int LeveransId { get; set; }
        public string Enhetskod { get; set; }
        public string Kortnamn { get; set; }
        public string Filnamn { get; set; }
        public string Filstatus { get; set; }
        [Key]
        [Column(Order = 1)]
        public int OrganisationsId { get; set; }
        [Key]
        [Column(Order = 4)]
        public int? OrganisationsenhetsId { get; set; }
        public string KontaktpersonId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int DelregisterId { get; set; }
        [Key]
        [Column(Order = 3)]
        public int ForvantadLeveransId { get; set; }
        public DateTime? Leveranstidpunkt { get; set; }
        public string Leveransstatus { get; set; }
        public string Period { get; set; }
    }
}
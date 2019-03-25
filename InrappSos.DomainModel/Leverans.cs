using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class Leverans
    {
        public int Id { get; set; }
        public int ForvantadleveransId { get; set; }
        public int OrganisationId { get; set; }
        [ForeignKey("Organisationsenhet")]
        public int? OrganisationsenhetsId { get; set; }
        public string ApplicationUserId { get; set; }
        public int DelregisterId { get; set; }
        public int? SFTPkontoId { get; set; }
        public DateTime Leveranstidpunkt { get; set; }
        public string Leveransstatus { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Organisationsenhet Organisationsenhet { get; set; }
        public virtual AdmForvantadleverans AdmForvantadleverans { get; set; }
        public virtual ICollection<LevereradFil> LevereradeFiler { get; set; }
    }
}
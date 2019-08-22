using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class Arende
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public string Arendenamn { get; set; }
        public string Arendenr { get; set; }
        public int ArendetypId { get; set; }
        public int ArendeansvarId { get; set; }
        public bool Aktiv { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Arendetyp Arendetyp { get; set; }
        public virtual ArendeAnsvarig ArendeAnsvarig { get; set; }
        public virtual ICollection<ArendeKontaktperson> ArendeKontaktperson { get; set; }
        public virtual ICollection<DroppadFil> DroppadFil { get; set; }
        public virtual ICollection<PreKontakt> PreKontakt { get; set; }

    }
}

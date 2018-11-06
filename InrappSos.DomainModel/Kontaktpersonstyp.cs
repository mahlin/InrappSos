using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class Kontaktpersonstyp
    {
        public int Id { get; set; }
        public int AdmKontaktpersonstypId { get; set; }
        public string KontaktpersonId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ApplicationUser Kontaktperson { get; set; }
        public virtual AdmKontaktpersonstyp AdmKontaktpersonstyp { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class AdmKontaktpersonstyp
    {
        public int Id { get; set; }
        public string KontaktpersonstypStr { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<Kontaktpersonstyp> Kontaktpersonstyp { get; set; }
    }
}

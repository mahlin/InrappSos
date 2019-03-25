using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class SFTPkonto
    {
        public int Id { get; set; }
        public string Kontonamn { get; set; }
        public int OrganisationsId { get; set; }
        public int RegisterId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual AdmRegister AdmRegister { get; set; }
        public virtual ICollection<KontaktpersonSFTPkonto> KontaktpersonSFTPkonto { get; set; }
        public virtual ICollection<Leverans> Leverans { get; set; }
    }
}

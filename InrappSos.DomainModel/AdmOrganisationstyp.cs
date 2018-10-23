using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class AdmOrganisationstyp
    {
        public int Id { get; set; }
        public string Typnamn { get; set; }
        public string Beskrivning { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<Organisationstyp> Organisationstyp { get; set; }
        public virtual ICollection<UppgiftsskyldighetOrganisationstyp> UppgiftsskyldighetOrganisationstyp { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class Organisationstyp
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public int OrganisationstypId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual AdmOrganisationstyp AdmOrganisationstyp { get; set; }
    }
}

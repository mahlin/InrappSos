using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class OrgUnitInfo
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public string Enhetsnamn { get; set; }
        public string Enhetskod { get; set; }
        public bool Selected { get; set; } = false;
    }
}

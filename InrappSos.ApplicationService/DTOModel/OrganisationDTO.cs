using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class OrganisationDTO
    {
        public int Id { get; set; }
        public string Landstingskod { get; set; }
        public string Kommunkod { get; set; }
        public string Organisationsnamn { get; set; }
        public string KommunkodOchOrgnamn { get; set; }
        public List<OrganisationsenhetDTO> Organisationsenheter { get; set; }
    }
}

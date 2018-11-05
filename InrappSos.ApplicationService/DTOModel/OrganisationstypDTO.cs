using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class OrganisationstypDTO
    {
        public int Organisationstypid { get; set; }
        public string Typnamn { get; set; }
        public string Beskrivning { get; set; }
        public bool Selected { get; set; } = false;
    }
}

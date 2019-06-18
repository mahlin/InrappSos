using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class AdmUppgiftsskyldighetOrganisationstypDTO
    {
        public int Id { get; set; }
        public int DelregisterId { get; set; }
        public int OrganisationstypId { get; set; }
        public string OrganisationstypNamn { get; set; }
        public bool Selected { get; set; } = false;
        public DateTime SkyldigFrom { get; set; }
        public DateTime? SkyldigTom { get; set; }
    }
}

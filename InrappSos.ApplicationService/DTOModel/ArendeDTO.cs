using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class ArendeDTO
    {
            public int Id { get; set; }
            public int OrganisationsId { get; set; }
            public string Organisationsnamn { get; set; }
            public string Arendenamn { get; set; }
            public string Arendenr { get; set; }
            public int ArendetypId { get; set; }
            public string Arendetyp { get; set; }
            public int ArendestatusId { get; set; }
            public string Arendestatus { get; set; }
            public DateTime StartDatum { get; set; }
            public DateTime? SlutDatum { get; set; }
            public string AnsvarigEpost { get; set; }
            public string Rapportorer { get; set; }


    }
}

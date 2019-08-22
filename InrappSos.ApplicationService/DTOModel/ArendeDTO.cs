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
        public int ArendeanvsarId { get; set; }
        public bool Aktiv { get; set; }
        public string Arendeansvar { get; set; }
        public string Rapportorer { get; set; }
        public List<ArendeKontaktpersonDTO> Kontaktpersoner { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class OrganisationsenhetDTO
    {
        public int Id { get; set; }
        public string Enhetsnamn { get; set; }
        public string Enhetskod { get; set; }
        public DateTime? AktivFrom { get; set; }
        public DateTime? AktivTom { get; set; }
        public string Filkod { get; set; }
        public bool Selected { get; set; }
    }
}

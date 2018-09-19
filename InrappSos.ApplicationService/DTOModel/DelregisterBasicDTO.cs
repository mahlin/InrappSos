using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class DelregisterBasicDTO
    {
        public int Id { get; set; }
        public string Delregisternamn { get; set; }
        public string Kortnamn { get; set; }
        public IEnumerable<ForvantadLeveransBasicDTO> ForvantadeLeveranserList { get; set; }
    }
}

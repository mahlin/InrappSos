using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class RegisterBasicDTO
    {
        public int Id { get; set; }
        public string Registernamn { get; set; }
        public string Kortnamn { get; set; }
        public IEnumerable<DelregisterBasicDTO> DelregisterList { get; set; }
    }
}

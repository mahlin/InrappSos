using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class ArendeKontaktpersonDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool Selected { get; set; } = false;
    }
}

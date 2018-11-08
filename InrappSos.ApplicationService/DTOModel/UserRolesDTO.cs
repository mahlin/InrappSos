using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class UserRolesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string Beskrivning { get; set; }
        public bool Selected { get; set; } = false;
    }
}

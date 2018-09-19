using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.ApplicationService.DTOModel
{
    public class RegisterLeveransDTO
    {
        public string RegisterKortnamn { get; set; }
        public List<LeveransStatusDTO> Leveranser { get; set; }

        

    }
}
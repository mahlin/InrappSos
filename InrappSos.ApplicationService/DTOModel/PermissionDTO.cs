using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class PermissionDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string PermissionName { get; set; }
        public bool Chosen { get; set; }
    }
}

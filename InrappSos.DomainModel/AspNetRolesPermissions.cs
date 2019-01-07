using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class AspNetRolesPermissions
    {
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public string RoleId { get; set; }
        public virtual AspNetPermissions AspNetPermissions { get; set; }

    }
}

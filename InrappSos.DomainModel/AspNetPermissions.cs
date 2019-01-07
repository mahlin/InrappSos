using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AspNetPermissions
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string PermissionName { get; set; }
        public virtual ICollection<AspNetRolesPermissions> AspNetRolesPermissions { get; set; }
    }
}
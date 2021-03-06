﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class RoleViewModels
    {
        public List<ApplicationRole> AstridRoller { get; set; }

        public List<ApplicationRole> FilipRoller { get; set; }

        public class RoleViewModel
        {
            public string RoleName { get; set; }
            public string BeskrivandeNamn { get; set; }
            public string Beskrivning { get; set; }
        }
        
        public class SelectRoleEditorViewModel
        {
            public SelectRoleEditorViewModel()
            {
            }

            public SelectRoleEditorViewModel(ApplicationRole role)
            {
                this.RoleName = role.Name;

                // Assign the new Descrption property:
                this.Description = role.Beskrivning;
            }

            public bool Selected { get; set; }

            [Required]
            public string RoleName { get; set; }

            // Add the new Description property:
            public string Description { get; set; }
        }

        public class EditRoleViewModel
        {
            public string OriginalRoleName { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }

            public EditRoleViewModel()
            {
            }

            public EditRoleViewModel(ApplicationRole role)
            {
                this.OriginalRoleName = role.Name;
                this.RoleName = role.Name;
                this.Description = role.Beskrivning;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class RegisterViewModels
    {

        public class RegisterViewModel
        {
            [Display(Name = "Registers kortnamn")]
            public string RegisterShortName { get; set; }

            public IEnumerable<AdmRegisterViewModel> Registers { get; set; }

            public IEnumerable<AdmDelregister> DelRegisters { get; set; }
            public int SelectedDirectoryId { get; set; }

            public string SelectedSubDirectoryId { get; set; }

        }

        public class AdmRegisterViewModel
        {
            public int Id { get; set; }
            public string Registernamn { get; set; }
            public string Beskrivning { get; set; }
            public string Kortnamn { get; set; }
            public bool Inrapporteringsportal { get; set; }

        }

        public class AdmDelregisterViewModel
        {
            public int Id { get; set; }
            public int RegisterId { get; set; }
            [DisplayName("Register")]
            public string RegisterShortName { get; set; }
            public string Delregisternamn { get; set; }
            public string Kortnamn { get; set; }
            public string Beskrivning { get; set; }
            public string Slussmapp { get; set; }
            public bool Inrapporteringsportal { get; set; }

        }
    }
}
﻿using System;
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

            public List<AdmDelregister> DelRegisters { get; set; }

            public List<AdmUppgiftsskyldighetOrganisationstypViewModel> DelRegistersOrganisationstyper { get; set; }
            public int SelectedDirectoryId { get; set; }

            public string SelectedSubDirectoryId { get; set; }

            public List<OrganisationstypViewModel> Organisationstyper { get; set; }

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

        public class AdmForeskriftViewModel
        {
            public IEnumerable<AdmForeskrift> ForeskriftList { get; set; }
            public AdmForeskrift NyForeskrift { get; set; }
            [DisplayName("Register")]
            public string RegisterShortName { get; set; }
            public int SelectedDirectoryId { get; set; }
            public int SelectedDirectoryIdInUpdate { get; set; }
            public int SelectedForeskriftId { get; set; }
            public AdmForeskrift SelectedForeskrift { get; set; }
        }


        public class AdmUppgiftsskyldighetOrganisationstypViewModel
        {
            public int Id { get; set; }
            public int DelregisterId { get; set; }
            public string DelregisterKortnamn { get; set; }
            public int OrganisationstypId { get; set; }
            public string OrganisationstypNamn { get; set; }
            public List<string> Orgtyper { get; set; }
            public string StringOfOrgtypes { get; set; }
            public List<OrganisationstypViewModel> ListOfOrgtypes { get; set; }
            public DateTime SkyldigFrom { get; set; }
            public DateTime? SkyldigTom { get; set; }

        }

        public class OrganisationstypViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; } = false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class OrganisationViewModels
    {
        public class OrganisationViewModel
        {
            [Display(Name = "Kommunkod")]
            [StringLength(4, ErrorMessage = "{0} måste vara {2} tecken långt.", MinimumLength = 4)]
            [RegularExpression("([0-9]+)", ErrorMessage = "Kommunkoden måste vara numerisk.")]
            public string Kommunkod { get; set; }
            public Organisation Organisation { get; set; }

            public IEnumerable<ApplicationUserViewModel> ContactPersons { get; set; }

            public string SelectedContactId { get; set; }

            public string SelectedCountyCode { get; set; }
            public int SelectedOrganisationId { get; set; }


            public IEnumerable<Organisationsenhet> OrgUnits { get; set; }

            public  IEnumerable<ReportObligationsViewModel> ReportObligations { get; set; }

        }
        
        public class OrganisationsenhetViewModel
        {
            public int Organisationsid { get; set; }
            public string Enhetsnamn { get; set; }
            public string Enhetskod { get; set; }
            public DateTime? AktivFrom { get; set; }
            public DateTime? AktivTom { get; set; }
            public IEnumerable<UnitReportObligationsViewModel> UnitReportObligations { get; set; }

        }

        public class ReportObligationsViewModel
        {
            public int Id { get; set; }
            public int OrganisationId { get; set; }
            public int DelregisterId { get; set; }
            public string DelregisterKortnamn { get; set; }
            public DateTime? SkyldigFrom { get; set; }
            public DateTime? SkyldigTom { get; set; }
            public bool RapporterarPerEnhet { get; set; }

        }

        public class UnitReportObligationsViewModel
        {
            public int Id { get; set; }
            public int SelectedOrganisationId { get; set; }
            public int SelectedOrganisationsenhetsId { get; set; }
            public int SelectedDelregisterId { get; set; }
            public List<OrganisationDTO> OrganisationList { get; set; }
            public IEnumerable<AdmEnhetsUppgiftsskyldighetViewModel> UnitReportObligations { get; set; }
            public string DelregisterKortnamn { get; set; }
            public string Organisationsnamn { get; set; }
            public int UppgiftsskyldighetId { get; set; }
            public DateTime? SkyldigFrom { get; set; }
            public DateTime? SkyldigTom { get; set; }

        }

        public class AdmEnhetsUppgiftsskyldighetViewModel
        {
            public int Id { get; set; }
            public int OrganisationsenhetsId { get; set; }
            public int UppgiftsskyldighetId { get; set; }
            public DateTime? SkyldigFrom { get; set; }
            public DateTime? SkyldigTom { get; set; }
            public string DelregisterKortnamn { get; set; }
        }


        public class ApplicationUserViewModel
        {
            public string ID { get; set; }
            public int OrganisationId { get; set; }
            public string Namn { get; set; }
            public DateTime? AktivFrom { get; set; }
            public DateTime? AktivTom { get; set; }
            public int? Status { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public string ValdaDelregister { get; set; }
            public DateTime SkapadDatum { get; set; }
            public string SkapadAv { get; set; }
            public DateTime AndradDatum { get; set; }
            public string AndradAv { get; set; }
            public bool OkToDelete { get; set; }

        }


    }
}
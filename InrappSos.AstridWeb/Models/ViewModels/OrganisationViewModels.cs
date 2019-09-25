using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
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

            public List<ApplicationUserViewModel> ContactPersons { get; set; }

            public List<SFTPkontoViewModel> SFTPAccounts { get; set; }

            public string SelectedContactId { get; set; }
            public int SelectedAccountId { get; set; }

            public string SelectedCountyCode { get; set; }
            public int SelectedOrganisationId { get; set; }
            public int SelectedOrgTypId { get; set; }
            public int SelectedArendetypId { get; set; }

            public int SelectedArendeId { get; set; }

            public int SelectedArendeansvarigId { get; set; }

            public int SelectedRegisterId { get; set; }

            public string ChosenOrganisationTypesNames { get; set; }
            public List<OrganisationstypDTO> OrgtypesForOrgList { get; set; }
            
            public List<List<Organisation>> SearchResult { get; set; }

            public List<List<ApplicationUserViewModel>> ContactSearchResult { get; set; }

            public List<Organisationsenhet> OrgUnits { get; set; }

            public  List<ReportObligationsViewModel> ReportObligations { get; set; }

            public List<AdmOrganisationstyp> OrganisationTypes { get; set; }

            public List<UndantagEpostadressViewModel> UndantagEpostadresser { get; set; }

            public List<UndantagForvantadfilViewModel> UndantagForvantadfiler { get; set; }
            public List<ArendeViewModel> Arenden { get; set; }
            public List<IdentityRoleViewModel> Roller { get; set; }
            public List<ContactViewModel> Kontaktpersoner { get; set; }
            public string Origin { get; set; }
            [DisplayName("Visa även inaktiva")]
            public bool VisaInaktiva { get; set; }

        }

        public class AdmOrganisationstypViewModel
        {
            public int Organisationstypid { get; set; }
            public string Typnamn { get; set; }
            public string Beskrivning { get; set; }
            public bool Selected { get; set; } = false;
        }


       

        public class OrganisationsenhetViewModel
        {
            public int Organisationsid { get; set; }
            public string Enhetsnamn { get; set; }
            public string Enhetskod { get; set; }
            public DateTime? AktivFrom { get; set; }
            public DateTime? AktivTom { get; set; }
            public string Filkod { get; set; }
            public IEnumerable<UnitReportObligationsViewModel> UnitReportObligations { get; set; }

        }

        public class ReportObligationsViewModel
        {
            public int Id { get; set; }
            public int OrganisationId { get; set; }
            public int DelregisterId { get; set; }
            public string DelregisterKortnamn { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime? SkyldigFrom { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
            [Required(ErrorMessage = "Fältet är obligatoriskt.")]
            public DateTime? SkyldigFrom { get; set; }
            public DateTime? SkyldigTom { get; set; }
            public List<List<Organisation>> SearchResult { get; set; }
            public Organisation Organisation { get; set; }
            public string Origin { get; set; }
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
            public string OrganisationsNamn { get; set; }
            public string Namn { get; set; }
            public DateTime? AktivFrom { get; set; }
            public DateTime? AktivTom { get; set; }
            public int? Status { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public string Kontaktnummer { get; set; }
            public string ValdaDelregister { get; set; }
            public string StringOfRoles { get; set; }
            public List<IdentityRoleViewModel> ListOfRoles { get; set; }
            public IList<string> Roles { get; set; }
            public DateTime SkapadDatum { get; set; }
            public string SkapadAv { get; set; }
            public DateTime AndradDatum { get; set; }
            public string AndradAv { get; set; }
            public bool OkToDelete { get; set; }
        }

        public class SFTPkontoViewModel
        {
            public int Id { get; set; }
            public string Kontonamn { get; set; }
            public int OrganisationsId { get; set; }
            public int RegisterId { get; set; }
            public string RegisterNamn { get; set; }
            public DateTime SkapadDatum { get; set; }
            public string SkapadAv { get; set; }
            public DateTime AndradDatum { get; set; }
            public string AndradAv { get; set; }
            public List<string> ChosenContacts { get; set; }
            public string StringOfChosenContacts { get; set; }
            public List<ContactViewModel> ListOfContacts { get; set; }
        }

        public class ContactViewModel
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public bool Selected { get; set; } = false;
        }

        public class UndantagEpostadressViewModel
        {
            public int Id { get; set; }
            public int ArendeId { get; set; }
            [Display(Name = "Ärendenr")]
            public int OrganisationsId { get; set; }
            public string Organisationsnamn { get; set; }
            [Required(ErrorMessage = "Fältet E-postadress är obligatoriskt.")]
            [Display(Name = "E-postadress")]
            public string PrivatEpostAdress { get; set; }
            public int Status { get; set; }
            [Display(Name = "Aktiv fr.o.m.")]
            public DateTime? AktivFrom { get; set; }
            [Display(Name = "Aktiv t.o.m.")]
            public DateTime? AktivTom { get; set; }
        }

        public class UndantagForvantadfilViewModel
        {
            public int OrganisationsId { get; set; }
            public int DelregisterId { get; set; }
            public int ForvantadfilId { get; set; }
            public string Filmask { get; set; }
            public bool Selected { get; set; }
        }

        public class ArendeViewModel
        {
            public int Id { get; set; }
            public int OrganisationsId { get; set; }
            public string Organisationsnamn { get; set; }
            [Display(Name = "Ärendenamn")]
            public string Arendenamn { get; set; }
            [Display(Name = "Ärendenummer")]
            [Required(ErrorMessage = "Fältet Ärendenummer är obligatoriskt.")]
            public string Arendenr { get; set; }

            public int ArendetypId { get; set; }
            //public SelectList ArendetypDDL { get; set; }
            [Display(Name = "Ärendetyp")]
            public string Arendetyp { get; set; }

            public int ArendeansvarId { get; set; }
            //public SelectList ArendeansvarigDDL { get; set; }
            [Display(Name = "Ärendeansvarig")]
            public string Arendeansvarig { get; set; }
            public bool Aktiv { get; set; }
            public string ChosenContactsStr { get; set; }
            public List<ContactViewModel> Kontaktpersoner { get; set; }
            public string KontaktpersonerStr { get; set; }
            [Display(Name = "Rapportörer")]
            public string Rapportorer { get; set; }

        }
    }
}
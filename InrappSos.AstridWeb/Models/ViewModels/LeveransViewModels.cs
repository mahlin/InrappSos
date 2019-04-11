using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.DomainModel;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class LeveransViewModels
    {
        public class LeveransViewModel
        {
            public IEnumerable<RegisterInfo> RegisterList { get; set; }
            public IEnumerable<AdmForvantadleveransViewModel> ForvantadeLeveranser { get; set; }
            public List<AdmForvantadleveransViewModel> BlivandeForvantadeLeveranser { get; set; }
            public IEnumerable<AdmForvantadfilViewModel> ForvantadeFiler { get; set; }
            public List<AdmFilkravViewModel> Filkrav { get; set; }
            public IEnumerable<FilloggDetaljDTO> Leveranser { get; set; }
            public IEnumerable<AdmInsamlingsfrekvens> Insamlingsfrekvenser { get; set; }
            public int SelectedRegisterId { get; set; }
            public int SelectedOrganisationId { get; set; }
            public int SelectedDelregisterId { get; set; }
            public int SelectedFilkravId { get; set; }
            public int SelectedInsamlingsfrekvensId { get; set; }
            public int SelectedForvLevId { get; set; }
            [DisplayName("Visa endast pågående")]
            public bool FilterPagaende{ get; set; }
            [Display(Name = "År")]
            public int SelectedYear { get; set; }
            public string SelectedCountyCode { get; set; }
            [Display(Name = "Kommunkod")]
            public string Kommunkod { get; set; }
            public AdmInsamlingsfrekvens Insamlingsfrekvens { get; set; }
            public List<List<Organisation>> SearchResult { get; set; }
            public string Origin { get; set; }
            public Organisation Organisation { get; set; }


            public List<FilloggDetaljDTO> HistorikLista { get; set; }
            public List<RegisterLeveransDTO> LeveransListaRegister { get; set; }

            public List<int> SelectableYears { get; set; }
        }

        public class SearchResultViewModel
        {
            public List<List<Organisation>> SearchResult { get; set; }
            public string Origin { get; set; }
        }

        public class AdmForvantadleveransViewModel
        {
            public int Id { get; set; }
            public int FilkravId { get; set; }
            public string FilkravNamn { get; set; }
            public int DelregisterId { get; set; }
            public int SelectedDelregisterId { get; set; }
            public int SelectedRegisterId { get; set; }
            public int SelectedFilkravId { get; set; }

            public string DelregisterKortnamn { get; set; }
            public IEnumerable<RegisterInfo> RegisterList { get; set; }
            public IEnumerable<FilloggDetaljDTO> Leveranser { get; set; }
            public string Period { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Uppgiftsstart { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Uppgiftsslut { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Rapporteringsstart { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Rapporteringsslut { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Rapporteringsenast { get; set; }
            public DateTime? Paminnelse1 { get; set; }
            public DateTime? Paminnelse2 { get; set; }
            public DateTime? Paminnelse3 { get; set; }
            public bool Pagaende { get; set; }
            public bool Sen { get; set; }
            public bool AlreadyExists { get; set; }
        }

        public class AdmForvantadfilViewModel
        {
            public int Id { get; set; }
            public IEnumerable<RegisterInfo> RegisterList { get; set; }
            public int SelectedDelregisterId { get; set; }
            public int SelectedRegisterId { get; set; }
            public int SelectedFilkravId { get; set; }
            public int FilkravId { get; set; }
            public string FilkravNamn { get; set; }
            public int? ForeskriftsId { get; set; }
            public string DelregisterKortnamn { get; set; }
            [Required(ErrorMessage = "Fältet Filmask är obligatoriskt.")]
            public string Filmask { get; set; }
            [Required(ErrorMessage = "Fältet Regexp är obligatoriskt.")]
            public string Regexp { get; set; }
            public bool Obligatorisk { get; set; }
            public bool Tom { get; set; }
        }

        public class AdmFilkravViewModel
        {
            public int Id { get; set; }
            public int DelregisterId { get; set; }
            public int SelectedDelregisterId { get; set; }
            public int SelectedRegisterId { get; set; }
            public string DelregisterKortnamn { get; set; }
            public int? InsamlingsfrekvensId { get; set; }
            public SelectList InsamlingsfrekvensDDL { get; set; }
            public string Insamlingsfrekvens { get; set; }
            [Required(ErrorMessage = "Fältet ForeskriftsId är obligatoriskt.")]
            public int? ForeskriftsId { get; set; }
            public string Namn { get; set; }
            public int? Uppgiftsstartdag { get; set; }
            public int? Uppgiftslutdag { get; set; }
            public int? Rapporteringsstartdag { get; set; }
            public int? Rapporteringsslutdag { get; set; }
            [Display(Name = "Rapportering senast dag")]
            public int? RapporteringSenastdag { get; set; }
            [Display(Name = "Påminnelse 1")]
            public int? Paminnelse1dag { get; set; }
            [Display(Name = "Påminnelse 2")]
            public int? Paminnelse2dag { get; set; }
            [Display(Name = "Påminnelse 3")]
            public int? Paminnelse3dag { get; set; }
            [Display(Name = "Rapportering efter antal månader")]
            public int? RapporteringEfterAntalManader { get; set; }
            [Display(Name = "Uppgifter antal manader")]
            public int? UppgifterAntalmanader { get; set; }

        }

        public class ReminderViewModel
        {
            public int SelectedRegisterId { get; set; }
            public int SelectedDelregisterId { get; set; }
            public string SelectedPeriod { get; set; }

            public IEnumerable<RegisterBasicDTO> RegisterList { get; set; }

            public List<RapporteringsresultatDTO> RapportResList { get; set; }
            public int AntRader { get; set; }
        }


        public class HistoryViewModel
        {
            public string Kommunkod { get; set; }
            public int SelectedOrganisationId { get; set; }
            public string OrganisationsNamn { get; set; }
            public List<FilloggDetaljDTO> HistorikLista { get; set; }
            public List<AdmRegisterViewModel> RegisterList { get; set; }
            public List<RegisterLeveransDTO> LeveransListaRegister { get; set; }

            public int SelectedYear { get; set; }
            public List<int> SelectableYears { get; set; }
            public List<List<Organisation>> SearchResult { get; set; }
            public Organisation Organisation { get; set; }
            public string Origin { get; set; }

        }

        public class AdmRegisterViewModel
        {
            public int Id { get; set; }
            public string Registernamn { get; set; }
            public string Beskrivning { get; set; }
            public string Kortnamn { get; set; }

            public IEnumerable<AdmDelregisterViewModel> DelRegister { get; set; }
        }

        public class AdmDelregisterViewModel
        {
            public int Id { get; set; }
            public int RegisterId { get; set; }
            public string Delregisternamn { get; set; }
            public string Kortnamn { get; set; }
            public string Beskrivning { get; set; }
        }
    }
}
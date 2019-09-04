
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Helpers;
using InrappSos.DomainModel;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class FilesViewModel
    {
        public ViewDataUploadFilesResult[] Files { get; set; }
        [Display(Name = "Välj register")]
        public string SelectedRegisterId { get; set; }
        public string SelectedUnitId { get; set; }
        public string SelectedPeriod { get; set; }
        [DisplayName("Inget att rapportera")]
        public bool IngetAttRapportera { get; set; }
        public string IngetAttRapporteraForPeriod { get; set; }
        public string IngetAttRapporteraForRegisterId { get; set; }
        public string IngetAttRapporteraForSelectedUnitId { get; set; }
        public List<RegisterInfo> RegisterList { get; set; }
        public List<FilloggDetaljDTO> HistorikLista { get; set; }
        public string GiltigKommunKod { get; set; }
        public string GiltigLandstingsKod { get; set; }
        public string GiltigInrapporteringsKod { get; set; }
        public string OrganisationsNamn { get; set; }
        public string StartUrl { get; set; }

    }
}
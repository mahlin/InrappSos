﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Helpers;
using InrappSos.DomainModel;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class FileDropViewModel
    {
        public ViewDataUploadFilesResult[] Files { get; set; }
        [Display(Name = "Välj ärende")]
        public string SelectedCaseId { get; set; }
        public List<Arende> CaseList { get; set; }
        public string OrganisationsNamn { get; set; }
        public int OrganisationsId { get; set; }
        public Arende SelectedCase { get; set; }
        [DisplayName("Kontaktpersoner")]
        public string KontaktpersonerStr { get; set; }
        [DisplayName("Ej registrerade kontaktpersoner")]
        public string EjRegKontaktpersoner { get; set; }
        public List<FildroppDetaljDTO> HistorikLista { get; set; }
        public string StartUrl { get; set; }

    }
}
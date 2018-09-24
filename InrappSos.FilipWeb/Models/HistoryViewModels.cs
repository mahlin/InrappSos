
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Helpers;
using InrappSos.DomainModel;

namespace InrappSos.FilipWeb.Models
{
    public class HistoryViewModels
    {

        public class HistoryViewModel
        {
            public string OrganisationsNamn { get; set; }
            public List<FilloggDetaljDTO> HistorikLista { get; set; }
            public List<AdmRegisterViewModel> RegisterList { get; set; }
            public List<RegisterLeveransDTO> LeveransListaRegister { get; set; }
            public int SelectedRegisterId { get; set; }
            public int SelectedYear { get; set; }
            public List<int> SelectableYears { get; set; }
            
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
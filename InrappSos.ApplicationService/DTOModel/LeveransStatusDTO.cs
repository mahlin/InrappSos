using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InrappSos.ApplicationService.DTOModel
{
    public class LeveransStatusDTO
    {
        public int Id { get; set; }
        public int RegisterId { get; set; }
        public string RegisterKortnamn { get; set; }
        public string Status { get; set; }
        public string Period { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Rapporteringsstart { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Rapporteringssenast { get; set; }
        public List<FilloggDetaljDTO> HistorikLista { get; set; }
    }
}
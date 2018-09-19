using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class ForvantadLeveransDTO
    {
        public int Id { get; set; }
        public int FilkravId { get; set; }
        public int DelregisterId { get; set; }
        public string Period { get; set; }
        public DateTime Uppgiftsstart { get; set; }
        public DateTime Uppgiftsslut { get; set; }
        public DateTime Rapporteringsstart { get; set; }
        public DateTime Rapporteringsslut { get; set; }
        public DateTime Rapporteringsenast { get; set; }
        public DateTime? Paminnelse1 { get; set; }
        public DateTime? Paminnelse2 { get; set; }
        public DateTime? Paminnelse3 { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public bool AlreadyExists { get; set; }
    }
}

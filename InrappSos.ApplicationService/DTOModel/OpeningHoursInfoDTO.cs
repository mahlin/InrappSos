using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.DTOModel
{
    public class OpeningHoursInfoDTO
    {
        public string ClosedFromHour { get; set; }
        public string ClosedFromMin { get; set; }
        public string ClosedToHour { get; set; }
        public  string ClosedToMin { get; set; }
        [DisplayName("Stäng portalen")]
        public bool ClosedAnyway { get; set; }
        public List<string> ClosedDays { get; set; }
        public string InfoTextForClosedPage { get; set; }
    }
}

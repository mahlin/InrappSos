using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmHelgdag
    {
        public int Id { get; set; }
        public int InformationsId { get; set; }
        public DateTime Helgdatum { get; set; }
        public string Helgdag { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmInformation AdmInformation { get; set; }
    }
}
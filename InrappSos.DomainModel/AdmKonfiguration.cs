using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmKonfiguration
    {
        public int Id { get; set; }
        public string Typ{ get; set; }
        public string Varde { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
    }
}
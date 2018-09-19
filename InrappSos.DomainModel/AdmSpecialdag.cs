using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmSpecialdag
    {
        public int Id { get; set; }
        public int InformationsId { get; set; }
        public DateTime Specialdagdatum { get; set; }
        public TimeSpan Oppna { get; set; }
        public TimeSpan Stang { get; set; }
        public string Anledning { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmInformation AdmInformation { get; set; }
    }
}
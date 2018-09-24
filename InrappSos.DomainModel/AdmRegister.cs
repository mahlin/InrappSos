using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmRegister
    {
        public int Id { get; set; }
        public string Registernamn { get; set; }
        public string Beskrivning { get; set; }
        public string Kortnamn { get; set; }
        public bool InrappSos { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<AdmDelregister> AdmDelregister { get; set; }
        public virtual ICollection<AdmForeskrift> AdmForeskrift { get; set; }

    }
}
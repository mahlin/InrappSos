using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmForvantadfil
    {
        public int Id { get; set; }
        public int FilkravId { get; set; }
        public int? ForeskriftsId { get; set; }
        public string Filmask { get; set; }
        public string Regexp { get; set; }
        public bool Obligatorisk { get; set; }
        public bool Tom { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmFilkrav AdmFilkrav { get; set; }
        public virtual AdmForeskrift AdmForeskrift { get; set; }
        public virtual ICollection<UndantagForvantadfil> UndantagForvantadfil { get; set; }
    }
}
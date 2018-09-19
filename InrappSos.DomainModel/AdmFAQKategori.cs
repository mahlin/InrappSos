using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmFAQKategori
    {
        public int Id { get; set; }
        public string Kategori { get; set; }
        public int? Sortering { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<AdmFAQ> AdmFAQ { get; set; }

    }
}
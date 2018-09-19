using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmFAQ
    {
        public int Id { get; set; }
        public int? RegisterId { get; set; }
        public int FAQkategoriId { get; set; }
        public string Fraga { get; set; }
        public string Svar { get; set; }
        public int? Sortering { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmFAQKategori AdmFAQKategori{ get; set; }


    }
}
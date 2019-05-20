using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmDokument
    {
        public int Id { get; set; }
        public string Filnamn { get; set; }
        public string Filtyp { get; set; }
        public string Suffix { get; set; }
        public byte Fil { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }

    }
}
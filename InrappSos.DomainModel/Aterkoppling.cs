using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class Aterkoppling
    {
        public int Id { get; set; }
        public int LeveransId { get; set; }
        public string Leveransstatus { get; set; }
        public string Resultatfil { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }

    }
}
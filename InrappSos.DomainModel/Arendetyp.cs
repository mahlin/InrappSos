﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class Arendetyp
    {
        public int Id { get; set; }
        [DisplayName("Ärendenamn")]
        public string ArendetypNamn { get; set; }
        public string Slussmapp { get; set; }
        public string KontaktpersonerStr { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<Arende> Arende { get; set; }
    }
}

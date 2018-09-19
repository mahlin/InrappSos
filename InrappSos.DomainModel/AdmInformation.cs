using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class AdmInformation
    {
        public int Id { get; set; }
        public string Informationstyp{ get; set; }
        public string Text { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<AdmHelgdag> AdmHelgdag { get; set; }
        public virtual ICollection<AdmSpecialdag> AdmSpecialdag { get; set; }
    }
}
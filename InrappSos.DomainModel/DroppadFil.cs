using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class DroppadFil
    {
        public int Id { get; set; }
        public int ArendeId { get; set; }
        public string ApplicationUserId { get; set; }
        public string Filnamn { get; set; }
        public string NyttFilnamn { get; set; }
        public Int64 Filstorlek { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Arende Arende { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}

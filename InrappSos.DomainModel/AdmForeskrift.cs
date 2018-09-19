using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class AdmForeskrift
    {
        public int Id { get; set; }
        public int RegisterId { get; set; }
        public string Forfattningsnr { get; set; }
        public string Forfattningsnamn { get; set; }
        public DateTime GiltigFrom { get; set; }
        public DateTime? GiltigTom { get; set; }
        public DateTime Beslutsdatum { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual AdmRegister AdmRegister { get; set; }
        public virtual ICollection<AdmForvantadfil> AdmForvantadfil { get; set; }
        public virtual ICollection<AdmFilkrav> AdmFilkrav { get; set; }
    }
}

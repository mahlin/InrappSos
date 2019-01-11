using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime GiltigFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? GiltigTom { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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

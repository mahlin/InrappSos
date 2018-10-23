using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class Organisation
    {
        public int Id { get; set; }
        public string Landstingskod { get; set; }
        public string Kommunkod { get; set; }
        public string Inrapporteringskod { get; set; }
        //public string Organisationstyp { get; set; }
        public string Organisationsnr { get; set; }
        public string Organisationsnamn { get; set; }
        public string Hemsida { get; set; }
        public string EpostAdress { get; set; }
        public string Telefonnr { get; set; }
        public string Adress { get; set; }
        public string Postnr { get; set; }
        public string Postort { get; set; }
        public string Epostdoman { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AktivFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AktivTom { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Leverans> Leveranser { get; set; }
        public virtual ICollection<AdmUppgiftsskyldighet> AdmUppgiftsskyldighet { get; set; }
        public virtual ICollection<Organisationsenhet> Organisationsenhet { get; set; }
        public virtual ICollection<Organisationstyp> Organisationstyp { get; set; }
        public virtual ICollection<Arende> Arende { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class Organisationsenhet
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public string Enhetsnamn { get; set; }
        public string Enhetskod { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AktivFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AktivTom { get; set; }
        public string Filkod { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<Leverans> Leveranser { get; set; }
        public virtual ICollection<AdmEnhetsUppgiftsskyldighet> AdmEnhetsUppgiftsskyldighet { get; set; }
    }
}


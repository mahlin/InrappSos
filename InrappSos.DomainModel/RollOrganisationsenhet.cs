using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class RollOrganisationsenhet
    {
        public int Id { get; set; }
        public int OrganisationsenhetsId { get; set; }
        public int RollId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public virtual Roll Roll { get; set; }
    }
}
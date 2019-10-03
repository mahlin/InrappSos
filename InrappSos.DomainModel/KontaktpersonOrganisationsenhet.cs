using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class KontaktpersonOrganisationsenhet
    {
        public int Id { get; set; }
        public int OrganisationsenhetsId { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
    }
}